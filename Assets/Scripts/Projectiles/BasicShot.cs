using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShot : MonoBehaviour
{
    private Unit target;
    private Vector3Int targetTile;
    public float speed;

    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        var currentTarget = target ? target.transform.position : Data.board.CellToWorld(targetTile); 
        
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed);
        if (transform.position == currentTarget){
            if (target){
                target.Damaged(damage);
                foreach (var propulsion in target.GetComponents<Propulsion>()){
                    propulsion.Interrupt();
                }
            }

            Destroy(gameObject);
        }
    }

    public void SetTarget(Unit newTarget){
        this.target = newTarget;
    }

    public void SetTarget(Vector3Int newTarget){
        targetTile = newTarget;
    }

}
