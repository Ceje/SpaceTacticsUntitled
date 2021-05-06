using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : Ailment
{

    public Stunned(int duration){
        Name = "Stunned";
        Stacks = duration;
    }

    public override void Recovery(Unit unit){
        Stacks--;
    }
}
