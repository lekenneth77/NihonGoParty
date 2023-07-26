using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class LocationSeeker : Minigame, Controls.IQuizGameActions
{
    public TextAsset txtfile;
    public Image theImage;
    public Transform theButtons;
    public Transform theAnswers;
    public GameObject[] thePlayers;
    public Timer timer;
    public int numPlayers;
    public TextMeshProUGUI roundText;
    public GameObject helpText;
    public TextMeshProUGUI[] pointText;

    private int currentPlayer;
    private int correctAnswerIndex;
    private int currentAnswerer;
    private int[] playerPoints = new int[4];
    private bool[] cancelled = new bool[4];
    private bool nono;
    private int numSquaresChosen;
    private int numCancelled;
    private Controls controls;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        //numPlayers = BoardController.numPlayers;

        controls = new Controls();
        controls.QuizGame.AddCallbacks(this);
        correctAnswerIndex = 2;
        timer.TimeUp += Timeout;

        for (int i = 0; i < numPlayers; i++) {
            thePlayers[i].SetActive(true);
        }

        Setup();
    }

    private void Setup() { 
        for (int i = 0; i < numPlayers; i++) {
            cancelled[i] = false;
            thePlayers[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        foreach (Transform child in theButtons)
        {
            child.gameObject.SetActive(true);
            child.gameObject.GetComponent<Button>().enabled = true;
        }
        theAnswers.gameObject.SetActive(false);
        //choose image and answers here

        currentPlayer = 0;
        NextRound();
    }

    private void NextRound() {
        nono = true;
        controls.QuizGame.Disable();
        foreach (Transform child in theButtons)
        {
            child.gameObject.GetComponent<Button>().enabled = true;
        }
        currentPlayer++;
        currentPlayer = currentPlayer > numPlayers ? 1 : currentPlayer;
        
        while (cancelled[currentPlayer - 1]) {
            currentPlayer++;
        }
        roundText.text = "Player " + currentPlayer + " Choose a Square";
        roundText.transform.parent.gameObject.SetActive(true);
    }

    public void ChooseSquare() {
        numSquaresChosen++;
        if (numSquaresChosen == 16) {
            Debug.Log("no more!");
            //idk figure something out
            return;
        }
        foreach(Transform child in theButtons) {
            child.gameObject.GetComponent<Button>().enabled = false;
        }

        helpText.SetActive(true);
        roundText.transform.parent.gameObject.SetActive(false);
        nono = false;
        timer.ResetTimer();
        timer.transform.parent.gameObject.SetActive(true);
        timer.StartTimer();
        controls.QuizGame.Enable();

    }

    public void Timeout() {
        timer.transform.parent.gameObject.SetActive(false);
        helpText.SetActive(false);
        NextRound();
    }

    public void Answer(int i) {
        //lol
        theAnswers.gameObject.SetActive(false);
        if (i == correctAnswerIndex) {
            Debug.Log("nice");
            playerPoints[currentAnswerer]++;
            pointText[currentAnswerer].text = playerPoints[currentAnswerer] + "";
            if (playerPoints[currentAnswerer] == 2) {
                //finish the round
                StartCoroutine("Finish");
            } else {
                //setup next round
                thePlayers[currentAnswerer].GetComponent<Animator>().Play("GoDown");
                Setup();
            }
        } else {
            Debug.Log("thats wrong");
            cancelled[currentAnswerer] = true;
            thePlayers[currentAnswerer].GetComponent<Animator>().Play("GoDown");
            thePlayers[currentAnswerer].transform.GetChild(2).gameObject.SetActive(true);
            numCancelled++;
            if (numCancelled == numPlayers) {
                //uhhhhhhh
                Setup();
            } else { 
                NextRound();
            }
        }
    }

    private IEnumerator Finish() {
        roundText.text = "Player " + currentAnswerer + 1 + " Has Won!";
        roundText.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        EndGame(currentAnswerer);
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed || cancelled[0] || nono) { return; }
        LetAnswer(0);
    }

    public void OnR(InputAction.CallbackContext context)
    {
        if (!context.performed || cancelled[1] || nono) { return; }
        LetAnswer(1);
    }

    public void OnH(InputAction.CallbackContext context)
    {
        if (!context.performed || cancelled[2] || nono || numPlayers < 3) { return; }
        LetAnswer(2);

    }
    public void OnL(InputAction.CallbackContext context)
    {
        if (!context.performed || cancelled[3] || nono || numPlayers < 4) { return; }
        LetAnswer(3);
    }

    public void LetAnswer(int index) {
        nono = true;
        controls.QuizGame.Disable();
        timer.StopTimer();
        timer.transform.parent.gameObject.SetActive(false);
        helpText.SetActive(false);
        currentAnswerer = index;
        thePlayers[index].GetComponent<Animator>().Play("GoUp");
        theAnswers.gameObject.SetActive(true);
        
    }

    
}
