using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinStars : MonoBehaviour
{
    public Transform threeStars;
    public Transform twoStars;
    public Transform threeX;
    public Transform twoX;
    public bool three;
    private int round;
    private int wins;
    public void Win() {
        if (three && round >= 3) { return; }
        if (!three && round >= 2) { return; }
        
        if (three) {
            threeStars.GetChild(round).gameObject.SetActive(true);
        } else {
            twoStars.GetChild(round).gameObject.SetActive(true);
        }
        wins++;
        round++;
    }

    public void Lose() {
        if (three && round >= 3) { return; }
        if (!three && round >= 2) { return; }

        if (three) {
            threeX.GetChild(round).gameObject.SetActive(true);
        } else {
            twoX.GetChild(round).gameObject.SetActive(true);
        }
        round++;
    }

    public int GetWins() {
        return wins;
    }

    public int GetLosses() {
        return round - wins;
    }
}
