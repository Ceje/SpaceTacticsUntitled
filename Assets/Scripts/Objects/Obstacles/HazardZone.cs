using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HazardZone : MonoBehaviour
{
    private Hazard _hazard;
    private MeshRenderer _renderer;

    private List<Vector3Int> affectedTiles = new List<Vector3Int>();
    public void Start(){
        _hazard = GetComponentInParent<Hazard>();
        _renderer = GetComponent<MeshRenderer>();
        Data.HazardZones.Add(this);
        FogMap.ScheduleFogUpdate();
    }

    private void OnTriggerEnter(Collider other){
        var tile = other.gameObject.GetComponent<TileCollider>();
        if (tile is null
            || Data.board.TileWeight(Data.board.WorldToCell(tile.transform.position)) < 0
            || !Data.board.isVisible(
                Data.board.WorldToCell(_hazard.transform.position), Data.board.WorldToCell(tile.transform.position))){
            return;
        }
        tile.AddHazard(_hazard);
        FogMap.ScheduleFogUpdate();

    }
    
    private void OnTriggerExit(Collider other){
        var tile = other.gameObject.GetComponent<TileCollider>();
        if (tile is null){
            return;
        }
        tile.RemoveHazard(_hazard);
        FogMap.ScheduleFogUpdate();

    }
}
