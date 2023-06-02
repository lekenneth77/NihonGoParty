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
    public Camera playerTwoCamera;
    public GameObject twoPlayerContainer;
    public Timer timer;
    public GameObject subtractTimerText;

    public static bool duel;
    private string[] centerLetters = new string[] { "A", "B", "C", "D", "E" };

    private Controls controls;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        controls = new Controls();
        controls.KanjiCrossRotate.AddCallbacks(this);
        controls.Disable();

        timer.TimeUp += TimeOut;
        duel = true;
        
        if (duel)
        {
            timer.gameObject.transform.localPosition = new Vector3(0, 425, 0);
            playerTwoCamera.gameObject.SetActive(true);
            twoPlayerContainer.SetActive(true);
            Camera.main.rect = new Rect(0, 0, 0.5f, 1);
        } else
        {
            timer.gameObject.transform.localPosition = new Vector3(750, 375, 0);
            playerTwoCamera.gameObject.SetActive(false);
            twoPlayerContainer.SetActive(false);
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }

        StartCoroutine("RandomizeIt");
    }

    private IEnumerator RandomizeIt()
    {
        playerOne.ChangeSpeed(1.5f);
        playerOne.GoHomeBoxes();
        if (duel)
        {
            playerTwo.ChangeSpeed(1.5f);
            playerTwo.GoHomeBoxes();
        }

        //randomize center
        for (int i = 0; i < 25; i++)
        {
            string str = centerLetters[Random.Range(0, centerLetters.Length)];
            playerOne.centerText.text = str;
            if (duel) { playerTwo.centerText.text = str; }
            yield return new WaitForSeconds(0.05f);
        }

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
        if (duel) { playerTwo.sphere.damping = 4; }
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
            Debug.Log("Nice");
            if (player.Win())
            {
                Debug.Log("They Won! Stop the game!");
            } else
            {
                Debug.Log("Next Round!");
                yield return new WaitForSeconds(2f);
                StartCoroutine("RandomizeIt");
            }
        } else
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
        //uh do something
        Debug.Log("Times up!");
        if (duel)
        {
            StartCoroutine("RandomizeIt");
        } else
        {
            controls.Disable();
            Debug.Log("Ah shit.");
            EndGame(false);
        }
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
