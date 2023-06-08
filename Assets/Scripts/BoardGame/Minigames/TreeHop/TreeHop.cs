using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TreeHop : Minigame, Controls.ITreeHopActions
{
    // Start is called before the first frame update
    public TreeTrunk playerOne;

    private Controls controls;

    public override void Start()
    {
        base.Start();

        controls = new Controls();
        controls.TreeHop.AddCallbacks(this);
        controls.Enable();
    }
    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        playerOne.Jump(true);
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        playerOne.Jump(false);

    }

}
