using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Interactable
{
    private Collider _collider;
    private bool _inInventory = false;
    private bool _initialized = false;

    public bool InInventory{
        get{ return _inInventory; }
        set{
            _inInventory = value;
            _collider.enabled = Data.board.transform == transform.parent;
            _spriteRenderer.enabled = !value;
            if (Inspector.IsBeingInspected(this)){
                Data.Inspector.ClearInspected();
            }

            if (_inInventory){
                Data.LooseItems.Remove(this);
            }
            else{
                Data.LooseItems.Add(this);
                
            }

        }
    }

    public string itemName;

    public override void Start(){
        base.Start();
        _collider = GetComponent<Collider>();
    }

    public override void Update(){
        if (!_initialized){
            if (transform.parent == Data.board.transform){
                InInventory = false;
            }else{
                InInventory = true;
            }

            _initialized = true;
        }

        if (!InInventory){
            base.Update();
        }

    }
}