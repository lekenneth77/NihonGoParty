using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//huh, what do you mean followoneaxis exists? what do you mean having these two SEPERATE scripts is redundant?
public class FollowTwoAxis : MonoBehaviour
{
    public Transform follow;
    //this function is actually stupid i don't need this im so tired please help

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(follow.position.x, transform.position.y, follow.position.z - 3f);
    }


}
