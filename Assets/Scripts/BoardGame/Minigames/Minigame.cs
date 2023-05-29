using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Minigame : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    public void EndGame(bool won)
    {
        BoardController.wonMinigame = won;
        BoardSpace.InvokeFinish();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
    }

    //maybe add a how to play loader here?


}
