using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TunnelRunner : Minigame, Controls.ITunnelRunnerActions
{
    //camera work and setup
    public Camera[] playerCams;
    public GameObject[] characters;
    private Vector3[] characterCamPos = new Vector3[] { new Vector3(0, 1.5f, -12f), new Vector3(0f, 3f, -12f), new Vector3(0f, 3f, -12f), new Vector3(0f, 2.75f, -12f), new Vector3(0f, 3f, -12f) };
    public PlayableDirector[] directors;
    private Vector3[] spawnPts = new Vector3[] { new Vector3(0, 0, -9f), new Vector3(40f, 0, -9f) };
    public Cave[] defaultCaves; //DUCTTAPEPPPP

    //ui
    public GameObject congratImg;
    public GameObject failureImg;
    public Timer timer;
    public static Timer ductTapeTimer;

    //final screen
    public Image whiteScreen;
    public Transform finalWP;
    public GameObject finalCam;
    public GameObject lighting;

    //games
    public Cave p1Cave;
    public Cave p2Cave;
    public TextAsset textFile;
    private int p1Points;
    private int p2Points;
    private Controls controls;
    public int caveLimit;
    // Start is called before the first frame update
    public override void Start()
    {
        if (SceneManager.sceneCount > 1) {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        }
        ductTapeTimer = timer;
        controls = new Controls();
        controls.TunnelRunner.AddCallbacks(this);
        Cave.chosenQ = new HashSet<int>();
        Cave.questions = textFile.text.Split("\n"[0]);

        //singleplayer = true;

        
        if (singleplayer) {
            int characterI = 4;
            if (BoardController.players != null) { 
                characterI = BoardController.currentPlayer.GetComponent<PlayerInfo>().characterIndex;
            }
            p1Cave.player = characters[characterI].GetComponent<MoveObject>();
            p1Cave.followCam.transform.position = characterCamPos[characterI];
            p1Cave.followCam.GetComponent<FollowTwoAxis>().follow = p1Cave.player.transform;
            p1Cave.player.transform.position = spawnPts[0];
            defaultCaves[0].player = p1Cave.player;
            defaultCaves[0].followCam = p1Cave.followCam;
            p1Cave.player.gameObject.SetActive(true);
            p1Cave.ChangeText("", "", "", false);
        } else { 
            //get the duel model from board controller
            for (int i = 0; i < 2; i++) {
                int characterI = BoardController.duelists[i].GetComponent<PlayerInfo>().characterIndex;
                Cave cave = i == 0 ? p1Cave : p2Cave;

                cave.player = characters[characterI].GetComponent<MoveObject>();
                cave.followCam.transform.position = characterCamPos[characterI];
                cave.followCam.GetComponent<FollowTwoAxis>().follow = cave.player.transform;
                cave.player.transform.position = spawnPts[i];
                defaultCaves[i].player = cave.player;
                defaultCaves[i].followCam = cave.followCam;
                cave.player.gameObject.SetActive(true);
                cave.ChangeText("", "", "", false);
            }
        }
        int index = 4; 
        if (BoardController.players != null) { 
            index = BoardController.currentPlayer.GetComponent<PlayerInfo>().characterIndex;
        }
        directors[index].stopped += StartGame;
        directors[index].gameObject.SetActive(true);
        directors[index].Play();
        Cave.caveLimit = caveLimit;
    }

    public void StartGame(PlayableDirector dir) {
        int index = 4; 
        if (BoardController.players != null) { 
            index = BoardController.currentPlayer.GetComponent<PlayerInfo>().characterIndex;
        }
        directors[index].gameObject.SetActive(false);
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
        Time.timeScale = 0;
        p2Cave.StopAllCoroutines();
        p2Cave.player.enabled = false;
        controls.TunnelRunner.LeftArrowKey.Disable();
        controls.TunnelRunner.RightArrowKey.Disable();
        Time.timeScale = 1;

        timer.StopTimer();
        StartCoroutine(FinalCutscene(true));
    }

    private IEnumerator FinalCutscene(bool p1) {
        Cave winner;
        if (p1) {
            winner = p1Cave;
        } else {
            winner = p2Cave;
        }

        for (int i = 0; i < 100; i++)
        {
            Color temp = whiteScreen.color;
            temp.a += 0.01f;
            whiteScreen.color = temp;
            yield return new WaitForSeconds(0.01f);
        }
        lighting.SetActive(true);
        finalCam.SetActive(true);
        winner.player.gameObject.transform.position = finalWP.position;
        winner.player.gameObject.transform.eulerAngles = new Vector3(0, 270f, 0);
        winner.player.transform.GetChild(1).gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        if (!singleplayer) { 
            if (p1) {
                playerCams[1].gameObject.SetActive(false);
                playerCams[0].rect = new Rect(0, 0, 1f, 1);
            } else {
                playerCams[0].gameObject.SetActive(false);
                playerCams[1].rect = new Rect(0, 0, 1f, 1);
            }
        }
        whiteScreen.gameObject.SetActive(false);
        winner.player.transform.GetChild(0).GetComponent<Animator>()?.Play("victory");
        congratImg.SetActive(true);

        yield return new WaitForSeconds(5f);
        if (singleplayer) {
            EndGame(2);
            yield break;
        }

        if (p1) { 
            EndGame(1);
        } else {
            EndGame(2);
        }

    }

    public void P2NewCaveReached(Cave newCave, bool correct)
    {
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
        Time.timeScale = 0;
        p1Cave.StopAllCoroutines();
        p1Cave.player.enabled = false;
        controls.TunnelRunner.A.Disable();
        controls.TunnelRunner.D.Disable();
        Time.timeScale = 1;
        timer.StopTimer();
        StartCoroutine(FinalCutscene(false));
    }

    public void Timeout() {
        StartCoroutine("TIMEOVER");
    }

    private IEnumerator TIMEOVER() {
        whiteScreen.color = Color.black; //LOL LAZY
        Color temp = whiteScreen.color;
        temp.a = 0;
        whiteScreen.color = temp;

        for (int i = 0; i < 100; i++)
        {
            temp = whiteScreen.color;
            temp.a += 0.01f;
            whiteScreen.color = temp;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1f);
        failureImg.SetActive(true);
        yield return new WaitForSeconds(5f);
        EndGame(-1);
    }

    public static void StopThatTimer() {
        ductTapeTimer.StopTimer();
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
