using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class WordOrder : Minigame
{

    private AudioSource deal_card, failure_sfx, success_sfx, buttonclick_sfx, kids_cheer;
    private Sprite[] seacreatures;

    public Transform startingWP;
    public Transform toContainer;
    public Transform fromContainer;
    public static Vector3 nextPos;
    public WordOrderWord root;
    public GameObject defClick;
    public GameObject defLetter;
    public GameObject defDrop;
    public GameObject defLock;
    public GameObject defNum;

    public GameObject successImg, failureImg, incorrectImg;
    public GameObject nextRoundButton;
    public Button giveupButton;
    public TextMeshProUGUI category;
    public TextMeshProUGUI english;
    public Timer timer;
    public TextMeshProUGUI roundText;

    public int totalRounds;

    //handle text functionality
    private string[] unparsedText;
    private HashSet<int> chosenProbs;
    private List<string> solutions;
    private int[] locked;

    private int numDropped;
    private int rounds;
    private int wins;
    
    private string[] words;

    // Start is called before the first frame update
    public override void Start() {
        base.Start();
        chosenProbs = new HashSet<int>();
        solutions = new List<string>();
        nextPos = startingWP.position;
        WordOrderDrop.drop += AnotherDrop;
        WordOrderWord.remove += AnotherRemove;
        ChooseText();
        GetProblem();
        timer.ResetTimer();
    }

    private void ChooseText() {
        TextAsset[] texts = Resources.LoadAll<TextAsset>("Minigames/Grammar/WordOrder/Texts/");
        int random = Random.Range(0, texts.Length);
        random = 4;
        unparsedText = texts[random].text.Split("\n"[0]);
        category.text = unparsedText[0];
    }

    private void GetProblem() {
        //Reset Structures
        solutions.Clear();
        root.gameObject.SetActive(true);

        //Choose a random problem
        int random = Random.Range(1, unparsedText.Length);
        while (!chosenProbs.Add(random)) {
            random = Random.Range(1, unparsedText.Length);
        }
        random = 8;

        string[] parsedProb = unparsedText[random].Split("_"[0]);

        //words used to generate clickables
        words = parsedProb[0].Split("　"[0]);

        //get string solutions, -2 due to english and locked
        for (int i = 0; i < parsedProb.Length - 2; i++) {
            string solution = "";
            string[] soluArr = parsedProb[i].Split("　"[0]);
            foreach (string s in soluArr) {
                solution += s;
            }
            solutions.Add(solution);
        }
        //english
        english.text = parsedProb[parsedProb.Length - 2];
        //locked numbers
        string[] lockStr = parsedProb[parsedProb.Length - 1].Split(","[0]);
        locked = new int[lockStr.Length];
        for (int i = 0; i < lockStr.Length; i++) {
            locked[i] = int.Parse(lockStr[i]) - 1;
        }
        GenerateLocked(words);
        GenerateClickables(words);
        WordOrderWord.mrWorldwideDrag = true;
        timer.StartTimer();
        giveupButton.enabled = true;
    }


    //THIS GAME ASSUMES THAT EVERY PROBLEM HAS THE FIRST AND THE LAST ONE LOCKED AT LEAST, not anymore
    private void GenerateLocked(string[] words) {
        WordOrderWord prev = root;
        for (int i = 0; i < locked.Length; i++) {
            //gen word
            string curWord = words[locked[i]];
            GameObject w = MakeWord(curWord, nextPos, toContainer);
            AdjustNextWP(curWord.Length);
            w.GetComponent<WordOrderWord>().allowDrag = false;
            w.GetComponent<WordOrderWord>().SetDrop(locked[i] != this.words.Length - 1);

            GameObject lockimg = Instantiate(defLock, Vector2.zero, Quaternion.identity, w.transform);
            lockimg.transform.localPosition = new Vector2((100f * (curWord.Length - 1)) + 35f, -35f);

            GameObject numtxt = Instantiate(defNum, Vector2.zero, Quaternion.identity, w.transform);
            numtxt.transform.localPosition = new Vector2(100 * (curWord.Length - 1) / 2, 75f);
            numtxt.GetComponent<TextMeshProUGUI>().text = locked[i] + 1 + "";

            prev.next = w.GetComponent<WordOrderWord>();
            w.GetComponent<WordOrderWord>().prev = prev;
            prev = w.GetComponent<WordOrderWord>();
            if (locked[i] == 0) {
                root.gameObject.SetActive(false);
            }
        }

    }

    private void GenerateClickables(string[] words) {
        HashSet<int> chosenWords = new HashSet<int>();
        foreach (int l in locked) {
            chosenWords.Add(l);
        }
        for (int i = 0; i < words.Length - locked.Length; i++) { 
            int random = Random.Range(0, words.Length);
            while (!chosenWords.Add(random)) {
                random = Random.Range(0, words.Length);
            }
            string word = words[random];
            Vector3 pos = new Vector3(Random.Range(100f, 1300f), Random.Range(100, 400f));
            MakeWord(word, pos, fromContainer);
        }
    }

    private GameObject MakeWord(string s, Vector3 pos, Transform parent)
    {
        GameObject word = Instantiate(defClick, pos, Quaternion.identity, parent);
        word.name = s;
        word.GetComponent<WordOrderWord>().ChangeWord(s);
        Vector2 temp = new Vector2(pos.x, pos.y);
        foreach (char c in s)
        {
            GameObject letter = Instantiate(defLetter, temp, Quaternion.identity, word.transform);
            letter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = c + "";
            temp.Set(temp.x + 100f, temp.y);
        }
        //add dropper
        temp.Set(temp.x - 25f, temp.y);
        Instantiate(defDrop, temp, Quaternion.identity, word.transform);
        word.GetComponent<WordOrderWord>().SetDrop(false);
        return word;
    }

    private void AdjustNextWP(int length) {
        nextPos = new Vector3(nextPos.x + (100f * length) + 50f, nextPos.y);
        if (nextPos.x > 1400) { //test number
            Debug.Log("Hello?");
            nextPos = new Vector3(startingWP.position.x, startingWP.position.y - 175f);
        }
    }

    public void AnotherDrop() {
        numDropped++;
        //check the answer
        if (numDropped == words.Length - locked.Length) {
            Debug.Log("Time to check!");
            WordOrderWord.mrWorldwideDrag = false;
            WordOrderWord next = root.next;
            string theirWord = "";
            while (next != null) {
                theirWord += next.word;
                next = next.next;
            }
            Debug.Log(theirWord);
            bool correct = true;
            foreach (string solution in solutions) {
                correct = true;
                for (int i = 0; i < solution.Length; i++) { 
                    if (!solution[i].Equals(theirWord[i])) {
                        correct = false;
                        break;
                    }
                }
                if (correct) {
                    //found a matching one
                    break;
                }
            }

            if (correct) {
                Debug.Log("Correct!");
                timer.StopTimer();
                wins++;
                rounds++;
                PostRound(false);
            } else {
                Debug.Log("Wrong!");
            }
        }
    }

    public void AnotherRemove() {
        numDropped--;
    }

    public void PostRound(bool failure) {
        giveupButton.enabled = false;
        if (failure) {
            failureImg.SetActive(true);
        } else {
            successImg.SetActive(true);
        }
        english.gameObject.SetActive(true);


        if (rounds >= totalRounds) { 
            //end screen button or something
        } else { 
            nextRoundButton.SetActive(true);
        }
    }


}
