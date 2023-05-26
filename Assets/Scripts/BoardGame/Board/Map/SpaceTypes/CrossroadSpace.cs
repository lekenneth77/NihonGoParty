using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossroadSpace : BoardSpace
{
    public BoardSpace alternateWP;
    private GameObject arrows;
    private bool finished = false;

    //probably add reference to the arrows perhaps


    public override void Start()
    {
        base.Start();
        TypeName = "crossroad";
        arrows = transform.GetChild(0).gameObject;
        DeactivateArrows();
    }

    public override void Action()
    {
        arrows.SetActive(true);
        Debug.Log("Cross Action!");
    }

    public void DeactivateArrows()
    {
        arrows.SetActive(false);
    }

}
