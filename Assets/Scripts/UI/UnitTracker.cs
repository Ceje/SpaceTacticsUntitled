using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UnitTracker : MonoBehaviour
{
    private static Unit _unit;

    private Text _healthValue;

    private Text _visionValue;
    
    private Text _actionValue;

    private Text _moveValue;

    private Canvas _parentCanvas;

    private ActionList _unitActionList;
    private InventoryPanel _inventoryPanel;
    public GameObject globalActionListPrefab;
    public GameObject inventoryPanelPrefab;
    
    // Start is called before the first frame update
    void Start(){
        Data.UnitTracker = this;
        _parentCanvas = GetComponentInParent<Canvas>();
        _unitActionList = GetComponentInChildren<ActionList>();
        _parentCanvas.enabled = false;
        foreach (var text in GetComponentsInChildren<Text>()){
            switch (text.name){
                case "MovesValue":
                    _moveValue = text;
                    break;
                case "ActionsValue":
                    _actionValue = text;
                    break;
                case "HealthValue":
                    _healthValue = text;
                    break;
                case "VisionValue":
                    _visionValue = text;
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update(){
        if (_unit is null){
            return;
        }
        _actionValue.text = _unit.actionLimit.ToString();
        _moveValue.text = _unit.remainingMovement.ToString();
        _visionValue.text = _unit.vision.ToString();
        _healthValue.text = _unit.health.ToString();

        if (!(_inventoryPanel is null) && _inventoryPanel.transform.childCount == 0){
            Destroy(_inventoryPanel.gameObject);
            _inventoryPanel = null;
        }
    }

    public void SetUnit([CanBeNull] Unit newUnit){
        if (newUnit is null){
            ClearUnit();
            return;
        } 
        _unit = newUnit;
        _parentCanvas.enabled = !Turns.GetCurrentTeam().IsAI();
    }

    public void ToggleActionList(){
        if (_unitActionList is null){
            if (_unit.actions.Count == 0){
                return;
            }
            var gal = Instantiate(globalActionListPrefab, transform);
            gal.transform.SetAsFirstSibling();
            _unitActionList = gal.GetComponentInChildren<ActionList>();
            _unitActionList.InstantiateActionList(_unit.actions.ToList(), gameObject);
        }
        else{
            Destroy(_unitActionList.gameObject);
            _unitActionList = null;
        }
    }

    public void RefreshActionList(){
        if (_unitActionList is null){
            return;
        }
        if (_unit.actions.Count == 0){
            Destroy(_unitActionList.gameObject);
            _unitActionList = null;
        }
        _unitActionList.InstantiateActionList(_unit.actions.ToList(), gameObject);
    }

    public void ToggleInventoryList(){
        if (_inventoryPanel is null){
            if (_unit.Items.Count == 0){
                return;
            }
            var inventoryPanel = Instantiate(this.inventoryPanelPrefab, transform);
            inventoryPanel.transform.SetAsFirstSibling();
            _inventoryPanel = inventoryPanel.GetComponentInChildren<InventoryPanel>();
            _inventoryPanel.InstantiateItemList(_unit);
        }
        else{
            Destroy(_inventoryPanel.gameObject);
            _inventoryPanel = null;
        }
    }

    public void RefreshInventoryList(){
        if (_inventoryPanel is null){
            return;
        }
        if (_unit.Items.Count == 0){
            Destroy(_inventoryPanel.gameObject);
            _inventoryPanel = null;
            return;
        }
        _inventoryPanel.InstantiateItemList(_unit);
    }

    public void ClearUnit(){
        _parentCanvas.enabled = false;    
        _unit = null;
    }

    public static bool IsActiveUnit(Unit unit){
        return _unit == unit;
    }

    public static bool IsTracking(){
        return !(_unit is null);
    }

    public Unit GetActiveUnit(){
        return _unit;
    }
    
    
}
