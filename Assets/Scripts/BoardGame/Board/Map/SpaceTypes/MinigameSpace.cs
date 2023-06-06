using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSpace : BoardSpace
{
    //todo a class for each category? or a big switcharoo?
    //i like the big switcheraoo
    public string category;
    private string[] KatakanaGames = {"KatakanaScramble"};
    private string[] GrammarGames = { "WordOrder"};
    private string[] KanjiGames = { "KanjiCrossRotate" };

    private string[] gamesToChooseFrom;

    Minigame currentMinigame;

    //you can avoid the big switch statement by using an array and define enums or something but this is more easy to remember/work with

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        typeName = category;
        switch (category.ToUpper())
        {
            case "KATAKANA":
                gamesToChooseFrom = KatakanaGames;
                break;
            case "GRAMMAR":
                gamesToChooseFrom = GrammarGames;
                break;
            case "KANJI":
                gamesToChooseFrom = KanjiGames;
                break;
            default:
                Debug.Log("SPELLING ERROR PROBABLY OR NOT IMPLEMENTED: " + category);
                InvokeFinish();
                return;
        }
    }
    public override void Action()
    {
        Debug.Log("Minigame!");
        string chosenGame = gamesToChooseFrom[UnityEngine.Random.Range(0, gamesToChooseFrom.Length)];
        if (chosenGame.Equals("KanjiCrossRotate"))
        {
            KanjiCRotate.duel = category.ToUpper().Equals("DUEL");
        }
        InvokeLoad(chosenGame, true);
    }

}
