using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApplePlayer : MonoBehaviour, Controls.IAppleDropActions
{
    public GameObject model;
    public AppleLever lever;
    public float moveSpeed;
    public bool alreadySlowed;
    public Controls controls;
    private float moveVal;
    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.AppleDrop.AddCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(moveVal * Time.deltaTime * moveSpeed, 0));
        if (transform.position.x > 11f) {
            transform.position = new Vector3(11f, transform.position.y, transform.position.z);
        } else if (transform.position.x < -11f) {
            transform.position = new Vector3(-11f, transform.position.y, transform.position.z);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<float>() * -1f;
        if (!context.performed || moveVal == 0) { return; }
        if (moveVal > 0) {
            lever.desiredRot = Quaternion.Euler(0, 0, 45f);
        } else {
            lever.desiredRot = Quaternion.Euler(0, 0, -45f);
        }
        lever.rotate = true;
    }

    public IEnumerator SlowDown() {
        alreadySlowed = true;
        moveSpeed -= 8f;
        model.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        model.transform.GetChild(0).gameObject.SetActive(false);
        moveSpeed += 8f;
        alreadySlowed = false;
    }
}
