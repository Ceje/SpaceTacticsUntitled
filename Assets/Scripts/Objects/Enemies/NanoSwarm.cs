using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoSwarm : Enemy
{
    private Vector3Int _currentTile;
    private Shove _shove;
    private bool _instantiated = false;
    private int _force;
    private ParticleSystem _particleSystem;

    public int nanoMoveDamage = 1;

    // Start is called before the first frame update
    protected new void Start(){
        base.Start();
        _shove = GetComponent<Shove>();
        _particleSystem = GetComponent<ParticleSystem>();
        ailments.Add(new NanoRegen());
    }

    // Update is called once per frame
    new void Update(){
        if (!_instantiated){
            _currentTile = Data.board.WorldToCell(transform.position);
            _force = health;
            _instantiated = true;
        }

        if (_force != health && !(_shove is null)){
            _shove.force = health;
            _force = health;
        }

        var tile = Data.board.WorldToCell(transform.position);
        if (_currentTile != tile){
            health -= nanoMoveDamage;
            CheckDeath();
            _currentTile = tile;
        }

        base.Update();

        if (!Data.fog.inFog(transform.position) && !Turns.GetCurrentTeam().IsAI()){
            _particleSystem.Play();
        }
        else if (!Turns.GetCurrentTeam().IsAI()){
            _particleSystem.Stop();
        }
    }
}