using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class VocabHunt : Minigame
{
    public HuntPlayer player;
    public HuntPlayer[] characters;
    public CinemachineVirtualCamera followCam;
    public TextAsset textfile;
    public Timer timer;
    public WinStars stars;
    public GameObject[] layouts;
    public TextMeshProUGUI findThisText;
    public Transform interactableParent;
    public Transform bananaParent;
    public GameObject defInteractable;
    public GameObject defBanana;
    public GameObject finishImg;
    public GameObject timeoutImg;
    public GameObject wrongImg;

    private string[] objectNames;
    private List<int> chosenIndices;
    private HashSet<int> chosenPositions; //reused for chosenInteractables too!
    private int currWordIndex;
    public int maxRounds;
    private int numCorrect;

    private float[] scalars = new float[] { 0.5f, 0.75f, 1f, 1.5f };


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        objectNames = textfile.text.Split("\n"[0]);
        timer.TimeUp += Timeout;

        int charIndex = 0;
        if (BoardController.players != null) {
            charIndex = BoardController.currentPlayer.GetComponent<PlayerInfo>().characterIndex;
        } 
        for (int i = 0; i < characters.Length; i++) { 
            if (i != charIndex) {
                characters[i].gameObject.SetActive(false);
            } else
            {
                characters[i].gameObject.SetActive(true);
            }
        }
        player = characters[charIndex];
        followCam.Follow = player.transform;
        player.interacted += CheckAnswer;

        SetupGame();
    }

    private void OnDestroy()
    {
        timer.TimeUp -= Timeout;
        player.interacted -= CheckAnswer;
        player.DisposeControls();
    }

    private void SetupGame() {
        chosenIndices = new List<int>();
        chosenPositions = new HashSet<int>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Minigames/Vocabulary/VocabHunt/ObjectDrawings/");
        Transform chosenLayout = layouts[Random.Range(0, layouts.Length)].transform;
        //generate interactables
        for (int i = 0; i < chosenLayout.childCount; i++) {
            int index = Random.Range(0, sprites.Length);
            while (chosenIndices.Contains(index)) {
                index = Random.Range(0, sprites.Length);
            }
            chosenIndices.Add(index);

            int position = Random.Range(0, chosenLayout.childCount);
            while (!chosenPositions.Add(position)) {
                position = Random.Range(0, chosenLayout.childCount);
            }

            GameObject newInter = Instantiate(defInteractable, chosenLayout.GetChild(position).position, Quaternion.identity, interactableParent);
            float scalar = scalars[Random.Range(0, scalars.Length)];
            newInter.transform.localScale = new Vector2(scalar, scalar);
            newInter.GetComponent<VocabObject>().id = index;
            newInter.GetComponent<SpriteRenderer>().sprite = sprites[index];
            newInter.GetComponent<BoxCollider2D>().size.Scale(new Vector2(scalar, scalar));
        }

        //generate bananas
        for (int i = 0; i < Random.Range(5, 15); i++) {
            Vector2 randomPosition = new Vector2(Random.Range(-26f, 26f), Random.Range(-20f, 20f));
            Instantiate(defBanana, randomPosition, Quaternion.identity, bananaParent);
        }
        chosenPositions.Clear();
        ChooseWord();
        
    }

    private void ChooseWord() {
        int random = Random.Range(0, chosenIndices.Count);
        while (!chosenPositions.Add(random)) {
            random = Random.Range(0, chosenIndices.Count);
        }
        currWordIndex = chosenIndices[random];
        findThisText.text = objectNames[currWordIndex];
        player.EnableControls();
        timer.ResetTimer();
        timer.StartTimer();
    }

    public void CheckAnswer(int id) {
        player.DisableControls();
        if (id == currWordIndex) {
            stars.Win();
            numCorrect++;
            if (numCorrect >= maxRounds) {
                timer.StopTimer();
                StartCoroutine("HandleFinish");
            } else {
                ChooseWord();
            }
        } else {
            StartCoroutine("HandleWrong");
        }

    }

    private IEnumerator HandleWrong() {
        wrongImg.SetActive(true);
        yield return new WaitForSeconds(3f);
        wrongImg.SetActive(false);
        player.EnableControls();
    }

    private IEnumerator HandleFinish() {
        finishImg.SetActive(true);
        yield return new WaitForSeconds(5f);
        FinishThis();
    }

    public void Timeout() {
        player.DisableControls();
        StartCoroutine("HandleTimeout");
    }

    private IEnumerator HandleTimeout() {
        timeoutImg.SetActive(true);
        yield return new WaitForSeconds(3f);
        timeoutImg.SetActive(false);
        stars.Lose();
        numCorrect++;
        if (numCorrect >= maxRounds)
        {
            timer.StopTimer();
            FinishThis();
        }
        else
        {
            ChooseWord();
        }
    }

    private void FinishThis() {
        EndGame(stars.GetWins() - 1);
    }




}
