using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CountingGame : Minigame, Controls.IQuizGameActions
{
    public int numPlayers;
    public GameObject[] pedestals;
    public GameObject fullViewCam;
    private Vector3[] initialCameraPos = new Vector3[] { new Vector3(-6f, 6f, -15f), new Vector3(-3f, 6f, -15f), new Vector3(0, 7f, -20f) };
    public Transform[] spawnPoints;
    public Transform runnerFolder;

    public GameObject defRunner;

    private int[] counts = new int[4];
    private int[] points = new int[4];
    private int round = 1;
    private int correctNumber;
    private int totalNumber;
    private List<GameObject> correctOnes;
    private List<GameObject> wrongOnes;


    private Controls controls;

    public override void Start()
    {
        base.Start();

        controls = new Controls();
        controls.QuizGame.AddCallbacks(this);

        //numPlayers = multiplayer ? BoardController.numPlayers : 2;

        for (int i = 0; i < numPlayers; i++)
        {
            pedestals[i].SetActive(true);
        }
        fullViewCam.transform.position = initialCameraPos[numPlayers - 2];

        correctOnes = new List<GameObject>();
        wrongOnes = new List<GameObject>();

        Runner.ReachedEnd += SomeoneGotToEnd;

        controls.QuizGame.Enable();
        StartCoroutine("RoundOne");
    }

    private IEnumerator RoundOne() {
        correctNumber = Random.Range(5, 10);
        for (int i = 0; i < correctNumber; i++) {
            correctOnes.Add(CreateRunner());
        }
        int wrongCount = Random.Range(5, 10);
        Debug.Log(correctNumber + " " + wrongCount);
        for (int i = 0; i < wrongCount; i++) {
            wrongOnes.Add(CreateRunner());
        }
        totalNumber = correctNumber + wrongCount;
        yield return new WaitForSeconds(1f); //BRUHHH ADD A STARTING SOMETHING ONCE DONE REMOVE THIS
        for (int i = 0; i < correctNumber + wrongCount; i++) {
            if (correctOnes.Count == 0) {
                wrongOnes[0].GetComponent<Runner>().Run();
                wrongOnes.RemoveAt(0);
            } else if (wrongOnes.Count == 0) {
                correctOnes[0].GetComponent<Runner>().Run();
                correctOnes.RemoveAt(0);
            } else
            {
                if (Random.Range(0, 2) == 0) {
                    wrongOnes[0].GetComponent<Runner>().Run();
                    wrongOnes.RemoveAt(0);
                } else {
                    correctOnes[0].GetComponent<Runner>().Run();
                    correctOnes.RemoveAt(0);
                }
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    public GameObject CreateRunner() {
        GameObject runner = Instantiate(defRunner, runnerFolder);
        runner.transform.localPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].localPosition;
        runner.GetComponent<Runner>().runSpeed = 5f;
        return runner;
    }

    public void SomeoneGotToEnd() {
        totalNumber--;
        if (totalNumber <= 0) {
            Debug.Log("We're done!");
            controls.QuizGame.Disable();
            //set up next round!
        }
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        counts[0]++;
        pedestals[0].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = counts[0] + "";
    }

    public void OnR(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        counts[1]++;
        pedestals[1].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = counts[1] + "";
    }

    public void OnH(InputAction.CallbackContext context)
    {
        if (!context.performed || numPlayers < 3) { return; }
        counts[2]++;
        pedestals[2].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = counts[2] + "";
    }

    public void OnP(InputAction.CallbackContext context)
    {
        if (!context.performed || numPlayers < 4) { return; }
        counts[3]++;
        pedestals[3].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = counts[3] + "";
    }


}
