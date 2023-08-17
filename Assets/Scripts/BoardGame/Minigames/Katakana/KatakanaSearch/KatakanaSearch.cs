using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KatakanaSearch : Minigame
{
    // Start is called before the first frame update
    //setup
    public TextAsset hiraganaText;
    public TextAsset katakanaText;
    public GameObject blackPanel;
    public GameObject preroundCon;
    public TextMeshProUGUI mainHiragana;
    public GameObject defKata;
    public Transform instFolder;
    public Transform[] startingPositions;
    public Timer setupTimer;
    public WinStars stars;
    public GameObject finish;
    private int wins;

    //game
    public GameObject flashlight;
    public Timer gameTimer;
    public bool noTouchy;

    private HashSet<int> chosenRandomChars;
    private HashSet<int> chosenCorrectChars;
    private HashSet<int> chosenPositions;
    private string hiraganas;
    private string katakanas;
    private KatakanaCollider correctOne;

    public override void Start()
    {
        base.Start();
        chosenCorrectChars = new HashSet<int>();
        chosenRandomChars = new HashSet<int>();
        chosenPositions = new HashSet<int>();
        hiraganas = hiraganaText.text;
        katakanas = katakanaText.text;
        KatakanaCollider.GotClicked += IsThatRight;
        setupTimer.TimeUp += SetupTimeout;
        gameTimer.TimeUp += GameTimeout;
        SetupRound();
    }

    private void SetupRound() {
        instFolder.gameObject.SetActive(false);
        flashlight.SetActive(false);
        blackPanel.SetActive(false);
        gameTimer.gameObject.SetActive(false);
        foreach (Transform child in instFolder) {
            Destroy(child.gameObject);
        }
        chosenRandomChars.Clear();
        chosenPositions.Clear();
        int random = Random.Range(0, hiraganas.Length);
        while (!chosenCorrectChars.Add(random)) {
            random = Random.Range(0, hiraganas.Length);
        }
        chosenRandomChars.Add(random);

        int position = Random.Range(0, startingPositions.Length);
        chosenPositions.Add(position);
        mainHiragana.text = hiraganas[random] + "";
        correctOne = CreateKatakana(random, startingPositions[position].localPosition).GetComponent<KatakanaCollider>();
        correctOne.correctOne = true;

        for (int i = 0; i < 19; i++) {
            random = Random.Range(0, hiraganas.Length);
            while (!chosenRandomChars.Add(random))
            {
                random = Random.Range(0, hiraganas.Length);
            }
            position = position = Random.Range(0, startingPositions.Length);
            while (!chosenPositions.Add(position)) {
                position = Random.Range(0, startingPositions.Length);
            }
            CreateKatakana(random, startingPositions[position].localPosition);
        }
        setupTimer.ResetTimer();
        preroundCon.SetActive(true);
        setupTimer.StartTimer();
    }

    private GameObject CreateKatakana(int stringIndex, Vector2 position) {
        GameObject obj = Instantiate(defKata, instFolder);

        obj.transform.localPosition = position;
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = katakanas[stringIndex] + "";
        return obj;
    }

    public void SetupTimeout() {
        StartCoroutine("AfterSetup");
    }

    private IEnumerator AfterSetup() {
        blackPanel.SetActive(true);
        preroundCon.SetActive(false);
        yield return new WaitForSeconds(2f);
        instFolder.gameObject.SetActive(true);
        flashlight.SetActive(true);
        blackPanel.SetActive(false);
        gameTimer.ResetTimer();
        gameTimer.gameObject.SetActive(true);
        gameTimer.StartTimer();
        noTouchy = false;
        KatakanaCollider.disable = false;
    }

    public void GameTimeout() {
        noTouchy = true;
        StartCoroutine("HandleTimeout");
    }

    public void IsThatRight(bool yes) { 
        if (noTouchy) { return; }
        if (yes) {
            noTouchy = true;
            StartCoroutine("ThatsRight");
        } else {
            StartCoroutine("ThatsWrong");
        }
    }

    //as you can tell i am kind of getting tired of coding today
    private IEnumerator ThatsRight() {
        KatakanaCollider.disable = true;
        correctOne.nono = true;
        correctOne.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        gameTimer.StopTimer();
        flashlight.SetActive(false);
        yield return new WaitForSeconds(3f);
        stars.Win();
        wins++;
        if (wins >= 3) {
            StartCoroutine("FinishGame");
        } else {
            SetupRound();
        }
    }

    private IEnumerator ThatsWrong() {
        flashlight.SetActive(false);
        blackPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (KatakanaCollider.disable) { 
            flashlight.SetActive(false);
            blackPanel.SetActive(false);
        } else {
            flashlight.SetActive(true);
            blackPanel.SetActive(false);
        }
    } 

    private IEnumerator FinishGame() {
        finish.SetActive(true);
        yield return new WaitForSeconds(5f);
        int result = stars.GetWins();
        EndGame(result - 1);
    }

    private IEnumerator HandleTimeout() {
        KatakanaCollider.disable = true;
        blackPanel.SetActive(false);
        flashlight.SetActive(false);
        correctOne.nono = true;
        correctOne.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        stars.Lose();
        yield return new WaitForSeconds(5f);
        wins++; //wins is the wrong variable name, it's more appropriate to call it rounds
        if (wins >= 3) {
            StartCoroutine("FinishGame");
        } else {
            SetupRound();
        }
    }
    
}
