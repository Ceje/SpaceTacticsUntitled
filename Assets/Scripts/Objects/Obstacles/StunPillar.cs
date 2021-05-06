using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunPillar : Hazard
{
    public override void TriggerEffect(Interactable unit){
        (unit as Unit)?.ailments.Add(new Stunned(1));
    }
}
