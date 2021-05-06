using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntry : MonoBehaviour
{
    public Item item;
    public IInventory unit;
    public IInventory inventory;
    public GameObject actionList;
    private UnitTracker _parent;
    
    private Text _itemName;
    private Text _itemCount;
    private ActionList _actionList;
    private List<Action> _actions;
    public void Start(){
        foreach (Text component in GetComponentsInChildren<Text>()) {
            switch (component.gameObject.name) {
                case "ItemName":
                    _itemName = component;
                    break;
                case "ItemCount":
                    _itemCount = component;
                    break;
            }
        }
        _actions = new List<Action>(item.GetComponents<Action>());
        _parent = GetComponentInParent<UnitTracker>();
    }

    public void Update(){
        if (!unit.Items.ContainsKey(item) || unit.Items[item] <= 0){
            Destroy(gameObject);
            return;
        }
        
        _itemCount.text = "x"+ unit.Items[item];
        _itemName.text = item.itemName;

        
        if (_actionList is null && Data.SelectedItem == this && _actions.Count > 0){
            _actionList = Instantiate(actionList, transform).GetComponent<ActionList>();
            _actionList.InstantiateActionList(_actions, _parent.gameObject);
        } 
        else if (Data.SelectedItem != this && !(_actionList is null)){
            Destroy(_actionList.gameObject);
            _actionList = null;
        }
    }

    public void OnClick(){
        if (Data.SelectedItem != this){
            Data.SelectedItem = this;
            return;
        }

        Data.SelectedItem = null;
    }
}
