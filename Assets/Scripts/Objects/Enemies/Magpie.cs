using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magpie : Enemy
{
    public MagpieNest nest;

    public bool AtNest(){
        return (transform.position - nest.transform.position).magnitude < 1.5;
    }
}
