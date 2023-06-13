using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HowToPlay : MonoBehaviour
{
    public GameObject[] tabBoards;
    public Image[] tabColors;

    public GameObject htpCanvas;
    private static GameObject staticCanvas;
    public GameObject quitPracticeCanvas;
    private static GameObject qpCanvas;

    public int currentTabIndex;
    public string myGame;
    private Color selectedColor = new(176f / 255f, 176f / 255f, 176f / 255f);

    void Start()
    {
        currentTabIndex = 0;
        staticCanvas = htpCanvas;
        qpCanvas = quitPracticeCanvas;
    }

    public void ChangeTab(int tabIndex)
    {
        if (tabIndex == currentTabIndex) { return; }
        tabColors[currentTabIndex].color = Color.white;
        tabBoards[currentTabIndex].SetActive(false);

        tabColors[tabIndex].color = selectedColor;
        tabBoards[tabIndex].SetActive(true);
        currentTabIndex = tabIndex;
    }

    public void StartGame() {
        Minigame.practice = false; //just to make sure
        BoardSpace.InvokeLoad(myGame, true);
        SceneManager.UnloadSceneAsync("HowToPlayTemp");
    }

    public void PracticeGame() {
        Minigame.practice = true;
        BoardSpace.InvokeLoad(myGame, true);
    }

    public void EndPracticeGame() {
        Minigame.practice = false;
        BoardSpace.InvokeUnload(2, false);
        //maybe add the loading screen here? SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(2));
    }

    public static void ResetScreen() {
        staticCanvas.SetActive(true);
        qpCanvas.SetActive(false);
    }



}
