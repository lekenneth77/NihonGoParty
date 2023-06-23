using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanjiFishing : Minigame
{
    public Hook hook;
    public GameObject defFish;
    public Transform fishParent;
    public Vector2 startingFishWP;
    private Vector2[] waypoints = new Vector2[10];
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        defFish.GetComponent<Fish>().StopSwim();
        for (int i = 0; i < 10; i++) {
            waypoints[i] = startingFishWP;
            startingFishWP = new Vector2(startingFishWP.x, startingFishWP.y - 3f);
            Instantiate(defFish, startingFishWP, Quaternion.identity, fishParent);
        }

    }



}
