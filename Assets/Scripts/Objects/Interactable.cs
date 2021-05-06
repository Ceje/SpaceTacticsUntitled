using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool blocksVision;
    public bool blocksMovement;
    public bool visible;
    public bool magnetic = false;
    public string interactName;
    public int weight;

    protected SpriteRenderer _spriteRenderer;

    public virtual void Start(){
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void OnDestroy(){
        if (!gameObject.scene.isLoaded){
            return;
        }
        
        if (Inspector.IsBeingInspected(this)){
            Data.Inspector.ClearInspected();
        }

        if (blocksMovement){
            Data.board.UpdatePathingMap(new List<Vector3Int>{Data.board.WorldToCell(transform.position)});
        }

        if (blocksVision){
            FogMap.ScheduleFogUpdate();
        }

        Data.board.GetTile(Data.board.WorldToCell(transform.position)).RemoveInteractable(this);
    }
    
    public virtual void Update()
    {
        if (!Data.fog.inFog(transform.position) && !Turns.GetCurrentTeam().IsAI()){
            visible = true;
            _spriteRenderer.enabled = true;
        }else if(!Turns.GetCurrentTeam().IsAI()){
            _spriteRenderer.enabled = false;
            visible = false;
        }
    }
}
