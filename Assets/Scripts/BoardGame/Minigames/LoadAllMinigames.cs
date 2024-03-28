using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LoadAllMinigames : MonoBehaviour
{
    public int numPlayers;
    public GameObject canva;
    public static GameObject canvas;

    private bool htpOn;
    public GameObject htpStar;
    void Start() {
        canvas = canva;
        Minigame.debug = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {   if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
                canvas.SetActive(true);
            }
        }
    }

    public static void ResetCanvas() {
        canvas.SetActive(true);
    }

    public void TurnOnOffHTP() {
        htpOn = !htpOn;
        htpStar.SetActive(htpOn);
    }

    public void LoadSingleplayer(string game) {
        numPlayers = 4;
        BoardController.numPlayers = numPlayers;
        Minigame.singleplayer = true;
        if (htpOn) {
            game = "HTP" + game;
        }
        SceneManager.LoadSceneAsync(game, LoadSceneMode.Additive);
        canvas.SetActive(false);
    }

    public void LoadDuel(string game) {
        numPlayers = 4;
        BoardController.numPlayers = numPlayers;
        Minigame.singleplayer = false;
        if (htpOn) {
            game = "HTP" + game;
        }
        SceneManager.LoadSceneAsync(game, LoadSceneMode.Additive);
        canvas.SetActive(false);
    }

    public void LoadMultiplayer(string game) {
        numPlayers = 4;
        BoardController.numPlayers = numPlayers;
        Minigame.singleplayer = false;
        if (htpOn) {
            game = "HTP" + game;
        }
        SceneManager.LoadSceneAsync(game, LoadSceneMode.Additive);
        canvas.SetActive(false);
    }

    public void LoadTitleScreen() {
        Minigame.debug = false;
        SceneManager.LoadSceneAsync("Title Screen");
    }
}
