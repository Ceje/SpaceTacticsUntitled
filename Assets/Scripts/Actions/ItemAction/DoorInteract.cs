using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorInteract : Action
{
    private Door _door;

    public void Start(){
        _door = GetComponent<Door>();
    }

    public override void Execute(){
        _door.ToggleState();
        var open = _door.IsOpen() ? "open" : "close";
        Debug.Log(Data.UnitTracker.GetActiveUnit() + " causes door to " + open); 
    }

    public override bool CanUse(){
        var withinRange = Turns.GetCurrentTeam().GetMembers()
            .Any(member => (member.transform.position - transform.position).magnitude < 1.1);
        return withinRange && _door.unlocked;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "Inspector";
    }

   public override int AssessPriority(){
       if (_door.IsOpen() && Data.UnitTracker.GetActiveUnit() is Gremlin){
           return 100;
       }

       return 0;
   }
   
   
   
   //not used in immediate actions
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
       throw new System.NotImplementedException();
   }

   public override void AIExecute(){
       throw new System.NotImplementedException();
   }
}
