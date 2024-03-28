using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class KanjiCRotate : Minigame, Controls.IKanjiCrossRotateActions
{
    public KCRotatePlayer playerOne;
    public KCRotatePlayer playerTwo;
    public Camera playerOneCamera;
    public Camera playerTwoCamera;
    public GameObject twoPlayerContainer;
    public Timer timer;
    public GameObject subtractTimerText;
    public KCSolutionPopup solution;
    public GameObject success;
    public GameObject failure;
    public GameObject nice;
    public int maxRounds;
    public int round;
    public Sprite[] pImages;
    public Image[] pIcons;
    public TextAsset textfile;

    private List<string> centerers;
    private List<string> rotators;
    private List<int> chosen;

    private bool duel;
    private bool won;
    private KCRotatePlayer winner; //only applies to duels

    private Controls controls;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        controls = new Controls();
        controls.KanjiCrossRotate.AddCallbacks(this);
        controls.Disable();

        timer.TimeUp += TimeOut;
        won = false;
        duel = !singleplayer; //jank renaming
        if (duel)
        {
            timer.gameObject.transform.localPosition = new Vector3(0, 425, 0);
            timer.countUp = true;
            timer.timeLimit = 0f;
            timer.ResetTimer();
            playerTwoCamera.gameObject.SetActive(true);
            twoPlayerContainer.SetActive(true);
            playerOneCamera.rect = new Rect(0, 0, 0.5f, 1);
            if (BoardController.players != null) {
                GameObject[] duelists = BoardController.duelists;
                for (int i = 0; i < 2; i++) {
                    int charIndex = duelists[i].GetComponent<PlayerInfo>().characterIndex;
                    pIcons[i].sprite = pImages[charIndex];
                }
            }
        } else
        {
            timer.gameObject.transform.localPosition = new Vector3(750, 375, 0);
            timer.ResetTimer();
            playerTwoCamera.gameObject.SetActive(false);
            twoPlayerContainer.SetActive(false);
            playerOneCamera.rect = new Rect(0, 0, 1, 1);
            if (BoardController.players != null) {
                int charIndex = BoardController.currentPlayer.GetComponent<PlayerInfo>().characterIndex;
                pIcons[0].sprite = pImages[charIndex];
            }
        }
        GetCrosses();
        StartCoroutine("RandomizeIt");
    }

    private void GetCrosses()
    {
        centerers = new List<string>();
        rotators = new List<string>();
        chosen = new List<int>();
        string[] crosses = textfile.text.Split("\n"[0]);
        foreach(string cross in crosses)
        {
            string[] split = cross.Split("="[0]);
            centerers.Add(split[0]);
            rotators.Add(split[1]);
        }
    }

    private IEnumerator RandomizeIt()
    {
        playerOne.ChangeSpeed(1.5f);
        playerOne.GoHomeBoxes();
        playerOne.SetRotatingText("    ");
        if (duel)
        {
            playerTwo.ChangeSpeed(1.5f);
            playerTwo.GoHomeBoxes();
            playerTwo.SetRotatingText("    ");
        }

        //randomize center
        int index = 0;
        for (int i = 0; i < 25; i++)
        {
            index = Random.Range(0, centerers.Count);
            string str = centerers[index];
            playerOne.centerText.text = str;
            if (duel) { playerTwo.centerText.text = str; }
            yield return new WaitForSeconds(0.05f);
        }
        if (chosen.Contains(index))
        {
            if (chosen.Count == centerers.Count)
            {
                chosen.Clear();
            }
            while (chosen.Contains(index))
            {
                index = Random.Range(0, centerers.Count);
            }
            string str = centerers[index];
            playerOne.centerText.text = str;
            if (duel) { playerTwo.centerText.text = str; }
        }
        chosen.Add(index);
        


        playerOne.sphere.damping = 20;
        playerTwo.sphere.damping = 20;

        int[] multiplier = new int[] { 1, -1 };
        for (int i = 0; i < Random.Range(10, 15); i++)
        {
            playerOne.RotateSphere(90f * multiplier[Random.Range(0, 2)]);
            if (duel) { playerTwo.RotateSphere(90f * multiplier[Random.Range(0, 2)]); }
            yield return new WaitForSeconds(0.2f);
        }

        playerOne.sphere.damping = 4;
        playerOne.SetRotatingText(rotators[index]);
        if (duel) { 
            playerTwo.sphere.damping = 4;
            playerTwo.SetRotatingText(rotators[index]);
        }
        gotIt = false;
        controls.Enable();
        timer.ResetTimer();
        timer.StartTimer();
        
    }

    private bool gotIt = false;
    private IEnumerator Confirm(KCRotatePlayer player)
    {
        if (gotIt || failure.activeInHierarchy) { yield break; }
        player.ChangeSpeed(0.25f);
        player.ConfirmBoxes();
        if (gotIt || failure.activeInHierarchy) { yield break; }
        yield return new WaitForSeconds(1.5f);
        if (gotIt || failure.activeInHierarchy) { yield break; }
        if (player.currentRotation == 0 || Mathf.Abs(player.currentRotation) <= Mathf.Epsilon)
        {
            timer.StopTimer();
            gotIt = true;
            winner = player;
            yield return new WaitForSeconds(1f);
            controls.KanjiCrossRotate.Disable();
            nice.gameObject.SetActive(true);
            won = player.Win();
            yield return new WaitForSeconds(3f);
            nice.gameObject.SetActive(false);
            ShowAnswer();
        }
        else
        {
            player.ChangeSpeed(1.5f);
            player.GoHomeBoxes();
            yield return new WaitForSeconds(0.5f);
            if (timer.CurrentTime() > 0)
            {
                controls.Enable();
            }
            timer.StartTimer();
        }
    }
    private void TimeOut()
    {
        if (gotIt) { return; }
        //todo it's very possible that a duel someone can just cheese it w spam but whatever fix it later
        if (!duel)
        {
            controls.Disable();
        }
        StartCoroutine("Failure");

    }

    public void ShowAnswer()
    {
        int current = chosen[chosen.Count - 1];
        solution.UpdateSolutionBoard(centerers[current], rotators[current]);
        solution.gameObject.SetActive(true);
    }

    public void CloseAnswer()
    {
        round++;
         if (won) {
            StartCoroutine("Win");
        } else if (round == maxRounds) {
            StartCoroutine("Win");
        } else
        {
            StartCoroutine("RandomizeIt");
        }
    }

    public IEnumerator Win()
    {
        yield return null;
        if (duel)
        {
            int result = playerOne == winner ? 1 : 2;
            EndGame(result);
        } else
        {
            success.SetActive(true);
            yield return new WaitForSeconds(5f);
            EndGame(playerOne.wins - 1); //do the wins later
        }
    }

    public IEnumerator Failure()
    {
        failure.SetActive(true);
        yield return new WaitForSeconds(3f);
        failure.SetActive(false);
        ShowAnswer();
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        playerOne.RotateSphere(90f);
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        playerOne.RotateSphere(-90f);
    }
    public void OnW(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        //controls.Disable();
        controls.KanjiCrossRotate.A.Disable();
        controls.KanjiCrossRotate.D.Disable();
        controls.KanjiCrossRotate.W.Disable();
        StartCoroutine(Confirm(playerOne));
    }

    public void OnLeftKey(InputAction.CallbackContext context)
    {
        if (!duel || !context.performed) { return; }
        playerTwo.RotateSphere(90f);
    }

    public void OnRightKey(InputAction.CallbackContext context)
    {
        if (!duel || !context.performed) { return; }
        playerTwo.RotateSphere(-90f);
    }

    public void OnUpKey(InputAction.CallbackContext context)
    {
        if (!duel || !context.performed) { return; }
        controls.KanjiCrossRotate.LeftKey.Disable();
        controls.KanjiCrossRotate.RightKey.Disable();
        controls.KanjiCrossRotate.UpKey.Disable();
        StartCoroutine(Confirm(playerTwo));
    }

   
}
