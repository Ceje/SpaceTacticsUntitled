using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Action
{
    private Item _item;

    public virtual void Start(){
        _item = GetComponent<Item>();
    }

    protected virtual void DoPickup(){
        var unit = Data.UnitTracker.GetActiveUnit();

        Transform transform1 = transform;
        transform1.SetParent(unit.gameObject.transform);
        transform1.position = unit.transform.position;
        unit.AddItem(_item);
        _item.InInventory = true;
    }

    public override bool CanUse(){
        var activeUnit = Data.UnitTracker.GetActiveUnit();
        if (activeUnit is null){
            return false;
        }
        return !_item.InInventory && (activeUnit.transform.position - transform.position).magnitude < 1
               && activeUnit.remainingActions > 0;    
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "Inspector" && transform.parent == Data.board.transform;
    }

    

    public override int AssessPriority(){
        if (Data.UnitTracker.GetActiveUnit() is Magpie magpie && !magpie.AtNest()){
            return 100;
        }

        return 0;
    }
    
    
    
    //unused in normal operations
    public override void Cancel(){
        throw new System.NotImplementedException();
    }

    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        throw new System.NotImplementedException();
    }

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        throw new System.NotImplementedException();
    }

    public override void SetHighlight(){
        throw new NotImplementedException();
    }

    public override void PlayerExecute(){
        DoPickup();
    }

    public override void AIExecute(){
        DoPickup();
    }
}
