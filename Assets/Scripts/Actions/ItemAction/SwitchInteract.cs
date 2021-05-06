using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwitchInteract : Action
{
    public List<Action> targets;
    private AudioSource _audioSource;

    public void Start(){
        _audioSource = GetComponent<AudioSource>();
    }

    public override void Execute(){
        Debug.Log(Data.UnitTracker.GetActiveUnit().interactName = " toggles switch");
        if (_audioSource != null){
            _audioSource.Play();
        }
        targets.ForEach(target => target.Execute());
    }

    public override bool CanUse(){
        var withinRange = Turns.GetCurrentTeam().GetMembers()
            .Any(member => (member.transform.position - transform.position).magnitude < 1.1);
        return withinRange;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "Inspector";
    }
    
    public override int AssessPriority(){
        return targets.Sum(action => action.AssessPriority());
    }

    //dont use in immediate actions
    public override void Cancel(){
        throw new System.NotImplementedException();
    }

    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        throw new System.NotImplementedException();
    }

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        throw new System.NotImplementedException();
    }

    public override void SetHighlight(){
        throw new System.NotImplementedException();
    }

    public override void PlayerExecute(){
        throw new System.NotImplementedException();
    }

    public override void AIExecute(){
        throw new System.NotImplementedException();
    }

    
}
