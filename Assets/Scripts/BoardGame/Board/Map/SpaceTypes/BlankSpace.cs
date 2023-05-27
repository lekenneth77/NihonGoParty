using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankSpace : BoardSpace
{

    public override void Start()
    {
        base.Start();
        typeName = "Blank";

    }

    public override void Action()
    {
        Debug.Log("I'm a blank guy");
        InvokeFinish();
        //TODO uh find a way to do nothing on this square
        //subscribe to the chosen minigame here like
        //minigame.OnFinish += InvokeFinish();
        //and then the board controller will subscribe to board space creating this chain!
    }

    
}
