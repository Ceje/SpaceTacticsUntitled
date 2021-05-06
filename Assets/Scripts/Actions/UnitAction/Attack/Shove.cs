using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shove : Action
{
    public int force;
    public float shoveSpeed = 0.1f;

    public Sprite uISprite;
    public Color uIColor;
    
    // Start is called before the first frame update
    private Unit _unit;

    // Start is called before the first frame update
    void Start(){
        actionName = "Shove";
        _unit = GetComponentInParent<Unit>();
    }

    protected void DoShove(Unit target){
        var enemyPosition = target.transform.position;
        var enemyTile = Data.board.WorldToCell(enemyPosition);
        
        Vector3 difference = enemyPosition - _unit.transform.position;
        var exact = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        Vector3Int tile = Data.board.GetTileByOrientation(enemyTile, force, exact);

        var propulsion = target.gameObject.AddComponent<Propulsion>();
        propulsion.Configure(target, tile, shoveSpeed);
        propulsion.StartMovement();
        
        target.Damaged(0);
    }

    

    public override bool CanUse(){
        List<Unit> nearbyUnits = Data.board.GetNearby<Unit>(Data.board.WorldToCell(transform.position), 1);
        nearbyUnits = FilterTargets(nearbyUnits);
        return _unit.remainingActions > 0 && nearbyUnits.Count > 0;
    }

    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }

    public override void Cancel(){
        InputHandler.HideUi();
    }
    //Player script
    public override void PlayerExecute(){
        InputHandler.SetSprite(uISprite, uIColor);
        InputHandler.SetActionMode(this);
    }

    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var enemy = hit.Value.collider.gameObject.GetComponent<Unit>();
            if (enemy != null && sprite.enabled){
                DoShove(enemy);
            }
            sprite.enabled = false;
        }
    }

    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var enemy = hit.GetValueOrDefault().collider.gameObject.GetComponent<Unit>();
            if (enemy != null && enemy.visible && !Teams.GetManagerInstance().ShareTeams(_unit, enemy)){
                if ((_unit.transform.position - enemy.transform.position).magnitude >= 2){
                    return;
                }

                targetingTransform.position = enemy.transform.position;
                sprite.enabled = true;
                return;
            }
        }

        sprite.enabled = false;
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
        
        var bestValue = nearbyUnits.Max(unit => unit.AssessPriority(force));

        var bestTarget = nearbyUnits.Find(target => target.AssessPriority(force) == bestValue);
        DoShove(bestTarget);
    }

    
    public override int AssessPriority(){
        List<Unit> nearbyUnits = Data.board.GetNearby<Unit>(Data.board.WorldToCell(transform.position), 1);
        nearbyUnits = FilterTargets(nearbyUnits);

        if (nearbyUnits.Count == 0){
            return -1;
        }
        return nearbyUnits.Max(unit => unit.AssessPriority(force));
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
