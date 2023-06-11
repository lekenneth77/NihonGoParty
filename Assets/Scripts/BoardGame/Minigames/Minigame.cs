using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Minigame : MonoBehaviour
{
    // Start is called before the first frame update

    //duel/multiplayer if false
    public static bool singleplayer;
    public virtual void Start()
    {
        
    }

    public void EndMultiplayerGame(int winnerIndex) {
        BoardController.multiWinIndex = winnerIndex;
        EndGame(true);
    }

    public void EndGame(bool won)
    {
        BoardController.wonMinigame = won;
        BoardSpace.InvokeFinish();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
    }

    //maybe add a how to play loader here?


}
