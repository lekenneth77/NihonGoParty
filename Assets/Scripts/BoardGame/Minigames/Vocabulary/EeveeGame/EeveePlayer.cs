using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EeveePlayer : MonoBehaviour
{
    public Transform myWP;
    private MoveObject moveObj;
    private JumpObject jumpObj;
    public StunStarSpin stars;
    public bool wentOnce;
    public static bool wrong;
    private Vector3 ouchRotation;
    public bool rotate;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = myWP.position;
        stars = transform.GetChild(1).GetComponent<StunStarSpin>();
        moveObj = GetComponent<MoveObject>();
        jumpObj = GetComponent<JumpObject>();
    }
  

    public void GoToCenter() {
        moveObj.SetTargetAndMove(Vector3.zero);
    }

    private void OnTriggerEnter(Collider other) {
        
        bool tempWrong = wrong;
        if (!wrong) { wrong = true; }
        moveObj.StopMove();
        transform.GetChild(0).GetComponent<Animator>().Play("walk");
        if (tempWrong) {
            ouchRotation = new Vector3(Random.Range(1f, 3f), Random.Range(1f, 3f), Random.Range(1f, 3f));
            Vector3 dir = transform.position.normalized;
            Vector3 tgt = transform.position + dir * 20f;
            jumpObj.SetupAndJump(tgt, 2f, 5f);
            stars.gameObject.SetActive(true);
            rotate = true;
        } 
    }
    private void Update()
    {
        if (!rotate) { return; }
        transform.Rotate(ouchRotation);
    }


}
