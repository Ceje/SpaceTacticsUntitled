using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogMap : MonoBehaviour
{
    public TileBase fogTile;
    public TileBase hazardTile;
    private static bool updated = false;
    private Tilemap fogMap;
    private static HashSet<IVisionProvider> _visionProviders = new HashSet<IVisionProvider>();
    private bool restrictedVision = false;
    private HashSet<Vector3Int> visibleTiles = new HashSet<Vector3Int>();

    // Start is called before the first frame update
    void Start(){
        fogMap = GetComponent<Tilemap>();
        foreach (var tilecoordinate in fogMap.cellBounds.allPositionsWithin){
            fogMap.SetTile(tilecoordinate, fogTile);
        }

        Data.fog = this;
    }

    // Update is called once per frame
    void Update(){
        if (updated){
            return;
        }

        visibleTiles.Clear();
        if (restrictedVision){
            foreach (var unit in _visionProviders){
                ClearVisionRadius(unit.GetPeripheralVision(), fogMap.WorldToCell(unit.GetTransform().position));
                ClearVisionCone(unit.GetForwardVision(), unit.GetFacing(),
                    fogMap.WorldToCell(unit.GetTransform().position), 45);
            }
        }
        else{
            ClearVisionZones();
        }

        updated = true;

        if (Turns.GetCurrentTeam().IsAI()){
            return;
        }

        RedrawFogMap();
    }

   


    private void RedrawFogMap(){
        foreach (var tilecoordinate in fogMap.cellBounds.allPositionsWithin){
            if (visibleTiles.Contains(tilecoordinate)){
                if (Data.board.GetTile(tilecoordinate).HasHazard()){
                    fogMap.SetTile(tilecoordinate, hazardTile);
                    continue;
                }

                fogMap.SetTile(tilecoordinate, null);
            }
            else if (Data.board.GetTile(tilecoordinate) is null || _visionProviders.Count == 0){
                fogMap.SetTile(tilecoordinate, null);
            }
            else{
                fogMap.SetTile(tilecoordinate, fogTile);
            }
        }
    }

    private void ClearVisionRadius(int unitVision, Vector3Int unitCell){
        HashSet<Vector3Int> tiles = new HashSet<Vector3Int>();
        List<Vector3Int> outer = new List<Vector3Int>();
        outer.Add(unitCell);
        tiles.Add(unitCell);
        outer.ForEach(tile => tiles.Add(tile));
        for (int i = 0; i < unitVision; i++){
            HashSet<Vector3Int> neighbors = new HashSet<Vector3Int>();
            outer.ForEach(outerTile => {
                if (Data.board.TileWeight(outerTile) >= 0){
                    neighbors.UnionWith(Data.board.GetNeighbors(outerTile));
                }
            });
            neighbors.ExceptWith(tiles);
            tiles.UnionWith(neighbors);
            outer = neighbors.ToList();
        }

        tiles.RemoveWhere(tile => !Data.board.isVisible(unitCell, tile));

        visibleTiles.UnionWith(tiles);
    }

    private void ClearVisionCone(int unitVision, float facing, Vector3Int unitCell, float coneAngle){
        HashSet<Vector3Int> tiles = new HashSet<Vector3Int>();
        List<Vector3Int> outer = new List<Vector3Int>();

        var start = Data.board.GetTileByOrientation(unitCell, unitVision, facing - coneAngle);
        var end = Data.board.GetTileByOrientation(unitCell, unitVision, facing + coneAngle);
        foreach (var tile in Data.board.GetTileLine(start, end, .8f, .1f)){
            tiles.UnionWith(Data.board.GetTileLine(tile, unitCell, .8f, .1f));
        }

        tiles.RemoveWhere(tile => !Data.board.isVisible(unitCell, tile));

        visibleTiles.UnionWith(tiles);
    }
    private void ClearVisionZones(){
        foreach (var visibleZone in ZoneMap.GetActiveZones()){
            var tilesInZone = Data.board.GetZoneTiles(visibleZone);
            foreach (var vector3Int in tilesInZone){
                Data.board.GetNeighbors(vector3Int).ForEach(tile => {
                    if (_visionProviders.Any(provider =>
                        Data.board.isVisible(Data.board.WorldToCell(provider.GetTransform().position), tile))){
                        visibleTiles.Add(tile);
                    }
                });
                if (_visionProviders.Any(provider =>
                    Data.board.isVisible(Data.board.WorldToCell(provider.GetTransform().position), vector3Int))){
                    visibleTiles.Add(vector3Int);
                }
            }
        }
    }

    public bool inFog(Vector3 transformPosition){
        return inFog(fogMap.WorldToCell(transformPosition));
    }

    public bool inFog(Vector3Int cell){
        if (Turns.GetCurrentTeam().IsAI()){
            return !visibleTiles.Contains(cell);
        }

        var tile = fogMap.GetTile(cell);
        return !(tile is null || tile == hazardTile);

        //might be overly cautious, but if I'm guessing right then:
        //.GetTile() is a hashset lookup, which will outpace the linear search in .Contains()
        //we need to do .Contains() for the AI as we don't update the tilemap. 
    }

    public static void ScheduleFogUpdate(){
        updated = false;
    }

    public static void SetVisionProviders(HashSet<Interactable> units){
        var temp = new HashSet<IVisionProvider>();
        foreach (var interactable in units){
            if (interactable is IVisionProvider vp){
                temp.Add(vp);
            }
        }

        _visionProviders = temp;
        ScheduleFogUpdate();
    }

    public interface IVisionProvider
    {
        int GetPeripheralVision();
        int GetForwardVision();
        float GetFacing();
        Transform GetTransform();
    }
}