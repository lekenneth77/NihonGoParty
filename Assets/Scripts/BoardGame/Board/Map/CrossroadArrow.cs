using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CrossroadArrow : MonoBehaviour
{
    public bool altPath;
    public static bool altPathChosen; //true if alt path is chosen, false if next path is chosen
    public static bool choseSomething;
    // Start is called before the first frame update
    void Start()
    {
        choseSomething = false;
    }

    private void OnMouseDown()
    {
        Debug.Log("I chose alternate: " + altPath);
        altPathChosen = altPath;
        choseSomething = true;
    }

    private void OnMouseEnter() {
        GetComponent<Animator>().Play("Big");
    }

    private void OnMouseExit() {
        GetComponent<Animator>().Play("idle");
    }
}
