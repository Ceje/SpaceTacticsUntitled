using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    private Canvas _canvas;
    private Button _button;
    private Text _text;
    void Start(){
        Data.EndTurnButton = this;
        _canvas = GetComponentInParent<Canvas>();
        _button = GetComponent<Button>();
        _text = transform.parent.GetChild(1).GetComponent<Text>();

    }


    public void EndTurn(){
        Unit.DeselectUnit();
        Turns.NextTurn();
    }

    public void Configure(Teams.TeamManager.Team nextTeam){
        _button.interactable = !nextTeam.IsAI();
        _text.text = "Waiting For " + nextTeam.GetName();
    }
    
}
