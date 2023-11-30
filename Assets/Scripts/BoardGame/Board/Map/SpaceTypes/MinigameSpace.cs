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
    public MinigameSelector selector;
    private int categoryIndex;
    private string[] KatakanaGames = { "KatakanaScramble", "KatakanaSearch", "AppleDrop" };
    private string[] GrammarGames = { "WordOrder2", "SpeedType", "GiveReceive", "ConjugationBlaster" };// particles please
    private string[] KanjiGames = { "KanjiCrossRotate", "KanjiDnD", "KanjiFishing" };
    private string[] VocabGames = { "TreeHop", "TunnelRunner", "VocabHunt" };
    private string[] DuelGames = { "KanjiCrossRotate", "KataSpeedType", "TunnelRunner" };
    private string[] MultiplayerGames = { "TreeHop", "QuizGame", "CountingGame", "LocationSeeker", "EeveeGame" };

    private string[] KatakanaNames = { "Katakana Scramble", "Katakana Search", "Go Ringo Go"};
    private string[] GrammarNames = { "Word Order", "Speed Type", "Receive Punch Give", "Space Protector" };
    private string[] KanjiNames = { "Kanji Cross Rotate", "Kanji Bakery", "Hook Them" };
    private string[] VocabNames = { "Head in the Clouds", "Tunnel Runner", "Made in Paint" };
    private string[] DuelNames = { "Kanji Cross Rotate", "Type Fight", "Tunnel Runner" };
    private string[] MultiplayerNames = { "Head in the Clouds", "Quiz Show", "Capy Count", "Eager Susan" };



    private string[] gamesToChooseFrom;
    private string[] namesToChooseFrom;

    public static event Action startedLoad; //DUCT TAPE AGHHHHH

    //you can avoid the big switch statement by using an array and define enums or something but this is more easy to remember/work with

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        typeName = category;
        switch (category.ToUpper())
        {
            case "KATAKANA":
                categoryIndex = 2;
                gamesToChooseFrom = KatakanaGames;
                namesToChooseFrom = KatakanaNames;
                break;
            case "GRAMMAR":
                categoryIndex = 0;
                gamesToChooseFrom = GrammarGames;
                namesToChooseFrom = GrammarNames;
                break;
            case "KANJI":
                categoryIndex = 1;
                gamesToChooseFrom = KanjiGames;
                namesToChooseFrom = KanjiNames;
                break;
            case "VOCAB":
                categoryIndex = 3;
                gamesToChooseFrom = VocabGames;
                namesToChooseFrom = VocabNames;
                break;
            case "DUEL":
                categoryIndex = 4;
                gamesToChooseFrom = DuelGames;
                namesToChooseFrom = DuelNames;
                break;
            case "MULTI":
                categoryIndex = 5;
                gamesToChooseFrom = MultiplayerGames;
                namesToChooseFrom = MultiplayerNames;
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
        Minigame.singleplayer = !(category.ToUpper().Equals("DUEL") || category.ToUpper().Equals("MULTI"));
        MinigameSelector.gotGame += LoadGame;
        selector.ChangeText(namesToChooseFrom, categoryIndex);
        selector.gameObject.SetActive(true);
        
    }

    public void LoadGame(int i) {
        MinigameSelector.gotGame -= LoadGame;
        startedLoad?.Invoke();
        string game = gamesToChooseFrom[i];
        //Minigame.singleplayer = false;
        game = "KatakanaScramble";
        InvokeLoad("HTP" + game, true);
    }



}
