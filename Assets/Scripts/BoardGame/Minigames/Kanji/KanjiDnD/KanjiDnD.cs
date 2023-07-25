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

    //camera
    public KanjiDnDCamera cam;
    private bool camGoUp;

    //text bubble
    public TextMeshProUGUI textBubble;

    //patience box
    public Image patienceBar;
    public Image emotion;
    private Sprite[] emotionImgs;

    //reaction image
    public Image reaction;

    //models
    public GameObject defWalker;
    public Transform walkerFolder;
    public Transform spawnPt;
    public Transform centerPt;

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

        emotionImgs = Resources.LoadAll<Sprite>("Images/Emotions/");
        parts = Resources.LoadAll<Sprite>("Minigames/Kanji/KanjiDnD/KanjiParts/");
        fullKanjis = Resources.LoadAll<Sprite>("Minigames/Kanji/KanjiDnD/FullImages/");
        problems = textFile.text.Split("\n"[0]);
        chosenProblems = new HashSet<int>();
        DropSpot.Dropped += CheckAnswer;
        cam.FinishMove += AfterCamera;
        currentRound = 0;
        StartCoroutine("SpawnAndWalk");
        //GenerateProblem();
    }

    void Update() {
        float timerSecs = timer.CurrentTime();
        
        if (timerSecs > timer.timeLimit * (2f / 3f)) {
            emotion.sprite = emotionImgs[3];
            patienceBar.color = Color.green;
        } else if (timerSecs > timer.timeLimit * (1f / 3f)) {
            emotion.sprite = emotionImgs[1];
            patienceBar.color = new Color(238f / 255f, 141f / 255f, 0f);
        } else {
            emotion.sprite = emotionImgs[0];
            patienceBar.color = Color.red;
        }
    }

    private IEnumerator SpawnAndWalk() {
        GameObject npc = Instantiate(defWalker, spawnPt.position, Quaternion.identity, walkerFolder);
        yield return new WaitForSeconds(1f);
        MoveObject move = npc.GetComponent<MoveObject>();
        move.SetTargetAndMove(centerPt.position);
        while (move.GetMoveFlag()) {
            yield return new WaitForSeconds(0.1f);
        }
        move.RotateToAngle(0, 180f, 0);
        yield return new WaitForSeconds(0.5f);
        move.transform.rotation = Quaternion.Euler(0, 180f, 0);
        GenerateProblem();
        yield return new WaitForSeconds(2.5f);
        textBubble.transform.parent.gameObject.SetActive(false);
        camGoUp = false;
        cam.RotateDown();
    }

    public void AfterCamera() { 
        if (camGoUp) { 

        } else {
            StartRound();
        }
    }

    private void StartRound() {
        leftSide.gameObject.SetActive(true);
        rightSide.gameObject.SetActive(true);
        DragNDrop.allowDrag = true;
        emotion.transform.parent.gameObject.SetActive(true);
        timer.StartTimer();
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
                StartCoroutine("Incorrect");
                //either you can make the correct ones stay or just tell them its wrong
                return;
            }
        }
        //all are correct!
        timer.StopTimer();
        //stars.Win();
        currentRound++;
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

        timer.ResetTimer();
        textBubble.text = parameters[1];
        textBubble.transform.parent.gameObject.SetActive(true);
        

       
    }

    //its very possible to put these two functions together but i like them seperate
    private void GenerateLeftItems(string[] ids) {
        for (int i = 0; i < ids.Length; i++) {
            int leftOrRight = Random.Range(0, 2);
            float randomX;
            float randomY;
            if (leftOrRight == 0) {
                //left
                randomX = Random.Range(-750f, -600f);
                randomY = Random.Range(-400f, 400f);
            } else {
                //right
                randomX = Random.Range(600f, 750f);
                randomY = Random.Range(-400f, 200f);

            }
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
        //answerCon.SetActive(true);
        reaction.sprite = emotionImgs[3];
        reaction.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        answerCon.SetActive(false);
        UIObjects[0].SetActive(false);
        reaction.transform.parent.gameObject.SetActive(false);
        GenerateProblem();
    }

    private IEnumerator Incorrect() {
        DragNDrop.allowDrag = false;
        reaction.sprite = timer.CurrentTime() > 10f ? emotionImgs[2] : emotionImgs[0];
        reaction.transform.parent.gameObject.SetActive(true);
        timer.StopTimer();
        timer.ChangeTime(timer.CurrentTime() - 5f);
        yield return new WaitForSeconds(1f);
        DragNDrop.allowDrag = true;
        timer.StartTimer();
        yield return new WaitForSeconds(2f);
        reaction.transform.parent.gameObject.SetActive(false);
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
        //stars.Lose();
        yield return new WaitForSeconds(4f);
        currentRound++;
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
