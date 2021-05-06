using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : Action
{
    private Item _item;

    public virtual void Start(){
        _item = GetComponent<Item>();
        actionName = "Remove " + _item.itemName;
    }

    public override void Execute(){
        var unit = Data.UnitTracker.GetActiveUnit();
        Transform transform1 = transform;

        transform1.SetParent(unit.gameObject.transform);
        transform1.position = unit.transform.position;
        Debug.Log(unit.interactName + " removes " + _item.interactName);
        unit.AddItem(_item);
        _item.InInventory = true;
        Data.AttachedItems.Remove(_item);
        Data.Inspector.ClearInspected();
    }

    public override bool CanUse(){
        var activeUnit = Data.UnitTracker.GetActiveUnit();
        if (activeUnit is null){
            return false;
        }

        return (activeUnit.transform.position - transform.position).magnitude < 1.1
               && transform.parent != Data.board.transform
               && !_item.InInventory
               && activeUnit.remainingActions > 0;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "Inspector" && transform.parent != Data.board.transform && !_item.InInventory;
    }

    public override int AssessPriority(){
        if (Data.UnitTracker.GetActiveUnit() is Gremlin){
            return 100;
        }

        return 0;
    }
    
    
    //unused on immediate actions
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
        throw new System.NotImplementedException();
    }

    public override void PlayerExecute(){
        throw new System.NotImplementedException();
    }

    public override void AIExecute(){
        throw new System.NotImplementedException();
    }
}