using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dice : MonoBehaviour, Controls.IDiceActions
{

    [SerializeField] private int debugRoll;
    public int roll;
    public bool debug;
    public SpriteRenderer spriteRenderer;
    private Sprite[] sprites;
    private bool stopRoll, allowStart, allowEnd;
    public static event Action<int> OnDiceFinish;
    public event Action clickedOnce;
    public bool disable;
    private Controls controls;

    //TODO add the inputsystem so that you can make the dice start with the space key? maybe?

    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.Dice.AddCallbacks(this);
        controls.Dice.Enable();
        debug = BoardController.staticDebug;
        sprites = Resources.LoadAll<Sprite>("DiceSides/");
        Reset();
    }

    private void OnMouseDown()
    {
        if (disable) { return; }
        clickedOnce?.Invoke();
        HandleDice();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (!context.performed || disable) { return; }
        clickedOnce?.Invoke();
        HandleDice();
    }

    private void HandleDice()
    {
        if (allowEnd)
        {
            EndRoll();
        }
        else if (allowStart)
        {
            StartCoroutine("RollDice");
        }
    }

    private IEnumerator RollDice()
    {
        allowStart = false;
        stopRoll = false;
        float tillAllowEnd = 0f;
        if (debug)
        {
            roll = debugRoll;
            if (roll < 6) { 
                spriteRenderer.sprite = sprites[roll - 1];
            } 
            EndRoll();
        }
        while (!stopRoll)
        {
            roll = UnityEngine.Random.Range(0, 6) + 1;
            spriteRenderer.sprite = sprites[roll - 1];
            tillAllowEnd += Time.fixedDeltaTime;
            allowEnd = tillAllowEnd > 0.1f;
            yield return new WaitForSeconds(0.075f);
        }
    }

    private void EndRoll()
    {
        allowStart = false;
        allowEnd = false;
        stopRoll = true;
        //roll = 1;
        OnDiceFinish?.Invoke(roll);
    }

    public void SetAllowStart(bool val)
    {
        allowStart = val;
    }

    public void Reset()
    {
        allowStart = true;
        allowEnd = false;
        stopRoll = false;
    }

    public bool GetStopRoll()
    {
        return stopRoll;
    }

    public int GetRoll()
    {
        return roll;
    }

    
}
