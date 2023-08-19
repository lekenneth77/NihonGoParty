using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class BoardGameIntro : MonoBehaviour
{
    // Start is called before the first frame update
    public BoardController myCon;
    public GameObject milo;
    public TextMeshProUGUI miloTxt;
    public PlayableDirector mapCutscene;

    private int numPlayers;
    private GameObject[] players;
    private Leaderboard lb;
    private string[] lbAnims = new string[] { "p1SlideIn", "p2SlideIn", "p3SlideIn", "p4SlideIn" };
    private string[] places = new string[] { "First", "Second", "Third", "Fourth" };

    void Start()
    {
        mapCutscene.stopped += AfterCutscene;
    }

    public void AfterCutscene(PlayableDirector unused) {
        StartCoroutine(AfterAfterCutscene());
    }

    private IEnumerator AfterAfterCutscene() {
        yield return new WaitForSeconds(0.5f);
        miloTxt.text = "";
        miloTxt.transform.parent.gameObject.SetActive(true);
        string dialogue = "Now, let us start the game.";
        int j = 0;
        while (j < dialogue.Length) {
            miloTxt.text += dialogue[j];
            if (dialogue[j] != ' ') {
                yield return new WaitForSeconds(0.025f);
            } 
            j++;
        }
        yield return new WaitForSeconds(1.5f);

        milo.GetComponent<Animator>().Play("miloAway");
        miloTxt.text = "";
        miloTxt.transform.parent.gameObject.SetActive(true);
        dialogue = "Good luck!";
        j = 0;
        while (j < dialogue.Length)
        {
            miloTxt.text += dialogue[j];
            if (dialogue[j] != ' ')
            {
                yield return new WaitForSeconds(0.025f);
            }
            j++;
        }
        yield return new WaitForSeconds(2f);
        miloTxt.transform.parent.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        foreach(GameObject p in players) {
            p.transform.GetChild(0).GetComponent<Animator>().Play("walk");
        }
        myCon.StartRound();
    }

    public void LetsStart() {
        //maybe do a black screen fade in?

        numPlayers = BoardController.numPlayers;
        players = BoardController.players;
        lb = myCon.leaderboard;
        StartCoroutine("WalkOn");
    }

    private IEnumerator WalkOn() {

        for (int i = 0; i < numPlayers; i++) {
            players[i].transform.position = myCon.startingWaypoints[4].position;
            DontDestroyOnLoad(players[i]);
        }

        for (int i = numPlayers - 1; i >= 0; i--) {
            players[i].GetComponent<MoveObject>().SetTargetAndMove(myCon.startingWaypoints[i].position);
            yield return new WaitForSeconds(0.75f);
        }
        milo.GetComponent<Animator>().Play("miloAppear");
        yield return new WaitForSeconds(1f);
        float initRotY = 200f;
        for (int i = 0; i < numPlayers; i++) {
            players[i].GetComponent<MoveObject>().RotateToAngle(0, initRotY, 0);
            initRotY -= 10f;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }

        yield return new WaitForSeconds(1f);
        miloTxt.text = "";
        miloTxt.transform.parent.gameObject.SetActive(true);
        string dialogue = "Welcome!";
        int j = 0;
        while (j < dialogue.Length) {
            miloTxt.text += dialogue[j];
            if (dialogue[j] != ' ') {
                yield return new WaitForSeconds(0.025f);
            } 
            j++;
        }
        yield return new WaitForSeconds(2f);

        dialogue = "First, let's decide the order of play.";
        miloTxt.text = "";
        j = 0;
        while (j < dialogue.Length) {
            miloTxt.text += dialogue[j];
            if (dialogue[j] != ' ') {
                yield return new WaitForSeconds(0.025f);
            } 
            j++;
        }
        yield return new WaitForSeconds(2f);
        miloTxt.transform.parent.gameObject.SetActive(false);

        //handle rolling for order
        SortedList playerOrder = new SortedList();
        List<int> illegalNums = new List<int>();
        for (int i = 0; i < numPlayers; i++) { 
            players[i].GetComponent<MoveObject>().RotateToIdentity();
            yield return new WaitForSeconds(0.5f);
            Dice currentDice = players[i].GetComponent<PlayerInfo>().dice;
            currentDice.gameObject.SetActive(true);
            currentDice.Reset();

            while (!currentDice.GetStopRoll())
            {
                //poll for the dice to finish.
                yield return new WaitForSeconds(0.01f);
            }
            currentDice.SetAllowStart(false); //statics are literally bum i hate it
            int roll = currentDice.GetRoll();
            if (illegalNums.IndexOf(roll) != -1) //no more rerolls
            {
                Debug.Log("Duplicate!");
                while (illegalNums.IndexOf(roll) != -1)
                {
                    roll = UnityEngine.Random.Range(0, 6) + 1;
                }
                currentDice.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = myCon.diceSprites[roll - 1];
            }
            playerOrder.Add(roll, players[i]);
            illegalNums.Add(roll);
            yield return new WaitForSeconds(0.5f);
        }

        GameObject[] temp = new GameObject[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            //players[i].GetComponent<PlayerInfo>().dice.gameObject.SetActive(false);
            temp[i] = (UnityEngine.GameObject)playerOrder.GetByIndex(numPlayers - (i + 1));
        }

        lb.SetRankings(temp);
        BoardController.players = temp;

        miloTxt.text = "";
        miloTxt.transform.parent.gameObject.SetActive(true);
        dialogue = "The order has been decided!";
        j = 0;
        while (j < dialogue.Length)
        {
            miloTxt.text += dialogue[j];
            if (dialogue[j] != ' ')
            {
                yield return new WaitForSeconds(0.025f);
            }
            j++;
        }
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < numPlayers; i++) {
            miloTxt.text = "";
            miloTxt.transform.parent.gameObject.SetActive(true);
            int containerPos = temp[i].GetComponent<PlayerInfo>().containerPosition;
            dialogue = places[i] + " is P" + (containerPos + 1) + "!";
           

            j = 0;
            while (j < dialogue.Length)
            {
                miloTxt.text += dialogue[j];
                if (dialogue[j] != ' ')
                {
                    yield return new WaitForSeconds(0.025f);
                }
                j++;
            }

            yield return new WaitForSeconds(1f);
            miloTxt.transform.parent.gameObject.SetActive(false);
            temp[i].GetComponent<PlayerInfo>().dice.gameObject.SetActive(false);
            lb.leaderboard[containerPos].SetActive(true);
            lb.GetComponent<Animator>().Play(lbAnims[containerPos]);
            string anim = i < numPlayers / 2 ? "victory" : "lose";
            temp[i].transform.GetChild(0).GetComponent<Animator>().Play(anim);

            yield return new WaitForSeconds(1.5f);
        }

        miloTxt.text = "";
        miloTxt.transform.parent.gameObject.SetActive(true);
        dialogue = "Now let's take a look at where we are going.";
        j = 0;
        while (j < dialogue.Length) {
            miloTxt.text += dialogue[j];
            if (dialogue[j] != ' ') {
                yield return new WaitForSeconds(0.025f);
            }
            j++;
        }
        yield return new WaitForSeconds(1.5f);
        miloTxt.transform.parent.gameObject.SetActive(false);

        mapCutscene.Play();
    }

}
