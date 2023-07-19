using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGPlayerPhase : MonoBehaviour
{
    public RPGCircle leftCircle;
    public RPGCircle rightCircle;
    public GameObject arrow;
    public int countClicks = 0;
    public event Action<bool> phaseComplete;
    // Start is called before the first frame update
    void Start()
    {
        leftCircle.GotClicked += CheckCircles;
        rightCircle.GotClicked += CheckCircles;
    }

    private void OnDisable()
    {
        leftCircle.aboveText.gameObject.SetActive(false);
        leftCircle.alreadyClicked = false;
        leftCircle.disable = false;
        rightCircle.aboveText.gameObject.SetActive(false);
        rightCircle.alreadyClicked = false;
        rightCircle.disable = false;
        countClicks = 0;
    }

    //terrible, terrible design
    public void CheckCircles(bool left, bool giver) {
        //im so tired
        countClicks++;
        if (countClicks == 2) {
            //first should be false because this is the second holy shit im so tired
            leftCircle.disable = true;
            rightCircle.disable = true;
            arrow.transform.eulerAngles = left ? new Vector3(0, 0, 270f) : new Vector3(0, 0, 90f);
            arrow.SetActive(true);
            phaseComplete?.Invoke(!giver);
        }
    }
    
   
}
