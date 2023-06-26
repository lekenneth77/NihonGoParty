using System;
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
    public event Action<Queue<GameObject>> reachTop;

    private Vector3 originalWP;
    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.KanjiFishing.AddCallbacks(this);
        hookedUp = new Queue<GameObject>();
        originalWP = fishWP.position;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.Sleep();
    }

    public void ResetAndStart() {
        fishWP.position = originalWP;
        gravity = Mathf.Abs(gravity) * -1f;
        transform.position = new Vector2(0, 3);
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, gravity);
        rb.WakeUp();
        controls.KanjiFishing.Enable();
    }

    public void HandleControls(bool enable) { 
        if (enable) {
            controls.KanjiFishing.Enable();
        } else {
            controls.KanjiFishing.Disable();

        }
    }

    public void ClearQueue() { 
        hookedUp.Clear(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.IsAwake()) { 
            rb.velocity = new Vector2(0, gravity);
            transform.rotation = Quaternion.identity;
            transform.Translate(new Vector2(moveVal * Time.deltaTime * moveSpeed, 0));
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<float>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.GetComponent<Seafloor>()) { return; }
        if (collision.gameObject.GetComponent<Seafloor>().floor) {
            gravity = -gravity;
            rb.velocity = new Vector2(0, gravity);
        } else if (!collision.gameObject.GetComponent<Seafloor>().floor) {
            rb.velocity = Vector2.zero;
            rb.Sleep();
            controls.KanjiFishing.Disable();
            reachTop?.Invoke(hookedUp);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.isTrigger = false;
        collision.gameObject.transform.position = fishWP.position;
        collision.gameObject.transform.localScale = new Vector2(60f, 60f);
        collision.gameObject.transform.SetParent(fishWP.parent);
        fishWP.position = new Vector2(fishWP.position.x, fishWP.position.y - 100f);
        collision.gameObject.GetComponent<Fish>().StopSwim();
        hookedUp.Enqueue(collision.gameObject);
    }
}
