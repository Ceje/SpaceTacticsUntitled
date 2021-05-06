using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoRemove : Remove
{
    private Gizmo _gizmo;
    
    public override void Start(){
        base.Start();
        _gizmo = GetComponent<Gizmo>();
    }

    public override void Execute(){
        base.Execute();
        Teams.GetManagerInstance().RemoveFromTeam(_gizmo.team, _gizmo.attached);
    }
}
