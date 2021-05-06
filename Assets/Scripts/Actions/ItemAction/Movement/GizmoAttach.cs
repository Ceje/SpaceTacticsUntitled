using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoAttach : Attach
{
    private Gizmo _gizmo;

    public override void Start(){
        base.Start();
        _gizmo = GetComponent<Gizmo>();
    }

    public override bool IsVisible(GameObject invoker){
        return (invoker.name == "UnitTracker" && transform.parent != Data.board.transform) 
               || (invoker.name == "Inspector" && transform.parent == Data.board.transform);
    }
    
    public override bool CanUse(){
        return transform.parent == Data.board.transform || base.CanUse();
    }

    protected override void DoAttach(Interactable target){
        base.DoAttach(target);
        _gizmo.team = Turns.GetCurrentTeam().GetName();
        _gizmo.attached = target;
        Teams.GetManagerInstance().AddToTeam(_gizmo.team, target);
    }
}
