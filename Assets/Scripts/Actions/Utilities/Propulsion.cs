using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Propulsion : MonoBehaviour
{
    private Vector3Int _targetLocation;
    private Interactable _affected;
    private Vector3Int _lastTile;
    private float _speed;
    private bool _moving = false;
    private bool _configured = false;
    private bool _interrupted = false;

    public void StartMovement(){
        if (!_configured){
            return;
        }

        _moving = true;
    }

    public void Configure(Interactable targetObject, Vector3Int location, float speed = .1f){
        _affected = targetObject;
        _targetLocation = location;
        _lastTile = Data.board.WorldToCell(targetObject.transform.position);
        _configured = true;
        _speed = speed;
    }

    public Vector3Int GetTargetLocation(){
        return _targetLocation;
    }

    public void Interrupt(){
        _interrupted = true;
    }

    void Update(){
        if (_interrupted){
            _targetLocation = _lastTile;
            _interrupted = false;
        }

        if (_moving){
            MoveTowardsTarget();
            
            var currentTile = Data.board.WorldToCell(transform.position);

            var collision = CollisionPrediction(currentTile);
            
            if (currentTile != _lastTile && !collision){
//                Debug.Log("tile changed, updated last tile to " + currentTile);
                _lastTile = currentTile;
            } else if(collision && CollisionStopsMovement(currentTile)){
//                Debug.Log("Collision at " + _predictedTile + " reorouting to " + _lastTile);
                _targetLocation = _lastTile;
                _interrupted = false;
                (_affected as Unit)?.Damaged(_affected.weight);
            } else if (collision){
//                Debug.Log("Collision at " + _predictedTile + " continuing through");

                Data.board.GetNearby<Unit>(currentTile, 0).ForEach(unit => unit.Damaged(_affected.weight));
            }
            
        }

    }

    private void MoveTowardsTarget(){
        var targetCoordinates = Data.board.CellToWorld(_targetLocation);
        _affected.transform.position =
            Vector3.MoveTowards(_affected.transform.position, targetCoordinates, _speed);
        if (_affected.transform.position == targetCoordinates){
//            Debug.Log("Ending movement at " + _targetLocation);
            _moving = false;
            Destroy(this);
        }
    }

    private bool CollisionStopsMovement(Vector3Int tileOfCollision){
        var blocked = Data.board.GetNearby<Interactable>(tileOfCollision, 0).Any(ObjectCollision);

        if (blocked || Data.board.TileWeight(tileOfCollision) < 0){
            return true;
        }

        return false;
    }

    private bool ObjectCollision(Interactable blockingObject){
        if (blockingObject.name != _affected.name && blockingObject.blocksMovement){
            if (blockingObject.weight >= _affected.weight){
                return true;
            }
        }

        return false;
    }

    private bool CollisionPrediction(Vector3Int nextTile){
        return Data.board.TileOccupied(nextTile, gameObject) || Data.board.TileWeight(nextTile) < 0;
    }
}