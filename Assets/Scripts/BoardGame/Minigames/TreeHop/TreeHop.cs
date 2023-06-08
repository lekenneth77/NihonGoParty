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
        int result = playerOne.Jump(true);
        AfterJump(playerOne, result, "A", "D");
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        int result = playerOne.Jump(false);
        AfterJump(playerOne, result, "A", "D");
    }

    private void AfterJump(TreeTrunk player, int result, string inputOne, string inputTwo)
    {
        if (result == -1) {
            //failed jump
            StartCoroutine(DisableEnable(player, 3f));
        } else if (result == 0) {
            //success jump
            StartCoroutine(DisableEnable(player, 0.5f));
        } else { 
            //reached finish
        }
    }

    private IEnumerator DisableEnable(TreeTrunk player, float disableTime)
    {
        if (player == playerOne) {
            controls.TreeHop.A.Disable();
            controls.TreeHop.D.Disable();
        }
        yield return new WaitForSeconds(disableTime);
        if (player == playerOne)
        {
            controls.TreeHop.A.Enable();
            controls.TreeHop.D.Enable();
        }
        Debug.Log("Reenabled!");
    }


}
