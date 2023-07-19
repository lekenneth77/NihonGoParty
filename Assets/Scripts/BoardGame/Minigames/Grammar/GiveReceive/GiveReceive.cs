using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiveReceive : Minigame
{
    public GameObject cmdList;
    public TextMeshProUGUI midTextBox;
    public int maxRound = 5;
    public int maxHP = 3;
    public RPGPlayerPhase playerAttk;
    public GameObject enemyAttk;
    private int playerHits; //terrible naming, this is how many times the player GOT hit
    private int enemyHits;
    private bool leftDodge;
    private int turn;
    private Color[] hpColors = new Color[] { Color.green, new Color(238f / 255f, 141f / 255f, 0f) ,Color.red, Color.white};
    public Image enemyPortrait;
    public Image enemyHP;
    public Image playerHP;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        playerAttk.phaseComplete += AfterPunch;
    }

    public void HandlePunch() {
        StartCoroutine("SetupPunch");
    }

    private IEnumerator SetupPunch() {

        string dialogue = "Order your punches correctly to deal damage!";
        cmdList.SetActive(false);
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(true);
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            } 
            i++;
        }
        yield return new WaitForSeconds(2f);
        midTextBox.text = "";
        //read the text file setup the punchies
        playerAttk.leftCircle.giver = true;
        playerAttk.arrow.SetActive(false);
        playerAttk.gameObject.SetActive(true);
    }

    public void AfterPunch(bool correct) { 
        if (correct) {
            StartCoroutine("InflictDamage");
        } else {
            StartCoroutine("NoDamage");
        }
    }

    private IEnumerator InflictDamage() {
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(true);
        string dialogue = "Your punches lands successfully!";
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            } 
            i++;
        }
        yield return new WaitForSeconds(1.5f);
        midTextBox.text = "";
        dialogue = "The enemy receives damage!";
        i = 0;
        while (i < dialogue.Length)
        {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ')
            {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        enemyHits++;
        for (float j = 1f - (.34f * (enemyHits - 1)); j >= 1f - (.34f * enemyHits); j -= 0.01f) {
            enemyHP.fillAmount = j;
            yield return new WaitForSeconds(0.01f);
        }
        enemyHP.color = hpColors[enemyHits];
        
        yield return new WaitForSeconds(1.5f);
        playerAttk.gameObject.SetActive(false);
        if (enemyHits == maxHP) {
            //Battle is won!
            StartCoroutine("Victory");
        } else { 
            midTextBox.text = "";
            midTextBox.transform.parent.parent.gameObject.SetActive(false);
            StartCoroutine("EnemyPhase");
        }

    }

    private IEnumerator NoDamage() {
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(true);
        string dialogue = "Your punches miss!";
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            } 
            i++;
        }
        yield return new WaitForSeconds(1f);
        playerAttk.gameObject.SetActive(false);
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(false);
        StartCoroutine("EnemyPhase");

    }

    private IEnumerator EnemyPhase() {
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(true);
        string dialogue = "The enemy is charging their attack!";
        int i = 0;
        while (i < dialogue.Length)
        {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ')
            {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(1f);
        midTextBox.text = "";
        dialogue = "Choose the correct term to dodge their attack!";
        i = 0;
        while (i < dialogue.Length)
        {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ')
            {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(1f);
        //set up the midtext box
        leftDodge = true;
        midTextBox.text = "";
        enemyAttk.SetActive(true);
    }

    public void EnemyPhaseClick(bool left) {
        enemyAttk.SetActive(false);
        if (leftDodge && left) {
            //correct left dodge
            StartCoroutine("SuccessfulDodge");
        } else if (!leftDodge && !left) { 
            //correct right dodge
            StartCoroutine("SuccessfulDodge");
        } else {
            StartCoroutine("FailedDodge");
        }
    }

    private IEnumerator SuccessfulDodge() {
        string dialogue = "You successfully dodged the attack!";
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(1f);
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(false);
        turn++;
        if (turn == maxRound) {
            StartCoroutine("Fedup");
        } else { 
            cmdList.SetActive(true);
        }
    }

    private IEnumerator FailedDodge() {
        string dialogue = "You failed to dodge the attack!";
        midTextBox.text = "";
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(1f);
        midTextBox.text = "";

        dialogue = "You receive damage!";
        i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        playerHits++;
        for (float j = 1f - (.34f * (playerHits - 1)); j >= 1f - (.34f * playerHits); j -= 0.01f) {
            playerHP.fillAmount = j;
            yield return new WaitForSeconds(0.01f);
        }
        playerHP.color = hpColors[playerHits];
        yield return new WaitForSeconds(1f);
        if (playerHits == maxHP) {
            //Death!
            StartCoroutine("Death");
        } else { 
            midTextBox.text = "";
            midTextBox.transform.parent.parent.gameObject.SetActive(false);
            turn++;
            if (turn == maxRound) {
                StartCoroutine("Fedup");
            } else { 
                cmdList.SetActive(true);
            }
        }
    }

    private IEnumerator Victory() {
        string dialogue = "You gave that enemy a beatdown!";
        midTextBox.text = "";
        int i = 0;
        while (i < dialogue.Length)
        {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ')
            {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }

        for (int j = 100; j >= 0; j--) {
            Color temp = enemyPortrait.color;
            temp.a -= 0.01f;
            enemyPortrait.color = temp;
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.5f);
        dialogue = "You gained 99 experience!";
        midTextBox.text = "";
        i = 0;
        while (i < dialogue.Length)
        {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ')
            {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(5f);
        EndGame(enemyHits - 1); //2 lol
    }

    private IEnumerator Death() {
        string dialogue = "You have received death!";
        midTextBox.text = "";
        int i = 0;
        while (i < dialogue.Length)
        {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ')
            {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(5f);
        EndGame(enemyHits - 1);
    }

    private IEnumerator Fedup() {
        string dialogue = "The enemy is tired of these shenanigans!";
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(true);
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(1f);
        dialogue = "They cast Zetaflare!";
        midTextBox.text = "";
        i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ')
            {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(1f);
        for (float j = 1f - (.34f * playerHits); j >= 0; j-= 0.01f) {
            playerHP.fillAmount = j;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1f);
        dialogue = "You lose 50G and your life!";
        midTextBox.text = "";
        i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(5f);
        EndGame(enemyHits - 1);
    }

    public void HandleItem() {
        string dialogue = "You don't have any items!";
        StartCoroutine(TrollCmd(dialogue));
    }
     public void HandleRunAway() {
        string dialogue = "You're unable to run away from this fight!";
        StartCoroutine(TrollCmd(dialogue));
    }

    private IEnumerator TrollCmd(string dialogue) {
        cmdList.SetActive(false);
        midTextBox.text = "";
        midTextBox.transform.parent.parent.gameObject.SetActive(true);
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.075f);
            } 
            i++;
        }
        yield return new WaitForSeconds(3f);
        midTextBox.transform.parent.parent.gameObject.SetActive(false);
        midTextBox.text = "";
        cmdList.SetActive(true);
    }


}
