using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public GameObject itemEntry;

    public void InstantiateItemList(IInventory unit){
        ClearActionList();
        foreach (KeyValuePair<Item, int> item in unit.Items){
            var button = Instantiate(itemEntry, transform);
            var script = button.GetComponent<ItemEntry>();
            script.item = item.Key;
            script.unit = unit;
        }
    }

    public void ClearActionList(){
        foreach (var action in GetComponentsInChildren<ActionButton>()){
            Destroy(action.gameObject);
        }
    }

}
