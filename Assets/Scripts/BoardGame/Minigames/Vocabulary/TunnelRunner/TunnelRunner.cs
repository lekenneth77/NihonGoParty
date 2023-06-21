using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TunnelRunner : Minigame, Controls.ITunnelRunnerActions
{
    public Camera[] playerCams;
    public Cave p1Cave;
    public Cave p2Cave;
    public TextAsset textFile;
    public GameObject congratImg;
    public Timer timer;
    private int p1Points;
    private int p2Points;
    private Controls controls;
    // Start is called before the first frame update
    public override void Start()
    {
        singleplayer = false;
        controls = new Controls();
        controls.TunnelRunner.AddCallbacks(this);
        Cave.chosenQ = new HashSet<int>();
        Cave.questions = textFile.text.Split("\n"[0]);
        p1Cave.GetWords();
        if (singleplayer) {
            timer.ResetTimer();
            timer.gameObject.SetActive(true);
            timer.TimeUp += Timeout;
            p1Cave.gameObject.SetActive(true);
        } else {
            playerCams[0].rect = new Rect(0, 0, 0.5f, 1);
            playerCams[1].rect = new Rect(0.5f, 0, 0.5f, 1);
            playerCams[1].gameObject.SetActive(true);
            p1Cave.gameObject.SetActive(true);
            p2Cave.GetWords();
            p2Cave.gameObject.SetActive(true);
            p2Cave.player.gameObject.SetActive(true);
            p2Cave.followCam.gameObject.SetActive(true);
            p2Cave.endMove += P2NewCaveReached;
            p2Cave.reachedFinish += P2Done;
        }

        p1Cave.endMove += P1NewCaveReached;
        p1Cave.reachedFinish += P1Done;
        timer.StartTimer();
        controls.TunnelRunner.Enable();
    }


    public void P1NewCaveReached(Cave newCave, bool correct) {
        Debug.Log("Here we are!");
        p1Cave.endMove -= P1NewCaveReached;
        p1Cave.reachedFinish -= P1Done;

        Destroy(p1Cave.gameObject);
        if (correct) { p1Points++; }
        p1Cave = newCave;
        p1Cave.endMove += P1NewCaveReached;
        p1Cave.reachedFinish += P1Done;

        controls.TunnelRunner.A.Enable();
        controls.TunnelRunner.D.Enable();
    }

    public void P1Done() {
        Debug.Log("Nice!");
        Time.timeScale = 0;
        p2Cave.StopAllCoroutines();
        p2Cave.player.enabled = false;
        controls.TunnelRunner.LeftArrowKey.Disable();
        controls.TunnelRunner.RightArrowKey.Disable();
        Time.timeScale = 1;

        timer.StopTimer();
        congratImg.SetActive(true);
        EndGame(true);
    }

    public void P2NewCaveReached(Cave newCave, bool correct)
    {
        Debug.Log("Here we are!");
        p2Cave.endMove -= P2NewCaveReached;
        p2Cave.reachedFinish -= P2Done;

        Destroy(p2Cave.gameObject);
        if (correct) { p2Points++; }
        p2Cave = newCave;
        p2Cave.endMove += P2NewCaveReached;
        p2Cave.reachedFinish += P2Done;

        controls.TunnelRunner.LeftArrowKey.Enable();
        controls.TunnelRunner.RightArrowKey.Enable();
    }

    public void P2Done()
    {
        Debug.Log("Nice!");
        Time.timeScale = 0;
        p1Cave.StopAllCoroutines();
        p1Cave.player.enabled = false;
        controls.TunnelRunner.A.Disable();
        controls.TunnelRunner.D.Disable();
        Time.timeScale = 1;
        timer.StopTimer();
        congratImg.SetActive(true);
        EndGame(false);
    }

    public void Timeout() {
        StartCoroutine("TIMEOVER");
    }

    private IEnumerator TIMEOVER() {
        Debug.Log("TIME OUT");
        yield return new WaitForSeconds(3f);
        EndGame(false);
    }


    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        p1Cave.GoLeft(p1Points);
        controls.TunnelRunner.A.Disable();
        controls.TunnelRunner.D.Disable();
    }
    public void OnD(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        p1Cave.GoRight(p1Points);
        controls.TunnelRunner.A.Disable();
        controls.TunnelRunner.D.Disable();
    }

    public void OnLeftArrowKey(InputAction.CallbackContext context)
    {
        if (!context.performed || singleplayer) { return; }
        p2Cave.GoLeft(p2Points);
        controls.TunnelRunner.LeftArrowKey.Disable();
        controls.TunnelRunner.RightArrowKey.Disable();
    }

    public void OnRightArrowKey(InputAction.CallbackContext context)
    {
        if (!context.performed || singleplayer) { return; }
        p2Cave.GoRight(p2Points);
        controls.TunnelRunner.LeftArrowKey.Disable();
        controls.TunnelRunner.RightArrowKey.Disable();
    }

  
}
