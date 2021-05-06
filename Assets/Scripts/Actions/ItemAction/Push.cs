using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Push : Action
{
    public Sprite uISprite;
    public Color uIColor = Color.blue;

    private Item _item;

    void Start(){
        _item = GetComponent<Item>();
    }


    private void DoPush(Vector3Int location){
        var parent = transform.parent.gameObject;
        var propulsion = parent.AddComponent<Propulsion>();
        propulsion.Configure(parent.GetComponent<Interactable>(), location);
        propulsion.StartMovement();
    }

    public override bool CanUse(){
        var withinRange = (Data.UnitTracker.GetActiveUnit().transform.position - transform.position).magnitude < 1.1;
        return transform.parent != Data.board.transform && !_item.InInventory && withinRange;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "Inspector" && transform.parent != Data.board.transform && !_item.InInventory;
    }

    public override void Cancel(){
        InputHandler.HideUi();
    }

    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var location = Data.board.WorldToCell(hit.Value.point);
            if (sprite.enabled){
                DoPush(location);
                InputHandler.ClearActionMode();
                InputHandler.HideUi();
                Data.Inspector.ClearInspected();
            }

            sprite.enabled = false;
        }
    }


    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var location = Data.board.WorldToCell(hit.Value.point);
            if ((Data.board.TileWeight(location) < 0 ||
                 Data.board.TileOccupied(location, gameObject.transform.parent.gameObject)) &&
                !Data.fog.inFog(location)){
                sprite.color = Color.red;
            }
            else{
                sprite.color = Color.blue;
            }

            targetingTransform.position = Data.board.CellToWorld(location);
            sprite.enabled = HighlightMap.HighlightContains(location);
            return;
        }

        sprite.enabled = false;
    }

    public override void SetHighlight(){
        var forceDirection = GetDirection(Data.UnitTracker.GetActiveUnit().transform.position, transform.position);
        forceDirection -= 120; //bandaid fixing of targeting line.
        var tileLine =  Data.board.GetTileLine(Data.board.WorldToCell(transform.position), forceDirection, 20, .8f, 0);
        var targetList = new HashSet<GameObject>();
        foreach (var vector3Int in tileLine){
            targetList.Add(Data.board.GetTile(vector3Int)?.gameObject);
        }
        HighlightMap.SetHighlights(targetList);
    }

    private static float GetDirection(Vector3 start, Vector3 finish){
        Vector3 difference = start - finish;
        var exact = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        var direction = (float) Math.Round(exact / 60) * 60;
        return direction;
    }

    public override void PlayerExecute(){
        InputHandler.SetActionMode(this);
        InputHandler.SetSprite(uISprite, uIColor);
    }

    public override void AIExecute(){
        var gremlin = Data.UnitTracker.GetActiveUnit() as Gremlin;
        if (gremlin == null){
            return;
        }
        var forceDirection = GetDirection(gremlin.transform.position, transform.position);
        var targetLocation = Data.board.GetTileByOrientation(Data.board.WorldToCell(transform.position), gremlin.strength, forceDirection);
        DoPush(targetLocation);
    }

    public override int AssessPriority(){
        var gremlin = Data.UnitTracker.GetActiveUnit() as Gremlin;
        if (gremlin == null){
            return 0;
        }
        
        var value = 10;
        var forceDirection = GetDirection(gremlin.transform.position, transform.position);
        var wouldHit = Data.board.GetOnLine<PlayerUnit>(Data.board.WorldToCell(transform.position), (int) forceDirection,
            gremlin.strength);
        value += wouldHit.Count * 10;

        return value;
    }
}