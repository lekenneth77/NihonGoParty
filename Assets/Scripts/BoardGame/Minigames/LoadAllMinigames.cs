using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LoadAllMinigames : MonoBehaviour, Controls.IDebugActions
{
    public TMP_InputField input;
    public int numPlayers;
    private Controls controls;
    public GameObject canva;
    public static GameObject canvas;
    void Start() {
        canvas = canva;
        controls = new Controls();
        controls.Debug.AddCallbacks(this);
        controls.Debug.Enable();
        Minigame.debug = true;
    }

    public static void ResetCanvas() {
        canvas.SetActive(true);
    }

    public void LoadSingleplayer(string game) {
        if (input.text == "") {
            numPlayers = 4;
        } else { 
            numPlayers = int.Parse(input.text);
        }
        BoardController.numPlayers = numPlayers;
        Minigame.singleplayer = true;
        SceneManager.LoadSceneAsync(game, LoadSceneMode.Additive);
        canvas.SetActive(false);
    }

    public void LoadDuel(string game) {
        if (input.text == "") {
            numPlayers = 4;
        } else { 
            numPlayers = int.Parse(input.text);
        }
        BoardController.numPlayers = numPlayers;
        Minigame.singleplayer = false;
        SceneManager.LoadSceneAsync(game, LoadSceneMode.Additive);
        canvas.SetActive(false);
    }

    public void LoadMultiplayer(string game) {
        if (input.text == "") {
            numPlayers = 4;
        } else { 
            numPlayers = int.Parse(input.text);
        }
        BoardController.numPlayers = numPlayers;
        Minigame.singleplayer = false;
        SceneManager.LoadSceneAsync(game, LoadSceneMode.Additive);
        canvas.SetActive(false);
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        Debug.Log("Slash!");
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
        canvas.SetActive(true);
    }
}
