using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class BoardController : MonoBehaviour, Controls.IBoardControllerActions
{

    //waypoint information
    public Transform[] waypoints; //most likely create a waypointinfo component
    public Transform[] startingWaypoints;

    //player and turn information
    public GameObject[] players;
    public static int numPlayers;
    private GameObject currentPlayer;
    private int currentPlayer_i; //index of current player

    //leaderboard
    public Leaderboard leaderboard;

    //dices
    public GameObject mainDice;
    public GameObject[] startingDie;

    //camera
    public GameObject stillCameraObj, moveCameraObj, freeCameraObj;
    private CinemachineVirtualCamera stillCameraCom, moveCameraCom;
    private const float START_FOV = 60f;
    private const float STILL_FOV = 30f;
    private const float MOVE_FOV = 50f;
    private bool freeCameraOn;

    //misc sprites
    private Sprite[] diceSprites;

    //debug flag
    public bool debug;

    //controls
    private Controls controls;


    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.BoardController.AddCallbacks(this);
        controls.Enable();

        freeCameraOn = false;
        diceSprites = Resources.LoadAll<Sprite>("DiceSides/");
        stillCameraCom = stillCameraObj.GetComponent<CinemachineVirtualCamera>();
        moveCameraCom = moveCameraObj.GetComponent<CinemachineVirtualCamera>();

        if (debug)
        {
            numPlayers = 4;
        }
        numPlayers = 4;
        leaderboard.SetNumPlayers(numPlayers);
        StartCoroutine("SetupOrder");
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    //Called once at the start
    private IEnumerator SetupOrder()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            players[i].transform.position = startingWaypoints[i].position;
        }

        if (!debug)
        {
            SortedList playerOrder = new SortedList();
            List<int> illegalNums = new List<int>();
            //get the dice rolls
            for (int i = 0; i < numPlayers; i++)
            {
                Dice currentDice = startingDie[i].GetComponent<Dice>();
                startingDie[i].SetActive(true);
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
                    currentDice.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = diceSprites[roll - 1];
                }
                playerOrder.Add(roll, players[i]);
                illegalNums.Add(roll);
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(2f);

            //setup the correct order
            GameObject[] temp = new GameObject[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                startingDie[i].SetActive(false);
                temp[i] = (UnityEngine.GameObject)playerOrder.GetByIndex(numPlayers - (i + 1));
            }

            players = temp;
        }

        leaderboard.SetRankings(players);
        leaderboard.SetVisibility(true);

        currentPlayer_i = -1;
        Dice.OnDiceFinish += SubscribeMovePlayer;
        SetupNextTurn();
    }

    private void SetupNextTurn()
    {
        //lol this is pretty jank and might be slow but it's kind of funny
        foreach (Transform obj in waypoints)
        {
            obj.gameObject.GetComponent<SpaceInfo>().ResetPlayers(true);
        }
        SetNextPlayer();

        Vector3 playerPosition = currentPlayer.transform.position;
        mainDice.SetActive(true);
        mainDice.transform.position = new Vector3(playerPosition.x, playerPosition.y + 1.5f, playerPosition.z);
        mainDice.GetComponent<Dice>().Reset();

        stillCameraCom.LookAt = currentPlayer.transform;
        stillCameraCom.Follow = currentPlayer.transform;
        stillCameraCom.m_Lens.FieldOfView = STILL_FOV;

    }


    //Event called after the dice is finished. Starts the coroutine in the actual MovePlayer function.
    private void SubscribeMovePlayer(int roll)
    {
        StartCoroutine(MovePlayer(roll));
    }

    //Moves the current player.
    private IEnumerator MovePlayer(int roll)
    {
        yield return new WaitForSeconds(1f);
        moveCameraCom.LookAt = currentPlayer.transform;
        moveCameraCom.Follow = currentPlayer.transform;
        moveCameraCom.m_Lens.FieldOfView = MOVE_FOV;
        moveCameraObj.SetActive(true);
        mainDice.SetActive(false);
        BoardMovement moveObj = currentPlayer.GetComponent<BoardMovement>();
        PlayerInfo infoObj = currentPlayer.GetComponent<PlayerInfo>();
        GameObject rollCountdown = currentPlayer.transform.GetChild(0).gameObject;
        SpriteRenderer countdownSprite = rollCountdown.GetComponent<SpriteRenderer>();
        int currentPosition = infoObj.currentPosition;
        if (currentPosition != -1)
        {
            waypoints[currentPosition].GetComponent<SpaceInfo>().RemovePlayer();
        }
        rollCountdown.SetActive(true);
        bool finished = false;
        for (int currentStep = 1; currentStep <= roll; currentStep++)
        {
            countdownSprite.sprite = diceSprites[roll - currentStep];
            infoObj.currentPosition = currentPosition + currentStep;

            //check if reached goal line
            finished = currentPosition + currentStep == (waypoints.Length - 1);

            SpaceInfo spaceInfo = waypoints[currentPosition + currentStep].GetComponent<SpaceInfo>();
            moveObj.SetTargetAndMove(spaceInfo.transform.position);
            spaceInfo.AdjustPlayers();
            while (moveObj.GetMoveFlag())
            {
                //essentially polling
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Reached Location: " + (currentPosition + currentStep));
            yield return new WaitForSeconds(0.05f);
            if (currentStep != roll)
            {
                spaceInfo.ResetPlayers(false);
            }
            leaderboard.UpdateBoard(players);
            if (finished) { break; }
        }
        
        yield return new WaitForSeconds(0.1f);
        moveCameraObj.SetActive(false);
        yield return new WaitForSeconds(2f);
        rollCountdown.SetActive(false);
        if (finished)
        {
            Finish();
            yield break;
        }
        waypoints[currentPosition + roll].GetComponent<SpaceInfo>().AddPlayer(currentPlayer);
        SetupNextTurn();
    }

    //Set the current player to be the next player.
    private void SetNextPlayer()
    {
        currentPlayer_i = (currentPlayer_i + 1) >= numPlayers ? 0 : currentPlayer_i + 1;
        currentPlayer = players[currentPlayer_i];
    }

    private void Finish()
    {
        Debug.Log("Finish!");
    }

    public void OnToggleFreelook(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (!freeCameraOn)
        {
            freeCameraObj.SetActive(true);
        } else
        {
            freeCameraObj.SetActive(false);
        }
        freeCameraOn = !freeCameraOn;
    }
}
