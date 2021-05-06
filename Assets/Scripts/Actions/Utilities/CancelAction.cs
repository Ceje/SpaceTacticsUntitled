using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelAction : Action
{

    // Start is called before the first frame update
    void Start()
    {
        actionName = "Cancel";
        aiAccessible = false;
    }

    // Update is called once per frame
    public override bool CanUse(){
        return true;
    }
    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }
    public override void Cancel(){
    }
    
    //PlayerScript
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
        Unit.DeselectUnit();
    }

    //AI script
    public override void AIExecute(){
        throw new System.NotImplementedException();
    }

    public override int AssessPriority(){
        throw new System.NotImplementedException();
    }
}
