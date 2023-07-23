using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class KataSpeedType : Minigame, Controls.ISpeedTypeActions
{
    // Start is called before the first frame update
    public TextAsset textFile;
    public TextMeshProUGUI centerText;
    public TextMeshProUGUI typing;
    public Timer gameTimer;
    public Timer loseTimer;
    public TextMeshProUGUI p1WinText;
    public TextMeshProUGUI p2WinText;
    public GameObject redX;
    public GameObject playerOnePlays;
    public GameObject playerTwoPlays;
    public GameObject finishedText;
    public TextMeshProUGUI winnerText;

    private string katakana;
    private string[] roman;
    private int currentI;
    private int prevI;
    Stack<string> typeStack;

    
    private bool playerOneTurn;
    private int playerOneWins = 0;
    private int playerTwoWins = 0;
    private bool noTypey;
        
    private Controls controls;

    public override void Start()
    {
        base.Start();
        typeStack = new Stack<string>();
        string[] split = textFile.text.Split("_"[0]);
        katakana = split[0];
        roman = split[1].Split(" "[0]);

        controls = new Controls();
        controls.SpeedType.AddCallbacks(this);
        gameTimer.TimeUp += Timeout;
        loseTimer.TimeUp += NoMoreX;

        playerOneTurn = true;
        StartCoroutine(StartTurn(playerOnePlays));
    }

    private IEnumerator StartTurn(GameObject turn) {

        turn.SetActive(true);
        yield return new WaitForSeconds(3f);
        turn.SetActive(false);
        redX.SetActive(false);
        loseTimer.gameObject.SetActive(false);
        noTypey = false;
        gameTimer.ResetTimer();
        NextCharacter();
        gameTimer.StartTimer();
        controls.SpeedType.Enable();
    }

    private void NextCharacter()
    {
        prevI = currentI;
        while (currentI == prevI) { 
            currentI = Random.Range(0, katakana.Length);
        }
        centerText.text = katakana[currentI] + "";
    }

    public void OnKeyboard(InputAction.CallbackContext context)
    {
        if (!context.performed || Input.inputString == "" || Input.inputString == " " || noTypey) { return; }

        if (Input.inputString == "\b")
        {
            if (typeStack.Count != 0)
            {
                Debug.Log("Backspace!");
                typeStack.Pop();
                typing.text = typing.text.Substring(0, typeStack.Count);
            }
        }
        else
        {
            Debug.Log(Input.inputString);
            if (Input.GetKeyDown("k"))
            {
                typeStack.Push("k"); //WHAT HAPPENED TO MY K KEY
            }
            else
            {
                typeStack.Push(Input.inputString);
            }
            HandleKeyboard();
        }
    }

    private void HandleKeyboard()
    {
        typing.text += typeStack.Peek();
        if (typeStack.Count == 1)
        {
            typing.text = typeStack.Peek();
        }

        if (typeStack.Count == roman[currentI].Length)
        {
            //check how many
            string word = roman[currentI];
            bool correct = true;
            for (int i = roman[currentI].Length - 1; i >= 0; i--)
            {
                if (!(roman[currentI][i] + "").Equals(typeStack.Pop().ToLower())) {
                    correct = false;
                }
            }
            if (correct)
            {
                if (playerOneTurn) { 
                    playerOneWins++;
                    p1WinText.text = playerOneWins + "";
                } else {
                    playerTwoWins++;
                    p2WinText.text = playerTwoWins + "";
                }
                NextCharacter();
            }
            else
            {
                loseTimer.ResetTimer();
                loseTimer.gameObject.SetActive(true);
                loseTimer.StartTimer();
                redX.SetActive(true);
                noTypey = true;
            }
        }

    }

    public void NoMoreX() {
        redX.SetActive(false);
        loseTimer.gameObject.SetActive(false);
        noTypey = false;
    }

    public void Timeout()
    {
        Debug.Log("Times up!");
        loseTimer.StopTimer();
        controls.SpeedType.Disable();
        if (playerOneTurn) {
            playerOneTurn = false;
            StartCoroutine(StartTurn(playerTwoPlays));
        } else {
            //End game!;
            StartCoroutine("FinishGame");
        }
        //yada yada yada switch to player two
    }

    private IEnumerator FinishGame() {
        //bro who cares about ties
        winnerText.text = playerOneWins >= playerTwoWins ? "Player One Wins!" : "Player Two Wins!";
        finishedText.SetActive(true);
        yield return new WaitForSeconds(10f);
        int result = playerOneWins >= playerTwoWins ? 1 : 2;
        EndGame(result);
    }


}
