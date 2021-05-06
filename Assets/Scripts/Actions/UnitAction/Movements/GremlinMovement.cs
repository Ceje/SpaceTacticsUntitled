using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GremlinMovement : Movement
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
        if (bestMove == location){
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
            && vector3Int != Data.board.WorldToCell(transform.position)){
            value = float.MinValue;
        }
        var worldLocation = Data.board.CellToWorld(vector3Int);
        var playerList = Teams.GetManagerInstance().GetTeam("Player").GetMembers();

        //target 3 spaces from player units
        foreach (var player in playerList){
            if (!Data.board.TilesConnected(vector3Int, Data.board.WorldToCell(player.transform.position))){
                continue;
            }
            var distanceToPlayer = (worldLocation - player.transform.position).magnitude;
            if (distanceToPlayer < 3){
                value += (distanceToPlayer - 3) *20; //emphasize: don't get close
            } else if (distanceToPlayer > 3){
                value += (3 - distanceToPlayer);
            }
            else{
                value += 1;
            }
        }
        
        value -= playerList.Count;
        
        //approach attached items
        if (Data.AttachedItems.Count > 0){
            var nearestLocation = float.MaxValue;
            foreach (var attachedItem in Data.AttachedItems){
                var distance = float.MaxValue;
                if (Data.board.TilesConnected(vector3Int, Data.board.WorldToCell(attachedItem.transform.position))){
                    distance = (worldLocation - attachedItem.transform.position).magnitude;
                }
                if (distance < nearestLocation){
                    nearestLocation = distance;
                }
            }

            if(nearestLocation > 0) value += 20 / nearestLocation;
        }

        return value;
    }
}
