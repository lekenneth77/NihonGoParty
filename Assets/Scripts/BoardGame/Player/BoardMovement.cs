using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMovement : MonoBehaviour
{

    private float movementSpeed;
    public bool moveFlag;
    public Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveFlag)
        {
            Move();
        }
    }

    private void Move()
    {
        if (Mathf.Abs(Vector3.Magnitude(target) - Vector3.Magnitude(this.transform.localPosition)) <= Mathf.Epsilon)
        {
            moveFlag = false;
        } else
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, target, movementSpeed * Time.deltaTime);
        }

    }

}
