using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class EeveeGame : Minigame, Controls.IQuizGameActions
{
    public Transform[] wps;
    public EeveePlayer[] characterModels;
    public Transform playerFolder;
    public Transform nonRotator;
    public TextMeshProUGUI middleTxt;
    public EeveeCircle rotator;
    public WinStars[] winners;

    //ui
    public TextMeshProUGUI categoryText;
    public Image[] portraits;
    private Sprite[] charPortraits;

    private EeveePlayer[] players = new EeveePlayer[4];
    private int numPlayers;

    private List<string> correctWords;
    private List<string> wrongOnes;
    private List<EeveePlayer> queue = new List<EeveePlayer>();
    private bool itsWrong;
    private int numGotItWrong;
    private bool someoneGotItRight;
    private TextAsset[] texts;



    private Controls controls;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        controls = new Controls();
        controls.QuizGame.AddCallbacks(this);
        controls.QuizGame.Enable();
        texts = Resources.LoadAll<TextAsset>("Minigames/Vocabulary/ChapterVocabulary/");
        charPortraits = Resources.LoadAll<Sprite>("Images/CharacterPortraits/");


        if (BoardController.players == null) {
            numPlayers = 4;
            for (int i = 0; i < numPlayers; i++) {
                characterModels[i].myWP = wps[i];
                players[i] = characterModels[i];
                players[i].transform.SetParent(playerFolder);
                portraits[i].transform.parent.gameObject.SetActive(true);
            }
        } else {
            GameObject[] p = BoardController.originalOrderPlayers;
            for (int i = 0; i < numPlayers; i++) {
                int charIndex = p[i].GetComponent<PlayerInfo>().characterIndex;
                characterModels[charIndex].myWP = wps[i];
                portraits[i].sprite = charPortraits[charIndex];
                players[i] = characterModels[charIndex];
                players[i].transform.SetParent(playerFolder);
                portraits[i].transform.parent.gameObject.SetActive(true);
            }
            if (numPlayers == 2) {
                players[1].myWP = wps[2];
            }
        }

        ResetRound();

    }

    public void ResetRound() { 
        float initialRotY = 0;
        for (int i = 0; i < numPlayers; i++) {
            rotator.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            players[i].transform.SetParent(playerFolder);
            players[i].rotate = false;
            players[i].transform.eulerAngles = new Vector3(0, initialRotY, 0f);
            players[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            initialRotY -= 90f;
            if (numPlayers == 2) { initialRotY -= 90f; }
            players[i].gameObject.SetActive(true);
            players[i].transform.position = players[i].myWP.position;
            players[i].transform.GetChild(0).GetComponent<Animator>().Play("just_walk");
            players[i].stars.gameObject.SetActive(false);
            players[i].wentOnce = false;
            EeveePlayer.wrong = false;
        }
        someoneGotItRight = false;
        numGotItWrong = 0;
        middleTxt.text = "";
        ChooseProblem();
        StartCoroutine("HandleRound"); //round loop

    }

    //copied logic from treehop, could be a lot faster but im very tired
    public void ChooseProblem() {
        itsWrong = true;
        EeveePlayer.wrong = itsWrong;

        HashSet<int> chosenIndicies = new HashSet<int>();
        int random = Random.Range(0, texts.Length);
        chosenIndicies.Add(random);
        TextAsset txtfile = texts[random];
        string[] words = txtfile.text.Split("\n"[0]);
        categoryText.text = words[0];
        //add to correct
        correctWords = new List<string>();
        for (int i = 1; i < words.Length; i++)
        {
            correctWords.Add(words[i]);
        }

        //get two random text files and add to incorrect
        wrongOnes = new List<string>();
        random = Random.Range(0, texts.Length);
        while (!chosenIndicies.Add(random))
        {
            random = Random.Range(0, texts.Length);
        }
        txtfile = texts[random];
        words = txtfile.text.Split("\n"[0]);
        for (int i = 1; i < words.Length; i++)
        {
            wrongOnes.Add(words[i]);
        }

        random = Random.Range(0, texts.Length);
        while (!chosenIndicies.Add(random))
        {
            random = Random.Range(0, texts.Length);
        }
        txtfile = texts[random];
        words = txtfile.text.Split("\n"[0]);
        for (int i = 1; i < words.Length; i++)
        {
            wrongOnes.Add(words[i]);
        }

    }

    private IEnumerator HandleRound() {
        controls.QuizGame.Enable();
        yield return new WaitForSeconds(Random.Range(1f, 3f)); //initial waitup
        while (numGotItWrong != numPlayers && !someoneGotItRight) {
            //..50 50?
            bool correct = Random.Range(0, 2) == 0;
            itsWrong = !correct;
            EeveePlayer.wrong = itsWrong;
            //grab from the right pile
            string s = correct ? correctWords[Random.Range(0, correctWords.Count)] : wrongOnes[Random.Range(0, wrongOnes.Count)];
            middleTxt.text = s;

            //how long the word appears in the middle
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            if (!someoneGotItRight) { 
                middleTxt.text = "";
            }
            itsWrong = true;
            EeveePlayer.wrong = itsWrong;

            //how long until the next word appears
            yield return new WaitForSeconds(Random.Range(0.5f, 5f));
        
        }

        if (numGotItWrong == numPlayers) {
            yield return new WaitForSeconds(3f);
            ResetRound();
        }
    }

    private IEnumerator SomeoneGotIt(int i) {
        StopCoroutine("HandleRound");
        yield return new WaitForSeconds(2f);
        controls.QuizGame.Disable();
        yield return new WaitForSeconds(3f);
        winners[i].Win();
        if (winners[i].GetWins() == 3) {
            Debug.Log("Done!");
            yield return new WaitForSeconds(3f);
            EndMultiplayerGame(i);
        } else { 
            ResetRound();
        }
    }

    

    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed || players[0].wentOnce) { return; }
        players[0].wentOnce = true;
        players[0].transform.SetParent(nonRotator);
        players[0].GoToCenter();
        if (itsWrong) {
            numGotItWrong++;
        } else if (!someoneGotItRight){
            queue.Add(players[0]);
            someoneGotItRight = true;
            StartCoroutine(SomeoneGotIt(0));
        }
    }
    public void OnR(InputAction.CallbackContext context)
    {
        if (!context.performed || players[1].wentOnce) { return; }
        players[1].wentOnce = true;
        players[1].transform.SetParent(nonRotator);
        players[1].GoToCenter();
        if (itsWrong) {
            numGotItWrong++;
        } else if (!someoneGotItRight) {
            queue.Add(players[1]);
            someoneGotItRight = true;
            StartCoroutine(SomeoneGotIt(1));
        }
    }

    public void OnH(InputAction.CallbackContext context)
    {
        if (!context.performed || numPlayers < 3 || players[2].wentOnce) { return; }
        players[2].wentOnce = true;
        players[2].transform.SetParent(nonRotator);
        players[2].GoToCenter();
        if (itsWrong) {
            numGotItWrong++;
        } else if (!someoneGotItRight){
            queue.Add(players[2]);
            someoneGotItRight = true;
            StartCoroutine(SomeoneGotIt(2));
        }
    }

    public void OnL(InputAction.CallbackContext context)
    {
        if (!context.performed || numPlayers < 4 || players[3].wentOnce) { return; }
        players[3].wentOnce = true;
        players[3].transform.SetParent(nonRotator);
        players[3].GoToCenter();
        if (itsWrong) {
            numGotItWrong++;
        } else if(!someoneGotItRight){
            queue.Add(players[3]);
            someoneGotItRight = true;
            StartCoroutine(SomeoneGotIt(3));
        }
    }



}
