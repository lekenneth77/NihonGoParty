using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EeveeCircle : MonoBehaviour
{
    // Start is called before the first frame update
    public float spinSpeed;
    public bool bruh;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bruh) { transform.Rotate(0, -spinSpeed, 0); } else { 
            transform.Rotate(0, 0, spinSpeed);
        }
    }
}
