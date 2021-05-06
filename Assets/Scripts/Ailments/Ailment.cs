using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ailment
{
    public abstract void Recovery(Unit unit);
    public string Name;
    public int Stacks = 0;
}

public class Test : Ailment
{
    public Test(int stacks){
        Stacks = stacks;
    }   
    public override void Recovery(Unit unit){
        Stacks--;
    }
}
