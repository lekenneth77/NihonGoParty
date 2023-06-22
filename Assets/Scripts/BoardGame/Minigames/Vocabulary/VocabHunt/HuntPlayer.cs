using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HuntPlayer : MonoBehaviour, Controls.IVocabHuntActions
{
    // Start is called before the first frame update
    public GameObject exclamation;
    public StunStarSpin stars;

    public Vector2 moveVal;
    public float moveSpeed;
    private Controls controls;
    void Start()
    {
        controls = new Controls();
        controls.VocabHunt.AddCallbacks(this);
        controls.VocabHunt.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveVal * Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.identity;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        exclamation.SetActive(true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        exclamation.SetActive(false);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //enter the banana
        collision.isTrigger = false;
        controls.VocabHunt.Disable();
        StartCoroutine(Bananaed(collision.gameObject));
    }

    private IEnumerator Bananaed(GameObject obj) {
        stars.StartSpin();
        yield return new WaitForSeconds(stars.activeTime);
        obj.SetActive(false);
        controls.VocabHunt.Enable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || !exclamation.activeInHierarchy) { return; }
        Debug.Log("hey!");
    }
}
