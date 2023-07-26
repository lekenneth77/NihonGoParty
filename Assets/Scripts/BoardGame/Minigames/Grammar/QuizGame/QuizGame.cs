using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QuizGame : Minigame, Controls.IQuizGameActions
{
    public TextAsset txtfile;
    public Timer timer;
    public Pedestal[] pedestals;
    private Vector3[] initialCameraPos = new Vector3[] { new Vector3(-6f * 0.45f, 6f * 0.45f, -15f * 0.45f), new Vector3(-3f * 0.45f, 6f * 0.45f, -15f * 0.45f), new Vector3(0, 6.5f * 0.45f, -17f * 0.45f)};
    public GameObject fullViewCam;
    public GameObject[] playerCameras;
    public GameObject[] resultObjects;
    public TextMeshProUGUI question;
    public TextMeshProUGUI[] answerChoices;
    public int numPlayers;

    private string[] problems;
    private HashSet<int> chosen;
    private int currentPlayerI;
    private int correctAnswerChoice;
    private bool allowAnswer;
    private bool noMorePeople;
    private int numAnswered;
    

    private Controls controls;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        controls = new Controls();
        controls.QuizGame.AddCallbacks(this);
        timer.TimeUp += Timeout;

        numPlayers = BoardController.numPlayers;
        numPlayers = 4;

        for (int i = 0; i < numPlayers; i++) {
            pedestals[i].gameObject.SetActive(true);
        }
        fullViewCam.transform.position = initialCameraPos[numPlayers - 2];
        problems = txtfile.text.Split("\n"[0]);
        chosen = new HashSet<int>();
        controls.QuizGame.Enable();
        StartCoroutine("SetupRound");
    }

    private IEnumerator SetupRound() {
        foreach(Pedestal ped in pedestals) {
            ped.ResetPedestal();
        }
        foreach (TextMeshProUGUI ac in answerChoices) {
            ac.transform.parent.GetComponent<Button>().enabled = false;
        }
        allowAnswer = false;
        timer.transform.parent.gameObject.SetActive(false);
        answerChoices[0].transform.parent.parent.gameObject.SetActive(false);
        playerCameras[currentPlayerI].SetActive(false);
        numAnswered = 0;

        //parse text here
        int random = Random.Range(0, problems.Length);
        while (!chosen.Add(random)) {
            random = Random.Range(0, problems.Length);
        }
        Debug.Log(random);
        string[] split = problems[random].Split("_"[0]);
        question.text = split[0];
        string[] answers = split[1].Split(","[0]);
        correctAnswerChoice = Random.Range(0, 4);
        Debug.Log(correctAnswerChoice);
        answerChoices[correctAnswerChoice].text = answers[0];
        HashSet<int> randomChoices = new HashSet<int>();
        randomChoices.Add(correctAnswerChoice);
        for (int i = 1; i < 4; i++) {
            int r = Random.Range(0, 4);
            while (!randomChoices.Add(r)) {
                r = Random.Range(0, 4);
            }
            answerChoices[r].text = answers[i];
        }
        //lol for now
        yield return new WaitForSeconds(2f);
        answerChoices[0].transform.parent.parent.gameObject.SetActive(true);
        noMorePeople = false;
        Debug.Log("Round Start!");
    }

    public void ChooseAnswer(int i) {
        if (!allowAnswer) { Debug.Log("Lol denied"); return; }
        allowAnswer = false;
        timer.StopTimer();
        foreach (TextMeshProUGUI ac in answerChoices) {
            ac.transform.parent.GetComponent<Button>().enabled = false;
        }
        numAnswered++;
        if (i - 1 == correctAnswerChoice) {
            Debug.Log("Correct Answer!");
            StartCoroutine("CorrectAnswer");
        } else {
            Debug.Log("Wrong Answer!");
            StartCoroutine(WrongAnswer(resultObjects[1]));
        }
    }

    public void Timeout() {
        Debug.Log("Times up!");
        foreach (TextMeshProUGUI ac in answerChoices) {
            ac.transform.parent.GetComponent<Button>().enabled = false;
        }
        StartCoroutine(WrongAnswer(resultObjects[2]));
    }

    private IEnumerator CorrectAnswer() {
        resultObjects[0].SetActive(true);
        yield return new WaitForSeconds(2f);
        resultObjects[0].SetActive(false);
        bool win = pedestals[currentPlayerI].Win();
        if (win) {
            StartCoroutine("FinishGame");
        } else { 
            StartCoroutine("SetupRound");
        }
    }

    private IEnumerator WrongAnswer(GameObject resultImg) {
        allowAnswer = false;
        resultImg.SetActive(true);
        yield return new WaitForSeconds(2f);
        resultImg.SetActive(false);
        pedestals[currentPlayerI].Loss();
        if (numAnswered == numPlayers) {
            //bruh someone mustve answered wrong
            Debug.Log("Choose a different question!");
            StartCoroutine("SetupRound");
        } else { 
            playerCameras[currentPlayerI].SetActive(false);
            noMorePeople = false;
            timer.transform.parent.gameObject.SetActive(false);
        }
    }

    private IEnumerator FinishGame() {
        resultObjects[3].SetActive(true);
        yield return new WaitForSeconds(5f);
        EndMultiplayerGame(currentPlayerI);
    }

    private void LetThemAnswer(int playerI) {
        noMorePeople = true;
        currentPlayerI = playerI;
        StartCoroutine(SetupAnswering(pedestals[playerI]));
    }

    private IEnumerator SetupAnswering(Pedestal ped) {
        ped.Answered();
        yield return new WaitForSeconds(1f);
        playerCameras[currentPlayerI].SetActive(true);
        timer.ResetTimer();
        timer.transform.parent.gameObject.SetActive(true);
        timer.StartTimer();
        foreach (TextMeshProUGUI ac in answerChoices) {
            ac.transform.parent.GetComponent<Button>().enabled = true;
        }
        allowAnswer = true;
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed || pedestals[0].unableToAnswer || noMorePeople) { return; }
        LetThemAnswer(0);
    }

    public void OnR(InputAction.CallbackContext context)
    {
        if (!context.performed || pedestals[1].unableToAnswer || noMorePeople) { return; }
        LetThemAnswer(1);

    }

    public void OnH(InputAction.CallbackContext context)
    {
        if (!context.performed || numPlayers < 3 || pedestals[2].unableToAnswer || noMorePeople) { return; }
        LetThemAnswer(2);

    }

    public void OnL(InputAction.CallbackContext context)
    {
        if (!context.performed || numPlayers < 4 || pedestals[3].unableToAnswer || noMorePeople) { return; }
        LetThemAnswer(3);
    }
}
