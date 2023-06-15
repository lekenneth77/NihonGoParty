using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pedestal : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] stars;
    public Image letterSprite;
    public bool unableToAnswer;

    private int wins;
    void Start()
    {
        wins = 0;
    }

    public bool Win() {
        stars[wins].SetActive(true);
        wins++;
        return wins >= 2;
    }

    public void Answered() {
        //maybe add like an exclamation mark or something? eh figure it out after we get all models
        letterSprite.color = Color.green;
    }

    public void Loss() {
        letterSprite.color = Color.black;
        unableToAnswer = true;
    }

    public void ResetPedestal() {
        letterSprite.color = Color.white;
        unableToAnswer = false;

    }

}
