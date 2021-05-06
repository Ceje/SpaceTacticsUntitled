using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadePickup : Pickup
{
    private BarricadeDeploy _deploy;

    public override void Start(){
        base.Start();
        _deploy = GetComponent<BarricadeDeploy>();
    }
    protected override void DoPickup(){
        base.DoPickup();
        _deploy.SetDeployed(false);
    }

    public override int AssessPriority(){
        if (_deploy.IsDeployed()){
            if (Data.UnitTracker.GetActiveUnit() is Gremlin){
                return 100;
            }

            return 0;
        }
        
        
        if (Data.UnitTracker.GetActiveUnit() is Magpie magpie && !magpie.AtNest()){
            return 100;
        }

        return 0;
    }
}
