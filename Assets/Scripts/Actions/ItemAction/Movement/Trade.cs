using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trade : Action
{
    private Item _item;
    public void Start(){
        _item = GetComponent<Item>();
    }

    public override bool IsVisible(GameObject invoker){
        var inInventory = _item.InInventory;
        var inInspector = invoker.name == Data.Inspector.name;
        return inInventory && inInspector;
    }
    public override bool CanUse(){
        var activeUnit = Data.UnitTracker.GetActiveUnit();
        if (activeUnit is null){
            return false;
        }
        var inInventory = _item.InInventory;
        var inTradeRange = (activeUnit.transform.position - transform.position).magnitude < 1.2;
        var actorCanAct = activeUnit.remainingActions > 0;
        return inInventory && inTradeRange && actorCanAct;
    }

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
        var actor = Data.UnitTracker.GetActiveUnit();
        var holder = transform.parent.GetComponent<IInventory>();
        
        DoTrade(actor, holder);
        Data.Inspector.ClearInspected();
    }

    private void DoTrade(Unit actor, IInventory holder){
        Transform transform1 = transform;
        transform1.SetParent(actor.gameObject.transform);
        transform1.position = actor.transform.position;
        actor.AddItem(_item);
        holder.RemoveItem(_item);
    }

    public override void AIExecute(){
        var magpie = Data.UnitTracker.GetActiveUnit() as Magpie;
        if (magpie is null || !magpie.AtNest()){
            return;
        }
        
        DoTrade(magpie.nest, magpie);
    }

    public override int AssessPriority(){
        var magpie = Data.UnitTracker.GetActiveUnit() as Magpie;
        if (magpie is null || !magpie.AtNest()){
            return 0;
        }

        return 100;
    }
}
