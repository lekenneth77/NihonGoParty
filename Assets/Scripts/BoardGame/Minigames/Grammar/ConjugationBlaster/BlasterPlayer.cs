using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class BlasterPlayer : Minigame, Controls.IConjBlasterActions
{
    public TextAsset[] texts;
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

    public List<string>[] words = new List<string>[3];
    private string[] forms = new string[3];
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
        ChooseTexts();
        currForm.text = forms[formIndex];
        StartCoroutine("Round");
    }

    private void OnDestroy()
    {
        Bullet.boom -= HandleBoom;
        KillEnemy.enemyEnter -= LoseLife;
        controls.Dispose();
    }

    private void ChooseTexts() {
        HashSet<int> chosen = new HashSet<int>();
        for (int i = 0; i < 3; i++) {
            int index = Random.Range(0, texts.Length);
            while (!chosen.Add(index)) {
                index = Random.Range(0, texts.Length);
            }

            string[] textfile = texts[index].text.Split("\n"[0]);
            forms[i] = textfile[0];
            words[i] = new List<string>();
            for (int j = 1; j < textfile.Length; j++) {
                words[i].Add(textfile[j]);
            }

        }
    }

    private IEnumerator Round() {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < Random.Range(12, 20); i++) {
            int whichType = Random.Range(0, 3);
            List<string> curList = words[whichType];
            string word = curList[Random.Range(0, curList.Count)];

            blasterEnemy.GetComponent<BlasterEnemy>().Spawn(whichType, word);
            yield return new WaitForSeconds(Random.Range(2f, 3.5f));
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
