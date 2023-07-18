using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    // Start is called before the first frame update
    public string text;
    private float fallSpeed = 0;
    void Start()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public IEnumerator WaitThenFall(float delay, float speed) {
        yield return new WaitForSeconds(delay);
        fallSpeed = speed * -1;
    }

    // Update is called once per frame
    void Update() {
        GetComponent<Rigidbody>().velocity = new Vector3(0, fallSpeed, 0f);
    }
}
