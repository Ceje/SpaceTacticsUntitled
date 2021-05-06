using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Hazard
{
    public GameObject bullet;

    public override void TriggerEffect(Interactable interactable){
        if (interactable is Unit unit && !Teams.GetManagerInstance().ShareTeams(this, unit)){
            var bulletObject = Instantiate(bullet, transform);
            var dodged = unit.RespondToTargeted();
            var shot = bulletObject.GetComponent<BasicShot>();
            if (dodged.HasValue){
                shot.SetTarget(dodged.Value);
            }
            else{
                shot.SetTarget(unit);
            }
        }
    }
}
