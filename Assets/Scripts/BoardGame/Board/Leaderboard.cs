using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] leaderboard;
    private int numPlayers;
    public BoardController boardController; //ONLY USED TO GET DEBUG, kind of jank
    private Sprite[] rankingSprites;


    void Start()
    {
        //TODO change the sprites to the real rank sprites!
        rankingSprites = Resources.LoadAll<Sprite>("DiceSides/");
    }

    public void SetNumPlayers(int num)
    {
        numPlayers = num;
    }

    public void SetVisibility(bool val)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            leaderboard[i].SetActive(val);
        }
    }
    
    public void UpdateBoard(GameObject[] players)
    {
        GameObject[] copy = new GameObject[players.Length];
        players.CopyTo(copy, 0);
        Array.Sort(copy, new Comparator());
        SetRankings(copy);
        //update rankings lmao this is kind of jank
        for (int i = 0; i < players.Length; i++)
        {
            PlayerInfo thisInfo = players[i].GetComponent<PlayerInfo>();
            for (int j = 0; j < copy.Length; j++)
            {
                PlayerInfo thatInfo = copy[i].GetComponent<PlayerInfo>();
                if (thisInfo.containerPosition == thatInfo.containerPosition)
                {
                    thisInfo.currentRanking = thatInfo.currentRanking;
                    break;
                }
            }
        }
    }

    public void SetRankings(GameObject[] players)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerInfo info = players[i].GetComponent<PlayerInfo>();
            info.currentRanking = i + 1;
            if (!boardController.debug) //taking this out causes errors so idk
            {
                leaderboard[info.containerPosition].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = rankingSprites[i];
            }
        }
    }

    public class Comparator : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            return y.GetComponent<PlayerInfo>().currentPosition - x.GetComponent<PlayerInfo>().currentPosition;
        }
    }
}


