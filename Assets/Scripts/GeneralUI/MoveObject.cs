using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{

    public float movementSpeed = 5f;
    private bool moveFlag;
    public Vector3 target;
    public Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveFlag)
        {
            Move();
        }
    }

    public void TriggerMove()
    {
        moveFlag = true;
    }

    public void SetTargetAndMove(Vector3 tgt)
    {
        //TODO change once you have real models!
        tgt.y = tgt.y + 0.5f;
        target = tgt;
        moveFlag = true;
    }

    public void ChangeSpeed(float speedVal)
    {
        movementSpeed = speedVal;
    }

    public void GoHome()
    {
        target = originalPosition;
        moveFlag = true;
    }

    public bool GetMoveFlag()
    {
        return moveFlag;
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
