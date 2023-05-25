using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotateSpeed = 250;
    public float damping = 10;
    private float rotAngle;
    private Quaternion desiredRot;
    private bool rotate;
    void Start()
    {
        rotAngle = 0;
        rotate = false;
        transform.eulerAngles = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * damping);
            rotate = Quaternion.Angle(transform.rotation, desiredRot) > 0.1;
        }
    }

    //uhhh might be bugged i think lerp doesn't go in the direction i want to force it to go into
    public void Rotate(float angleToAdd)
    {
        rotAngle += angleToAdd;
        rotAngle = Mathf.Abs(rotAngle) == 360 ? 0 : rotAngle;
        desiredRot = Quaternion.Euler(0, rotAngle, 0);
        rotate = true;
    }
}
