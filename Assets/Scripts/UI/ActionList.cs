using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class ActionList : MonoBehaviour
{
    public GameObject actionButton;
    // Start is called before the first frame update
    List<Action> _actions = new List<Action>();
    void Start(){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateActionList(List<Action> actions, GameObject parent){
        foreach (var action in actions){
            if (!action.IsVisible(parent) && !(action.GetType() == typeof(Action))){
                continue;
            }
            _actions.Add(action);
            var button = Instantiate(actionButton, transform);
            button.GetComponent<ActionButton>().Instantiate(action);
        }
    }

    public void ClearActionList(){
        foreach (var action in GetComponentsInChildren<ActionButton>()){
            Destroy(action.gameObject);
        }
    }
}
