using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : MonoBehaviour
{
    private static Interactable _inspected;

    private Text _interactionName;

    private Canvas _parentCanvas;

    private ActionList _interactionList;
    // Start is called before the first frame update
    void Start(){
        Data.Inspector = this;
        _parentCanvas = GetComponentInParent<Canvas>();
        _parentCanvas.enabled = false;
        _interactionName = GetComponentInChildren<Text>();
        _interactionList = GetComponentInChildren<ActionList>();
    }

    // Update is called once per frame
    void Update(){
        if (_inspected is null){
            return;
        }

        _interactionName.text = _inspected.interactName;
    }

    public void SetInspected([CanBeNull] Interactable inspected){
        ClearInspected();

        if (inspected is null){
            return;
        } 
        _inspected = inspected;
        _parentCanvas.enabled = true;
        _interactionList.InstantiateActionList(new List<Action>(inspected.GetComponentsInChildren<Action>()), gameObject);
    }

    public void ClearInspected(){
        _parentCanvas.enabled = false;
        _interactionList.ClearActionList();
        _inspected = null;
    }

    public static bool IsBeingInspected(Interactable interactable){
        return _inspected == interactable;
    }

    public static bool IsInspecting(){
        return !(_inspected is null);
    }
}
