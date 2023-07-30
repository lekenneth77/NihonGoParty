using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoardController : MonoBehaviour, Controls.IBoardControllerActions
{
    //all assets on boardgame
    public GameObject allAssets;

    //waypoint information
    public BoardSpace root;
    public Transform[] startingWaypoints;
    public GameObject wpFolder;
    public GameObject wpLineFolder;

    //player and turn information
    public GameObject[] characters; //MUST match the same order as setup screen
    public static GameObject[] players;
    public static int numPlayers;
    public static GameObject currentPlayer;
    private int currentPlayer_i; //index of current player

    //minigame results information
    public static bool wonMinigame;
    public static int minigameResult;
    //singleplayer: int represents number of spaces the player will travel (<= 0 means loss)
    //duel: 1 means player one, 2 means player two
    //multiplayer: index of winner [0, numPlayers - 1]

    //duels
    public GameObject duelPopup;
    public GameObject duelWinChoices;
    private GameObject[] duelists = new GameObject[2];
    private bool duelOn;

    //multiplayer
    private bool multiOn;

    //leaderboard
    public Leaderboard leaderboard;

    //spinner
    public Spinner spinner;

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
    public static bool staticDebug;

    //controls
    private Controls controls;

    //lighting
    public Light lighting;


    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.BoardController.AddCallbacks(this);
        controls.Enable();

        wonMinigame = false;
        freeCameraOn = false;
        diceSprites = Resources.LoadAll<Sprite>("DiceSides/");
        stillCameraCom = stillCameraObj.GetComponent<CinemachineVirtualCamera>();
        moveCameraCom = moveCameraObj.GetComponent<CinemachineVirtualCamera>();
        staticDebug = debug;

        if (debug) {
            //skip deciding who goes first, go straight to immediate roll for movement
            //also able to skip having to go through multiplayer setup
            numPlayers = 4;
            players = new GameObject[numPlayers];
            for (int i = 0; i < 4; i++) {
                players[i] = characters[i];
            }
        } else {
            //must have multiplayer setup to work without debug
            int[] whatTheyChose = MultiplayerSetup.whatThePlayersChose;
            players = new GameObject[numPlayers];
            for (int i = 0; i < numPlayers; i++) {
                players[i] = characters[whatTheyChose[i]];
                players[i].GetComponent<PlayerInfo>().containerPosition = i;
            }
        }


        //numPlayers = 4;


        FinishController.tempResults = players;
        leaderboard.SetNumPlayers(numPlayers);
        MinigameSpace.startedLoad += BeforeMinigameLoad;
        StartCoroutine("SetupOrder");
    }

    //Called once at the start
    private IEnumerator SetupOrder()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            players[i].transform.position = startingWaypoints[i].position;
            DontDestroyOnLoad(players[i]);
        }

        if (!debug)
        {
            SortedList playerOrder = new SortedList();
            List<int> illegalNums = new List<int>();
            //get the dice rolls
            for (int i = 0; i < numPlayers; i++)
            {
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
                players[i].GetComponent<PlayerInfo>().dice.gameObject.SetActive(false);
                temp[i] = (UnityEngine.GameObject)playerOrder.GetByIndex(numPlayers - (i + 1));
            }

            players = temp;
        }

        leaderboard.SetRankings(players);
        leaderboard.SetVisibility(true);

        currentPlayer_i = -1;
        Dice.OnDiceFinish += SubscribeMovePlayer;
        BoardSpace.ActionFinish += AfterSpaceAction;
        spinner.OnSpinFinish += ChooseDuelPlayers;

        SetupNextTurn();
    }
  
    private void SetupNextTurn()
    {
        //lol this is pretty jank and might be slow but it's kind of funny
        //TODO change this so that you only need to store the root of the map not all waypoints!
        ResetSpaces(root);
        /*
        foreach (Transform obj in waypoints)
        {
            obj.gameObject.GetComponent<BoardSpace>().ResetPlayers(true);
        }
        */

        SetNextPlayer();

        Vector3 playerPosition = currentPlayer.transform.position;
        currentPlayer.GetComponent<PlayerInfo>().dice.gameObject.SetActive(true);
        currentPlayer.GetComponent<PlayerInfo>().dice.Reset();
        controls.Enable();
        stillCameraCom.LookAt = currentPlayer.transform;
        stillCameraCom.Follow = currentPlayer.transform;
        stillCameraCom.m_Lens.FieldOfView = STILL_FOV;
    }

    private void ResetSpaces(BoardSpace node) {
        if (!node) { return; }
        node.ResetPlayers(true);
        ResetSpaces(node.nextWP);
        if (node is CrossroadSpace) {
            CrossroadSpace crossNode = (CrossroadSpace)node;
            ResetSpaces(crossNode.alternateWP);
        }
    }


    //Event called after the dice is finished. Starts the coroutine in the actual MovePlayer function.
    private void SubscribeMovePlayer(int roll)
    {
        StartCoroutine(MovePlayer(currentPlayer, roll, true, true));
    }

    //Moves the current player. TODO THINK ABOUT POSSIBLE BUG WHERE GETTING SENT BACK PUTS YOU ON A CROSSROAD MAYBE PUT A LIMIT?
    //IF PREV IS CROSSROAD DON'T GO BACK?
    private IEnumerator MovePlayer(GameObject player, int roll, bool forward, bool triggerAction)
    {
        yield return new WaitForSeconds(1f);
        moveCameraCom.LookAt = player.transform;
        moveCameraCom.Follow = player.transform;
        moveCameraCom.m_Lens.FieldOfView = MOVE_FOV;
        moveCameraObj.SetActive(true);
        player.GetComponent<PlayerInfo>().dice.gameObject.SetActive(false);
        MoveObject moveObj = player.GetComponent<MoveObject>();
        PlayerInfo infoObj = player.GetComponent<PlayerInfo>();
        GameObject rollCountdown = player.transform.GetChild(player.transform.childCount - 1).gameObject;
        SpriteRenderer countdownSprite = rollCountdown.GetComponent<SpriteRenderer>();
        BoardSpace currentSpace = infoObj.currentSpace;
        currentSpace?.RemovePlayer();
        rollCountdown.SetActive(true);

        if (forward)
        {
            //uh should work to count maybe double check this tomorrow?
            if (roll <= 3)
            {
                player.GetComponent<PlayerInfo>().numLowRolls += 1;
            } else
            {
                player.GetComponent<PlayerInfo>().numHighRolls += 1;
            }
        }

        for (int currentStep = 1; currentStep <= roll; currentStep++)
        {
            currentSpace = infoObj.currentSpace;
            BoardSpace nextSpace;
            if (currentSpace)
            {
                nextSpace = forward ? currentSpace.chosenPath : currentSpace.prevWP;
            } else
            {
                nextSpace = root;
                //nextSpace = waypoints[0].GetComponent<BoardSpace>(); 
            }
            //handles going backwards onto a crossroad or going past the starting point
            if (nextSpace is CrossroadSpace && !forward || !nextSpace && !forward) { break; }

            countdownSprite.sprite = diceSprites[roll - currentStep];
            infoObj.numCrossed = forward ? infoObj.numCrossed + 1 : infoObj.numCrossed - 1;
            moveObj.SetTargetAndMove(nextSpace.transform.position);
            nextSpace.AdjustPlayers();
            while (moveObj.GetMoveFlag())
            {
                //essentially polling
                yield return new WaitForSeconds(0.01f);
            }
            //maybe takeout the slight pause when youre going backwards
            //yield return new WaitForSeconds(0.025f);
            if (currentStep != roll)
            {
                nextSpace.ResetPlayers(false);
            }
            leaderboard.UpdateBoard(players);
            //crossroad logic
            if (nextSpace is CrossroadSpace) //YOU ONLY WANT TO DO AN ACTION MIDLOOP IF ITS A CROSSROAD SPACE...
            {
                bool finished = false;
                nextSpace.Action(); 
                while (!finished)
                {
                    finished = CrossroadArrow.choseSomething;
                    if (finished)
                    {
                        nextSpace.chosenPath = CrossroadArrow.altPathChosen ? ((CrossroadSpace)nextSpace).alternateWP : nextSpace.nextWP;
                        ((CrossroadSpace)nextSpace).DeactivateArrows();
                    }
                    yield return new WaitForSeconds(0.01f);
                }
                CrossroadArrow.choseSomething = false;
                currentStep--; 
            }

            infoObj.currentSpace = nextSpace; //nextWP if forward, prevWP if backwards.
            if (nextSpace is FinishSpace) {
                break; 
            }
        }
        yield return new WaitForSeconds(0.1f); 
        moveCameraObj.SetActive(false);
        moveObj.RotateToIdentity();
        yield return new WaitForSeconds(2f);
        rollCountdown.SetActive(false);
        infoObj.currentSpace.AddPlayer(player);

        //don't think you need the second conditional???
        if (triggerAction || infoObj.currentSpace is FinishSpace)
        {
            //EVENTUALLY ADD THE RANDOM MINIGAME THING SELECTION LIKE MARIO PARTY
            if (infoObj.currentSpace is MinigameSpace) { 

                if(((MinigameSpace)infoObj.currentSpace).category.ToUpper().Equals("DUEL")) {
                    //hanldes duels
                    if (numPlayers == 2) {
                        ChooseDuelPlayers(0);
                    } else { 
                        List<Sprite> spinnerSprites = new List<Sprite>();
                        foreach (GameObject p in players)
                        {
                            if (p != player)
                            {
                                spinnerSprites.Add(p.GetComponent<PlayerInfo>().sprite);
                            }
                        }
                        spinner.TriggerSpin(spinnerSprites);
                    }
                } else if (((MinigameSpace)infoObj.currentSpace).category.ToUpper().Equals("MULTI"))
                {
                    //multiplayer games
                    multiOn = true;
                    SpaceAction();
                } else {
                    //singleplayer minigame
                    SpaceAction();
                }

            } else {
                //regular action spaces
                SpaceAction();
            }

        } else {
            SetupNextTurn();
        }
    }

    //Set the current player to be the next player.
    private void SetNextPlayer()
    {
        currentPlayer_i = (currentPlayer_i + 1) >= numPlayers ? 0 : currentPlayer_i + 1;
        currentPlayer = players[currentPlayer_i];
    }

    private void SpaceAction()
    {
        controls.Disable();
        currentPlayer.GetComponent<PlayerInfo>().currentSpace.Action();
    }

    private void BoardVisibility(bool vis) {
        allAssets.SetActive(vis);
        wpFolder.SetActive(vis);
        wpLineFolder.SetActive(vis);
        lighting.gameObject.SetActive(vis);
    }

    public void BeforeMinigameLoad() {
        BoardVisibility(false);
        foreach (GameObject player in players) {
            player.SetActive(false);
        }
    }

    private void AfterSpaceAction()
    {
        BoardVisibility(true);
        foreach (GameObject player in players)
        {
            player.SetActive(true);
        }
        BoardSpace currentSpace = currentPlayer.GetComponent<PlayerInfo>().currentSpace;
        if (currentSpace is MinigameSpace)
        {
            if (multiOn)
            {
                //TODO be more creative with this at some point...!!! maybe use the spinner?
                multiOn = false;
                StartCoroutine(MovePlayer(players[minigameResult], 4, true, false));
                return;
            }
            if (duelOn)
            {
                duelWinChoices.SetActive(true);
                return;
            }
            //due to probably change, a result of 0 or less movement means they loss the minigame
            if (minigameResult > 0) {
                currentPlayer.GetComponent<PlayerInfo>().numMinigamesWon += 1;
                StartCoroutine(MovePlayer(currentPlayer, minigameResult, true, false));
            } else {
                StartCoroutine(MovePlayer(currentPlayer, Mathf.Abs(minigameResult), false, false));
            }
        }
        else
        {
            SetupNextTurn();
        }
    }

    //challenger index is zero indexed
    private void ChooseDuelPlayers(int challengerIndex)
    {
        duelOn = true;
        duelists[0] = currentPlayer;
        foreach (GameObject player in players)
        {
            if (player == currentPlayer) { continue; }
            if (challengerIndex == 0)
            {
                duelists[1] = player;
                break;
            }
            challengerIndex--;
        }

        StartCoroutine("PopupDuel");
    }

    private IEnumerator PopupDuel()
    {
        leaderboard.SetVisibility(false);
        for (int i = 0; i < 2; i++)
        {
            duelPopup.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = duelists[i].GetComponent<PlayerInfo>().sprite;
        }
        duelPopup.SetActive(true);
        yield return new WaitForSeconds(5f);
        duelPopup.SetActive(false);
        SpaceAction();
    }

    public void DuelChoice(bool winnerForward)
    {
        duelWinChoices.SetActive(false);
        //minigame result = 1 means player one wins
        GameObject winner = minigameResult == 1 ? duelists[0] : duelists[1];
        GameObject loser = minigameResult == 1 ? duelists[1] : duelists[0];
        winner.GetComponent<PlayerInfo>().numMinigamesWon += 1;
        duelOn = false;
        leaderboard.SetVisibility(true);
        if (winnerForward)
        {
            StartCoroutine(MovePlayer(winner, 3, true, false));
        } else
        {
            //TODO might be a bug if the loser is on the starting platform!
            StartCoroutine(MovePlayer(loser, 3, false, false));
        }
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
