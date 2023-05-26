using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankSpace : BoardSpace
{
    public override void Start()
    {
        base.Start();
        TypeName = "blank";

    }

    public override void Action()
    {
        Debug.Log("I'm a blank guy");
    }

    
}
