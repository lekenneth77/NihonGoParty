using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleLever : MonoBehaviour
{
    public Transform stick;
    public float speed;
    public Quaternion desiredRot;
    public bool rotate;


    // Update is called once per frame
    void Update()
    {
        if (!rotate) { return; }
        stick.rotation = Quaternion.Lerp(stick.rotation, desiredRot, Time.deltaTime * speed);
        rotate = Quaternion.Angle(stick.rotation, desiredRot) > 0.1;

    }
}
