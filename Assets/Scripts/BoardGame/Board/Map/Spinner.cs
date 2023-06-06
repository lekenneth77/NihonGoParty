using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float currentAngle;
    public Transform pointer;
    private bool spin;
    private bool slowDown;
    private float slowDownSpeed;
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        currentAngle = 0;
        StartCoroutine("TriggerSpin");
    }

    public IEnumerator TriggerSpin()
    {
        yield return new WaitForSeconds(1f);
        spin = true;
        float normalSpinTime = Random.Range(1f, 4f);
        slowDownSpeed = Random.Range(0.96f, 0.99f);
        yield return new WaitForSeconds(normalSpinTime);
        slowDown = true;
    }




    // Update is called once per frame
    void Update()
    {
        if (spin)
        {
            if (slowDown)
            {
                speed *= 0.97f;
            }
            pointer.Rotate(0, 0, speed);
            if (speed < 0.05f) { spin = false; slowDown = false; }
        }
    }
}
