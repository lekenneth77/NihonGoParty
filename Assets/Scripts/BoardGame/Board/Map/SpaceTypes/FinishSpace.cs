using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpace : BoardSpace
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        TypeName = "finish";
    }

    public override void Action()
    {
        throw new System.NotImplementedException();
    }

}
