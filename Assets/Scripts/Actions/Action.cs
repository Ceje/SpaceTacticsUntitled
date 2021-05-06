using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class Action: MonoBehaviour
{

    public string actionName;
    public bool aiAccessible = true;
    public int cost;
    
    //common & ui
    public abstract bool CanUse();
    public abstract bool IsVisible(GameObject invoker);
    public abstract void Cancel();
    public virtual void Execute(){
        Debug.Log(Data.UnitTracker.GetActiveUnit().interactName + " performs " + actionName);
        if (Turns.GetCurrentTeam().IsAI()){
            AIExecute();
            return;
        }
        PlayerExecute();
    }
    
    //players
    public abstract void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform);
    public abstract void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform);
    public abstract void SetHighlight();
    public abstract void PlayerExecute();

    //ai
    public abstract void AIExecute();
    public abstract int AssessPriority();
}
