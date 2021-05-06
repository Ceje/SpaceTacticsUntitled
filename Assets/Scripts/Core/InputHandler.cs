using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Sprite unitSelectSprite;
    public GameObject SelectionList;
    private static Action _mode;
    private static ObjectSelectionList _list;
    private static LineRenderer _lineRenderer;
    private static SpriteRenderer _spriteRenderer;

    private void Start(){
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.enabled = false;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    private void Update(){
        if (_mode is null && (_spriteRenderer.sprite != unitSelectSprite || _spriteRenderer.color != Color.blue)){
            _spriteRenderer.sprite = unitSelectSprite;
            _spriteRenderer.color = Color.blue;
        }
    }

    public void OnPause(){
        if (!Data.PauseButton.isOpen()){
            Data.PauseButton.OpenPauseMenu();
        }
        else{
            Data.PauseButton.ClosePauseMenu();
        }
    }
    public void OnHover(){
        if (_mode is null){
            UnitSelectHover();
            return;
        }

        _mode.OnHover(_spriteRenderer, _lineRenderer, transform);
    }

    private void UnitSelectHover(){
        RaycastHit? hit = CursorRaycast();
        if (hit.HasValue && !Data.fog.inFog(Data.board.WorldToCell(hit.Value.point))){
            var unit = hit.GetValueOrDefault().collider.gameObject.GetComponent<Interactable>();
            if (unit != null){
                transform.position = unit.transform.position;
                _spriteRenderer.enabled = true;
                return;
            }
        }

        _spriteRenderer.enabled = false;
    }

    public void OnClick(){
        if (_mode is null){
            UnitSelectClick();
            return;
        }

        _mode.OnClick(_spriteRenderer, _lineRenderer, transform);
    }

    private void UnitSelectClick(){
        RaycastHit? hit = CursorRaycast();
        if (hit.HasValue && !Data.fog.inFog(Data.board.WorldToCell(hit.Value.point))){
            var tile = hit.Value.collider.gameObject.GetComponent<TileCollider>();
            if (tile != null && tile.ObjectsOnTile().Count > 0){
                if (_list != null){
                    _list.Close();
                }
                var list = Instantiate(SelectionList);
                _list = list.GetComponent<ObjectSelectionList>();
                _list.Configure(tile, _lineRenderer);
            }
            
            var interactable = hit.Value.collider.gameObject.GetComponent<Interactable>();

            if (interactable == null || !interactable.visible){
                return;
            }
            
            if (interactable is Unit unit && Teams.GetManagerInstance().GetUnitTeams(unit).Contains(Turns.GetCurrentTeam())){
                Data.UnitTracker.SetUnit(unit);
                return;
            }

            Data.Inspector.SetInspected(interactable);
        }
    }

    public void OnAltClick(){
        if (_list != null){
            _list.Close();
            return;
        }
        
        if (Inspector.IsInspecting()){
            Data.Inspector.ClearInspected();
            return;
        }

        if (UnitTracker.IsTracking()){
            Data.UnitTracker.ClearUnit();
        }
    }

    public static RaycastHit? CursorRaycast(){
        if (EventSystem.current.IsPointerOverGameObject()){
            return null;
        }

        var mousePosition = Mouse.current.position;

        var camera = Camera.main;
        if (!(camera is null)){
            var ray = camera.ScreenPointToRay(new Vector3(mousePosition.x.ReadValue(), mousePosition.y.ReadValue(), 0));
            if (Physics.Raycast(ray, out var hit)){
                return hit;
            }
        }

        return null;
    }

    public static void ClearActionMode(){
        _mode = null;
        var unitSelection = new HashSet<GameObject>();
        if (!Turns.GetCurrentTeam().IsAI()){
            foreach (var interactable in Turns.GetCurrentTeam().GetMembers()){
                if (interactable is Unit unit && (unit.remainingActions > 0 || unit.remainingMovement > 0)){
                    unitSelection.Add(unit.gameObject);
                }
            }    
        }
        HighlightMap.SetHighlights(unitSelection);
    }

    public static void SetActionMode(Action action){
        _mode = action;
        action.SetHighlight();
    }

    public static void CancelAction(){
        if (_mode is null){
            return;
        }
        _mode.Cancel();
    }

    public static void HideUi(){
        _lineRenderer.enabled = false;
        _spriteRenderer.enabled = false;
    }

    public static void SetSprite(Sprite uiSprite, Color uiColor){
        _spriteRenderer.sprite = uiSprite;
        _spriteRenderer.color = uiColor;
    }
}