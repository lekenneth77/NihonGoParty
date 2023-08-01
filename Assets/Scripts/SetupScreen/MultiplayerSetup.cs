using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MultiplayerSetup : MonoBehaviour
{
    //choose numplayers
    public GameObject chooseNumPlayersCon;

    //choose character
    public GameObject chooseYourCharCon;
    public Animator[] characters;
    public GameObject[] playerChosens;
    private int currentChooser;
    //honestly only used for disabling the button once confirm
    public GameObject[] characterChoosers;
    private int recentChosen;

    //for holding information to pass to the boardgame
    public static int[] whatThePlayersChose; //indicies of the characters in the order the players have chosen

    //oh god save my sphere
    public RotateObject daSphere;
    public int numMaps;
    private int currentMapI;

    private bool choseACharacter;

    //cameras
    public GameObject numCharsCamera, chooseCharsCamera, mapCamera, mapZoomCamera;

    //overlay canvas
    public GameObject overlayCanvas;

    private int numPlayers;
    private Sprite[] charSprites;

    // Start is called before the first frame update
    void Start()
    {
        charSprites = Resources.LoadAll<Sprite>("CharacterPortraits/");
        currentChooser = 0;
        choseACharacter = false;
    }

    public void ChooseNumPlayers(int numPlayers)
    {
        this.numPlayers = numPlayers;
        BoardController.numPlayers = numPlayers;
        whatThePlayersChose = new int[numPlayers];
        chooseCharsCamera.SetActive(true);
        numCharsCamera.SetActive(false);
    }

    public void ChooseYourCharacter(int charIndex)
    {
        if (choseACharacter) {
            characters[recentChosen].gameObject.transform.position = new Vector3(0, -1000, 0);
            characters[recentChosen].Play("idle");
        }

        characters[charIndex].gameObject.transform.position = playerChosens[currentChooser].transform.GetChild(0).position;
        characters[charIndex].Play("spin");
        recentChosen = charIndex;
        whatThePlayersChose[currentChooser] = charIndex;
        choseACharacter = true;
    }

    public void ConfirmCharacter()
    {
        if (!choseACharacter)
        {
            Debug.Log("Hey choose a character!");
            //Maybe add a message popup that disappears when ChooseYourCharacter is called?
            return;
        }

        choseACharacter = false;
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
            mapCamera.SetActive(true);
            chooseCharsCamera.SetActive(false);
            //Load board game! or actually make a choose map screen? TODO
        }
        else
        {
            currentChooser++;
            playerChosens[currentChooser].SetActive(true);
        }

    }

    public void RotateSphere(bool left)
    {
        int direction = left ? -1 : 1;
        if (left)
        {
            currentMapI = (currentMapI - 1) < 0 ? numMaps - 1: currentMapI - 1;
        } else
        {
            currentMapI = (currentMapI + 1) >= numMaps ? 0 : currentMapI + 1;
        }
        daSphere.Rotate(180f, "y");
    } 

    public void ZoomInSphere()
    {
        //probably have to check so that you can't select left right select buttons
        mapZoomCamera.SetActive(true);
        //might need to delay
        CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
        brain.m_DefaultBlend.m_Time = 2f; // 0 Time equals a cut
        StartCoroutine("iamsotired");
    }

    //literally just to delay the zoom in into the map sphere when selecting. Very hot.
    private IEnumerator iamsotired()
    {
        yield return new WaitForSeconds(2.1f);
        overlayCanvas.SetActive(true);
    }


    public void ZoomOutSphere()
    {
        mapZoomCamera.SetActive(false);
        overlayCanvas.SetActive(false);
        CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
        brain.m_DefaultBlend.m_Time = 1f; // 0 Time equals a cut
    }

    public void FinalSelectMap()
    {
        Debug.Log("Show time! " + currentMapI);
        SceneManager.LoadSceneAsync("Map Template");
        //TODO maybe do like a dramatic zoom in
    }


}
