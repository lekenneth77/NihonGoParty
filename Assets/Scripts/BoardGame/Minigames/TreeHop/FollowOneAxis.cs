using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOneAxis : MonoBehaviour
{

    public Transform follow;
    public float speed = 2f;
    private bool move;
    // Update is called once per frame
    void Update()
    {
        if (!move) { return; }
        Move();
    }

    private void Move() {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, 
                new Vector3(gameObject.transform.position.x, follow.position.y + 1.5f, gameObject.transform.position.z), Time.deltaTime * speed);
        if (Mathf.Abs(gameObject.transform.position.y - (follow.position.y + 1.5f)) <= Mathf.Epsilon)
        {
            move = false;
        }
    }

    public void MoveCamera()
    {
        move = true;
    }
}
