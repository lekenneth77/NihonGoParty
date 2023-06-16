using System;
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

    private string[] japnPieces;
    private string[] englPieces;
    private int index = 0;
    Stack<string> typeStack;

    private Controls controls;

    public override void Start()
    {
        base.Start();
        typeStack = new Stack<string>();
        controls = new Controls();
        controls.SpeedType.AddCallbacks(this);
        SetupGame();
        controls.SpeedType.Enable();
    }

    private void SetupGame() {
        typeStack = new Stack<string>();
        string[] split = textFile.text.Split("_"[0]);
        japnPieces = split[0].Split(" "[0]);

        string temp = "";
        foreach (string s in japnPieces)
        {
            temp += (s);
        }
        fullSentence.text = temp;

        englPieces = split[1].Split(" "[0]);
        centerText.text = japnPieces[0];

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnKeyboard(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
       
        if (Input.inputString == "\b") {
            if (typeStack.Count != 0)
            {
                Debug.Log("Backspace!");
                typeStack.Pop();
                typing.text = typing.text.Substring(0, typeStack.Count);
            }
        } else
        {
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
                    Debug.Log("Wrong!");
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
        fullSentence.text = temp;
        centerText.text = japnPieces[index];
    }

}
