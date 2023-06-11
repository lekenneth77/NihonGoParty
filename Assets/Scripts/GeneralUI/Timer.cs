using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float timeLimit;
    private float secondsPassed;
    public event Action TimeUp;
    public bool countUp;
    private bool tick;
    // Start is called before the first frame update
    void Start()
    {
        //ResetTimer();
    }

    public void StartTimer()
    {
        tick = true;
    }

    public float CurrentTime()
    {
        return secondsPassed;
    }


    public void ChangeTime(float val)
    {
        secondsPassed = val < 0 ? 0 : val;
        timerText.text = secondsPassed.ToString("0.00");
    }

    public void StopTimer()
    {
        tick = false;
    }

    public void ChangeToCountUp(bool val) {
        countUp = val;
        if (countUp) {
            timeLimit = 0;
        }
    }

    public void ResetTimer()
    {
        tick = false;
        secondsPassed = timeLimit;
        timerText.text = secondsPassed.ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {
        if (tick)
        {
            if (countUp) {
                secondsPassed += Time.deltaTime;
            } else { 
                secondsPassed -= Time.deltaTime;
            }
            if (secondsPassed <= 0f)
            {
                secondsPassed = 0f;
                timerText.text = secondsPassed.ToString("0.00");
                tick = false;
                TimeUp?.Invoke();
            }
            else
            {
                timerText.text = secondsPassed.ToString("0.00");
            }
        }
    }
}
