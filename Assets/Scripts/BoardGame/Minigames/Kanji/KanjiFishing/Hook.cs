using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hook : MonoBehaviour, Controls.IKanjiFishingActions
{
    public Transform fishWP;
    public GameObject followCam;
    public float moveSpeed;
    public float moveVal;
    public float gravity;
    private Rigidbody2D rb;
    private Controls controls;
    Queue<GameObject> hookedUp;


    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.KanjiFishing.AddCallbacks(this);
        controls.KanjiFishing.Enable();

        hookedUp = new Queue<GameObject>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, gravity);
    }

    public void Setup() {
        gravity = Mathf.Abs(gravity) * -1f;
        moveSpeed = 2f;
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(0, gravity);
        transform.rotation = Quaternion.identity;
        transform.Translate(new Vector2(moveVal * Time.deltaTime * moveSpeed, 0));
    }
    public void OnMove(InputAction.CallbackContext context)
    {

        moveVal = context.ReadValue<float>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Seafloor>().floor) {
            gravity = -gravity;
            rb.velocity = new Vector2(0, gravity);
            moveSpeed = 4f;
        } else if (!collision.gameObject.GetComponent<Seafloor>().floor) {
            Debug.Log("THEY'RE DONE!");
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.isTrigger = false;
        collision.gameObject.transform.position = fishWP.position;
        collision.gameObject.transform.localScale = new Vector2(100f, 100f);
        collision.gameObject.transform.SetParent(fishWP.parent);
        fishWP.position = new Vector2(fishWP.position.x, fishWP.position.y - 150f);
        collision.gameObject.GetComponent<Fish>().StopSwim();
        hookedUp.Enqueue(collision.gameObject);
    }
}
