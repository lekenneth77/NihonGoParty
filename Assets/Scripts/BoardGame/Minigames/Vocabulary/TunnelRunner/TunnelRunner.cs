using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TunnelRunner : Minigame, Controls.ITunnelRunnerActions
{

    public Cave p1Cave;
    private int p1Points;
    private Controls controls;
    // Start is called before the first frame update
    public override void Start()
    {
        controls = new Controls();
        controls.TunnelRunner.AddCallbacks(this);
        controls.TunnelRunner.Enable();
        p1Cave.endMove += P1NewCaveReached;
    }

    public void P1NewCaveReached(Cave newCave, bool correct) {
        Debug.Log("Here we are!");
        p1Cave.endMove -= P1NewCaveReached;
        Destroy(p1Cave.gameObject);
        if (correct) { p1Points++; }
        p1Cave = newCave;
        p1Cave.endMove += P1NewCaveReached;
        controls.TunnelRunner.A.Enable();
        controls.TunnelRunner.D.Enable();
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        p1Cave.GoLeft();
        controls.TunnelRunner.A.Disable();
        controls.TunnelRunner.D.Disable();
    }
    public void OnD(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        p1Cave.GoRight();
        controls.TunnelRunner.A.Disable();
        controls.TunnelRunner.D.Disable();
    }

    public void OnLeftArrowKey(InputAction.CallbackContext context)
    {
        if (!context.performed || singleplayer) { return; }
        controls.TunnelRunner.LeftArrowKey.Disable();
        controls.TunnelRunner.RightArrowKey.Disable();
    }

    public void OnRightArrowKey(InputAction.CallbackContext context)
    {
        if (!context.performed || singleplayer) { return; }
        controls.TunnelRunner.LeftArrowKey.Disable();
        controls.TunnelRunner.RightArrowKey.Disable();
    }

  
}
