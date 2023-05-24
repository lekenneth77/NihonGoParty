using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{

    [SerializeField] private int debugRoll;
    public int roll;
    public bool debug;
    public SpriteRenderer spriteRenderer;
    private Sprite[] sprites;
    private bool stopRoll, allowStart, allowEnd;
    public static event Action<int> OnDiceFinish;

    //TODO add the inputsystem so that you can make the dice start with the space key? maybe?

    // Start is called before the first frame update
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("DiceSides/");
        Reset();
    }

    private void OnMouseDown()
    {
        if (allowEnd)
        {
            EndRoll();
        } else if (allowStart)
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
            spriteRenderer.sprite = sprites[roll - 1];
            EndRoll();
        }
        while (!stopRoll)
        {
            roll = UnityEngine.Random.Range(0, 6) + 1;
            spriteRenderer.sprite = sprites[roll - 1];
            tillAllowEnd += Time.deltaTime;
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
