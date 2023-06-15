using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//im sure there's a smarter way but for now we don't need anything that complicated right :D
public class WallCollider : MonoBehaviour
{
    public bool horizontal;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rbody = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        rbody.velocity = horizontal ? new Vector2(-rbody.velocity.x, rbody.velocity.y) : new Vector2(rbody.velocity.x, -rbody.velocity.y);
    }

    
}
