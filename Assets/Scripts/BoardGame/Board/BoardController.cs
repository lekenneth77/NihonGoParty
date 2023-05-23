using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    //waypoint information
    public Transform[] waypoints; //most likely create a waypointinfo component
    public Transform[] startingWaypoints;
    public GameObject playerOne;

    //player and turn information
    public GameObject[] players;
    public int numPlayers;
    private GameObject currentPlayer;
    private int currentPlayer_i; //index of current player
    
    //dices
    public GameObject mainDice;
    public GameObject[] startingDie;

    //misc sprites
    private Sprite[] diceSprites;


    // Start is called before the first frame update
    void Start()
    {
        diceSprites = Resources.LoadAll<Sprite>("DiceSides/");
        StartCoroutine("SetupOrder");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SetupOrder()
    {
         //setup players at start TODO randomize order based on the dice they roll
        //or something like mario party if you do that make sure to serialize field for main dice
        for (int i = 0; i < numPlayers; i++)
        {
            players[i].transform.position = startingWaypoints[i].position;
        }

        SortedList playerOrder = new SortedList();

        //get the dice rolls
        for (int i = 0; i < numPlayers; i++)
        {
            startingDie[i].SetActive(true);
            Dice currentDice = startingDie[i].GetComponent<Dice>();
            currentDice.Reset();
            while (!currentDice.GetStopRoll())
            {
                //poll for the dice to finish.
                yield return new WaitForSeconds(0.01f);
            }
            startingDie[i].GetComponent<Dice>().SetAllowStart(false);
            int roll = currentDice.GetRoll();
            //its unfair like this but, whatever, does order really matter? this is just for fun
            if (playerOrder.ContainsKey(roll))
            {
                Debug.Log("Reroll!");
                i--;
            } else
            {
                playerOrder.Add(currentDice.GetRoll(), players[i]);
            }
            yield return new WaitForSeconds(0.5f);
        }


        yield return new WaitForSeconds(1f);

        //setup the correct order
        GameObject[] temp = new GameObject[4];
        for (int i = 0; i < numPlayers; i++)
        {
            //startingDie[i].SetActive(false);
            temp[i] = (UnityEngine.GameObject) playerOrder.GetByIndex(numPlayers - (i + 1));
        }

        players = temp;

        currentPlayer_i = 0;
        currentPlayer = players[0];
        Dice.OnDiceFinish += SubscribeMovePlayer;
        mainDice.GetComponent<Dice>().Reset();
    }


    //Event called after the dice is finished. Starts the coroutine in the actual MovePlayer function.
    private void SubscribeMovePlayer(int roll)
    {
        StartCoroutine(MovePlayer(roll));
    }

    //Moves the current player.
    private IEnumerator MovePlayer(int roll)
    {
        BoardMovement moveObj = currentPlayer.GetComponent<BoardMovement>();
        PlayerInfo infoObj = currentPlayer.GetComponent<PlayerInfo>();
        GameObject rollCountdown = currentPlayer.transform.GetChild(0).gameObject;
        SpriteRenderer countdownSprite = rollCountdown.GetComponent<SpriteRenderer>();
        int currentPosition = infoObj.currentPosition;
        rollCountdown.SetActive(true);
        for (int currentStep = 0; currentStep < roll; currentStep++)
        {
            countdownSprite.sprite = diceSprites[roll - currentStep - 1];
            moveObj.target = waypoints[currentPosition + currentStep].localPosition;

            moveObj.moveFlag = true;
            while (moveObj.moveFlag)
            {
                //essentially polling
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Reached Location: " + (currentPosition + currentStep));
            yield return new WaitForSeconds(0.05f);
        }
        rollCountdown.SetActive(false);
        infoObj.currentPosition = currentPosition + roll;
        SetNextPlayer();
        Dice.allowStart = true;
        //for now allow dice to be clicked after movement, but in the future make it happen after a game
    }

    //Set the current player to be the next player.
    private void SetNextPlayer()
    {
        currentPlayer_i = (currentPlayer_i + 1) >= numPlayers ? 0 : currentPlayer_i + 1;
        currentPlayer = players[currentPlayer_i];
    }
}
