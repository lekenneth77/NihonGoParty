using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KanjiDnD : Minigame
{
    public Transform leftSide, rightSide;
    public List<DnDInfo> dropSpots; //generate solution boxes here
    public HashSet<int> chosenProblems;
    public GameObject defDraggable;
    public GameObject defDropper;
    public Canvas canvas;
    public Image fullImg;
    public TextMeshProUGUI hiragana;
    public GameObject answerCon;
    public GameObject[] UIObjects;

    public TextAsset textFile;
    private Sprite[] parts;
    private Sprite[] fullKanjis;
    private string[] problems;

    public Timer timer;
    public WinStars stars;

    public int totalRounds;
    private int currentRound;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        timer.TimeUp += TimeOut;

        parts = Resources.LoadAll<Sprite>("Minigames/Kanji/KanjiDnD/KanjiParts/");
        fullKanjis = Resources.LoadAll<Sprite>("Minigames/Kanji/KanjiDnD/FullImages/");
        problems = textFile.text.Split("\n"[0]);
        chosenProblems = new HashSet<int>();
        DropSpot.Dropped += CheckAnswer;
        currentRound = 0;
        GenerateProblem();
    }

    public void CheckAnswer() {
        foreach (DnDInfo sq in dropSpots) { 
            if (!sq.onMeOnThem) { return; }
        }
        //all spots are filled!

        DragNDrop.allowDrag = false;
        foreach(DnDInfo sq in dropSpots) {
            DnDInfo squareOnMe = sq.onMeOnThem;
            if (sq.id != squareOnMe.id) {
                Debug.Log("Wrong Answer!");
                StartCoroutine("Incorrect");
                //either you can make the correct ones stay or just tell them its wrong
                DragNDrop.allowDrag = true;
                return;
            }
        }
        //all are correct!
        timer.StopTimer();
        stars.Win();
        currentRound++;
        Debug.Log(currentRound);
        if (totalRounds == currentRound) {
            StartCoroutine("HandleWin");
        } else {
            StartCoroutine("NextRound");
        }
    }

    private void GenerateProblem() {
        //clean up last problem
        dropSpots.Clear();
        foreach (Transform child in leftSide) {
            Destroy(child.gameObject);
        }
        foreach(Transform child in rightSide) {
            Destroy(child.gameObject);
        }

        int random = Random.Range(0, problems.Length);
        while(!chosenProblems.Add(random)) { 
            random = Random.Range(0, problems.Length);
        }
        fullImg.sprite = fullKanjis[random];
        string problem = problems[random];
        //parse string 
        string[] parameters = problem.Split("_"[0]);
        hiragana.text = parameters[1];
        string[] ids = parameters[2].Split(","[0]);
        string[] xy = parameters[3].Split(","[0]);

        GenerateLeftItems(ids);
        GenerateRightItems(ids, xy);

        DragNDrop.allowDrag = true;
        timer.ResetTimer();
        timer.StartTimer();
    }

    //its very possible to put these two functions together but i like them seperate
    private void GenerateLeftItems(string[] ids) {
        for (int i = 0; i < ids.Length; i++) {
            float randomX = Random.Range(-800f, 0);
            float randomY = Random.Range(-400f, 400f);
            int id = int.Parse(ids[i]);
            GameObject newObj = Instantiate(defDraggable, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(leftSide);
            newObj.transform.localPosition = new Vector3(randomX, randomY);
            newObj.GetComponent<SpriteRenderer>().sprite = parts[id - 1];
            newObj.AddComponent<RectTransform>().localScale = new Vector2(200f, 200f);
            newObj.GetComponent<DnDInfo>().id = id;
            newObj.AddComponent<DragNDrop>().SetCanvas(canvas);
            newObj.AddComponent<Image>().sprite = parts[id - 1];
            Destroy(newObj.GetComponent<SpriteRenderer>());
        }
    }

    private void GenerateRightItems(string[] ids, string[] xyPositions) { 
        for (int i = 0; i < ids.Length; i++) {
            GameObject newObj = Instantiate(defDropper, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(rightSide);
            newObj.transform.localPosition = new Vector3(float.Parse(xyPositions[i * 2]), float.Parse(xyPositions[(i * 2) + 1]));
            newObj.GetComponent<DnDInfo>().id = int.Parse(ids[i]);
            dropSpots.Add(newObj.GetComponent<DnDInfo>());
        }
    }

    private IEnumerator NextRound() {
        UIObjects[0].SetActive(true);
        answerCon.SetActive(true);
        yield return new WaitForSeconds(5f);
        answerCon.SetActive(false);
        UIObjects[0].SetActive(false);

        GenerateProblem();
    }

    private IEnumerator Incorrect() {
        DragNDrop.allowDrag = false;
        UIObjects[1].SetActive(true);
        yield return new WaitForSeconds(4f);
        DragNDrop.allowDrag = true;
        UIObjects[1].SetActive(false);
    }

    private IEnumerator HandleWin() {
        UIObjects[2].SetActive(true);
        timer.StopTimer();
        answerCon.SetActive(true);
        yield return new WaitForSeconds(4f);
        FinalResult();
    }


    public void TimeOut() {
        DragNDrop.allowDrag = false;
        StartCoroutine("HandleTimeOut");

    }

    private IEnumerator HandleTimeOut() {
        UIObjects[3].SetActive(true);
        answerCon.SetActive(true);
        stars.Lose();
        yield return new WaitForSeconds(4f);
        currentRound++;
        Debug.Log(currentRound);
        if (totalRounds == currentRound) {
            FinalResult();
        } else {
            UIObjects[3].SetActive(false);
            answerCon.SetActive(false);
            GenerateProblem();
        }
    }

    private void FinalResult() {
        int result = stars.GetWins() - 1;
        EndGame(result);

    }

}
