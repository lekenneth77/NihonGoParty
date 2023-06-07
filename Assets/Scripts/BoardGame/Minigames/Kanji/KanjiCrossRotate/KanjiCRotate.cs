using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    private List<string> centerers;
    private List<string> rotators;
    private List<int> chosen;

    private bool loss; //only applies to single player
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
        //duel = true;
        loss = false;
        won = false;
        
        if (duel)
        {
            timer.gameObject.transform.localPosition = new Vector3(0, 425, 0);
            playerTwoCamera.gameObject.SetActive(true);
            twoPlayerContainer.SetActive(true);
            playerOneCamera.rect = new Rect(0, 0, 0.5f, 1);
        } else
        {
            Debug.Log("hey");
            timer.gameObject.transform.localPosition = new Vector3(750, 375, 0);
            playerTwoCamera.gameObject.SetActive(false);
            twoPlayerContainer.SetActive(false);
            playerOneCamera.rect = new Rect(0, 0, 1, 1);
        }
        GetCrosses();
        StartCoroutine("RandomizeIt");
    }

    private void GetCrosses()
    {
        centerers = new List<string>();
        rotators = new List<string>();
        chosen = new List<int>();
        TextAsset txtFile = Resources.Load<TextAsset>("Minigames/Kanji/KanjiCross/kanjicrosses");
        string[] crosses = txtFile.text.Split("\n"[0]);
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
                Debug.Log("Ran out of crosses!");
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
        controls.Enable();
        timer.ResetTimer();
        timer.StartTimer();
        
    }

    private IEnumerator Confirm(KCRotatePlayer player)
    {
        player.ChangeSpeed(0.25f);
        player.ConfirmBoxes();
        
        yield return new WaitForSeconds(1.5f);
        if (player.currentRotation == 0 || Mathf.Abs(player.currentRotation) <= Mathf.Epsilon)
        {
            timer.StopTimer();
            won = player.Win();
            winner = player;
            yield return new WaitForSeconds(1f);
            ShowAnswer();
        }
        else
        {
            if (!duel)
            {
                timer.StopTimer();
                timer.ChangeTime(timer.CurrentTime() - 30f);
                subtractTimerText.GetComponent<FadeInOutObject>().InitiateFade();
                subtractTimerText.GetComponent<MoveObject>().TriggerMove();
            }
            
            player.ChangeSpeed(1.5f);
            player.GoHomeBoxes();
            yield return new WaitForSeconds(0.5f);
            if (timer.CurrentTime() > 0)
            {
                controls.Enable();
            }
            timer.StartTimer();
            if (!duel)
            {
                subtractTimerText.transform.localPosition = subtractTimerText.GetComponent<MoveObject>().originalPosition;
            }
        }
    }
    private void TimeOut()
    {
        //todo it's very possible that a duel someone can just cheese it w spam but whatever fix it later
        Debug.Log("Times up!");
        if (!duel)
        {
            controls.Disable();
            Debug.Log("Ah shit.");
            loss = true;
        }
        ShowAnswer();

    }

    public void ShowAnswer()
    {
        int current = chosen[chosen.Count - 1];
        solution.UpdateSolutionBoard(centerers[current], rotators[current]);
        solution.gameObject.SetActive(true);
    }

    public void CloseAnswer()
    {
        if (loss)
        {
            //TODO maybe feature like a failure at the end or something...
            Debug.Log("Failure...");
            StartCoroutine("Failure");
        }
        else if (won) {
            Debug.Log("Won!");
            StartCoroutine("Win");
        }
        else
        {
            Debug.Log("Next Round!");
            StartCoroutine("RandomizeIt");
        }
    }

    public IEnumerator Win()
    {
        yield return null;
        if (duel)
        {
            EndGame(playerOne == winner);
        } else
        {
            EndGame(true);
        }
    }

    public IEnumerator Failure()
    {
        yield return null;
        EndGame(false);
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
        controls.Disable();
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
        controls.Disable();
        StartCoroutine(Confirm(playerTwo));
    }

   
}
