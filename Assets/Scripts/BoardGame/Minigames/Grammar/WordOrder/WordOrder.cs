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
    public GameObject defClick;
    public GameObject defLetter;
    public GameObject defDrop;

    public TextMeshProUGUI category;
    public TextMeshProUGUI english;


    //handle text functionality
    private string[] unparsedText;
    private HashSet<int> chosenProbs;
    private List<string> solutions;
    private int[] locked;
    Queue<GameObject> theFront;
    private List<GameObject> clicks;
    
    private string[] words;
    private string sentence;

    // Start is called before the first frame update
    public override void Start() {
        base.Start();
        clicks = new List<GameObject>();
        chosenProbs = new HashSet<int>();
        solutions = new List<string>();
        theFront = new Queue<GameObject>();
        nextPos = startingWP.position;
        //WordOrderWord.gotClicked += HandleClick;
        ChooseText();
        GetProblems();
    }

    private void ChooseText() {
        TextAsset[] texts = Resources.LoadAll<TextAsset>("Minigames/Grammar/WordOrder/Texts/");
        int random = Random.Range(0, texts.Length);
        random = 0;
        unparsedText = texts[random].text.Split("\n"[0]);
        //category.text = unparsedText[0];
    }

    private void GetProblems() {
        //Reset Structures
        solutions.Clear();
        theFront.Clear();

        //Choose a random problem
        int random = Random.Range(1, unparsedText.Length);
        while (!chosenProbs.Add(random)) {
            random = Random.Range(1, unparsedText.Length);
        }
        random = 1;

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
        //english.text = parsedProb[parsedProb.Length - 2];
        //locked numbers
        string[] lockStr = parsedProb[parsedProb.Length - 1].Split(","[0]);
        locked = new int[lockStr.Length];
        for (int i = 0; i < lockStr.Length; i++) {
            locked[i] = int.Parse(lockStr[i]) - 1;
        }
        Debug.Log(words.Length);
        GenerateLocked(words);
        GenerateClickables(words);
    }

  

    private void GenerateLocked(string[] words) {
        WordOrderWord prev = null;
        for (int i = 0; i < locked.Length - 1; i++) {
            //gen word
            GameObject w = MakeWord(words[locked[i]], nextPos, toContainer);
            AdjustNextWP(words[locked[i]].Length);
            w.GetComponent<WordOrderWord>().allowDrag = false;
            w.GetComponent<WordOrderWord>().SetDrop(true);
            if (prev != null) {
                prev.next = w.GetComponent<WordOrderWord>();
                w.GetComponent<WordOrderWord>().prev = prev;
            }
            prev = w.GetComponent<WordOrderWord>();
        }

        if (locked[0] != 0) { 
           //tbh might be easier to whip out like an invisible one or something
        }

        GameObject word = MakeWord(words[locked[locked.Length - 1]], nextPos, toContainer);
        AdjustNextWP(words[locked[locked.Length - 1]].Length);
        word.GetComponent<WordOrderWord>().allowDrag = false;
        //word.GetComponent<WordOrderWord>().SetDrop(false); //this assumes the last locked is always the last character
        prev.next = word.GetComponent<WordOrderWord>();
        word.GetComponent<WordOrderWord>().prev = prev;
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
        if (nextPos.x >= 800) { //test number
            nextPos = new Vector3(startingWP.position.x, startingWP.position.y - 100f);
        }
    }



    public void HandleClick(GameObject obj) {
        clicks.Add(obj);
        if (clicks.Count == words.Length) {
            Debug.Log("No more.");
        }
    }



    private IEnumerator LoadResources()
    {
        //inter_sprites = Resources.LoadAll<Sprite>("Minigames/Grammar/WordOrder/interactables/");
        //seacreatures = Resources.LoadAll<Sprite>("Minigames/Grammar/WordOrder/seacreatures/");
        //seacreature_img.sprite = seacreatures[Random.Range(0, seacreatures.Length)];
        //TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>("SU3DJPFont/TextMeshProFont/Selected/");
        //font = fonts[0];
        yield break;
    }

}
