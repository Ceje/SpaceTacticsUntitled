using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
    }

    private HashSet<Unit> _sortedUnits = new HashSet<Unit>();

    public void Update(){
        var currentTeam = Turns.GetCurrentTeam();
        var temp = new HashSet<Unit>();
        foreach (var interactable in currentTeam.GetMembers()){
            var unit = interactable.GetComponent<Unit>();
            if (unit is null){
                continue;
            }

            temp.Add(unit);
        }

        if (!currentTeam.IsAI() || !GlobalGrid.Initialized || temp.Any(unit => unit.IsBusy())){
            return;
        }


        _sortedUnits = temp;

        if (_sortedUnits.All(UnitCannotAct)){
            Turns.NextTurn();
            return;
        }

        Action selectedAction = null;
        Unit actingUnit = null;
        int actionValue = 0;
        foreach (var sortedUnit in _sortedUnits){
            if (UnitCannotAct(sortedUnit)){
                continue;
            }
            Data.UnitTracker.SetUnit(sortedUnit);
            var availableActions = GetNearbyItemActions(sortedUnit);
            //might not need to recollect the action components from the unit each time, attached & inventory actions would be the worry
            sortedUnit.RefreshActions();
            availableActions.UnionWith(sortedUnit.actions);
            var bestUnitAction = BestAction(availableActions);
            if (bestUnitAction is null){
                continue;
            }

            var newActionValue = bestUnitAction.AssessPriority();
            if (newActionValue > actionValue && bestUnitAction.CanUse()){
                actingUnit = sortedUnit;
                selectedAction = bestUnitAction;
                actionValue = newActionValue;
            }
        }
        
        if (selectedAction is null){
            Turns.NextTurn();
            return;
        }

        Data.UnitTracker.SetUnit(actingUnit);
        selectedAction.Execute();
        actingUnit.remainingActions -= selectedAction.cost;
    }

    private HashSet<Action> GetNearbyItemActions(Unit sortedUnit){
        var actions = new HashSet<Action>();
        var items = new List<Item>();
        items = Data.board.GetNearby<Item>(Data.board.WorldToCell(sortedUnit.transform.position), 1);
        foreach (var item in items){
            var itemActions = item.GetComponents<Action>();
            foreach (var itemAction in itemActions){
                if (itemAction.aiAccessible && itemAction.CanUse() && itemAction.IsVisible(Data.Inspector.gameObject)){
                    actions.Add(itemAction);
                }
            }
        }

        return actions;
    }

    private bool UnitCannotAct(Unit unit){
        var actionsRestricted = !Teams.GetManagerInstance().GetTeam("Player").GetMembers()
            .Any(player => Data.board.InSameZone(player, unit));
        return actionsRestricted || unit.IsBusy() || !unit.actions.Any(action => { return action.CanUse() && action.aiAccessible; });
    }

    [CanBeNull]
    public Action BestAction(HashSet<Action> actions){
        Action bestAction = null;
        int actionValue = int.MinValue;
        foreach (var action in actions){
            if (!(action.aiAccessible)){
                continue;
            }

            var newActionValue = action.AssessPriority();
            if (newActionValue > actionValue && action.CanUse()){
                bestAction = action;
                actionValue = newActionValue;
            }
        }
        
        return bestAction;
    }
}