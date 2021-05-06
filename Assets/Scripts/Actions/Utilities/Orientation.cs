 using System;
 using System.Collections;
using System.Collections.Generic;
 using Unity.Mathematics;
 using UnityEngine;

public class Orientation : Action
{
    private Unit _unit;
    private Grid _grid;

    private float _oldFacing;
    void Start(){
        _grid = gameObject.GetComponentInParent<Grid>();
        _unit = gameObject.GetComponentInParent<Unit>();
        aiAccessible = false;
        actionName = "Adjust Unit";
    }

    //Action Content
    void Update(){
        transform.rotation = _grid.transform.rotation;
        transform.Rotate(Vector3.forward, _unit.facing);
    }

    //Common & UI
    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }
    public override bool CanUse(){
        return true;
    }

    public override void Cancel(){
        _unit.AdjustFacing(_oldFacing);
    }
    
    //Player script
    public override void SetHighlight(){
        //pass
    }

    public override void PlayerExecute(){
        _oldFacing = _unit.facing + 0;
        InputHandler.SetActionMode(this);
    }

    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        _oldFacing = _unit.facing;
        Unit.DeselectUnit();
    }

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? cast = InputHandler.CursorRaycast();
        if (cast.HasValue){

            Vector3 difference = cast.Value.point - transform.position;
            var exact = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            _unit.AdjustFacing((float) Math.Round(exact/60) * 60);
        }
    }

    //AIScript
    public override void AIExecute(){
        throw new NotImplementedException();
    }

    public override int AssessPriority(){
        throw new NotImplementedException();
    }
}
