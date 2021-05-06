using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class AdjustCamera : MonoBehaviour
{
    public InputActionAsset _processor;
    public float speed;

    private Vector3 direction;

    private Grid _grid;

    // Start is called before the first frame update
    void Start(){
        _grid = Data.board.gameObject.GetComponentInParent<Grid>();
        var moveAction = _processor.FindAction("Move");
        moveAction.performed += Move;
        moveAction.canceled += Move;
        moveAction.Enable();
    }

    // Update is called once per frame
    void Update(){
//        var lookPos = Vector3.zero - transform.position;
//        lookPos.y = 0;
//        var rotation = Quaternion.LookRotation(lookPos);
        CameraApproach();

        RotateBoard();
    }

    private void CameraApproach(){
        var position = transform.position;
        if (direction.y > 0){
            transform.position = Vector3.MoveTowards(position, new Vector3(0, 0, position.z), speed);
        }
        else if (direction.y < 0){
            transform.position =
                Vector3.MoveTowards(position, new Vector3(position.x, position.y - speed, position.z), speed);
        }
        transform.LookAt(Vector3.zero);

    }

    private void RotateBoard(){
        if (direction.x != 0){
            _grid.transform.Rotate(Vector3.forward, direction.x * speed);
        }
    }

    void Move(InputAction.CallbackContext context){
        var input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, input.y, 0);
    }

}
