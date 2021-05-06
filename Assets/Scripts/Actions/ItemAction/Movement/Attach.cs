using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : Action
{
    private Item _item;
    public Sprite uISprite;
    public Color uIColor = Color.blue;
    public virtual void Start(){
        _item = GetComponent<Item>();
        aiAccessible = false;
    }
    
    protected virtual void DoAttach(Interactable target){
        Transform transform1 = transform;
        var held = transform1.parent != Data.board.transform;
        transform1.SetParent(target.gameObject.transform);
        transform1.position = target.transform.position;
        if (held){
            var unit = Data.UnitTracker.GetActiveUnit();
            unit.RemoveItem(_item);
            _item.InInventory = false;
        }
        Data.AttachedItems.Add(_item);
    }

    public override bool CanUse(){
        var unit = Data.UnitTracker.GetActiveUnit();
        List<Interactable> nearby = Data.board.GetNearby<Interactable>(Data.board.WorldToCell(transform.position), 1);
        return !(unit is null) && unit.Items.ContainsKey(_item) && nearby.Count > 0;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }

    public override void Cancel(){
        InputHandler.HideUi();
    }

    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var enemy = hit.Value.collider.gameObject.GetComponent<Interactable>();
            if (enemy != null && sprite.enabled){
                DoAttach(enemy);
                InputHandler.ClearActionMode();
            }
            sprite.enabled = false;
        }
    }

    

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var target = hit.GetValueOrDefault().collider.gameObject.GetComponent<Interactable>();
            if (target != null && target.visible){
                if ((gameObject.transform.position - target.transform.position).magnitude >= 2){
                    return;
                }

                targetingTransform.position = target.transform.position;
                sprite.enabled = true;
                return;
            }
        }

        sprite.enabled = false;
    }

    public override void SetHighlight(){
        var thisTile = Data.board.WorldToCell(transform.position);
        var highlightSet = new HashSet<GameObject>();
        foreach (var interactable in Data.board.GetNearby<Interactable>(thisTile, 1)){
            if (!interactable.visible){
                continue;
            }
            highlightSet.Add(interactable.gameObject);
        }
        
        HighlightMap.SetHighlights(highlightSet);
    }

    public override void PlayerExecute(){
        InputHandler.SetActionMode(this);
        InputHandler.SetSprite(uISprite, uIColor);
    }

    public override void AIExecute(){
        throw new System.NotImplementedException();
    }

    public override int AssessPriority(){
        throw new System.NotImplementedException();
    }
}
