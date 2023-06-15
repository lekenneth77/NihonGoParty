using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    void Start() {
        transform.position = Input.mousePosition;
    }

    void OnEnable() {
        transform.position = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
