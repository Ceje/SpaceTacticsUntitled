using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MagpieMovement : Movement
{
    public override void AIExecute(){
        var location = Data.board.WorldToCell(transform.position);
        var neighbors = Data.board.GetNeighbors(location);
        
        Vector3Int bestMove = location;
        float bestWeight = float.MinValue;
        foreach (var vector3Int in neighbors){
            var weight = GetWeight(vector3Int);
            if (weight > bestWeight){
                bestWeight = weight;
                bestMove = vector3Int;
            }
        }
        if (bestMove == location || bestMove == Data.board.WorldToCell(((Magpie) Unit).nest.transform.position)){
            Unit.remainingMovement--;
            return;
        }
        Path(location, bestMove);
        Move();
    }

    private float GetWeight(Vector3Int vector3Int){
        float value = 0;
        if (Data.board.TileWeight(vector3Int) < 0 
            || Data.board.TileOccupied(vector3Int, gameObject) 
            || !Data.board.TilesConnected(vector3Int, Data.board.WorldToCell(transform.position))
            && vector3Int != Data.board.WorldToCell(transform.position)){
            value = float.MinValue;
        }
        var worldLocation = Data.board.CellToWorld(vector3Int);
        var playerCount = Data.board.GetNearby<PlayerUnit>(vector3Int, 1);
        value -= playerCount.Count;
        
        if (Data.LooseItems.Count > 0){
            var nearestLocation = float.MaxValue;
            foreach (var looseItem in Data.LooseItems){
                if (!Data.board.TilesConnected(vector3Int, Data.board.WorldToCell(looseItem.transform.position))){
                    continue;
                }
                var distance = (worldLocation - looseItem.transform.position).magnitude;
                if (distance < nearestLocation){
                    nearestLocation = distance;
                }
            }

            if (Unit.Items.Count <= 0){
                value += 20 / nearestLocation;
            }
        }

        var nest = ((Magpie) Unit).nest;
        if (Data.board.TilesConnected(vector3Int, Data.board.WorldToCell(nest.transform.position)) 
            &&Unit.Items.Count > 0 || Data.LooseItems.Count == 0){
            var distanceToNest = (nest.transform.position - worldLocation).magnitude;
            value += 20 / distanceToNest;
        }

        return value;
    }
}