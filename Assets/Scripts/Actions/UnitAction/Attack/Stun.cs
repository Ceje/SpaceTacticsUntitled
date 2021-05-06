using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stun : Action
{
    protected Unit _unit;
    public int duration;
    public Sprite uISprite;
    public Color uIColor;
    
    void Start(){
        actionName = "Stun";
        _unit = GetComponentInParent<Unit>();
    }
    public override bool CanUse(){
        return _unit.remainingActions > 0;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }

    public override void Cancel(){
        InputHandler.HideUi();
    }

    protected void DoStun(Unit enemy){
        enemy.ailments.Add(new Stunned(duration));
        enemy.Damaged(0);
        _unit.SpendActions(1);
        Unit.DeselectUnit();
        _unit.Attack();
    }
    
    //Player script
    

    public override void PlayerExecute(){
        InputHandler.SetSprite(uISprite, uIColor);
        InputHandler.SetActionMode(this);
    }

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var enemy = hit.GetValueOrDefault().collider.gameObject.GetComponent<Unit>();
            if (enemy != null && enemy.visible && !Teams.GetManagerInstance().ShareTeams(_unit, enemy)){
                if ((_unit.transform.position - enemy.transform.position).magnitude >= 1.1){
                    return;
                }

                targetingTransform.position = enemy.transform.position;
                sprite.enabled = true;
                return;
            }
        }

        sprite.enabled = false;
    }
    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var enemy = hit.Value.collider.gameObject.GetComponent<Unit>();
            if (enemy != null && sprite.enabled){
                DoStun(enemy);
            }
            sprite.enabled = false;
        }
    }
    
    public override void SetHighlight(){
        var highlights = new HashSet<GameObject>();
        var targetList = Data.board.GetNearby<Interactable>(Data.board.WorldToCell(transform.position), 1);
        foreach (var interactable in targetList){
            if (!Teams.GetManagerInstance().ShareTeams(_unit, interactable)){
                highlights.Add(interactable.gameObject);
            }
        }
        HighlightMap.SetHighlights(highlights);
    }
    
    //AI script
    public override void AIExecute(){
        List<Unit> nearbyUnits = Data.board.GetNearby<Unit>(Data.board.WorldToCell(transform.position), 1);
        nearbyUnits = FilterTargets(nearbyUnits);
        
        if (nearbyUnits.Count == 0){
            return;
        }
        
        var bestValue = nearbyUnits.Max(unit => unit.AssessPriority(unit.health/2));

        var bestTarget = nearbyUnits.Find(target => target.AssessPriority(target.health/2) == bestValue);
        Debug.Log("Stun " + bestTarget.name);
        DoStun(bestTarget);
    }

    public override int AssessPriority(){
        List<Unit> nearbyUnits = Data.board.GetNearby<Unit>(Data.board.WorldToCell(transform.position), 1);
        nearbyUnits = FilterTargets(nearbyUnits);

        if (nearbyUnits.Count == 0){
            return -1;
        }
        return nearbyUnits.Max(unit => unit.AssessPriority(unit.health/2));
    }

    public List<Unit> FilterTargets(List<Unit> targets){
        List<Unit> filteredTargets = new List<Unit>();
        targets.ForEach(target => {
            if (!Teams.GetManagerInstance().ShareTeams(_unit, target)){
                filteredTargets.Add(target);
            }
        });
        
        return filteredTargets;
    }
}
