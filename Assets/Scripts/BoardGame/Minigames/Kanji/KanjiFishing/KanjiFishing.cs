using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KanjiFishing : Minigame
{
    public TextAsset textFile;
    public TextAsset hiraganaFile;
    public Hook hook;
    public GameObject defFish;
    private Sprite[] fishSprites;
    public Transform fishParent;
    public Vector2 startingFishWP;
    public int numSpawns; //min = 10
    private Vector2[] waypoints;
    private string[] kanjis;
    private string hiraganas;
    private HashSet<int> chosen;
    private string theAnswer; //FES???
    private int numCorrect = 0;
    private int numRounds = 0;
    private List<GameObject> fishies;
    public GameObject finish;
    

    //ui
    public TextMeshProUGUI currentKanjiText;
    public TextMeshProUGUI chooseThisText;
    public Timer timer;
    public GameObject answerBox;
    public TextMeshProUGUI answerKanji;
    public TextMeshProUGUI answerSpelling;
    public TextMeshProUGUI yourAnswer;
    public Image correct;
    public Image wrong;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        kanjis = textFile.text.Split("\n"[0]);
        hiraganas = hiraganaFile.text;
        fishSprites = Resources.LoadAll<Sprite>("Images/KanjiFishing/Fishes/");
        chosen = new HashSet<int>();
        fishies = new List<GameObject>();

        if (numSpawns < 10) { numSpawns = 10; }
        waypoints = new Vector2[numSpawns];
        for (int i = 0; i < numSpawns; i++) {
            waypoints[i] = startingFishWP;
            startingFishWP = new Vector2(startingFishWP.x, startingFishWP.y - 3f);
        }
        hook.reachTop += CheckAnswer;
        timer.TimeUp += StartRound;
        StartCoroutine("SetupRound");
    }

    private IEnumerator SetupRound()
    {
        yield return new WaitForSeconds(0.1f);
        defFish.GetComponent<Fish>().StopSwim();
        MakeFish();
        chooseThisText.text = currentKanjiText.text;
        timer.ResetTimer();
        chooseThisText.transform.parent.gameObject.SetActive(true);
        timer.StartTimer(); //
    }

    public void StartRound() {
        chooseThisText.transform.parent.gameObject.SetActive(false);
        hook.ResetAndStart();
    }



    private void MakeFish() {
        hook.ClearQueue();
        foreach (GameObject obj in fishies) {
            Destroy(obj);
        }
        int random = Random.Range(0, kanjis.Length);
        while (!chosen.Add(random)) {
            random = Random.Range(0, kanjis.Length);
        }
        //[0] is kanji, [1] is hiragana, [2] is english meaning
        string[] problem = kanjis[random].Split(",");
        currentKanjiText.text = problem[0];
        theAnswer = problem[1];
        //Debug.Log(problem[0] + " " + problem[1]);
        HashSet<int> randomPositions = new HashSet<int>();
        //correct spawns
        for (int i = 0; i < problem[1].Length; i++) {
            random = Random.Range(0, numSpawns);
            while (!randomPositions.Add(random)) {
                random = Random.Range(0, numSpawns);
            }

            GameObject fish = Instantiate(defFish, waypoints[random], Quaternion.identity, fishParent);
            fish.GetComponent<Fish>().ChangeLetter(problem[1][i] + "");
            fish.transform.GetChild(0).GetComponent<Image>().sprite = fishSprites[Random.Range(0, fishSprites.Length)];
            fishies.Add(fish);
        }

        //wrong spawns
        for (int i = 0; i < numSpawns - problem[1].Length; i++) {
            random = Random.Range(0, numSpawns);
            while (!randomPositions.Add(random))
            {
                random = Random.Range(0, numSpawns);
            }
            GameObject fish = Instantiate(defFish, waypoints[random], Quaternion.identity, fishParent);
            fish.GetComponent<Fish>().ChangeLetter(hiraganas[Random.Range(0, 26)] + "");
            fish.transform.GetChild(0).GetComponent<Image>().sprite = fishSprites[Random.Range(0, fishSprites.Length)];
            fishies.Add(fish);
        }

    }

    public void CheckAnswer(Queue<GameObject> q) {
        bool wrong = false;
        string build = "";
        HashSet<string> chosen = new HashSet<string>();
        foreach (GameObject obj in q) { 
            build += obj.GetComponent<Fish>().letter;
            chosen.Add(obj.GetComponent<Fish>().letter);
        }

        yourAnswer.text = "";
        foreach (string s in chosen) {
            yourAnswer.text += s + " ";
        }

        answerKanji.text = currentKanjiText.text;
        answerSpelling.text = theAnswer;

        for (int i = 0; i < theAnswer.Length; i++) { 
            if (!build.Contains(theAnswer[i])) {
                //Debug.Log("Wrong!");
                wrong = true;
                break;
            }
        }
        //im so fucking tired
        for (int i = 0; i < build.Length; i++)
        {
            if (!theAnswer.Contains(build[i]))
            {
                //Debug.Log("Wrong!");
                wrong = true;
                break;
            }
        }

        numRounds++;
        if (!wrong) {
            numCorrect++;
        }
        
        if (wrong) {
            StartCoroutine("HandleWrong");
        } else {
            StartCoroutine("HandleCorrect");
        }
    }

    //maybe change it so that the more you get correct, the more spaces you go forward?
    private IEnumerator HandleCorrect() {
        correct.gameObject.SetActive(true);
        answerBox.SetActive(true);
        yield return new WaitForSeconds(7f);
        correct.gameObject.SetActive(false);
        answerBox.SetActive(false);

        if (numRounds == 2) {
            if (numCorrect == 0) { numCorrect = -1; }
            finish.SetActive(true);
            yield return new WaitForSeconds(3f);
            EndGame(numCorrect);
        } else {
            StartCoroutine("SetupRound");
        }
    }

    private IEnumerator HandleWrong() {
        wrong.gameObject.SetActive(true);
        answerBox.SetActive(true);
        yield return new WaitForSeconds(7f);
        wrong.gameObject.SetActive(false);
        answerBox.SetActive(false);

        if (numRounds == 2) {
            if (numCorrect == 0) { numCorrect = -1; }
            finish.SetActive(true);
            yield return new WaitForSeconds(3f);
            EndGame(numCorrect);
        } else { 
            StartCoroutine("SetupRound");
        }
    }


}
