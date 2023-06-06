using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public int numCrossed;
    public BoardSpace currentSpace;
    public int currentRanking;
    public int containerPosition; //used to tell which leaderboard container is this player
    public int numMinigamesWon; //maybe add a stats? is it neccessary?
    public int numHighRolls; //how many >= fours rolled
    public int numLowRolls; //how many <= threes rolled

    public Sprite sprite;
    void Start()
    {
        numCrossed = 0;
    }

}
