using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class Cave : MonoBehaviour
{

    public MoveObject player;
    public GameObject followCam;
    public Transform caveFolder;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI topText;
    private bool leftCorrect;

    public GameObject frontDarkness;
    public Transform[] enterRight;
    public Transform[] enterLeft;
    public Transform[] outRight;
    public Transform[] outLeft;
    public GameObject defCave;
    public GameObject defFinish;
    public static int caveLimit;

    private List<Transform> movement;

    public static string[] questions;
    public static HashSet<int> chosenQ;
    public event Action<Cave, bool> endMove;
    public event Action reachedFinish;

    // Start is called before the first frame update
    void Start()
    {
        movement = new List<Transform>();
    }

    public void GoLeft(int points) {
        if ((points + 1) == caveLimit && leftCorrect) {
            TunnelRunner.StopThatTimer();
            SpawnFinish(true);
        } else { 
            SpawnCave(leftCorrect, true);
        }
    }

    public void GoRight(int points) {
        if ((points + 1) == caveLimit && !leftCorrect) {
            TunnelRunner.StopThatTimer();
            SpawnFinish(false);
        } else {
            SpawnCave(!leftCorrect, false);
        }
    }

    public void GetWords() {
        int random = UnityEngine.Random.Range(0, questions.Length);
        while (!chosenQ.Add(random)) {
            random = UnityEngine.Random.Range(0, questions.Length);
        }
        string[] split = questions[random].Split("="[0]);
        bool leftCorr = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
        random = UnityEngine.Random.Range(0, 2); //0 means left is trans right is intrans

        if (leftCorr) { 
            if (random == 0) {
                ChangeText(split[0], split[1], "Transitive", true);
            } else {
                ChangeText(split[1], split[0], "Intransitive", true);
            }
        } else {
            if (random == 0) {
                ChangeText(split[1], split[0], "Transitive", false);
            } else {
                ChangeText(split[0], split[1], "Intransitive", false);
            }
        }
    }

   

    public void SpawnCave(bool correct, bool wentLeft) {
       
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 21.1f);
        GameObject newCave = Instantiate(defCave, newPos, Quaternion.identity, caveFolder);
        if (correct) {
            newCave.GetComponent<Cave>().GetWords();
        } else {
            //if they chose the wrong path, make them do the cave again!
            newCave.GetComponent<Cave>().ChangeText(leftText.text, rightText.text, topText.text, leftCorrect);
        }
        frontDarkness.SetActive(false);
        movement.Clear();//just to make sure
        if (wentLeft) { 
            foreach(Transform t in outLeft) {
                movement.Add(t);
            }
            foreach(Transform t in newCave.GetComponent<Cave>().enterLeft) {
                movement.Add(t);
            }
        } else {
            foreach (Transform t in outRight)
            {
                movement.Add(t);
            }
            foreach (Transform t in newCave.GetComponent<Cave>().enterRight)
            {
                movement.Add(t);
            }
        }
        StartCoroutine(Move(newCave, correct, false));
    }

    public void SpawnFinish(bool wentLeft)
    {
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 21.1f);
        GameObject newCave = Instantiate(defFinish, newPos, Quaternion.identity, caveFolder);
        movement.Clear();//just to make sure
        if (wentLeft)
        {
            foreach (Transform t in outLeft)
            {
                movement.Add(t);
            }
            foreach (Transform t in newCave.GetComponent<Cave>().enterLeft)
            {
                movement.Add(t);
            }
        }
        else
        {
            foreach (Transform t in outRight)
            {
                movement.Add(t);
            }
            foreach (Transform t in newCave.GetComponent<Cave>().enterRight)
            {
                movement.Add(t);
            }
        }
        StartCoroutine(Move(gameObject, true, true));
    }

    private IEnumerator Move(GameObject newCave, bool correct, bool finish) {
        while (movement.Count > 0) {
            if (finish && movement.Count == 1) {
                followCam.GetComponent<FollowTwoAxis>().enabled = false;
            }
            Transform current = movement[0];
            movement.RemoveAt(0);
            player.SetTargetAndMove(current.position);
            while (player.GetMoveFlag())
            {
                //essentially polling
                yield return new WaitForSeconds(0.01f);
            }
        }
        if (finish) {
            reachedFinish?.Invoke();
        } else { 
            endMove?.Invoke(newCave.GetComponent<Cave>(), correct);
        }
    }

    public void ChangeText(string left, string right, string top, bool leftCorr) {
        leftText.text = left;
        rightText.text = right;
        topText.text = top;
        leftCorrect = leftCorr;
    }
}
