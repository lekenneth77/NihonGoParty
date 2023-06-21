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
    private string[] KatakanaGames = {"KatakanaScramble", "KatakanaSearch", "KataSpeedType"}; //...katakana speed type, type as many characters?
    private string[] GrammarGames = { "WordOrder", "SpeedType"}; //speed type particle uhh smarticle, honor vs humble, give receive?
    private string[] KanjiGames = { "KanjiCrossRotate" }; //kanji dnd
    private string[] VocabGames = { "TreeHop" }; //some kind of trans versus intrans, english japanese match
    private string[] DuelGames = { "KanjiCrossRotate", "KataSpeedType"}; //katakana speed type
    private string[] MultiplayerGames = { "TreeHop", "QuizGame", "CountingGame" };


    private string[] gamesToChooseFrom;

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
            case "VOCAB":
                gamesToChooseFrom = VocabGames;
                break;
            case "DUEL":
                gamesToChooseFrom = DuelGames;
                break;
            case "MULTI":
                gamesToChooseFrom = MultiplayerGames;
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
        Minigame.singleplayer = !(category.ToUpper().Equals("DUEL") || category.ToUpper().Equals("MULTI"));
        InvokeLoad(chosenGame, true);
        //InvokeLoad("HowToPlayTemp", true); //TODO once how to plays are made, put them in here!
    }

}
