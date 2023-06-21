using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CountingGame : Minigame, Controls.IQuizGameActions
{
    public int numPlayers;
    public GameObject[] pedestals;
    public GameObject[] leaderboard;
    public TextMeshProUGUI currentCounter;
    public GameObject fullViewCam;
    private Vector3[] initialCameraPos = new Vector3[] { new Vector3(-6f, 7f, -20f), new Vector3(-3f, 7f, -20f), new Vector3(0, 7f, -20f) };
    public GameObject nextRoundImage;
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

    private TextAsset[] counters;
    private HashSet<int> chosens;
    private HashSet<int> wrongChosens;

    //parameters
    private int[] correctCounts;
    private int[] wrongCounts;
    private float[] minSpeed;
    private float[] maxSpeed;
    private float[] minWaitTime;
    private float[] maxWaitTime;

    

    private Controls controls;

    public override void Start()
    {
        base.Start();

        counters = Resources.LoadAll<TextAsset>("Minigames/Vocabulary/CountingGame/Texts/");

        chosens = new HashSet<int>();
        wrongChosens = new HashSet<int>();
        controls = new Controls();
        controls.QuizGame.AddCallbacks(this);

        numPlayers = BoardController.numPlayers;

        for (int i = 0; i < numPlayers; i++)
        {
            pedestals[i].SetActive(true);
            leaderboard[i].SetActive(true);
        }
        fullViewCam.transform.position = initialCameraPos[numPlayers - 2];

        correctOnes = new List<GameObject>();
        wrongOnes = new List<GameObject>();

        correctCounts = new int[] { Random.Range(3, 7), Random.Range(5, 10), Random.Range(1, 10) };
        wrongCounts = new int[] { Random.Range(4, 6), Random.Range(6, 8), Random.Range(8, 10) }; //due to jank, this will be multiplied by 2!
        minSpeed = new float[] { 5f, 7f, 5f};
        maxSpeed = new float[] { 5f, 10f, 15f };
        minWaitTime = new float[] { 0.5f, 0.25f, 0.15f };
        maxWaitTime = new float[] { 2f, 1f, 0.5f };

        Runner.ReachedEnd += SomeoneGotToEnd;

        round = 0;
        StartCoroutine(StartRound(correctCounts[round], wrongCounts[round], minSpeed[round], maxSpeed[round], minWaitTime[round], maxWaitTime[round], round != 0));
    }

    private IEnumerator StartRound(int numCorr, int numWrong, float minS, float maxS, float minWait, float maxWait, bool disableImg)
    {
        wrongOnes.Clear(); //just to make sure
        correctOnes.Clear();
        wrongChosens.Clear();


        for (int i = 0; i < numPlayers; i++) {
            counts[i] = 0;
            pedestals[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = counts[i] + "";
        }

        if (disableImg) { 
            yield return new WaitForSeconds(5f);
            //choose new counter category
            nextRoundImage.SetActive(false);
        }

        int index = Random.Range(0, counters.Length);
        while (!chosens.Add(index))
        {
            index = Random.Range(0, counters.Length);
        }
        GetRunners(correctOnes, numCorr, index, minS, maxS);
        wrongChosens.Add(index);
        correctNumber = numCorr;
        GetRunners(wrongOnes, numWrong, -1, minS, maxS);
        GetRunners(wrongOnes, numWrong, -1, minS, maxS);

        Debug.Log(correctNumber + " " + (numWrong * 2));
        totalNumber = correctNumber + (numWrong * 2);
        yield return new WaitForSeconds(1f); //TODO ADD A STARTING SOMETHING ONCE DONE REMOVE THIS
        controls.QuizGame.Enable();
        for (int i = 0; i < correctNumber + (numWrong * 2); i++)
        {
            if (correctOnes.Count == 0)
            {
                wrongOnes[0].GetComponent<Runner>().Run();
                wrongOnes.RemoveAt(0);
            }
            else if (wrongOnes.Count == 0)
            {
                correctOnes[0].GetComponent<Runner>().Run();
                correctOnes.RemoveAt(0);
            }
            else
            {
                if (Random.Range(0, 2) == 0)
                {
                    wrongOnes[0].GetComponent<Runner>().Run();
                    wrongOnes.RemoveAt(0);
                }
                else
                {
                    correctOnes[0].GetComponent<Runner>().Run();
                    correctOnes.RemoveAt(0);
                }
            }
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        }
    }

    private void GetRunners(List<GameObject> list, int count, int index, float minS, float maxS) {
        //a very specific laziness
        if (wrongChosens.Count > 0) {
            index = Random.Range(0, counters.Length);
            while (!wrongChosens.Add(index)) {
                index = Random.Range(0, counters.Length);
            }
        }
        string[] splitted = counters[index].text.Split("\n"[0]);
        if (wrongChosens.Count == 0) { 
            //god i really should've seperated these functions eh?
            string[] title = splitted[0].Split("_"[0]);
            currentCounter.text = title[0];
            //title[1] is the text for what the counter is used for, eventually TODO add a results study screen?
        }
        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(1, splitted.Length);
            if (wrongChosens.Count == 0) { Debug.Log(splitted[random]); }
            list.Add(CreateRunner(Random.Range(minS, maxS), splitted[random]));
        }
    }

    private IEnumerator Finish() {
        yield return new WaitForSeconds(5f);
        nextRoundImage.SetActive(false);
        int max = -1;
        int gamer = 0;
        for (int i = 0; i < numPlayers; i++) { 
            if (points[i] > max) {
                //handle ties...eventually!
                max = points[i];
                gamer = i;
            }
        }
        Debug.Log("Winner Index: " + gamer);
        EndMultiplayerGame(gamer);
    }

    public GameObject CreateRunner(float runSpeed, string text) {
        GameObject runner = Instantiate(defRunner, runnerFolder);
        runner.transform.localPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].localPosition;
        runner.GetComponent<Runner>().runSpeed = runSpeed;
        runner.GetComponent<Runner>().ChangeText(text); //LOL
        return runner;
    }

    public void SomeoneGotToEnd() {
        totalNumber--;
        if (totalNumber <= 0) {
            Debug.Log("We're done!");
            controls.QuizGame.Disable();
            nextRoundImage.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "The correct count was " + correctNumber + "!";
            nextRoundImage.SetActive(true);
            //count points
            //TODO handle ties later make the game work first, always could just use a spinner LOL
            int min = 1000;
            int min_index = -1;
            for (int i = 0; i < counts.Length; i++) { 
                if (counts[i] == correctNumber) {
                    points[i]++;
                    leaderboard[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = points[i] + "";
                    min = -1; //at least one person got the right number!
                } else if (Mathf.Abs(correctNumber - counts[i]) < min) {
                    min = Mathf.Abs(correctNumber - counts[i]);
                    min_index = i;
                }
            }

            if (min < 1000 && min != -1) {
                points[min_index]++;
                leaderboard[min_index].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = points[min_index] + "";
            }

            //set up next round!
            round++;
            if (round == 3)
            {
                StartCoroutine("Finish");
            } else {
                StartCoroutine(StartRound(correctCounts[round], wrongCounts[round], minSpeed[round], maxSpeed[round], minWaitTime[round], maxWaitTime[round], round != 0));
            }
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
