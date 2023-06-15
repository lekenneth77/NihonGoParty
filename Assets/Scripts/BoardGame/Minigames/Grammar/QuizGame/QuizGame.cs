using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuizGame : Minigame, Controls.IQuizGameActions
{
    public Timer timer;
    public Pedestal[] pedestals;
    public GameObject[] playerCameras;
    public GameObject[] resultObjects;
    public int numPlayers;

    //lol lazy lets make this the only game who is a duel and multiplayer! lololol
    public static bool multiplayer;

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
        controls.QuizGame.Enable();

        timer.TimeUp += Timeout;
        StartCoroutine("SetupRound");
    }

    private IEnumerator SetupRound() {
        foreach(Pedestal ped in pedestals) {
            ped.ResetPedestal();
        }
        timer.gameObject.SetActive(false);
        playerCameras[currentPlayerI].SetActive(false);
        numAnswered = 0;

        //parse text here

        //lol for now
        correctAnswerChoice = Random.Range(1, 5);
        yield return new WaitForSeconds(2f);
        noMorePeople = false;
        Debug.Log("Round Start!");
    }

    public void ChooseAnswer(int i) { 
        if (!allowAnswer) { Debug.Log("Lol denied"); return; }
        allowAnswer = false;
        timer.StopTimer();
        numAnswered++;
        if (i == correctAnswerChoice) {
            Debug.Log("Correct Answer!");
            StartCoroutine("CorrectAnswer");
        } else {
            Debug.Log("Wrong Answer!");
            StartCoroutine(WrongAnswer(resultObjects[1]));
        }
    }

    public void Timeout() {
        Debug.Log("Times up!");
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
            timer.gameObject.SetActive(false);
        }
    }

    private IEnumerator FinishGame() {
        Debug.Log("Finish!");
        yield return new WaitForSeconds(2f);
        if (multiplayer) {
            EndMultiplayerGame(currentPlayerI);
        } else {
            //duel!
            EndGame(currentPlayerI == 0);
        }
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
        timer.gameObject.SetActive(true);
        timer.StartTimer();
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

    public void OnP(InputAction.CallbackContext context)
    {
        if (!context.performed || numPlayers < 4 || pedestals[3].unableToAnswer || noMorePeople) { return; }
        LetThemAnswer(3);

    }

}
