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
    public static bool practice;
    public static bool debug;
    public virtual void Start()
    {
        
    }

    public void EndMultiplayerGame(int winnerIndex) {
        EndGame(winnerIndex);
    }

    public void EndGame(int result)
    {
        if (!practice) {
            Debug.Log("Final Results:" + result);
            BoardController.minigameResult = result;
            if (debug) {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
                LoadAllMinigames.ResetCanvas();
            } else { 
                BoardSpace.InvokeUnload(1, true);
            }
        } else { 
            practice = false;
            HowToPlay.ResetScreen();
            BoardSpace.InvokeUnload(2, false);
        }
    }

    //maybe add a how to play loader here?


}
