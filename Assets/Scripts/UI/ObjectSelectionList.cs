using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectSelectionList : MonoBehaviour
{
    private TileCollider _tile;
    private Canvas _canvas;
    private LineRenderer _line;
    private Camera _camera;
    private GameObject _list;
    private RectTransform _referenceRect;
    public GameObject buttons;
    public void Start(){
        
        _canvas = GetComponent<Canvas>();
        var mainCamera = Camera.main;
        _canvas.worldCamera = mainCamera;
        _camera = mainCamera;
        _list = GetComponentInChildren<Scrollbar>().gameObject;
        _referenceRect = _list.GetComponent<RectTransform>();

    }

    public void Update(){
        if (_line is null){
            return;
        }
        _line.enabled= true;
        _line.positionCount = 3;
        var cameraDisplacement = _tile.transform.position - _camera.transform.position;
        var cameraAdjustedX = _camera.pixelWidth - _referenceRect.anchoredPosition.x - _referenceRect.rect.width;
        var cameraAdjustedY1 = _camera.pixelHeight - _referenceRect.anchoredPosition.y -_referenceRect.offsetMax.y - _referenceRect.rect.height;
        var cameraAdjustedY2 = _camera.pixelHeight - _referenceRect.anchoredPosition.y -_referenceRect.offsetMax.y;

        var point1 = _camera.ScreenToWorldPoint(new Vector3(cameraAdjustedX, cameraAdjustedY1, cameraDisplacement.magnitude));
        var point2 = _camera.ScreenToWorldPoint(new Vector3(cameraAdjustedX, cameraAdjustedY2, cameraDisplacement.magnitude));

        _line.SetPosition(0, _tile.transform.position);
        _line.SetPosition(1, point1);
        _line.SetPosition(2, point2);

    }

    public void Configure(TileCollider tile, LineRenderer line){
        _tile = tile;
        _line = line;
        var list = GetComponentInChildren<Scrollbar>().gameObject;
        foreach (var interactable in _tile.ObjectsOnTile()){
            if (!interactable.enabled){
                continue;
            }
            var button = Instantiate(buttons, list.transform);
            button.GetComponent<SelectButton>().Configure(interactable, this);
        }
    }

    public void OnDestroy(){
        if (SceneManager.GetActiveScene().isLoaded){
            _line.enabled = false;
        }
    }

    public void Close(){
        Destroy(_canvas.gameObject);
    }
    
    
}
