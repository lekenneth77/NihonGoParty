using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spinner : MonoBehaviour
{
    public Transform pointer;
    public float speed;

    public GameObject[] spinners;
    public GameObject currentSpinner;

    public Image[] twoPlayerSprites, threePlayerSprites, fourPlayerSprites;
    private Image[][] playerSprites = new Image[3][];

    private bool spin;
    private bool slowDown;
    private int numPlayers;

    public event Action<int> OnSpinFinish;

    public void TriggerSpin(List<Sprite> sprites)
    {
        //could be put into start function but that requires the spinner gameobj to be active so nope
        //i wanna see my game screen
        playerSprites[0] = twoPlayerSprites;
        playerSprites[1] = threePlayerSprites;
        playerSprites[2] = fourPlayerSprites;
        numPlayers = sprites.Count;

        currentSpinner.SetActive(false);
        currentSpinner = spinners[numPlayers - 2];

        Image[] spinnerSprites = playerSprites[numPlayers - 2];
        for (int i = 0; i < numPlayers; i++)
        {
            spinnerSprites[i].sprite = sprites[i];
        }

        currentSpinner.SetActive(true);
        this.gameObject.SetActive(true);
        StartCoroutine("SpinIt");
    }

    public void DebugSpin()
    {
        if (spin) { return; } 
        Transform pointerRot = pointer.gameObject.transform;
        pointerRot.eulerAngles = Vector3.zero;
        numPlayers = 4;
        currentSpinner.SetActive(false);
        currentSpinner = spinners[numPlayers - 2];
        currentSpinner.SetActive(true);
        StartCoroutine("SpinIt");
    }


    public IEnumerator SpinIt()
    {
        yield return new WaitForSeconds(0.5f);
        spin = true;
        float normalSpinTime = UnityEngine.Random.Range(1f, 3f);
        yield return new WaitForSeconds(normalSpinTime);
        slowDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (spin)
        {
            if (slowDown)
            {
                speed *= 0.97f;
            }
            pointer.Rotate(0, 0, -speed);
            if (speed < 0.05f) { 
                spin = false; 
                slowDown = false;
                StartCoroutine("GetPosition");
            }
        }
    }

    private IEnumerator GetPosition()
    {
        speed = 8;
        yield return new WaitForSeconds(1f);
        float angle = pointer.rotation.eulerAngles.z;
        int position = (int) (angle / (360f / (float)numPlayers));
        this.gameObject.SetActive(false);
        OnSpinFinish?.Invoke(position);
    }
}
