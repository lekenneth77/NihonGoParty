using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordOrder : Minigame
{
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
    public GameObject goToFinishButton;
    public TextMeshProUGUI category;
    public TextMeshProUGUI english;
    public TextMeshProUGUI answer;
    public Timer timer;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI finalText;

    public int totalRounds;

    //handle text functionality
    public TextAsset[] texts;
    private string[] unparsedText;
    private HashSet<int> chosenProbs;
    private List<string> solutions;
    private List<GameObject> generated;
    private int[] locked;

    private int numDropped;
    private int rounds;
    private int wins;
    
    private string[] words;

    // Start is called before the first frame update
    public override void Start() {
        chosenProbs = new HashSet<int>();
        solutions = new List<string>();
        generated = new List<GameObject>();
        nextPos = startingWP.position;
        WordOrderDrop.drop += AnotherDrop;
        WordOrderWord.remove += AnotherRemove;
        timer.TimeUp += Timeout;
        timer.ResetTimer();
        ChooseText();
        GetProblem();
    }

    private void ChooseText() {
        int random = Random.Range(0, texts.Length);
        unparsedText = texts[random].text.Split("\n"[0]);
        category.text = unparsedText[0];
    }

    public void GetProblem() {
        //Reset Structures
        solutions.Clear();
        root.gameObject.SetActive(true);
        nextPos = startingWP.position;
        lenCount = 0;
        ycounter = 0;
        numDropped = 0;
        foreach (GameObject obj in generated) {
            Destroy(obj);
        }
        roundText.text = rounds + 1 + " / " + totalRounds;

        //Choose a random problem
        int random = Random.Range(1, unparsedText.Length);
        while (!chosenProbs.Add(random)) {
            random = Random.Range(1, unparsedText.Length);
        }

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
        timer.ResetTimer();
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
            w.transform.localPosition = new Vector3(-800f + (100f * lenCount) + 50f * i, 150f - (175f * ycounter), 0);
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
            lenCount += words[locked[i]].Length;
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
            GameObject w = MakeWord(word, Vector3.zero, toContainer);
            float xCoord = Random.Range(-825f, 200f - (100f * (w.GetComponent<WordOrderWord>().length - 3)));
            w.transform.localPosition = new Vector3(xCoord, Random.Range(-675f, -375f));
        }
    }


    private int lenCount = 0;
    private int ycounter = 0;
    private GameObject MakeWord(string s, Vector3 pos, Transform parent)
    {
        GameObject word = Instantiate(defClick, startingWP.position, Quaternion.identity, parent);
        word.name = s;
        word.GetComponent<WordOrderWord>().ChangeWord(s);
        Vector3 temp = new Vector3(pos.x, pos.y, 0);
        Vector3 localPos = Vector3.zero;
        foreach (char c in s)
        {
            GameObject letter = Instantiate(defLetter, temp, Quaternion.identity, word.transform);
            letter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = c + "";
            letter.transform.localPosition = localPos;
            localPos.Set(localPos.x + 100f, 0, 0);
        }
        //add dropper
        localPos.Set(localPos.x - 25f, 0, 0);
        GameObject dropper = Instantiate(defDrop, temp, Quaternion.identity, word.transform);
        dropper.transform.localPosition = localPos;
        word.GetComponent<WordOrderWord>().SetDrop(false);
        generated.Add(word);
        return word;
    }

    private void AdjustNextWP(int length) {
        nextPos = new Vector3(nextPos.x + (100f * length) + 50f, nextPos.y);
        if (nextPos.x > 1400) { //test number
            ycounter++;
            nextPos = new Vector3(startingWP.position.x, startingWP.position.y - 175f);
        }
    }

    public void AnotherDrop() {
        numDropped++;
        //check the answer
        if (numDropped == words.Length - locked.Length) {
            WordOrderWord.mrWorldwideDrag = false;
            WordOrderWord next = root.next;
            string theirWord = "";
            while (next != null) {
                theirWord += next.word;
                next = next.next;
            }
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
                    break;
                }
            }

            if (correct) {
                wins++;
                PostRound(false);
            } else {
                StartCoroutine("Wrong");
            }
        }
    }

    public void AnotherRemove() {
        numDropped--;
    }

    public void PostRound(bool failure) {
        giveupButton.enabled = false;
        root.gameObject.SetActive(false);
        rounds++;
        timer.StopTimer();
        if (failure) {
            failureImg.SetActive(true);
            answer.text = solutions[0];
            answer.gameObject.SetActive(true);
            foreach(GameObject obj in generated) {
                Destroy(obj);
            }
        } else {
            successImg.SetActive(true);
        }
        english.gameObject.SetActive(true);


        if (rounds >= totalRounds) {
            //end screen button or something

            goToFinishButton.SetActive(true);
        } else { 
            nextRoundButton.SetActive(true);
        }
    }

    private IEnumerator Wrong() {
        incorrectImg.SetActive(true);
        WordOrderWord.mrWorldwideDrag = false;
        yield return new WaitForSeconds(5f);
        incorrectImg.SetActive(false);
        WordOrderWord.mrWorldwideDrag = true;
    }

    public void Timeout() {
        WordOrderWord.mrWorldwideDrag = false;
        PostRound(true);
    }

    public void SetupFinish() {
        finalText.text = "You completed " + totalRounds + " in " + timer.CurrentTime() + " seconds!";
        finalText.transform.parent.gameObject.SetActive(true);
    }

    public void Finish() {
        EndGame(wins - 1);
    }

}
