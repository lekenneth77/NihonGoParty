using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class KanjiDnD : Minigame
{
    public Transform leftSide, rightSide;
    private List<DnDInfo> dropSpots = new List<DnDInfo>(); //generate solution boxes here
    public HashSet<int> chosenProblems;
    public GameObject defDraggable;
    public GameObject defDropper;
    public Canvas canvas;

    //camera
    public KanjiDnDCamera cam;
    private bool camGoUp;

    //text bubble
    public TextMeshProUGUI textBubble;

    //patience box
    public Image patienceBar;
    public Image emotion;
    public Timer timer;
    private Sprite[] emotionImgs;

    //reaction image
    public Image reaction;

    //models
    public GameObject defWalker;
    private GameObject currentWalker;
    public Transform walkerFolder;

    public GameObject defCake;
    public Transform wsCanvas;

    public Transform spawnPt;
    public Transform centerPt;
    public Transform exitPt;

    public TextAsset textFile;
    private Sprite[] parts;
    private Sprite[] fullKanjis;
    private string[] problems;

    //result container
    public WinStars stars;
    public Image[] resultKanjis;
    public TextMeshProUGUI[] resultHiras;
    public TextMeshProUGUI resultDialogue;
    public GameObject exit;
    
    private string[] resultText = new string[] { "I should study...", "Not great....", "Not bad!", "That was amazing!" };
    public GameObject[] tiredObjects;

    //cutscenes
    public PlayableDirector outro;

    //round info
    public int totalRounds;
    private int currentRound;
    private bool loss;
   
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
        outro.stopped += HandleEnding;


        currentRound = 0;
        StartCoroutine("SpawnAndWalk");
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
        currentWalker = Instantiate(defWalker, spawnPt.position, Quaternion.identity, walkerFolder);
        yield return new WaitForSeconds(1f);
        MoveObject move = currentWalker.GetComponent<MoveObject>();
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
            if (loss) {
                StartCoroutine("RoundFail");
            } else {
                StartCoroutine("RoundWin");
            }
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

    private IEnumerator RoundWin() {
        GameObject faller = Instantiate(defCake, new Vector3(0, 10f, -11f), Quaternion.identity, wsCanvas);
        MoveObject move = faller.GetComponent<MoveObject>();
        move.SetTargetAndMove(new Vector3(0, 4f, -11f));
        while (move.GetMoveFlag()) { 
            yield return new WaitForSeconds(0.1f);
        }
        move.transform.SetParent(currentWalker.transform);
        yield return new WaitForSeconds(1f);
        GameObject copy = currentWalker;
        move = copy.GetComponent<MoveObject>();
        move.SetTargetAndMove(exitPt.position);

        //handle next round!
        if (totalRounds == currentRound) {
            outro.Play();
        } else {
            StartCoroutine("SpawnAndWalk");
        }
        while (move.GetMoveFlag()) {
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(copy);
    }

    private IEnumerator RoundFail() {
        GameObject copy = currentWalker;
        MoveObject move = copy.GetComponent<MoveObject>();
        move.SetTargetAndMove(exitPt.position);

        //handle next round!
        //yield return new WaitForSeconds(1f);
        if (totalRounds == currentRound) {
            outro.Play();
        } else {
            StartCoroutine("SpawnAndWalk");
        }
        while (move.GetMoveFlag()) {
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(copy);
    }

    public void HandleEnding(PlayableDirector dir) {
        StartCoroutine("Ending");
    }
    private IEnumerator Ending() {
        yield return new WaitForSeconds(1f);
        resultDialogue.text = resultText[stars.GetWins()];
        foreach (GameObject tired in tiredObjects) {
            tired.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.75f);
        resultDialogue.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        FinalResult();
        //exit.SetActive(true);
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
                return;
            }
        }
        //all are correct!
        timer.StopTimer();
        stars.Win();
        loss = false;
        currentRound++;
        StartCoroutine("Correct");
        
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
        //fullImg.sprite = fullKanjis[random];
        string problem = problems[random];
        //parse string 
        string[] parameters = problem.Split("_"[0]);
        string[] ids = parameters[2].Split(","[0]);
        string[] xy = parameters[3].Split(","[0]);

        GenerateLeftItems(ids);
        GenerateRightItems(ids, xy);

        timer.ResetTimer();

        resultKanjis[currentRound].sprite = fullKanjis[random];
        resultHiras[currentRound].text = parameters[1];
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

    private IEnumerator Correct() {
        //make some box closing animation instead? if you find a way to do that?
        //right now its just a quick camera cut
        reaction.sprite = emotionImgs[3];
        reaction.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        reaction.transform.parent.gameObject.SetActive(false);
        leftSide.gameObject.SetActive(false);
        rightSide.gameObject.SetActive(false);
        emotion.transform.parent.gameObject.SetActive(false);
        cam.transform.rotation = Quaternion.identity;
        camGoUp = true;
        AfterCamera();
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
        if (!loss) { 
            reaction.transform.parent.gameObject.SetActive(false);
        }
    }

    public void TimeOut() {
        DragNDrop.allowDrag = false;
        currentRound++;
        loss = true;
        stars.Lose();
        StartCoroutine("HandleTimeOut");
    }

    private IEnumerator HandleTimeOut() {
        reaction.sprite = emotionImgs[0];
        reaction.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        reaction.transform.parent.gameObject.SetActive(false);
        leftSide.gameObject.SetActive(false);
        rightSide.gameObject.SetActive(false);
        emotion.transform.parent.gameObject.SetActive(false);
        cam.transform.rotation = Quaternion.identity;
        camGoUp = true;
        AfterCamera();
    }

    public void FinalResult() {
        int result = stars.GetWins() - 1;
        EndGame(result);
    }

}
