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
    public TextMeshProUGUI correctWord;
    public GameObject[] UIObjects;

    public TextAsset textFile;
    private Sprite[] kanjis;
    private string[] problems;

    public Timer timer;

    public int totalRounds;
    private int currentRound;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        timer.TimeUp += TimeOut;

        kanjis = Resources.LoadAll<Sprite>("Minigames/Kanji/KanjiDnD/KanjiParts/");
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

        foreach(DnDInfo sq in dropSpots) {
            DnDInfo squareOnMe = sq.onMeOnThem;
            if (sq.id != squareOnMe.id) {
                Debug.Log("Wrong Answer!");
                StartCoroutine("Incorrect");
                //either you can make the correct ones stay or just tell them its wrong
                return;
            }
        }
        //all are correct!
        DragNDrop.allowDrag = false;
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
        /*
        while(!chosenProblems.Add(random)) { 
            random = Random.Range(0, problems.Length);
        }
        */

        string problem = problems[random];
        //parse string 
        string[] parameters = problem.Split("_"[0]);
        correctWord.text = parameters[0];
        string[] ids = parameters[1].Split(","[0]);
        string[] wh = parameters[2].Split(","[0]);
        string[] xy = parameters[3].Split(","[0]);

        GenerateLeftItems(ids, wh);
        GenerateRightItems(ids, xy);

        DragNDrop.allowDrag = true;
        timer.ResetTimer();
        timer.StartTimer();
    }

    //its very possible to put these two functions together but i like them seperate
    private void GenerateLeftItems(string[] ids, string[] widthHeights) {
        for (int i = 0; i < ids.Length; i++) {
            float randomX = Random.Range(-800f, 0);
            float randomY = Random.Range(-400f, 400f);
            int id = int.Parse(ids[i]);
            GameObject newObj = Instantiate(defDraggable, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(leftSide);
            newObj.transform.localPosition = new Vector3(randomX, randomY);
            newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(float.Parse(widthHeights[i * 2]), float.Parse(widthHeights[(i * 2) + 1]));

            newObj.GetComponent<DnDInfo>().id = id;
            newObj.GetComponent<Image>().sprite = kanjis[id - 1];
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
        correctWord.gameObject.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        UIObjects[0].SetActive(false);
        correctWord.gameObject.transform.parent.gameObject.SetActive(false);

        GenerateProblem();
    }

    private IEnumerator Incorrect() {
        DragNDrop.allowDrag = false;
        UIObjects[1].SetActive(true);
        yield return new WaitForSeconds(2f);
        DragNDrop.allowDrag = true;
        UIObjects[1].SetActive(false);
    }

    private IEnumerator HandleWin() {
        UIObjects[2].SetActive(true);
        correctWord.gameObject.transform.parent.gameObject.SetActive(true);
        timer.StopTimer();
        yield return new WaitForSeconds(4f);
        EndGame(true);
    }


    public void TimeOut() {
        DragNDrop.allowDrag = false;
        StartCoroutine("HandleTimeOut");

    }

    private IEnumerator HandleTimeOut() {
        UIObjects[3].SetActive(true);
        correctWord.gameObject.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);
        EndGame(false);
    }

}
