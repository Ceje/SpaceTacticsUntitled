using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    private Unit _unit;

    public Sprite uiSprite;
    public Color uiColor;
    public GameObject bullet;

    void Start(){
        actionName = "Attack";
        _unit = GetComponent<Unit>();
        aiAccessible = false;
    }

    private void DoAttack(Unit enemy){
        _unit.SpendActions(1);
        Unit.DeselectUnit();
        var bulletObject = Instantiate(bullet, _unit.transform);
        bulletObject.GetComponent<BasicShot>().SetTarget(enemy);
        _unit.Attack();
    }

    //UI & Generic functions
    public override bool IsVisible(GameObject invoker){
        return invoker.name == "UnitTracker";
    }

    public override void Cancel(){
//        InputHandler.HideUi();
    }
    public override bool CanUse(){
        return _unit.remainingActions > 0;
    }

    //Player Functions
    public override void SetHighlight(){
        throw new System.NotImplementedException();
    }

    public override void PlayerExecute(){
        InputHandler.SetActionMode(this);
        InputHandler.SetSprite(uiSprite, uiColor);
    }
    public override void OnClick(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            sprite.enabled = false;
            var enemy = hit.Value.collider.gameObject.GetComponent<Unit>();
            if (enemy != null && enemy.visible){
                DoAttack(enemy);
            }
        }
    }
    public override void OnHover(SpriteRenderer sprite, LineRenderer line, Transform targetingTransform){
        RaycastHit? hit = InputHandler.CursorRaycast();
        if (hit.HasValue){
            var enemy = hit.GetValueOrDefault().collider.gameObject.GetComponent<Unit>();
            if (enemy != null && enemy.visible && !Teams.GetManagerInstance().ShareTeams(_unit, enemy)){
                targetingTransform.position = enemy.transform.position;
                sprite.enabled = true;
                return;
            }
        }

        sprite.enabled = false;
    }
    
    //AI Functions
    public override void AIExecute(){
        throw new System.NotImplementedException();
    }

    public override int AssessPriority(){
        throw new System.NotImplementedException();
    }
}