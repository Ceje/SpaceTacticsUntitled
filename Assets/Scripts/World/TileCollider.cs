using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollider : MonoBehaviour
{
    private HashSet<Interactable> _objectsOnTile = new HashSet<Interactable>();
    private List<Hazard> _hazards = new List<Hazard>();
    private void OnTriggerEnter(Collider other){
        foreach (var interactable in  other.gameObject.GetComponents<Interactable>()){
            if (Data.board.WorldToCell(interactable.transform.position) != Data.board.WorldToCell(transform.position)){
                continue;
            }
            _objectsOnTile.Add(interactable);
            _hazards.ForEach( hazard => hazard.TriggerEffect(interactable));
            Data.board.UpdatePathingMap(new List<Vector3Int>{Data.board.WorldToCell(transform.position)});
            FogMap.ScheduleFogUpdate();
            HighlightMap.RefreshHighlights();
        }
    }

    private void OnTriggerExit(Collider other){
        foreach (var interactable in  other.gameObject.GetComponents<Interactable>()){
            _objectsOnTile.Remove(interactable);
            Data.board.UpdatePathingMap(new List<Vector3Int>{Data.board.WorldToCell(transform.position)});
            FogMap.ScheduleFogUpdate();
            HighlightMap.RefreshHighlights();
        }
    }

    public HashSet<Interactable> ObjectsOnTile(){
        return _objectsOnTile;
    }

    public void AddHazard(Hazard hazard){
        _hazards.Add(hazard);
    }

    public void RemoveHazard(Hazard hazard){
        _hazards.Remove(hazard);
    }

    public bool HasHazard(){
        return _hazards.Count > 0;
    }

    public void RemoveInteractable(Interactable interactable){
        _objectsOnTile.Remove(interactable);
    }
}
