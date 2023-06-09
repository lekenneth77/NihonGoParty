using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TreeHop : Minigame, Controls.ITreeHopActions
{
    // Start is called before the first frame update
    public TreeTrunk[] players;
    public Timer timer;
    public GameObject finishImage;
    public int numPlayers;

    public Camera[] playerCameras;
    private float[] zCameraPositions = new float[] { -4f, -4.5f, -6f, -7f };
    private float[] yCameraPositions = new float[] { -.9f, -.9f, -.88f, -.85f };


    private Controls controls;

    public override void Start()
    {
        base.Start();

        controls = new Controls();
        controls.TreeHop.AddCallbacks(this);

        //numPlayers = BoardController.numPlayers;
        if (numPlayers == 1) {
            timer.gameObject.SetActive(true);
            playerCameras[0].rect = new Rect(0, 0, 1, 1);
        } else if (numPlayers == 2) {
            playerCameras[0].rect = new Rect(0, 0, 0.5f, 1);
            playerCameras[1].rect = new Rect(0.5f, 0, 0.5f, 1);
        } else if (numPlayers == 3) {
            playerCameras[0].rect = new Rect(0, 0, 0.3f, 1);
            playerCameras[1].rect = new Rect(0.3f, 0, 0.4f, 1);
            playerCameras[2].rect = new Rect(0.7f, 0, 0.3f, 1);
        } else {
            for (int i = 0; i < 4; i++) {
                playerCameras[i].rect = new Rect(0.25f * i, 0, 0.25f, 1);
            }
        }

        for (int i = 0; i < numPlayers; i++)
        {
            players[i].gameObject.SetActive(true);
            playerCameras[i].gameObject.SetActive(true);
            players[i].gameObject.transform.GetChild(0).localPosition = new Vector3(0, yCameraPositions[numPlayers - 1], zCameraPositions[numPlayers - 1]);
        }


        GetWords();
        controls.Enable();
        if (numPlayers == 1) {
            timer.ResetTimer();
            timer.StartTimer(); 
        }
    }

    private void GetWords() { 

    }
   

    private void AfterJump(int playerI, int result)
    {
        if (result == -1) {
            //failed jump
            StartCoroutine(DisableEnable(playerI, 2f, true));
        } else if (result == 0) {
            //success jump
            StartCoroutine(DisableEnable(playerI, 2.5f, false));
        } else {
            //reached finish
            controls.Disable();
            StartCoroutine(OnFinish(playerI));
        }
    }

    private IEnumerator DisableEnable(int playerI, float disableTime, bool showStars)
    {
        if (playerI == 0) {
            controls.TreeHop.A.Disable();
            controls.TreeHop.D.Disable();
        } else if (playerI == 1) {
            controls.TreeHop.R.Disable();
            controls.TreeHop.Y.Disable();
        } else if (playerI == 2) {
            controls.TreeHop.J.Disable();
            controls.TreeHop.L.Disable();
        } else {
            controls.TreeHop.LeftAKey.Disable();
            controls.TreeHop.RightAKey.Disable();
        }

        if (showStars) {
            yield return new WaitForSeconds(1f); //the jump time
            players[playerI].stars.StartSpin();
        }
        yield return new WaitForSeconds(disableTime);

        if (playerI == 0) {
            controls.TreeHop.A.Enable();
            controls.TreeHop.D.Enable();
        } else if (playerI == 1) {
            controls.TreeHop.R.Enable();
            controls.TreeHop.Y.Enable();
        } else if (playerI == 2) {
            controls.TreeHop.J.Enable();
            controls.TreeHop.L.Enable();
        } else {
            controls.TreeHop.LeftAKey.Enable();
            controls.TreeHop.RightAKey.Enable();
        }
    }

    private IEnumerator OnFinish(int playerI) {
        if (numPlayers == 1) {
            timer.StopTimer();
        }
        yield return new WaitForSeconds(1.5f);
        players[playerI].JumpToEnd();
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < numPlayers; i++) { 
            if (i == playerI) {
                players[playerI].jumper.gameObject.GetComponent<Animator>().Play("victory");
            } else {
                players[i].jumper.gameObject.GetComponent<Animator>().Play("lose");
            }
        }
        finishImage.SetActive(true);
        yield return new WaitForSeconds(4f);
        EndGame(false);
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        int result = players[0].Jump(true);
        AfterJump(0, result);
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        int result = players[0].Jump(false);
        AfterJump(0, result);
    }

    public void OnR(InputAction.CallbackContext context)
    {
        if (numPlayers < 2 || !context.performed) { return; }
        int result = players[1].Jump(true);
        AfterJump(1, result);
    }

    public void OnY(InputAction.CallbackContext context)
    {
        if (numPlayers < 2 || !context.performed) { return; }
        int result = players[1].Jump(false);
        AfterJump(1, result);
    }

    public void OnJ(InputAction.CallbackContext context)
    {
        if (numPlayers < 3 || !context.performed) { return; }
        int result = players[2].Jump(true);
        AfterJump(2, result);
    }

    public void OnL(InputAction.CallbackContext context)
    {
        if (numPlayers < 3 || !context.performed) { return; }
        int result = players[2].Jump(false);
        AfterJump(2, result);
    }

    public void OnLeftAKey(InputAction.CallbackContext context)
    {
        if (numPlayers < 4 || !context.performed) { return; }

        int result = players[3].Jump(true);
        AfterJump(3, result);
    }

    public void OnRightAKey(InputAction.CallbackContext context)
    {
        if (numPlayers < 4 || !context.performed) { return; }
        int result = players[3].Jump(false);
        AfterJump(3, result);
    }
}
