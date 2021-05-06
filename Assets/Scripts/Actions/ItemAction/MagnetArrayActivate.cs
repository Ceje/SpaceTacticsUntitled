using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnetArrayActivate : Action
{
    private List<MagnetPanelActivate> _panels;

    private Vector3Int location;

    private bool instantiated = false;
    // Start is called before the first frame update
    void Start(){
        _panels = new List<MagnetPanelActivate>(GetComponentsInChildren<MagnetPanelActivate>());
        aiAccessible = false;
    }

    public void Update(){
        if (!instantiated){
            location = Data.board.WorldToCell(transform.position);
        }
        
        var thisTile = Data.board.WorldToCell(transform.position);
        if ( thisTile == location){
            return;
        }

        location = thisTile;
        Execute();

    }

    public override void Execute(){
        _panels.ForEach(panel => panel.Execute());
    }

    public override bool CanUse(){
        var withinRange = Turns.GetCurrentTeam().GetMembers()
            .Any(member => (member.transform.position - transform.position).magnitude < 1.1);
        return withinRange;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "Inspector";
    }
    
    public override int AssessPriority(){
        return _panels.Sum(panel => panel.AssessPriority());
    }
    
    
    //not used in immediate actions
    public override void Cancel(){
        //instant action
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
