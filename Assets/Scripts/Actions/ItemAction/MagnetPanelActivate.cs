using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore;

public class MagnetPanelActivate : Action
{
    private MagnetPanel _panel;
    private Dictionary<Interactable, Propulsion> activePropulsions = new Dictionary<Interactable, Propulsion>();
    void Start(){
        _panel = GetComponent<MagnetPanel>();
    }

    void Update()
    {
    }
    


    public override void Execute(){
        var facing = transform.rotation;
        var quaternion = new Quaternion(0,0, facing.z, facing.w);
        List<Interactable> targets = 
            Data.board.GetOnLine<Interactable>(transform.position, quaternion, false, false);
        targets.ForEach(target => {
            if (!target.magnetic || (activePropulsions.ContainsKey(target) && !(activePropulsions[target] == null))){
                return;
            }

            var currentTile = Data.board.WorldToCell(target.transform.position);
            var targetTile = Data.board.GetTileByOrientation(currentTile, _panel.force, quaternion);
            Propulsion propulsion = target.gameObject.AddComponent<Propulsion>();
            propulsion.Configure(target, targetTile);
            propulsion.StartMovement();
            activePropulsions[target] = propulsion;
        });
    }

    public override bool CanUse(){
        var withinRange = Turns.GetCurrentTeam().GetMembers()
            .Any(member => (member.transform.position - transform.position).magnitude < 1.1);
        return withinRange;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "Inspector" && transform.parent == Data.board.transform;
    }
    
    public override int AssessPriority(){
        if (!(Data.UnitTracker.GetActiveUnit() is Gremlin)){
            return 0;
        }
        
        var facing = transform.rotation;
        var quaternion = new Quaternion(0,0, facing.z, facing.w);
        List<Interactable> targets = 
            Data.board.GetOnLine<Interactable>(transform.position, quaternion, false, false);
        if (targets.Count > 0){
            return 10;
        }

        return 0;
    }
    
    
    //not used in immediate actions
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
        throw new NotImplementedException();
    }

    public override void PlayerExecute(){
        throw new System.NotImplementedException();
    }

    public override void AIExecute(){
        throw new System.NotImplementedException();
    }
}
