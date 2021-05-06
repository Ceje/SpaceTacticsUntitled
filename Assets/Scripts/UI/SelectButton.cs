using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    private ObjectSelectionList _parent;
    private Interactable _item;
    public void Configure(Interactable item, ObjectSelectionList parent){
        _item = item;
        GetComponentInChildren<Text>().text = item.interactName;
        _parent = parent;
    }

    public void Select(){
        if (_item == null){
            return;
        }
        
        if (_item is Unit unit && Teams.GetManagerInstance().GetUnitTeams(unit).Contains(Turns.GetCurrentTeam())){
            Data.UnitTracker.SetUnit(unit);
            _parent.Close();
            return;
        }

        Data.Inspector.SetInspected(_item);
        _parent.Close();
    }
}
