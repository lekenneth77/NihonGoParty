using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FinishController : MonoBehaviour
{
    public static GameObject[] tempResults;
    public PlayerInfo[] playerInfos;
    public PlayableDirector[] introCutscenes;
    public GameObject goToResults;
    public Transform[] platformSpawns;
    private int numPlayers;
    public Animator[] backlines;
    public ParticleSystem confetti;

    public GameObject[] statBoards;
    public Image[] tabColors;
    public int currentTabIndex;
    private Color selectedColor = new(176f / 255f, 176f / 255f, 176f / 255f);

    public GameObject[] resultRanks;
    public GameObject[] minigameWins;
    public GameObject[] lowRolls;
    public GameObject[] highRolls;

    private int maxWins, maxLowRoll, maxHighRoll;
    private Sprite[] rankingSprites;
    private Sprite[] characterSprites;

    // Start is called before the first frame update
    void Start()
    {
        rankingSprites = Resources.LoadAll<Sprite>("Images/RankingSprites/");
        characterSprites = Resources.LoadAll<Sprite>("Images/CharacterPortraits/");

        numPlayers = tempResults.Length;
        playerInfos = new PlayerInfo[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            playerInfos[i] = tempResults[i].GetComponent<PlayerInfo>();
            tempResults[i].transform.position = platformSpawns[playerInfos[i].currentRanking - 1].position;
            tempResults[i].transform.localScale = new Vector3(2f, 2f, 2f);
            tempResults[i].transform.eulerAngles = new Vector3(0, 180f, 0);
            tempResults[i].SetActive(true);
        }
        
        currentTabIndex = 0;

        //setup results tab and get maxes and set appropriate numbers for other two tabs
        for (int i = 0; i < numPlayers; i++)
        {
            //resultRanks[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].currentRanking + "";
            PlayerInfo info = playerInfos[i];
            resultRanks[i].transform.GetChild(0).GetComponent<Image>().sprite = characterSprites[info.characterIndex];
            resultRanks[i].transform.GetChild(1).GetComponent<Image>().sprite = rankingSprites[info.currentRanking - 1];
            resultRanks[i].SetActive(true);

            minigameWins[i].GetComponent<Image>().sprite = characterSprites[info.characterIndex];
            lowRolls[i].GetComponent<Image>().sprite = characterSprites[info.characterIndex];
            highRolls[i].GetComponent<Image>().sprite = characterSprites[info.characterIndex];

            maxWins = playerInfos[i].numMinigamesWon > maxWins ? playerInfos[i].numMinigamesWon : maxWins;
            maxLowRoll = playerInfos[i].numLowRolls > maxLowRoll ? playerInfos[i].numLowRolls : maxLowRoll;
            maxHighRoll = playerInfos[i].numHighRolls > maxLowRoll ? playerInfos[i].numHighRolls : maxHighRoll;

            minigameWins[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].numMinigamesWon + "";
            lowRolls[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].numLowRolls + "";
            highRolls[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playerInfos[i].numHighRolls + "";
        }

        PlayableDirector director = introCutscenes[numPlayers - 2];
        director.stopped += AfterCutscene;
        director.Play();
    }

    public void AfterCutscene(PlayableDirector unused) {
        StartCoroutine("Celebration");
    }

    public void GoToTitle() {
        TitleScreen.skipStart = true;
        SceneManager.LoadSceneAsync("Title Screen");
    }

    private IEnumerator Celebration() {
        //handle victory/lose animation 
        //handle confetti particles
        confetti.Play();
        StartCoroutine("Hops");
        yield return new WaitForSeconds(10f);
        goToResults.SetActive(true);
        yield return new WaitForSeconds(5f);
        confetti.Stop();
    }

    private IEnumerator Hops() { 
         for (int i = 0; i < 4; i++) {
            backlines[Random.Range(0, backlines.Length)].Play("hop");
            yield return new WaitForSeconds(Random.Range(0.15f, 0.4f));
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject obj in tempResults)
        {
            Destroy(obj);
        }
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
        if (value == 0) { bar.fillAmount = 0f; }
        for (float i = 0; i < (value / max); i += 0.025f)
        {
            bar.fillAmount = i;
            yield return new WaitForSeconds(0.008f);
        }
    }

   
}
