using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanjiDnDCamera : MonoBehaviour
{
    public float speed;
    public event Action FinishMove;
    private bool rotate;
    private Quaternion tgt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (rotate) {
            transform.rotation = Quaternion.Lerp(transform.rotation, tgt, Time.deltaTime * speed);
            rotate = Quaternion.Angle(transform.rotation, tgt) > 0.1;
            if (!rotate) {
                FinishMove?.Invoke();
            }
        }
    }

    public void RotateUp() {
        tgt = Quaternion.Euler(0, 0, 0);
        rotate = true;
    }

    public void RotateDown() {
        tgt = Quaternion.Euler(90f, 0, 0);
        rotate = true;
    }
}
