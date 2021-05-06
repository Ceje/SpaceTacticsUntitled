using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventory
{
    Dictionary<Item, int> Items{ get; set; }
    int MaxItems{ get; set; }
    
    void AddItem(Item item);
    void RemoveItem(Item item);
    int CountItem(Item item);
}
