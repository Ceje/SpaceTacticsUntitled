using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightMap : MonoBehaviour
{
    [SerializeField]
    private TileBase assignedTile;

    private static TileBase highlightTile;
    private static HashSet<GameObject> _tileHighlights = new HashSet<GameObject>();
    private static Tilemap _highlightMap;

    public void Start(){
        _highlightMap = GetComponent<Tilemap>();
        highlightTile = assignedTile;
    }

    public static void SetHighlights(HashSet<GameObject> hashSet){
        _tileHighlights = hashSet;
        RefreshHighlights();
    }

    public static void RefreshHighlights(){
        _highlightMap.ClearAllTiles();
        foreach (var tileHighlight in _tileHighlights){
            _highlightMap.SetTile(Data.board.WorldToCell(tileHighlight.transform.position), highlightTile);
        }
    }

    public static bool HighlightContains(Vector3Int location){
        return _tileHighlights.Any(obj => Data.board.WorldToCell(obj.transform.position) == location);
    }
}
