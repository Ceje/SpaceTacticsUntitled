using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadePlace : PlaceItem
{
    private BarricadeDeploy _deploy;

    public override void Start(){
        base.Start();
        _deploy = GetComponent<BarricadeDeploy>();
    }
    
    public override void DoPlace(Vector3Int location){
        base.DoPlace(location);
        if (Turns.GetCurrentTeam().IsAI()){
            return;
        }
        _deploy.SetDeployed(true);    
    }
}
