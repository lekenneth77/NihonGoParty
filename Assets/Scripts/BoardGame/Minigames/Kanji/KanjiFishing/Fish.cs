using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fish : MonoBehaviour
{
    public string letter;
    public float swimSpeed;
    private Rigidbody2D rb;
    bool left;
    // Start is called before the first frame update
    void Start()
    {
        letter = transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        rb = GetComponent<Rigidbody2D>();
        left = Random.Range(0, 2) == 1 ? true : false;
        transform.position = new Vector2(Random.Range(-5.5f, 5.5f), transform.position.y);
        rb.velocity = left ? new Vector2(swimSpeed * -1, 0) : new Vector2(swimSpeed, 0);
        ChangeRotation();
    }

    public void StopSwim() {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
    }

    //might need to keep constant velocity
    public void RotateFish() {
        rb.velocity = -rb.velocity;
        left = !left;
        ChangeRotation();
    }   

    private void ChangeRotation() { 
        if (!left) {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 270f);
        } else {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 180f, 270f);
        }
    }


}
