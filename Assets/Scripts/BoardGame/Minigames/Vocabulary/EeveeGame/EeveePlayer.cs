using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EeveePlayer : MonoBehaviour
{
    public EeveeWP currentlyOn;
    private MoveObject moveObj;

    // Start is called before the first frame update
    void Start()
    {
        moveObj = GetComponent<MoveObject>();
    }

  

    public void GoToCenter() { 

    }

    
}
