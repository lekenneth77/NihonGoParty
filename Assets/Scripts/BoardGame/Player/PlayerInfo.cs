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

    void Start()
    {
        numCrossed = 0;
    }

}
