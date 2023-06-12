using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KanjiDnD : Minigame
{
    public Transform leftSide, rightSide;
    public List<DnDInfo> dropSpots; //generate solution boxes here
    public HashSet<int> chosenProblems;
    public GameObject defDraggable;
    public GameObject defDropper;

    public TextAsset textFile;
    public Sprite[] kanjis;
    private string[] problems;

    public Timer timer;

    public int totalRounds;
    private int currentRound;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        DropSpot.Dropped += CheckAnswer;
        timer.TimeUp += TimeOut;

        kanjis = Resources.LoadAll<Sprite>("Minigames/Kanji/KanjiDnD/KanjiParts/");
        problems = textFile.text.Split("\n"[0]);
        chosenProblems = new HashSet<int>();

        /*
        List<int> test = new List<int>();
        test.Add(10);
        test.Add(11);
        test.Add(12);
        test.Add(13);
        test.Add(14);
        GenerateLeftItems(test, new List<float[]>());
        */

        //timer.ResetTimer();
        //timer.StartTimer();
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
                //either you can make the correct ones stay or just tell them its wrong
                break;
            }
        }
        //all are correct!
        currentRound++;
        if (totalRounds == currentRound) {
            StartCoroutine("HandleWin");
        } else { 
            
        }
    }

    private void GenerateProblem() {
        //clean up last problem
        for (int i = 0; i < leftSide.childCount; i++) {
            Destroy(leftSide.GetChild(0).gameObject);
            Destroy(rightSide.GetChild(0).gameObject);
        }

        int random = Random.Range(0, problems.Length);
        while(!chosenProblems.Add(random)) { 
            random = Random.Range(0, problems.Length);
        }

        string problem = problems[random];
        //parse string 
        List<int> identifiers = new List<int>();
        List<float[]> widthHeights = new List<float[]>();
        List<float[]> XYpositions = new List<float[]>();
        GenerateLeftItems(identifiers, widthHeights);
    }

    private void GenerateLeftItems(List<int> ids, List<float[]> widthHeights) {
        for (int i = 0; i < ids.Count; i++) {
            float randomX = Random.Range(-800f, 0);
            float randomY = Random.Range(-400f, 400f);
            int id = ids[i];
            GameObject newObj = Instantiate(defDraggable, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(leftSide);
            newObj.transform.localPosition = new Vector3(randomX, randomY);

            /*
            Rect size = newObj.GetComponent<RectTransform>().rect;
            size.width = widthHeights[i][0];
            size.height = widthHeights[i][1];
            */

            newObj.GetComponent<DnDInfo>().id = id;
            newObj.GetComponent<Image>().sprite = kanjis[id - 1];

        }
    }

    private IEnumerator HandleWin() {
        yield return new WaitForSeconds(2f);
        EndGame(true);
    }


    public void TimeOut() {
        StartCoroutine("HandleTimeOut");
    }

    private IEnumerator HandleTimeOut() {
        yield return new WaitForSeconds(2f);
        EndGame(false);
    }

}
