using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SpeedType : Minigame, Controls.ISpeedTypeActions
{
    // Start is called before the first frame update
    public TextAsset textFile;
    public TextMeshProUGUI centerText;
    public TextMeshProUGUI typing;
    public TextMeshProUGUI fullSentence;
    public TextMeshProUGUI yourSentence;
    public Timer timer;
    public TextMeshProUGUI liveText;
    public int totalLives;
    private string[] sentences;
    private HashSet<int> chosenSentences;

    private int[] fontSizes = new int[] { 400, 200, 100 };
    private string[] japnPieces;
    private string[] englPieces;
    private int index;
    Stack<string> typeStack;

    private Controls controls;

    public override void Start()
    {
        base.Start();
        typeStack = new Stack<string>();
        chosenSentences = new HashSet<int>();
        sentences = textFile.text.Split("\n"[0]);
        controls = new Controls();
        controls.SpeedType.AddCallbacks(this);
        timer.TimeUp += Timeout;

        SetupGame();
        controls.SpeedType.Enable();
    }

    private void SetupGame() {
        int random = Random.Range(0, sentences.Length);
        while (!chosenSentences.Add(random)) {
            random = Random.Range(0, sentences.Length);
        }
        

        string[] split = sentences[2].Split("_"[0]);
        japnPieces = split[0].Split(" "[0]);

        string temp = "";
        foreach (string s in japnPieces)
        {
            temp += (s);
        }
        fullSentence.text = temp;

        englPieces = split[1].Split(" "[0]);
        centerText.fontSize = fontSizes[japnPieces[index].Length - 1];
        centerText.text = japnPieces[0];
        index = 0;
        timer.ResetTimer();
        timer.StartTimer();

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnKeyboard(InputAction.CallbackContext context)
    {
        if (!context.performed || Input.inputString == "" || Input.inputString == " ") { return; }

        if (Input.inputString == "\b") {
            if (typeStack.Count != 0)
            {
                Debug.Log("Backspace!");
                typeStack.Pop();
                typing.text = typing.text.Substring(0, typeStack.Count);
            }
        } else
        {
            Debug.Log(Input.inputString);
            if (Input.GetKeyDown("k")) {
                typeStack.Push("k"); //WHAT HAPPENED TO MY K KEY
            } else { 
                typeStack.Push(Input.inputString);
            }
            HandleKeyboard();
        }
    }

    private void HandleKeyboard() {

        typing.text += typeStack.Peek();
        if (typeStack.Count == 1)
        {
            typing.text = typeStack.Peek();
        } 

        if (typeStack.Count == englPieces[index].Length)
        {
            //reached the count! 
            //check how many
            string word = englPieces[index];
            bool correct = true;
            for (int i = englPieces[index].Length - 1; i >= 0; i--) { 
                if (!(englPieces[index][i] + "").Equals(typeStack.Pop().ToLower())) {
                    correct = false;
                    break;
                }
            }
            if (correct) {
                yourSentence.text += japnPieces[index];
            } else {
                yourSentence.text += "<color=\"blue\">X";
                yourSentence.text += "<color=\"black\">";
            }
            MoveOnNext();
        } 

    }

    private void MoveOnNext() {
        //might be a better way to deal with this other than rebuilding everytime LOL
        typeStack.Clear();
        string temp = "";
        index++;
        for (int i = 0; i < japnPieces.Length; i++)
        {
            if (i == index)
            {
                temp += ("<u>" + japnPieces[i] + "</u>");
            }
            else
            {
                temp += japnPieces[i];
            }
        }
        if (index >= japnPieces.Length) {
            //Next Round!
            Debug.Log("Next Round!");
            timer.StopTimer();
        } else { 
            fullSentence.text = temp;
            centerText.fontSize = fontSizes[japnPieces[index].Length - 1];
            centerText.text = japnPieces[index];
        }
    }

    public void Timeout() {
        Debug.Log("Times up!");
    }

}
