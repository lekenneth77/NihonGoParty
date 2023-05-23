using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public Transform wp;
    public float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, wp.localPosition, moveSpeed * Time.deltaTime);
    }
}
