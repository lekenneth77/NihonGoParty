using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject playerOnePlays;
    public GameObject playerTwoPlays;
    public GameObject finishedText;
    public Image[] playerIcons;
    public Image p1TurnIcon;
    public Image p2TurnIcon;
    public Image winnerIcon;
    public Animator its345AM;

    private Sprite[] duelistIcons = new Sprite[2];
    private Sprite[] charPortraits;

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
        charPortraits = Resources.LoadAll<Sprite>("Images/CharacterPortraits/");

        typeStack = new Stack<string>();
        string[] split = textFile.text.Split("_"[0]);
        katakana = split[0];
        roman = split[1].Split(" "[0]);

        controls = new Controls();
        controls.SpeedType.AddCallbacks(this);
        gameTimer.TimeUp += Timeout;
        loseTimer.TimeUp += NoMoreX;
        playerOneTurn = true;

        if (BoardController.players != null) {
            GameObject[] duelists = BoardController.duelists;
            for (int i = 0; i < 2; i++) {
                int charIndex = duelists[i].GetComponent<PlayerInfo>().characterIndex;
                duelistIcons[i] = charPortraits[charIndex];
            }
        } else {
            duelistIcons[0] = charPortraits[0];
            duelistIcons[1] = charPortraits[1];
        }
        playerIcons[0].sprite = duelistIcons[0];
        playerIcons[1].sprite = duelistIcons[1]; 
        StartCoroutine(StartTurn(playerOnePlays));
    }

    private IEnumerator StartTurn(GameObject turn) {

        turn.SetActive(true);
        yield return new WaitForSeconds(3f);
        turn.SetActive(false);
        if (playerOneTurn) {
            playerIcons[0].transform.parent.GetComponent<Image>().color = new Color(0, 194f / 255f, 1f);
            playerIcons[1].transform.parent.GetComponent<Image>().color = Color.white;
        } else {
            playerIcons[1].transform.parent.GetComponent<Image>().color = new Color(0, 194f / 255f, 1f);
            playerIcons[0].transform.parent.GetComponent<Image>().color = Color.white;
        }
        typeStack.Clear();
        typing.text = "";
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
        its345AM.Play("idle");
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
                    its345AM.Play("p1");
                } else {
                    playerTwoWins++;
                    p2WinText.text = playerTwoWins + "";
                    its345AM.Play("p2");
                }
                NextCharacter();
            }
            else
            {
                loseTimer.ResetTimer();
                loseTimer.gameObject.SetActive(true);
                loseTimer.StartTimer();
                noTypey = true;
            }
        }

    }

    public void NoMoreX() {
        loseTimer.gameObject.SetActive(false);
        noTypey = false;
    }

    public void Timeout()
    {
        Debug.Log("Times up!");
        loseTimer.StopTimer();
        controls.SpeedType.Disable();
        centerText.text = "";
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
        int result = playerOneWins >= playerTwoWins ? 1 : 2;
        winnerIcon.sprite = duelistIcons[result - 1];
        playerIcons[0].transform.parent.GetComponent<Image>().color = Color.white;
        playerIcons[1].transform.parent.GetComponent<Image>().color = Color.white;
        playerIcons[result - 1].transform.parent.GetComponent<Image>().color = new Color(1f, 245f/255f, 0f);

        finishedText.SetActive(true);
        yield return new WaitForSeconds(8f);
        EndGame(result);
    }


}
