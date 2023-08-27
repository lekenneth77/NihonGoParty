using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    public float moveSpeed;
    public float rotateSpeed;
    public int bulletType;
    public bool rotate;
    public static event Action<Vector3> boom;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(moveSpeed, 0, 0);
        rotate = true;
    }

    public void StopBullet() {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rotate = false;
    }

    private void Update()
    {
        if (!rotate) { return; }
        transform.Rotate(0, 0, rotateSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<BlasterEnemy>() && collision.GetComponent<BlasterEnemy>().enemyType == bulletType) {
            boom?.Invoke(collision.transform.position);
            Destroy(collision.gameObject);
        } 
        Destroy(gameObject);
    }

}
