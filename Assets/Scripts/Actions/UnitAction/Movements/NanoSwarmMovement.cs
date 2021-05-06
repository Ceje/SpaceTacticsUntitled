using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoSwarmMovement : Movement
{
    public GameObject nanoswarm;
    private bool duplicated = true;
    private Vector3Int lastTile;
    private bool instantiated = false;
    public override bool CanUse(){
        if (Unit.health <= ((NanoSwarm) Unit).nanoMoveDamage){
            return false;
        }
        return base.CanUse();
    }

    protected override void Update(){
        var currentTile = Data.board.WorldToCell(transform.position);
        if (!instantiated){
            instantiated = true;
            lastTile = currentTile;
        }

        if (CurrentMove != null && lastTile != currentTile ){
            GameObject nanoChild = Instantiate(nanoswarm, Data.board.transform);
            nanoChild.transform.position = Data.board.CellToWorld(lastTile);
            
            var childScript = nanoChild.GetComponent<NanoSwarm>();
            childScript.health = 1;
            childScript.nanoMoveDamage += 1;
            childScript.remainingActions = 0;
            childScript.remainingMovement = 0;
            
            lastTile = currentTile;
        }
        base.Update();
        
        

        if (!duplicated){
            duplicated = true;
            
        }
    }
}
