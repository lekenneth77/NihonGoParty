using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    public Transform[] waypoints;
    public GameObject playerOne;
    // Start is called before the first frame update
    void Start()
    {
        Dice.OnDiceFinish += SubscribeMovePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SubscribeMovePlayer(int roll)
    {
        StartCoroutine(MovePlayer(roll));
    }

    private IEnumerator MovePlayer(int roll)
    {
        BoardMovement moveObj = playerOne.GetComponent<BoardMovement>();
        PlayerInfo infoObj = playerOne.GetComponent<PlayerInfo>();
        int currentPosition = infoObj.currentPosition;
        for (int currentStep = 0; currentStep < roll; currentStep++)
        {
            moveObj.target = waypoints[currentPosition + currentStep].localPosition;

            moveObj.moveFlag = true;
            while (moveObj.moveFlag)
            {
                //essentially polling
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("Reached Location: " + (currentPosition + currentStep));
            yield return new WaitForSeconds(0.5f);
        }
        infoObj.currentPosition = currentPosition + roll;
    }
}
