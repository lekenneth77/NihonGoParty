using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpace : BoardSpace
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        typeName = "Finish";
    }

    public override void Action()
    {
        Debug.Log("Hey we're finished!!");
        InvokeLoad("FinishScreen", false);
        //for now just load the scene, maybe play an animation or something?
    }

}
