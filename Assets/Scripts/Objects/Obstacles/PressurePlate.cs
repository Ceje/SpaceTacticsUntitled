using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Obstacle
{
    public List<Action> targets;
    public bool doubleAction;
    
    private readonly Color _pressedColor = Color.blue;
    private readonly Color _releasedColor = Color.cyan;
    public void OnTriggerEnter(Collider other){
        var interact = other.gameObject.GetComponent<Interactable>();
        if (interact == null || !interact.blocksMovement){
            return;
        }
        targets.ForEach(action => action.Execute());
        _spriteRenderer.color = _pressedColor;
    }

    public void OnTriggerExit(Collider other){
        if (!doubleAction){
            return;
        }

        var interact = other.gameObject.GetComponent<Interactable>();
        if (interact == null || !interact.blocksMovement){
            return;
        }

        
        targets.ForEach(action => action.Execute());
        _spriteRenderer.color = _releasedColor;
    }
}
