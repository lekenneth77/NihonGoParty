using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action enemyEnter;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BlasterEnemy>()) {
            enemyEnter?.Invoke();
            Destroy(collision.gameObject);
        }
    }
}
