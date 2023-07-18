using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AppleDrop : Minigame
{
    public ApplePlayer player;
    public AppleBasket basket;
    public GameObject defApple;
    public Transform appleFolder;
    public Transform[] spawnPts;
    public TextAsset justKatakanaTxt;
    public TextAsset kataSolutionTxt;
    public Timer setupTimer;

    public TextMeshProUGUI rowToFindTxt;
    private string katakanas;
    private string[] solutions;
    private string currentSolution;
    private HashSet<int> chosen;

    //results screen
    private List<string> gottenApples; //pretty bad design but whatever im really tired
    private List<int> badOnes; //only used for result screen
    public TextMeshProUGUI resultRow;
    public TextMeshProUGUI numCollected;
    public TextMeshProUGUI correct;
    public TextMeshProUGUI wrong;
    public Transform applesCollected;
    public Transform startingUIApple;
    public GameObject defUIApple;
    //maybe make the game determined by if they collected more right than wrong? could cheese but eh

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        basket.gotApple += CheckApple;
        setupTimer.TimeUp += StartRound;
        katakanas = justKatakanaTxt.text;
        solutions = kataSolutionTxt.text.Split("\n"[0]);
        chosen = new HashSet<int>();
        SetupRound();
    }

    private void SetupRound() {
        gottenApples = new List<string>();
        badOnes = new List<int>();
        setupTimer.ResetTimer();

        int random = Random.Range(0, solutions.Length);
        while (!chosen.Add(random)) {
            random = Random.Range(0, solutions.Length);
        }
        string[] splitted = solutions[random].Split("_"[0]);
        rowToFindTxt.text = splitted[0] + "-row";
        currentSolution = splitted[1];
        rowToFindTxt.transform.parent.gameObject.SetActive(true);
        setupTimer.StartTimer();
    }

    public void StartRound() {
        rowToFindTxt.transform.parent.gameObject.SetActive(false);
        player.controls.AppleDrop.Enable();
        StartCoroutine("GenerateApples");
    }


    private IEnumerator GenerateApples() {
        //create initial apples
        HashSet<int> initals = new HashSet<int>();
        for (int i = 0; i < Random.Range(2, 6); i++) {
            int randomSP = Random.Range(0, spawnPts.Length);
            while (!initals.Add(randomSP)) {
                randomSP = Random.Range(0, spawnPts.Length);
            }
            CreateApple(spawnPts[randomSP], katakanas[Random.Range(0, katakanas.Length)] + "", 4f);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < Random.Range(20, 30); i++) {
            for (int j = 0; j < Random.Range(1, 4); j++) { 
                CreateApple(spawnPts[Random.Range(0, spawnPts.Length)], katakanas[Random.Range(0, katakanas.Length)] + "", 4f);
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 1.5f));
        }
        yield return new WaitForSeconds(5f);
        StartCoroutine(DisplayResult());
    }

    private void CreateApple(Transform pos, string s, float spd) {
        GameObject appleObj = Instantiate(defApple, pos.position, Quaternion.identity, appleFolder);
        Apple app = appleObj.GetComponent<Apple>();
        app.text = s;
        appleObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = s;
        float delay = Random.Range(1f, 4f);
        app.StartCoroutine(app.WaitThenFall(delay, spd)); //probably doesn't have to be app but whatever
    }

    public void CheckApple(string appleStr) {
        gottenApples.Add(appleStr);
        if (!currentSolution.Contains(appleStr)) {
            badOnes.Add(gottenApples.Count - 1);
            //start slowdown
            if (!player.alreadySlowed) {
                player.StartCoroutine(player.SlowDown());
            }
        }
    }

    private IEnumerator DisplayResult() {
        player.controls.AppleDrop.Disable();
        resultRow.text = rowToFindTxt.text;
        numCollected.gameObject.SetActive(false);
        correct.gameObject.SetActive(false);
        wrong.gameObject.SetActive(false);
        numCollected.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        Vector3 position = startingUIApple.position;
        List<GameObject> resultings = new List<GameObject>();
        foreach (string s in gottenApples) {
            resultings.Add(CreateUIApple(position, s));

            position = new Vector3(position.x + 125f, position.y, position.z);
            if (position.x > 1580f) {
                position = new Vector3(startingUIApple.position.x, position.y - 125f, position.z);
            }
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(2f);
        foreach(int i in badOnes) {
            resultings[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        numCollected.text = "Total: " + gottenApples.Count;
        numCollected.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        correct.text = "Correct: "  + (gottenApples.Count - badOnes.Count);
        wrong.text = "Wrong: " + badOnes.Count;
        correct.gameObject.SetActive(true);
        wrong.gameObject.SetActive(true);
    }

    private GameObject CreateUIApple(Vector3 pos, string s) {
        GameObject apple = Instantiate(defUIApple, pos, Quaternion.identity, applesCollected);
        apple.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = s;
        return apple;
    }

}
