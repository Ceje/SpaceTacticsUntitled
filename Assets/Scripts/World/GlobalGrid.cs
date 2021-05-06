using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GlobalGrid : MonoBehaviour
{
    private Dictionary<TileBase, float> terrainWeight = new Dictionary<TileBase, float>();
    private Dictionary<Vector3Int, float> tileWeights = new Dictionary<Vector3Int, float>();
    private Dictionary<Vector3Int, TileCollider> _tileContents = new Dictionary<Vector3Int, TileCollider>();
    private Tilemap _map;
    private PathingMap _pathingMap = new PathingMap();
    private ZoneMap _zoneMap = new ZoneMap();
    private Grid _grid;

    public GameObject TileCollider;

    public static bool Initialized = false;

    // Start is called before the first frame update
    void Start(){
        _map = GetComponent<Tilemap>();
        _grid = gameObject.GetComponentInParent<Grid>();
        //set global access
        Data.board = this;

        //construct tile/weight dictionary
        var tileArry = new TileBase[_map.GetUsedTilesCount()];
        _map.GetUsedTilesNonAlloc(tileArry);
        foreach (TileBase tileBase in tileArry){
            //determin typing and cast to get access to colliders
            Tile.ColliderType type = Tile.ColliderType.None;
            if (tileBase.GetType() == typeof(Tile)){
                var tile = (Tile) tileBase;
                type = tile.colliderType;
            }
            else if (tileBase.GetType() == typeof(HexagonalRuleTile)){
                var tile = (HexagonalRuleTile) tileBase;
                type = tile.m_DefaultColliderType;
            }

            //add entries based on colliders
            switch (type){
                case Tile.ColliderType.Sprite:
                    terrainWeight.Add(tileBase, -1);
                    break;
                case Tile.ColliderType.Grid:
                    terrainWeight.Add(tileBase, 1);
                    break;
                case Tile.ColliderType.None:
                    Debug.Log(tileBase.name);
                    break;
            }
        }

        foreach (var tile in _map.cellBounds.allPositionsWithin){
            tileWeights[tile] = CalculateTileWeight(tile);
            if (tileWeights[tile] > -2){
                var tileCollider = Instantiate(TileCollider, transform);
                tileCollider.transform.position = CellToWorld(tile);
                _tileContents[tile] = tileCollider.GetComponent<TileCollider>();
            }
        }

        //center all pieces
        for (int i = 0; i < transform.childCount; i++){
            var child = transform.GetChild(i);
            child.position = CellToWorld(WorldToCell(child.position));
        }

        _pathingMap.UpdateTileGroups();
        _zoneMap.UpdateTileGroups();
        _zoneMap.SetZoneActivators(Teams.GetManagerInstance().GetTeam("Player").GetMembers());
        MusicManager.UpdateBgm();
        Initialized = true;
    }

    // Update is called once per frame
    void Update(){
    }

    public Vector3Int WorldToCell(Vector3 point){
        return _map.WorldToCell(point);
    }

    public Vector3 CellToWorld(Vector3Int tile){
        return _map.CellToWorld(tile);
    }

    private static List<Vector3Int> GeneratePotentialNeighbors(Vector3Int tile){
        var list = new List<Vector3Int>();
        for (int i = -1; i < 2; i++){
            for (int j = -1; j < 2; j++){
                list.Add(new Vector3Int(tile.x + i, tile.y + j, 0));
            }
        }

        return list;
    }

    public List<Vector3Int> GetNeighbors(Vector3Int tile){
        var list = GeneratePotentialNeighbors(tile);

        //remove neighbors that go too far
        list = list.FindAll(neighbor => (_map.CellToWorld(neighbor) - _map.CellToWorld(tile)).magnitude <= 1);

        list.Remove(tile);
        return list;
    }

    public List<Vector3Int> GetFilteredNeighbors(Vector3Int tile){
        var list = GeneratePotentialNeighbors(tile);

        list = list.FindAll(neighbor => FilterTile(neighbor, tile));

        list.Remove(tile);
        return list;
    }

    public bool FilterTile(Vector3Int tile, Vector3Int referenceTile){
        return (_map.CellToWorld(tile) - _map.CellToWorld(referenceTile)).magnitude <= 1
               && TileWeight(tile) >= 0
               && !TileOccupied(tile, gameObject)
               && !Data.fog.inFog(tile);
    }

    private float CalculateTileWeight(Vector3Int coordinates){
        var tile = _map.GetTile(coordinates);
        if (tile is null){
            return -2;
        }

        return terrainWeight[tile];
    }

    public float TileWeight(Vector3Int coordinates){
        if (!tileWeights.ContainsKey(coordinates)){
            return -2;
        }

        return tileWeights[coordinates];
    }


    public bool TileOccupied(Vector3Int coordinates, GameObject invoker){
        var interactables = _tileContents[coordinates].ObjectsOnTile();
        return interactables.Any(interactor => {
            if (interactor == null || interactor.gameObject == invoker || interactor.enabled == false){
                return false;
            }
            return interactor.blocksMovement && Data.board.WorldToCell(interactor.transform.position) == coordinates;

        });
    }

    private bool TileBlocksVision(Vector3Int coordinates){
        var interactables = _tileContents[coordinates].ObjectsOnTile();

        return interactables.Any(interactable => {
            if (interactable == null || interactable.enabled == false){
                return false;
            }

            return interactable.blocksVision;
        });
    }

    public List<Vector3Int> AStar(Vector3Int start, Vector3Int finish){
        if (!TilesConnected(start, finish)){
            Debug.Log("Pathing groups suggest no path");
            return new List<Vector3Int>();
        }

        SortedSet<AStarChild> OpenTiles = new SortedSet<AStarChild>();
        OpenTiles.Add(new AStarChild(start, null, 0, 0));
        Dictionary<Vector3, AStarChild> FoundTiles = new Dictionary<Vector3, AStarChild>();

        while (!FoundTiles.ContainsKey(finish)){
            AStarChild currentTile = OpenTiles.Min;
            GetFilteredNeighbors(currentTile.tile).ForEach(neighbor => {
                var cost = currentTile.cost + TileWeight(neighbor);
                if (!FoundTiles.ContainsKey(neighbor) || FoundTiles[neighbor].cost > cost){
                    var distanceToGoal = (_map.CellToWorld(neighbor) - _map.CellToWorld(finish)).magnitude;
                    var neighborChild = new AStarChild(neighbor, currentTile, cost, cost + distanceToGoal);
                    FoundTiles[neighbor] = neighborChild;
                    OpenTiles.Add(neighborChild);
                }
            });
            OpenTiles.Remove(currentTile);
            if (OpenTiles.Count == 0){
//                Debug.Log(FoundTiles.Count + " " + FoundTiles.ContainsKey(finish));
                return new List<Vector3Int>();
            }
        }

        List<Vector3Int> path = new List<Vector3Int>();
        AStarChild tile = FoundTiles[finish];
        while (tile.parent != null){
            path.Add(tile.tile);
            tile = tile.parent;
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    private class AStarChild : IComparable<AStarChild>
    {
        public float cost;
        public float priority;
        public Vector3Int tile;
        public AStarChild parent;

        public AStarChild(Vector3Int tile, AStarChild parent, float cost, float priority){
            this.cost = cost;
            this.tile = tile;
            this.parent = parent;
            this.priority = priority;
        }

        public int CompareTo(AStarChild other){
            return priority.CompareTo(other.priority);
        }

        public override string ToString(){
            return tile + "<-" + parent;
        }
    }

    public TileCollider GetTile(Vector3 tile){
        return GetTile(WorldToCell(tile));
    }
    public TileCollider GetTile(Vector3Int tilecoordinate){
        if (!_tileContents.ContainsKey(tilecoordinate)){
            return null;
        }
        return _tileContents[tilecoordinate];
    }

    public HashSet<Vector3Int> TilesOnPath(Vector3 start, Vector3 end, float frequency, float thickness){
        HashSet<Vector3Int> tileCoordinates = new HashSet<Vector3Int>();
        var step = (end - start).normalized * frequency;
        tileCoordinates.Add(_map.WorldToCell(start));
        tileCoordinates.UnionWith(TilesOnPath(start, end,
            new Vector3(start.x + thickness, start.y + thickness, start.z), step));

        if (thickness == 0) return tileCoordinates;

        tileCoordinates.UnionWith(TilesOnPath(start, end,
            new Vector3(start.x + thickness, start.y - thickness, start.z), step));
        tileCoordinates.UnionWith(TilesOnPath(start, end,
            new Vector3(start.x - thickness, start.y + thickness, start.z), step));
        tileCoordinates.UnionWith(TilesOnPath(start, end,
            new Vector3(start.x - thickness, start.y - thickness, start.z), step));
        return tileCoordinates;
    }

    private HashSet<Vector3Int> TilesOnPath(Vector3 start, Vector3 end, Vector3 point, Vector3 step){
        HashSet<Vector3Int> tileCoordinates = new HashSet<Vector3Int>();
        while (((point + step) - start).magnitude < (end - start).magnitude){
            point += step;
            tileCoordinates.Add(_map.WorldToCell(point));
        }

        return tileCoordinates;
    }

    public bool isVisible(Vector3Int unit, Vector3Int target){
        HashSet<Vector3Int> path = TilesOnPath(_map.CellToWorld(unit), _map.CellToWorld(target), 0.5f, 0);
        path.Remove(unit);
        path.Remove(target);
        return path.All(tile => TileWeight(tile) >= 0 && !TileBlocksVision(tile));
    }

    public HashSet<Vector3Int> GetTileLine(Vector3Int start, float facing, int length, float frequency,
        float thickness){
        Vector3Int end = GetTileByOrientation(start, length, facing);
        return TilesOnPath(_map.CellToWorld(start), _map.CellToWorld(end), frequency, thickness);
    }
    private static float GetDirection(Vector3 start, Vector3 finish){
        Vector3 difference = start - finish;
        var exact = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        var direction = (float) Math.Round(exact / 60) * 60;
        return direction;
    }
    public HashSet<Vector3Int> GetTileLine(Vector3Int start, Vector3Int end, float frequency, float thickness){
        return TilesOnPath(_map.CellToWorld(start), _map.CellToWorld(end), frequency, thickness);
    }

    public Vector3Int GetTileByOrientation(Vector3Int start, float distance, float facing){
        var rotation = _grid.transform.rotation;
        rotation *= Quaternion.Euler(0, 0, facing);
        var end = rotation * Vector3.right * distance;
        return GetTileByVector(_map.CellToWorld(start), end);
    }

    public Vector3Int GetTileByOrientation(Vector3Int start, int distance, Quaternion facing){
        var end = facing * Vector3.right * distance;
        return GetTileByVector(_map.CellToWorld(start), end);
    }

    private Vector3Int GetTileByVector(Vector3 start, Vector3 end){
        return WorldToCell(start + end);
    }

    public List<T> GetNearby<T>(Vector3Int tile, int range){
        List<T> nearby = new List<T>();
        HashSet<Vector3Int> tilesToCheck = new HashSet<Vector3Int>{tile};

        for (int i = 0; i < range; i++){
            var tilesToExpand = new HashSet<Vector3Int>();
            foreach (var vector3Int in tilesToCheck){
                foreach (var neighbor in GetNeighbors(vector3Int)){
                    tilesToExpand.Add(neighbor);
                }
            }
            tilesToCheck.UnionWith(tilesToExpand);
        }
        
        foreach (var vector3Int in tilesToCheck){
            if (!_tileContents.ContainsKey(vector3Int)){
                continue;
            }
            foreach (var interactable in _tileContents[vector3Int].ObjectsOnTile()){
                if (interactable is T subclass){
                    nearby.Add(subclass);
                }
            }
        }

        return nearby;
    }

    public List<T> GetOnLine<T>(Vector3Int start, int facing, int distance){
        var foundT = new List<T>();
        foreach (var tile in GetTileLine(start, facing, distance, .9f, .1f)){
            GetNearby<T>(tile, 0).ForEach(found => foundT.Add(found));
        }

        return foundT;
    }

    public List<T> GetOnLine<T>(Vector3Int start, Vector3Int end){
        var foundT = new List<T>();
        foreach (var tile in GetTileLine(start, end, .9f, .1f)){
            GetNearby<T>(tile, 0).ForEach(found => foundT.Add(found));
        }

        return foundT;
    }

    public List<T> GetOnLine<T>(Vector3 start, Quaternion facing, bool respectsObstacles, bool respectsWalls){
        var tileToCheck = WorldToCell(start);
        List<T> foundT = new List<T>();
        var distance = 1;
        while (true){
            var compoundingVector = facing * Vector3.right * distance;
            tileToCheck = GetTileByVector(start, compoundingVector);
            if (GetTile(tileToCheck) is null
                || (respectsWalls && TileWeight(tileToCheck) < 0)
                || (respectsObstacles && TileOccupied(tileToCheck, gameObject))){
                break;
            }

            GetNearby<T>(tileToCheck, 0).ForEach(found => foundT.Add(found));
            distance++;
        }

        return foundT;
    }

    public Dictionary<Vector3Int, float> AllTileWeights(){
        return tileWeights;
    }

    public void UpdatePathingMap(){
        _pathingMap.UpdateTileGroups();
    }

    public void UpdatePathingMap(List<Vector3Int> changes){
        _pathingMap.UpdateTileGroups(changes);
    }

    public bool TilesConnected(Vector3Int tilaA, Vector3Int tileB){
        return _pathingMap.Connected(tilaA, tileB);
    }

    public int TilePathingGroup(Vector3Int vector3Int){
        return _pathingMap.GetTileGroup(vector3Int);
    }

    public List<Vector3Int> GetZoneTiles(int visibleZone){
        return _zoneMap.AllTilesInGroup(visibleZone);
    }

    public bool InSameZone(Interactable player, Unit unit){
        var playerTile = WorldToCell(player.transform.position);
        var unitTile = WorldToCell(unit.transform.position);
        return TilesConnected(playerTile, unitTile);
    }

    public int ThreatLevel(){
        var teamManager = Teams.GetManagerInstance();
        var playerUnits = teamManager.GetTeam("Player").GetMembers();
        var activeZones = new HashSet<int>();
        foreach (var playerUnit in playerUnits){
            activeZones.Add(_pathingMap.GetTileGroup(WorldToCell(playerUnit.transform.position)));
        }

        if (activeZones.Any(BossInZone)){
            return 2;
        }
        
        if (activeZones.Any(zone => EnemyInZone(zone, teamManager, playerUnits.First()))){
            return 1;
        }

        return 0;
    }

    private bool BossInZone(int activeZone){
        return _pathingMap.AllTilesInGroup(activeZone).Any(tile => {
            return GetTile(tile).ObjectsOnTile().Any(thing => thing is NanoSwarm);
        });
    }

    private bool EnemyInZone(int activeZone, Teams.TeamManager teamManager, Interactable playerUnit){
        return _pathingMap.AllTilesInGroup(activeZone).Any(tile => {
            return GetTile(tile).ObjectsOnTile()
                .Any(thing => thing is Unit && !teamManager.ShareTeams(thing, playerUnit));
        });
    }
}