using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KatakanaSearch : Minigame
{
    // Start is called before the first frame update
    public TextAsset hiraganaText;
    public TextAsset katakanaText;
    public Transform instFolder;
    public Transform[] startingPositions;
    public GameObject defKata;

    private HashSet<int> chosenRandomChars;
    private HashSet<int> chosenCorrectChars;
    private HashSet<int> chosenPositions;
    private string hiraganas;
    private string katakanas;

    public override void Start()
    {
        base.Start();
        chosenCorrectChars = new HashSet<int>();
        chosenRandomChars = new HashSet<int>();
        chosenPositions = new HashSet<int>();
        hiraganas = hiraganaText.text;
        katakanas = katakanaText.text;
        KatakanaCollider.GotClicked += IsThatRight;
        SetupRound();
    }

    private void SetupRound() { 
        foreach (Transform child in instFolder) {
            Destroy(child.gameObject);
        }
        chosenRandomChars.Clear();
        chosenPositions.Clear();
        int random = Random.Range(0, hiraganas.Length);
        while (!chosenCorrectChars.Add(random)) {
            random = Random.Range(0, hiraganas.Length);
        }
        chosenRandomChars.Add(random);

        int position = Random.Range(0, startingPositions.Length);
        chosenPositions.Add(position);

        CreateKatakana(random, startingPositions[position].localPosition).GetComponent<KatakanaCollider>().correctOne = true;
        Debug.Log(katakanas[random]);

        for (int i = 0; i < 19; i++) {
            random = Random.Range(0, hiraganas.Length);
            while (!chosenRandomChars.Add(random))
            {
                random = Random.Range(0, hiraganas.Length);
            }
            position = position = Random.Range(0, startingPositions.Length);
            while (!chosenPositions.Add(position)) {
                position = Random.Range(0, startingPositions.Length);
            }
            CreateKatakana(random, startingPositions[position].localPosition);
        }
    }

    private GameObject CreateKatakana(int stringIndex, Vector2 position) {
        GameObject obj = Instantiate(defKata, instFolder);

        obj.transform.localPosition = position;
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = katakanas[stringIndex] + "";
        return obj;
    }

    public void IsThatRight(bool yes) { 
        if (yes) {
            Debug.Log("Correct!");
        } else {
            Debug.Log("Wrong!");
        }
    }
    
}
