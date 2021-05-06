using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    // Start is called before the first frame update
    protected override void RegisterToTeam(){
        Teams.GetManagerInstance().AddToAITeam(team, this);
    }
    
    public override void Damaged(int damage){
        health -= damage;
        _animator.SetTrigger(Hurt);
    }
}
