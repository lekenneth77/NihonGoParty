using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{

    [SerializeField] private int roll;
    public bool debug;
    public SpriteRenderer spriteRenderer;
    private Sprite[] sprites;
    private bool allowStart, allowEnd, stopRoll; //TODO make allowStart a public static, so that the controller can set the dice
                                                 //to be interactable again instead of it resetting immediately once the dice is done.
    public static event Action<int> OnDiceFinish;

    // Start is called before the first frame update
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("DiceSides/");
        allowStart = true;
        allowEnd = false;
        stopRoll = false;
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
        stopRoll = true;
        Debug.Log("Roll: " + roll);
        allowStart = true;
        allowEnd = false;
        //roll = 1;
        OnDiceFinish?.Invoke(roll);
    }


}
