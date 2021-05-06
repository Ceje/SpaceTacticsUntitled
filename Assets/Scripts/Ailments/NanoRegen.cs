using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoRegen : Ailment
{
    public NanoRegen(){
        Name = "NanoRegen";
        Stacks = 1;
    }
    public override void Recovery(Unit unit){
        unit.health += Stacks;
    }
}
