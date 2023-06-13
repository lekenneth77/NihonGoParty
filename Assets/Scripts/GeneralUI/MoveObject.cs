using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{

    public float movementSpeed = 5f;
    private bool moveFlag;
    public Vector3 target;
    public Vector3 originalPosition;

    //all for rotates before moving, please comment these the list keeps growing
    public bool rotateToo;
    private bool rotateFirst;
    private Quaternion currentRotation;
    private Quaternion tgtRotation;
    private float rotationSpeed = 6f;
    private float timeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (!moveFlag) {
            if (gameObject.GetComponent<Animator>()) {
                gameObject.GetComponent<Animator>().SetFloat("IdleToWalk", 0f, 0.05f, Time.deltaTime);
            }
            return; 
        }

        if (rotateFirst) {
            InitialRotate();
        }  else {
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
        if (rotateToo)
        {
            timeCount = 0;
            currentRotation = transform.rotation;
            tgtRotation = Quaternion.LookRotation((target - transform.localPosition).normalized);
            rotateFirst = true;
        } 
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
    public void RotateToIdentity() {
        timeCount = 0;
        currentRotation = transform.rotation;
        tgtRotation = Quaternion.identity;
        rotateFirst = true;
        moveFlag = true;
    }

    private void InitialRotate() {
        if (timeCount > (1f / rotationSpeed))
        {
            rotateFirst = false;
            return;
        }

        transform.rotation = Quaternion.Lerp(currentRotation, tgtRotation, timeCount * rotationSpeed);
        timeCount += Time.deltaTime;
    }

    private void Move()
    {
        if (Mathf.Abs(Vector3.Magnitude(target) - Vector3.Magnitude(transform.localPosition)) <= Mathf.Epsilon)
        {
            moveFlag = false;
        } else
        {
            if (gameObject.GetComponent<Animator>()) { 
                gameObject.GetComponent<Animator>()?.SetFloat("IdleToWalk", 1f, 0.05f, Time.deltaTime);
            }
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, movementSpeed * Time.deltaTime);
        }

    }

}
