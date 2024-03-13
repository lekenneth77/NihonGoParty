using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TreeHop : Minigame, Controls.ITreeHopActions
{
    // Start is called before the first frame update
    public TreeTrunk[] players;
    public JumpObject[] characterModels;
    public GameObject[] platformFolders; //literally just to enable at the start
    public Timer timer;
    public TextMeshProUGUI categoryText;
    public GameObject finishImage;
    public GameObject failImage;
    private int numPlayers;
    public GameObject Instructions;
    public TextAsset[] texts;

    public PlayableDirector[] directors;

    public Camera[] playerCameras;
    private float[] zCameraPositions = new float[] { -3.5f, -4.5f, -6.5f, -7f };
    private float[] yCameraPositions = new float[] { 1f, 1f, 1f, 2.5f };

    private Controls controls;

    public override void Start()
    {
        base.Start();

        controls = new Controls();
        controls.TreeHop.AddCallbacks(this);
        timer.TimeUp += TimeOut;

        GetWords();
        if (singleplayer) {
            numPlayers = 1;
            timer.gameObject.transform.localPosition = new Vector3(775, 425, 0);
            if (BoardController.players != null) {
                players[0].jumper = characterModels[BoardController.currentPlayer.GetComponent<PlayerInfo>().characterIndex];
            }

        } else { //multiplayer!
            if (BoardController.players == null) {
                numPlayers = 4;
                //numPlayers = 2;
                players[0].jumper = characterModels[0];
            } else {
                numPlayers = BoardController.numPlayers;
                GameObject[] p = BoardController.originalOrderPlayers;
                for (int i = 0; i < numPlayers; i++) {
                    int charIndex = p[i].GetComponent<PlayerInfo>().characterIndex;
                    players[i].jumper = characterModels[charIndex];
                }
            }
            timer.gameObject.transform.localPosition = new Vector3(0, 425, 0);
        }
        //numPlayers = 2;
        for (int i = 0; i < numPlayers; i++) {
            players[i].jumper.gameObject.SetActive(true);
            players[i].gameObject.SetActive(true);
            platformFolders[i].SetActive(true);
        }

        directors[numPlayers - 1].stopped += SetupCameras;
        directors[4].stopped += SetupGame;
        directors[numPlayers - 1].Play();
    }

    public void SetupCameras(PlayableDirector dir) { 
        if (numPlayers == 1) {
            timer.gameObject.SetActive(true);
            Instructions.SetActive(true);
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

        for (int i = 0; i < numPlayers; i++) {
            playerCameras[i].gameObject.SetActive(true);
            players[i].gameObject.transform.GetChild(0).localPosition = new Vector3(0, yCameraPositions[numPlayers - 1], zCameraPositions[numPlayers - 1]);
        }

        directors[4].Play();
    }

    public void SetupGame(PlayableDirector dir) {
        categoryText.transform.parent.gameObject.SetActive(true);
        controls.Enable();
        if (!singleplayer) { 
            timer.ChangeToCountUp(true);
        }
        timer.ResetTimer();
        timer.gameObject.SetActive(true);
        timer.StartTimer();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void GetWords() {
        HashSet<int> chosenIndicies = new HashSet<int>();
        int random = UnityEngine.Random.Range(0, texts.Length);
        chosenIndicies.Add(random);
        TextAsset txtfile = texts[random];
        string[] words = txtfile.text.Split("\n"[0]);
        categoryText.text = words[0];
        //add to correct
        List<string> tempCorrect = new List<string>();
        for (int i = 1; i < words.Length; i++) {
            tempCorrect.Add(words[i]);
        }
        TreeTrunk.correctWords = tempCorrect;

        //get two random text files and add to incorrect
        List<string> tempWrong = new List<string>();
        random = UnityEngine.Random.Range(0, texts.Length);
        while (!chosenIndicies.Add(random)) {
            random = UnityEngine.Random.Range(0, texts.Length);
        }
        txtfile = texts[random];
        words = txtfile.text.Split("\n"[0]);
        for (int i = 1; i < words.Length; i++) {
            tempWrong.Add(words[i]);
        }

        random = UnityEngine.Random.Range(0, texts.Length);
        while (!chosenIndicies.Add(random))
        {
            random = UnityEngine.Random.Range(0, texts.Length);
        }
        txtfile = texts[random];
        words = txtfile.text.Split("\n"[0]);
        for (int i = 1; i < words.Length; i++)
        {
            tempWrong.Add(words[i]);
        }
        TreeTrunk.wrongWords = tempWrong;
    }
   

    private void AfterJump(int playerI, int result)
    {
        if (result == -1) {
            //failed jump
            StartCoroutine(DisableEnable(playerI, 2f, true));
        } else if (result == 0) {
            //success jump
            StartCoroutine(DisableEnable(playerI, 1.75f, false));
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
        players[playerI].jumper.GetComponent<Animator>().enabled = true;
        players[playerI].jumper.GetComponent<Animator>().Play("idle");
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
        timer.StopTimer();
        yield return new WaitForSeconds(1.5f);
        players[playerI].JumpToEnd();
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < numPlayers; i++) { 
            if (i == playerI) {
                players[playerI].jumper.gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
                players[playerI].jumper.transform.GetChild(0).GetComponent<Animator>().Play("victory");
            } else {
                players[i].jumper.transform.GetChild(0).GetComponent<Animator>().Play("lose");
            }
        }
        finishImage.SetActive(true);

        yield return new WaitForSeconds(4f);
        if (singleplayer) {
            EndGame(2);
        } else {
            EndMultiplayerGame(playerI);
        }

    }

    private void TimeOut() {
        //TODO add a cool timeout animation or something
        StartCoroutine("OnTimeOut");
    }

    private IEnumerator OnTimeOut() {
        players[0].jumper.transform.GetChild(0).GetComponent<Animator>().Play("lose");
        failImage.SetActive(true);
        yield return new WaitForSeconds(5f);
        EndGame(-1);
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
