using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PathingMap
{
    private Dictionary<Vector3Int, int> tileGroups = new Dictionary<Vector3Int, int>();
    private Dictionary<int, List<Vector3Int>> groupedTiles = new Dictionary<int, List<Vector3Int>>();
    private List<Vector3Int> checkingNeighbors = new List<Vector3Int>();

    public int GetTileGroup(Vector3Int tile){
        if (!tileGroups.ContainsKey(tile)){
            FindTileGroup(tile);
        }

        return tileGroups[tile];
    }

    public bool Connected(Vector3Int start, Vector3Int end){
        var startZones = new HashSet<int>();
        var endZones = new HashSet<int>();
        Data.board.GetNeighbors(start).ForEach(tile => {
            if (!tileGroups.ContainsKey(tile)){
                FindTileGroup(tile);
            }
            if (tileGroups[tile] >= 0) startZones.Add(tileGroups[tile]);
        });
        Data.board.GetNeighbors(end).ForEach(tile => {
            if (!tileGroups.ContainsKey(tile)){
                FindTileGroup(tile);
            }
            if (tileGroups[tile] >= 0) endZones.Add(tileGroups[tile]);
        });

        return startZones.Any(zone => endZones.Contains(zone));
    }

    public void UpdateTileGroups(){
        groupedTiles.Clear();
        tileGroups.Clear();
        var tiles = Data.board.AllTileWeights();
        foreach (var tile in tiles.Keys){
            FindTileGroup(tile);
        }
    }

    private int FindTileGroup(Vector3Int tile){
        //we have one already
        if (tileGroups.ContainsKey(tile)){
            return tileGroups[tile];
        }

        //it's a blocked tile so we don't use it for propagation 
        if (Data.board.TileWeight(tile) < 0){
            tileGroups[tile] = -2;
            return -2;
        } 
        if(Data.board.TileOccupied(tile, Data.board.gameObject)){
            tileGroups[tile] = -1;
            return -1;
        }


        //we check neighbors for tile weights
        var neighbors = Data.board.GetNeighbors(tile);
        checkingNeighbors.Add(tile);
        neighbors.ForEach(neighbor => {
            //we have our key
            if (tileGroups.ContainsKey(tile)){
                return;
            }

            //neighbor doesn't have a key & isn't already searching for a key
            if (!tileGroups.ContainsKey(neighbor) && !checkingNeighbors.Contains(neighbor)){
                FindTileGroup(neighbor);
            }

            //neighbor has a key
            if (tileGroups.ContainsKey(neighbor) && tileGroups[neighbor] >= 0){
                tileGroups[tile] = tileGroups[neighbor];
                groupedTiles[tileGroups[neighbor]].Add(tile);
            }
        });
        checkingNeighbors.Remove(tile);

        //no keys found, make one
        if (!tileGroups.ContainsKey(tile)){
            var newGroup = 0;
            if (groupedTiles.Count > 0){
                newGroup = groupedTiles.Keys.Max() + 1;
            }

            groupedTiles[newGroup] = new List<Vector3Int>();
            tileGroups[tile] = newGroup;
            groupedTiles[newGroup].Add(tile);
        }


        return tileGroups[tile];
    }

    public void UpdateTileGroups(List<Vector3Int> changedTiles){
        //Rebuild this to only check the immediate vicinity of the tiles changed, and recalculate zones if necessary
        //atm this recalculates the entire board.
        UpdateTileGroups();   
    }

    public List<Vector3Int> AllTilesInGroup(int visibleZone){
        if (!groupedTiles.ContainsKey(visibleZone)){
            return new List<Vector3Int>();
        }
        return groupedTiles[visibleZone];
    }
}