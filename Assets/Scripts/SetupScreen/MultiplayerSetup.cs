using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiplayerSetup : MonoBehaviour
{
    public GameObject chooseNumPlayersCon;
    public GameObject chooseYourCharCon;

    public GameObject[] playerChosens;
    private int currentChooser;

    //honestly only used for disabling the button once confirm
    public GameObject[] characterChoosers;
    private int recentChosen;

    private int numPlayers;
    private Sprite[] charSprites;

    // Start is called before the first frame update
    void Start()
    {
        charSprites = Resources.LoadAll<Sprite>("CharacterPortraits/");
        currentChooser = 0;
    }

    public void ChooseNumPlayers(int numPlayers)
    {
        this.numPlayers = numPlayers;
        BoardController.numPlayers = numPlayers;
        chooseNumPlayersCon.SetActive(false);
        chooseYourCharCon.SetActive(true);
    }

    public void ChooseYourCharacter(int charIndex)
    {
        playerChosens[currentChooser].GetComponent<Image>().sprite = charSprites[charIndex];
        recentChosen = charIndex;
    }

    public void ConfirmCharacter()
    {
        if (playerChosens[currentChooser].GetComponent<Image>().sprite == null)
        {
            Debug.Log("Hey choose a character!");
            //Maybe add a message popup that disappears when ChooseYourCharacter is called?
            return;
        }

        GameObject chosenChar = characterChoosers[recentChosen];
        chosenChar.GetComponent<Button>().enabled = false;
        Color temp = chosenChar.GetComponent<Image>().color;
        temp.a = 0.3f;
        chosenChar.GetComponent<Image>().color = temp;

        if (currentChooser == numPlayers - 1)
        {
            characterChoosers[recentChosen].GetComponent<Button>().enabled = false;
            //just to make sure they can't change they character last second
            foreach (GameObject obj in characterChoosers)
            {
                obj.GetComponent<Button>().enabled = false;
            }
            Debug.Log("Board game time!");
            //Load board game! or actually make a choose map screen? TODO
        }
        else
        {
            currentChooser++;
            playerChosens[currentChooser].SetActive(true);
        }

    }

}
