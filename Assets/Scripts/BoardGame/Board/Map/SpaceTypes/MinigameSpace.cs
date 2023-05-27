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

    Minigame currentMinigame;

    //you can avoid the big switch statement by using an array and define enums or something but this is more easy to remember/work with

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        typeName = category;
    }
    public override void Action()
    {
        Debug.Log("Minigame!");
        string[] gamesToChooseFrom = null;
        switch (category.ToUpper())
        {
            case "KATAKANA":
                gamesToChooseFrom = KatakanaGames;
                break;
            case "GRAMMAR":
                gamesToChooseFrom = GrammarGames;
                break;
            default:
                Debug.Log("SPELLING ERROR PROBABLY OR NOT IMPLEMENTED: " + category);
                InvokeFinish();
                return;
        }

        string chosenGame = gamesToChooseFrom[Random.Range(0, gamesToChooseFrom.Length)];
        SceneManager.LoadSceneAsync(chosenGame, LoadSceneMode.Additive);
    }

}
