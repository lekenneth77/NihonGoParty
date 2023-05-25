using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreelookCamera : MonoBehaviour, Controls.IFreeCameraActions
{
    private Vector2 moveVal;
    public float moveSpeed = 10f;
    private bool move;
    private Controls controls;

    void Start()
    {
        controls = new Controls();
        controls.FreeCamera.AddCallbacks(this);
        controls.Enable();
    }

    void Update()
    {
        if (move)
        {
            Vector3 movement = new Vector3();
            movement.x = moveVal.x * -1;
            movement.y = 0;
            movement.z = moveVal.y * -1;
            this.transform.Translate(moveSpeed * Time.deltaTime * movement, Space.World);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<Vector2>();
        move = true;
        if (context.canceled)
        {
            Debug.Log("Hey I'm not moving here!");
            move = false;
        }
    }

}
