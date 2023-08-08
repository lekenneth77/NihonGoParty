using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOneAxis : MonoBehaviour
{

    public Transform follow;
    public Vector3 target;
    public bool followFollow; //i hate myself
    public float speed = 2f;
    public bool move;
    // Update is called once per frame
    void Update()
    {
        if (!move) { return; }
        Move();
    }

    private void Move() {
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
        if (Mathf.Abs(gameObject.transform.position.y - target.y) <= 0.01f) {
            move = false;
        }
    }

    public void MoveCamera()
    {
        move = true;
    }
}
