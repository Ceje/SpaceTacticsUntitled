using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceItem : Action
{
    public Sprite uISprite;
    public Color uIColor = Color.blue;
    
    private Item _item;

    public virtual void Start(){
        _item = GetComponent<Item>();
    }

    public virtual void DoPlace(Vector3Int location){
        var unit = Data.UnitTracker.GetActiveUnit();

        transform.SetParent(Data.board.transform);
        transform.position = Data.board.CellToWorld(location);
        unit.RemoveItem(_item);
        _item.InInventory = false;
    }

    public override bool CanUse(){
        var unit = Data.UnitTracker.GetActiveUnit();
        if (unit == null 
            || unit.transform != transform.parent 
            || unit is MagpieNest){
            return false;
        }

        return unit.Items.ContainsKey(_item);
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }

    public override void Cancel(){
        throw new System.NotImplementedException();
    }

    //AI commands
    public override void AIExecute(){
        var unit = Data.UnitTracker.GetActiveUnit();
        DoPlace(Data.board.WorldToCell(unit.transform.position));
    }
    public override int AssessPriority(){
        var magpie = transform.parent.gameObject.GetComponent<Magpie>();
        if (magpie != null){
            return 0;
        }
        return 100;
    }
    
    
    
    //player commands
    public override void PlayerExecute(){
        InputHandler.SetSprite(uISprite, uIColor);
        InputHandler.SetActionMode(this);
    }
    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var location = Data.board.WorldToCell(hit.Value.point);
            if (sprite.enabled){
                DoPlace(location);
                InputHandler.ClearActionMode();
                InputHandler.HideUi();
            }
            sprite.enabled = false;
        }
    }

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var tileCenter = Data.board.CellToWorld(Data.board.WorldToCell(hit.Value.point));
            targetingTransform.position = tileCenter;
            sprite.enabled = (transform.position-tileCenter).magnitude < 1.5f;
            return;
        }

        sprite.enabled = false;
    }

    public override void SetHighlight(){
        var thisTile = Data.board.WorldToCell(transform.position);
        var highlightSet = new HashSet<GameObject>();
        
        highlightSet.Add(Data.board.GetTile(thisTile).gameObject);
        Data.board.GetNeighbors(thisTile).ForEach(tile => highlightSet.Add(Data.board.GetTile(tile).gameObject));
        HighlightMap.SetHighlights(highlightSet);
    }
}
