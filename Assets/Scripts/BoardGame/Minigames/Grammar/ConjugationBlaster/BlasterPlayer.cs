using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class BlasterPlayer : Minigame, Controls.IConjBlasterActions
{
    public float moveSpeed;
    public Blaster blaster;
    public float cooldownTime;
    public GameObject blasterEnemy;
    public TextMeshProUGUI currForm;
    public Transform boomFolder;
    public GameObject boomImg;
    public GameObject[] hearts;
    public int ouchies;
    public GameObject success;
    public GameObject failure;
    public bool loss;

    private string[] forms = new string[] { "A", "B", "C" };
    private int formIndex;
    private float moveVal;
    private bool cooldown;
    private Controls controls;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        formIndex = 0;
        blasterEnemy.GetComponent<BlasterEnemy>().StopEnemy();
        Bullet.boom += HandleBoom;
        KillEnemy.enemyEnter += LoseLife;
        controls = new Controls();
        controls.ConjBlaster.AddCallbacks(this);
        controls.ConjBlaster.Enable();
        StartCoroutine("Round");
    }

    private IEnumerator Round() {

        for (int i = 0; i < Random.Range(30, 45); i++) {
            int whichType = Random.Range(0, 3);
            blasterEnemy.GetComponent<BlasterEnemy>().Spawn(whichType, forms[whichType]);
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        }
        yield return new WaitForSeconds(5f);
        if (loss) { yield break; }
        StartCoroutine("Success");
    }

    public void HandleBoom(Vector3 position) {
        StartCoroutine(BoomBoom(position));
    }

    private IEnumerator BoomBoom(Vector3 position) {
        GameObject boom = Instantiate(boomImg, position, Quaternion.identity, boomFolder);
        yield return new WaitForSeconds(1f);
        Destroy(boom);
    }

    public void LoseLife() {
        ouchies++;
        if (ouchies >= hearts.Length) {
            //endgame 
            if (!loss) { 
                loss = true;
                hearts[hearts.Length - ouchies].SetActive(false);
                StopCoroutine("Round");
                controls.ConjBlaster.Disable();
                StartCoroutine("Failure");
            }

        } else {
            hearts[hearts.Length - ouchies].SetActive(false);
        }
        
    }
    private IEnumerator Success() {
        success.SetActive(true);
        yield return new WaitForSeconds(5f);
        int result;
        if (ouchies < 2) {
            result = 2;
        } else if (ouchies < 4) {
            result = 1;
        } else {
            result = 0;
        }
        EndGame(result);
    }

    private IEnumerator Failure() {
        failure.SetActive(true);
        yield return new WaitForSeconds(5f);
        EndGame(-1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        transform.Translate(new Vector2(0, moveVal * Time.deltaTime * moveSpeed));
    }

    private IEnumerator HandleCooldown() {
        yield return new WaitForSeconds(cooldownTime);
        cooldown = false;
    }


    public void OnBlast(InputAction.CallbackContext context) {
        if (!context.performed || cooldown) { return; }
        cooldown = true;
        blaster.Blast(formIndex);
        StartCoroutine("HandleCooldown");
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveVal = context.ReadValue<float>();
    }

    public void OnSwap(InputAction.CallbackContext context) {
        if (!context.performed) { return; }
        formIndex = formIndex >= 2 ? formIndex = 0 : formIndex + 1;
        currForm.text = forms[formIndex];
        blaster.Swap();
    }
}
