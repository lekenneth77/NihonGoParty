using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreelookCamera : MonoBehaviour, Controls.IFreeCameraActions
{
    private Vector2 moveVal;
    public float moveSpeed = 10f;
    public float xLeftBounds;
    public float xRightBounds;
    public float zUpperBounds;
    public float zLowerBounds;

    private Vector3 initialPos;

    private bool move;
    private Controls controls;

    void Start()
    {
        controls = new Controls();
        controls.FreeCamera.AddCallbacks(this);
        controls.Enable();

        initialPos = this.transform.position;
    }

    void Update()
    {
        if (move)
        {
            Vector3 movement = new Vector3();
            movement.x = moveVal.x * -1;
            movement.y = 0;
            movement.z = moveVal.y * -1;
            if (this.transform.position.x >= xLeftBounds  && this.transform.position.x <= xRightBounds 
                && this.transform.position.z <= zUpperBounds && this.transform.position.z >= zLowerBounds)
            {
                this.transform.Translate(moveSpeed * Time.deltaTime * movement, Space.World);
            } else
            {
                this.transform.position = initialPos;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<Vector2>();
        move = true;
        if (context.canceled)
        {
            move = false;
        }
    }

}
