using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HuntPlayer : MonoBehaviour, Controls.IVocabHuntActions
{
    // Start is called before the first frame update
    public GameObject exclamation;
    public StunStarSpin stars;
    public Sprite normal;
    public Sprite sad;

    public Vector2 moveVal;
    public float moveSpeed;
    private Controls controls;

    public event Action<int> interacted;
    private GameObject mostRecentInter;
    void Start()
    {
        controls = new Controls();
        controls.VocabHunt.AddCallbacks(this);
    }

    public void EnableControls() {
        if (controls != null) { 
            controls.VocabHunt.Enable();
        } else {
            controls = new Controls();
            controls.VocabHunt.AddCallbacks(this);
            controls.VocabHunt.Enable();
        }
    }

    public void DisableControls() {
        controls.VocabHunt.Disable();
    }

    public void SwapToSad() {
        GetComponent<SpriteRenderer>().sprite = sad;
    }

    public void SwapToNormal() {
        GetComponent<SpriteRenderer>().sprite = normal;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveVal * Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.identity;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.GetComponent<VocabObject>()) { return; }
        mostRecentInter = collision.gameObject;
        exclamation.SetActive(true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.GetComponent<VocabObject>()) { return; }
        exclamation.SetActive(false);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //enter the banana
        collision.isTrigger = false;
        controls.VocabHunt.Disable();
        controls.Disable();
        GetComponent<Animator>().Play("Fall");
        StartCoroutine(Bananaed(collision.gameObject));
    }

    private IEnumerator Bananaed(GameObject obj) {
        stars.StartSpin();
        yield return new WaitForSeconds(stars.activeTime);
        GetComponent<Animator>().Play("Idle");
        obj.SetActive(false);
        controls.VocabHunt.Enable();
        controls.Enable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveVal = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed || !exclamation.activeInHierarchy) { return; }
        interacted?.Invoke(mostRecentInter.GetComponent<VocabObject>().id);
    }
}
