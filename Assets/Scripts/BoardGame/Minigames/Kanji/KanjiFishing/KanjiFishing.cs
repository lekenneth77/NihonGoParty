using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class KanjiFishing : Minigame
{
    public TextAsset textFile;
    public TextAsset hiraganaFile;
    public Hook hook;
    public GameObject defFish;
    public Transform fishParent;
    public Vector2 startingFishWP;
    public int numSpawns; //min = 10
    public TextMeshProUGUI currentKanjiText;
    private Vector2[] waypoints;
    private string[] kanjis;
    private string hiraganas;
    private HashSet<int> chosen;
    private string theAnswer; //FES???
    private int numCorrect = 0;
    private int numRounds = 0;
    private List<GameObject> fishies;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        kanjis = textFile.text.Split("\n"[0]);
        hiraganas = hiraganaFile.text;
        chosen = new HashSet<int>();
        fishies = new List<GameObject>();
        if (numSpawns < 10) { numSpawns = 10; }
        waypoints = new Vector2[numSpawns];
        for (int i = 0; i < numSpawns; i++) {
            waypoints[i] = startingFishWP;
            startingFishWP = new Vector2(startingFishWP.x, startingFishWP.y - 3f);
        }
        hook.reachTop += CheckAnswer;
        StartCoroutine("temp");
    }

    private IEnumerator temp() {
        yield return new WaitForSeconds(0.1f);
        defFish.GetComponent<Fish>().StopSwim();
        SetupRound();
    }

    private void SetupRound() {
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
        Debug.Log(problem[0] + " " + problem[1]);
        HashSet<int> randomPositions = new HashSet<int>();
        //correct spawns
        for (int i = 0; i < problem[1].Length; i++) {
            random = Random.Range(0, numSpawns);
            while (!randomPositions.Add(random)) {
                random = Random.Range(0, numSpawns);
            }

            GameObject fish = Instantiate(defFish, waypoints[random], Quaternion.identity, fishParent);
            fish.GetComponent<Fish>().ChangeLetter(problem[1][i] + "");
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
            fishies.Add(fish);
        }

        hook.ResetAndStart();
    }

    public void CheckAnswer(Queue<GameObject> q) {
        bool wrong = false;
        foreach (GameObject obj in q) {
            string letter = obj.GetComponent<Fish>().letter;
            if (!theAnswer.Contains(letter)) {
                Debug.Log("Wrong!");
                wrong = true;
                break;
            }
        }
        numRounds++;
        if (!wrong) {
            numCorrect++;
        }
        if (numRounds == 3) {
            Debug.Log("Stop!");
            EndGame(numCorrect - 1);
            return;
        }
        if (wrong) {
            StartCoroutine("HandleWrong");
        } else {
            Debug.Log("Correct!");
            StartCoroutine("HandleCorrect");
        }
    }

    //maybe change it so that the more you get correct, the more spaces you go forward?
    private IEnumerator HandleCorrect() {

        yield return new WaitForSeconds(5f);
        SetupRound();
    }

    private IEnumerator HandleWrong() {

        yield return new WaitForSeconds(5f);
        SetupRound();
    }


}
