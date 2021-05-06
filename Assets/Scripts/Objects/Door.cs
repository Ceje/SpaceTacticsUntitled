using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Obstacle
{
    public bool unlocked = false;
    private Animator _animator;
    private static readonly int Open = Animator.StringToHash("Open");
    private AudioSource _audioSource;

    // Start is called before the first frame update
    new void Start(){
        blocksMovement = true;
        blocksVision = true;
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        base.Start(); 
    }

    // Update is called once per frame
    new void Update(){
        base.Update();
    }

    public void ToggleState(){
        var state = !_animator.GetBool(Open);
        blocksMovement = !state;
        blocksVision = !state;
        FogMap.ScheduleFogUpdate();
        Data.board.UpdatePathingMap(new List<Vector3Int>{Data.board.WorldToCell(transform.position)});
        _animator.SetBool(Open, !_animator.GetBool(Open));
        _audioSource.Play();
    }

    public bool IsOpen(){
        return _animator.GetBool(Open);
    }
}
