using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiveReceive : Minigame
{
    //problems
    public TextAsset playerTxt;
    public TextAsset inGroupTxt;
    public TextAsset outGroupTxt;
    public TextAsset highOutGroupTxt;
    private string[] playerProblems;
    private HashSet<int> chosenPlayerProbs;
    private string[] ingroup;
    private string[] outgroup;
    private string[] highgroup;
    private string[][] allgroups = new string[3][];

    public GameObject cmdList;
    public TextMeshProUGUI midTextBox;
    public RPGPlayerPhase playerAttk;
    public int maxRound = 5;
    public int maxHP = 3;
    public GameObject enemyAttk;
    public TextMeshProUGUI leftEnemyButton;
    public TextMeshProUGUI rightEnemyButton;
    public TextMeshProUGUI leftEnemyProb;
    public TextMeshProUGUI rightEnemyProb;


    private int playerHits; //terrible naming, this is how many times the player GOT hit
    private int enemyHits;
    private bool leftDodge;
    private int turn;
    private bool leftHit;
    private Color[] hpColors = new Color[] { Color.green, new Color(238f / 255f, 141f / 255f, 0f) ,Color.red, Color.white};

    //player
    public GameObject player;
    public Image playerHP;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI cmdPlayerName;
    private Sprite[] characterPortraits;
    private string[] charNames = new string[] { "カピバラ", "チェフ", "ベボ", "サラリマン", "ケンド" };

    //enemy
    public GameObject enemy;
    private Image enemyHP;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        chosenPlayerProbs = new HashSet<int>();
        playerProblems = playerTxt.text.Split("\n"[0]);
        characterPortraits = Resources.LoadAll<Sprite>("Images/CharacterPortraits/");


        if (BoardController.players != null) {
            int charIndex = BoardController.currentPlayer.GetComponent<PlayerInfo>().characterIndex;
            player.transform.GetChild(0).GetComponent<Image>().sprite = characterPortraits[charIndex];
            playerName.text = charNames[charIndex];
            cmdPlayerName.text = charNames[charIndex];
        }


        ingroup = inGroupTxt.text.Split("\n"[0]);
        outgroup = outGroupTxt.text.Split("\n"[0]);
        highgroup = highOutGroupTxt.text.Split("\n"[0]);
        allgroups[0] = ingroup;
        allgroups[1] = outgroup;
        allgroups[2] = highgroup;

        enemyHP = enemy.transform.GetChild(0).GetComponent<Image>();

        playerAttk.phaseComplete += AfterPunch;
    }

    public void HandlePunch() {
        StartCoroutine("SetupPunch");
    }

    private IEnumerator SetupPunch() {

        string dialogue = "Order your punches correctly to attack!";
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
        int random = Random.Range(0, playerProblems.Length);
        while (!chosenPlayerProbs.Add(random)) {
            random = Random.Range(0, playerProblems.Length);
        }
        string[] currentProb = playerProblems[random].Split("_"[0]);
        midTextBox.text = currentProb[0];
        random = Random.Range(0, 2);
        leftHit = random == 0;
        playerAttk.leftCircle.giver = random == 0;
        playerAttk.rightCircle.giver = random == 1;
        if (random == 0) {
            playerAttk.leftCircle.ChangeText(currentProb[1]);
            playerAttk.rightCircle.ChangeText(currentProb[2]);
        } else {
            playerAttk.rightCircle.ChangeText(currentProb[1]);
            playerAttk.leftCircle.ChangeText(currentProb[2]);
        }

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
        
        string dialogue = "Your punches land successfully!";
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            } 
            i++;
        }
        yield return new WaitForSeconds(0.5f);
        playerAttk.arrow.SetActive(false);
        playerAttk.gameObject.SetActive(false);
        string whichWay = "enemy_" + (leftHit ? "left" : "right");
        enemy.GetComponent<Animator>().Play(whichWay);
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
        enemy.GetComponent<Animator>().Play("idle");
        enemyHits++;
        for (float j = 1f - (.5f * (enemyHits - 1)); j >= 1f - (.5f * enemyHits); j -= 0.01f) {
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
        string dialogue = "The enemy is charging up!";
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
        dialogue = "Choose the correct term to evade!";
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
        /*
         * ageru vs kureru has to pull from one in group one out group for ageru, vice versa for kureru
         * morau vs itadaku??? check that out
         */
        midTextBox.text = "";
        leftEnemyProb.text = "";
        rightEnemyProb.text = "";
        //ageru vs kureru
        string inner = ingroup[Random.Range(0, ingroup.Length)];
        int random = Random.Range(0, 2);
        string outer = random == 0 ? outgroup[Random.Range(0, outgroup.Length)] : highgroup[Random.Range(0, highgroup.Length)];
        string ageru = random == 0 ? "あげる" : "さしあげる";
        string kureru = random == 0 ? "くれる" : "くださる";
        random = Random.Range(0, 2);
        if (random == 0) {
            //ageru is correct
            random = Random.Range(0, 2); //choose left or right
            leftEnemyButton.text = random == 0 ? ageru : kureru;
            rightEnemyButton.text = random == 0 ? kureru : ageru;
            leftDodge = random == 0;
            leftEnemyProb.text = inner;
            rightEnemyProb.text = outer;
        } else {
            //kureru is correct
            random = Random.Range(0, 2); //choose left or right
            leftEnemyButton.text = random == 0 ? kureru : ageru;
            rightEnemyButton.text = random == 0 ? ageru : kureru;
            leftDodge = random == 0;
            leftEnemyProb.text = outer;
            rightEnemyProb.text = inner;
        }
        /*
        else {
            //im just going to do morau vs itadaku, im getting way too burnt out
            random = Random.Range(0, 2);
            if (random == 0) {
                //morau
                random = Random.Range(0, allgroups.Length - 1);
                string[] group = allgroups[random];
                leftEnemyProb.text = group[Random.Range(0, group.Length)];
                random = Random.Range(0, allgroups.Length);
                group = allgroups[random];
                rightEnemyProb.text = group[Random.Range(0, group.Length)];

                random = Random.Range(0, 2); //choose left or right
                leftEnemyButton.text = random == 0 ? "もらう" : "いただく";
                rightEnemyButton.text = random == 0 ? "いただく" : "もらう";
                leftDodge = random == 0;

            } else {
                //itadaku
                leftEnemyProb.text = highgroup[Random.Range(0, highgroup.Length)];
                random = Random.Range(0, allgroups.Length - 1); //...no high group?
                string[] group = allgroups[random];
                rightEnemyProb.text = group[Random.Range(0, group.Length)];

                random = Random.Range(0, 2); //choose left or right
                leftEnemyButton.text = random == 0 ? "いただく" : "もらう";
                rightEnemyButton.text = random == 0 ? "もらう" : "いただく";
                leftDodge = random == 0;
            }
        }
        */
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
        string dialogue = "You successfully dodged!";
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
        string dialogue = "You failed to dodge!";
        midTextBox.text = "";
        int i = 0;
        while (i < dialogue.Length) {
            midTextBox.text += dialogue[i];
            if (dialogue[i] != ' ') {
                yield return new WaitForSeconds(0.05f);
            }
            i++;
        }
        yield return new WaitForSeconds(0.5f);
        string whichWay = "player_" + (leftDodge ? "right" : "left");
        player.GetComponent<Animator>().Play(whichWay);

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
        player.GetComponent<Animator>().Play("idle");

        playerHits++;
        for (float j = 1f - (.5f * (playerHits - 1)); j >= 1f - (.5f * playerHits); j -= 0.01f) {
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
        enemy.GetComponent<Animator>().Play("enemy_die");
        yield return new WaitForSeconds(1.2f);
        dialogue = "Congratulations! You gained 99 experience!";
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
        string dialogue = "You have received death and give them 50G!";
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
        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Animator>().Play("player_die");
        yield return new WaitForSeconds(12f);
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
