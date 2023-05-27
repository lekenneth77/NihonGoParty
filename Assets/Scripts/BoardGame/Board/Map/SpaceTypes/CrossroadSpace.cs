using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossroadSpace : BoardSpace
{
    public BoardSpace alternateWP;
    private GameObject arrows;

    public override void Start()
    {
        base.Start();
        TypeName = "Crossroad";
        arrows = transform.GetChild(0).gameObject;
        DeactivateArrows();
    }

    public override void Action()
    {
        arrows.SetActive(true);
    }

    public void DeactivateArrows()
    {
        arrows.SetActive(false);
    }

}
