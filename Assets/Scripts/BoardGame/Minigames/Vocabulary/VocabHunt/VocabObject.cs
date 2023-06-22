using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocabObject : MonoBehaviour
{
    public int id;
    Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
    }

    void Update() {
        transform.position = originalPos;
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //destroy the banana
        collision.isTrigger = false;
        Destroy(collision.gameObject);
    }

}
