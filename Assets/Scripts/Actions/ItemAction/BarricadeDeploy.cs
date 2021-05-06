using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeDeploy : MonoBehaviour
{
    public Sprite _itemSprite;
    public Sprite _obstacleSprite;
    
    private bool _deployed;
    private BarricadeItem _item;
    private BarricadeObstacle _obstacle;
    private SpriteRenderer _renderer;
    private BoxCollider _collider;

    private bool _initialized = false;
    public void Start(){
        _item = GetComponent<BarricadeItem>();
        _obstacle = GetComponent<BarricadeObstacle>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider>();
    }

    public void Update(){
        if (!_initialized){
            _initialized = true;
            if (transform.parent == Data.board.transform){
                SetDeployed(true);
            }
            else{
                SetDeployed(false);
            }
        }
    }
    
    public void SetDeployed(bool deploy){
        _deployed = deploy;
        _obstacle.enabled = deploy;
        _item.enabled = !deploy;
        if (deploy){
            _renderer.sprite = _obstacleSprite;
            Data.AttachedItems.Add(_item);
        }
        else{
            _renderer.sprite = _itemSprite;
            Data.AttachedItems.Remove(_item);
        }
    }

    public bool IsDeployed(){
        return _deployed;
    }
}
