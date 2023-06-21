using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cave : MonoBehaviour
{

    public MoveObject player;
    public Transform caveFolder;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI topText;
    private bool leftCorrect;

    public Transform[] enterRight;
    public Transform[] enterLeft;
    public Transform[] outRight;
    public Transform[] outLeft;
    public GameObject defCave;
    public int caveLimit;

    private List<Transform> movement;

    public event Action<Cave, bool> endMove;

    // Start is called before the first frame update
    void Start()
    {
        leftCorrect = true;
        movement = new List<Transform>();
    }

    public void GoLeft() {
        Debug.Log("Left!");
        SpawnCave(leftCorrect, true);
    }

    public void GoRight() {
        Debug.Log("Right!");
        SpawnCave(!leftCorrect, false);
    }

    public void GetWords() {


        //ChangeText();
    }

    public void SpawnCave(bool correct, bool wentLeft) {
       
        Vector3 newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 21.1f);
        GameObject newCave = Instantiate(defCave, newPos, Quaternion.identity, caveFolder);
        if (correct) {
            newCave.GetComponent<Cave>().GetWords();
            //FOR NOW, this is just for testing...eventually remove this
            newCave.GetComponent<Cave>().ChangeText(leftText.text, rightText.text, topText.text, leftCorrect);
        } else {
            //if they chose the wrong path, make them do the cave again!
            newCave.GetComponent<Cave>().ChangeText(leftText.text, rightText.text, topText.text, leftCorrect);
        }
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
        StartCoroutine(Move(newCave, correct));
    }

    private IEnumerator Move(GameObject newCave, bool correct) {
        Debug.Log("Started Moving!");
        while (movement.Count > 0) {
            Transform current = movement[0];
            movement.RemoveAt(0);
            player.SetTargetAndMove(current.position);
            while (player.GetMoveFlag())
            {
                //essentially polling
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Reached one spot!");
        }
        endMove?.Invoke(newCave.GetComponent<Cave>(), correct);
    }

    public void ChangeText(string left, string right, string top, bool leftCorr) {
        leftText.text = left;
        rightText.text = right;
        topText.text = top;
        leftCorrect = leftCorr;
    }
}
