using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Movement : Action
{
    protected List<Vector3Int> MoveList = new List<Vector3Int>();
    private List<Vector3Int> _moveProjection = new List<Vector3Int>();
    protected Vector3 TargetLocation;
    protected Unit Unit;

    protected Propulsion CurrentMove;
    // Start is called before the first frame update
    void Start(){
        Unit = GetComponentInParent<Unit>();
    }

    // Action Content
    protected virtual void Update(){

        if (CurrentMove != null){
            return;
        }

        if (MoveList.Count == 0 && CurrentMove == null){
            Unit.EndMove();
            return;
        }

        var nextMove = MoveList[0];
        MoveList.RemoveAt(0);

        
        Propulsion propulsion = Unit.gameObject.AddComponent<Propulsion>();
        CurrentMove = propulsion;

        propulsion.Configure(Unit, nextMove);
        propulsion.StartMovement();
        
    }

    protected void Path(Vector3Int unitLocation, Vector3Int targetLocation, [CanBeNull] LineRenderer line = null){
        
        
        if (MoveInvalid(unitLocation, targetLocation)){
//            Debug.Log(unitLocation + " " + targetLocation);
//            Debug.Log("Wall: " + (Data.board.TileWeight(targetLocation) < 0));
//            Debug.Log("No Move: " + (unitLocation == targetLocation));
//            Debug.Log("Occupied: " + Data.board.TileOccupied(targetLocation, Unit.gameObject));
//            Debug.Log("In Fog: " + Data.fog.inFog(targetLocation));
//            Debug.Log("No Path:" + !Data.board.TilesConnected(unitLocation, targetLocation));
            if (Teams.GetManagerInstance().GetUnitTeams(Unit).Any(team => team.IsAI())){
                Unit.remainingMovement--;
            }
            return;
        }

        List<Vector3Int> results = Data.board.AStar(unitLocation, targetLocation);

        if (results.Count - 1 > Unit.remainingMovement || results.Count == 0){
            return;
        }

        _moveProjection = results;
        if (line is null){
            return;
        }
        
        var projectedWorldMoves = new Vector3[_moveProjection.Count];
        for (var i = 0; i < projectedWorldMoves.Length; i++){
            projectedWorldMoves[i] = Data.board.CellToWorld(_moveProjection[i]);
        }

        line.positionCount = projectedWorldMoves.Length;
        line.SetPositions(projectedWorldMoves);
        line.enabled = true;
    }

    private bool MoveInvalid(Vector3Int unitLocation, Vector3Int targetLocation){
        return unitLocation == targetLocation || 
               Data.board.TileWeight(targetLocation) < 0 || 
               Data.board.TileOccupied(targetLocation, gameObject) ||
               Data.fog.inFog(targetLocation) ||
               !Data.board.TilesConnected(unitLocation, targetLocation);
    }

    protected void Move(){
        if (_moveProjection.Count < 2){
            return;
        }
        MoveList = _moveProjection;
        Unit.remainingMovement -= MoveList.Count-1;
        TargetLocation = Data.board.CellToWorld(MoveList[1]);
        Unit.AdjustFacing(Data.board.CellToWorld(MoveList[1]));
        Unit.StartMove();
        InputHandler.ClearActionMode();
    }

    //Common & UI Functions
    public override bool CanUse(){
        return Unit.remainingMovement > 0;
    }
    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }
    public override void Cancel(){
        MoveList.Clear();
        _moveProjection.Clear();
        InputHandler.HideUi();
    }

    //Player Script
    public override void PlayerExecute(){
        InputHandler.SetActionMode(this);
    }

    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? ray = InputHandler.CursorRaycast();
        if (ray.HasValue){
            if (_moveProjection.Count < 2){
                Debug.Log("bad movement list");
                return;
            }
            line.positionCount = 0;
            Move();
        }
    }

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        line.enabled = false;
        if (CurrentMove != null){
            return;
        }
        _moveProjection.Clear();

        RaycastHit? ray = InputHandler.CursorRaycast();
        if (ray.HasValue){
            var unitLocation = Data.board.WorldToCell(Unit.transform.position);
            var targetLocation = Data.board.WorldToCell(ray.Value.point);
            
            Path(unitLocation, targetLocation, line);
        }
    }
    public override void SetHighlight(){
        var highlights = new HashSet<GameObject>{gameObject};
        for (int i = 0; i < Unit.remainingMovement; i++){
            var tilesToExpand = new HashSet<GameObject>();
            foreach (var reference in highlights){
                var locationReference = Data.board.WorldToCell(reference.transform.position);
                foreach (var neighbor in Data.board.GetNeighbors(locationReference)){
                    if (!MoveInvalid(locationReference, neighbor)){
                        tilesToExpand.Add(Data.board.GetTile(neighbor).gameObject);
                    }
                }
            }
            highlights.UnionWith(tilesToExpand);
        }
        
        HighlightMap.SetHighlights(highlights);
    }
    
    //AI Script
    public override void AIExecute(){
        var currentLocation = Data.board.WorldToCell(Unit.transform.position);
        //determine target location
        var targetLocation = Data.board.GetTileByOrientation(currentLocation, 1, Random.value * 360);
        //execute move
        Path(currentLocation, targetLocation);
        if (_moveProjection.Count == 0){
            return;
        }

        Move();
    }

    public override int AssessPriority(){
        return 10;
    }
}