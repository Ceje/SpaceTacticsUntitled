using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPanel : Obstacle
{
    public int force;
    private int registeredForce = 0;
    private Color _color;


    public override void Update(){
        base.Update();
        
        if (force == registeredForce){
            return;
        }

        if (force > 0){
            _color = Color.blue;
        } else if (force < 0){
            _color = Color.red;
        }else{
            _color = Color.white;
        }

        _spriteRenderer.color = _color;
        registeredForce = force;
    }
}
