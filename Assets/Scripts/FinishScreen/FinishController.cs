using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishController : MonoBehaviour
{
    public static GameObject[] tempResults;
    public PlayerInfo[] playerInfos;
    private int numPlayers;

    public GameObject[] statBoards;
    public Image[] tabColors;
    public int currentTabIndex;
    private Color selectedColor = new(176f / 255f, 176f / 255f, 176f / 255f);

    public GameObject[] resultRanks;
    public GameObject[] minigameWins;
    public GameObject[] lowRolls;
    public GameObject[] highRolls;

    private int maxWins, maxLowRoll, maxHighRoll;

    // Start is called before the first frame update
    void Start()
    {
        numPlayers = tempResults.Length;
        playerInfos = new PlayerInfo[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            playerInfos[i] = tempResults[i].GetComponent<PlayerInfo>();
        }
        currentTabIndex = 0;

        //setup results tab and get maxes and set appropriate numbers for other two tabs
        for (int i = 0; i < numPlayers; i++)
        {
            resultRanks[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].currentRanking + "";
            resultRanks[i].SetActive(true);

            maxWins = playerInfos[i].numMinigamesWon > maxWins ? playerInfos[i].numMinigamesWon : maxWins;
            maxLowRoll = playerInfos[i].numLowRolls > maxLowRoll ? playerInfos[i].numLowRolls : maxLowRoll;
            maxHighRoll = playerInfos[i].numHighRolls > maxLowRoll ? playerInfos[i].numHighRolls : maxHighRoll;

            minigameWins[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].numMinigamesWon + "";
            lowRolls[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].numLowRolls + "";
            highRolls[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].numHighRolls + "";
        }

        //todo choose which cutscene to trigger
    }

    public void ChangeTab(int tabIndex)
    {
        if (tabIndex == currentTabIndex) { return; }
        tabColors[currentTabIndex].color = Color.white;
        statBoards[currentTabIndex].SetActive(false);

        tabColors[tabIndex].color = selectedColor;
        statBoards[tabIndex].SetActive(true);
        currentTabIndex = tabIndex;
        if (tabIndex == 1)
        {
            ShowMinigameTab();
        } else if (tabIndex == 2)
        {
            ShowRollTab();
        }
    }

    //it's late im tired this is all jank but i want it to be cool
    private void ShowMinigameTab()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            minigameWins[i].SetActive(true);
            Image bar = minigameWins[i].transform.GetChild(0).gameObject.GetComponent<Image>();
            StartCoroutine(DynamicBar(bar, playerInfos[i].numMinigamesWon, maxWins));
            
        }
    }

    private void ShowRollTab()
    {
        //low rolls
        for (int i = 0; i < numPlayers; i++)
        {
            lowRolls[i].SetActive(true);
            Image bar = lowRolls[i].transform.GetChild(0).gameObject.GetComponent<Image>();
            StartCoroutine(DynamicBar(bar, playerInfos[i].numLowRolls, maxLowRoll));
        }

        //high rolls
        for (int i = 0; i < numPlayers; i++)
        {
            highRolls[i].SetActive(true);
            Image bar = highRolls[i].transform.GetChild(0).gameObject.GetComponent<Image>();
            StartCoroutine(DynamicBar(bar, playerInfos[i].numHighRolls, maxHighRoll));
        }

    }

    private IEnumerator DynamicBar(Image bar, float value, float max)
    {
        for (float i = 0; i < (value / max); i += 0.025f)
        {
            bar.fillAmount = i;
            yield return new WaitForSeconds(0.008f);
        }
    }

   
}
