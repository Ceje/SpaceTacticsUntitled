using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public Action action;
    
    private Button _button;
    private Text _text;
    private Unit _unit;
    // Start is called before the first frame update
    void Start(){
        _button = GetComponent<Button>();
        _text = GetComponentInChildren<Text>();
    }

    public void Instantiate(Action newAction){
        action = newAction;
        if (_text == null){
            _text = GetComponentInChildren<Text>();
        }
        _text.text = newAction.actionName;
    }
    // Update is called once per frame
    void Update(){
        _button.interactable = action.CanUse();
    }

    public void OnMouseUp(){
        Execute();
    }

    public void Execute(){
        action.Execute();
    }
}
