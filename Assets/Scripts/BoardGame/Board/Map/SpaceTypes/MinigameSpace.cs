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
    private string[] KatakanaGames = { "KatakanaScramble", "KatakanaSearch", "AppleDrop" };
    private string[] GrammarGames = { "WordOrder2", "SpeedType", "GiveReceive" };// particles please
    private string[] KanjiGames = { "KanjiCrossRotate", "KanjiDnD", "KanjiFishing" };
    private string[] VocabGames = { "TreeHop", "TunnelRunner", "VocabHunt" };
    private string[] DuelGames = { "KanjiCrossRotate", "KataSpeedType", "TunnelRunner" };
    private string[] MultiplayerGames = { "TreeHop", "QuizGame", "CountingGame", "LocationSeeker" };

    private string[] KatakanaNames = { "Katakana Scramble", "Katakana Search", "AppleDrop"};
    private string[] GrammarNames = { "Word Order", "Speed Type", "Receive Punch Give" };
    private string[] KanjiNames = { "Kanji Cross Rotate", "Kanji Drag n Drop", "Kanji Fishing" };
    private string[] VocabNames = { "Tree Hop", "Tunnel Runner", "Vocabulary Hunt" };
    private string[] DuelNames = { "Kanji Cross Rotate", "Katakana Speed Type", "Tunnel Runner" };
    private string[] MultiplayerNames = { "Tree Hop", "Quiz Game", "Counting Game", "Location Seeker" };

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
                gamesToChooseFrom = KatakanaGames;
                namesToChooseFrom = KatakanaNames;
                break;
            case "GRAMMAR":
                gamesToChooseFrom = GrammarGames;
                namesToChooseFrom = GrammarNames;
                break;
            case "KANJI":
                gamesToChooseFrom = KanjiGames;
                namesToChooseFrom = KanjiNames;
                break;
            case "VOCAB":
                gamesToChooseFrom = VocabGames;
                namesToChooseFrom = VocabNames;
                break;
            case "DUEL":
                gamesToChooseFrom = DuelGames;
                namesToChooseFrom = DuelNames;
                break;
            case "MULTI":
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
        selector.ChangeText(namesToChooseFrom, category);
        selector.gameObject.SetActive(true);
        
    }

    public void LoadGame(int i) {
        MinigameSelector.gotGame -= LoadGame;
        startedLoad?.Invoke();
        string game = gamesToChooseFrom[i];
        InvokeLoad(game, true);
    }



}
