using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInfo : MonoBehaviour
{

    private Queue<GameObject> playersOnMe;
    private Vector3[] extraSpaces; //spaces used for when multiple players are on the same space

    //honestly a lot of unncessary variables will probably populate this but yeah
    public bool amCrossroad;
    public SpaceInfo nextWP; //shitty naming but whatever it's up to the editor which is which just make sure it lines up
    public SpaceInfo alternateWP; //only for crossroads
    public SpaceInfo prevWP;
    public string gameType;
    public bool finishLine;

    // Start is called before the first frame update
    void Start()
    {
        playersOnMe = new Queue<GameObject>();
        extraSpaces = new Vector3[3];
        for (int x = -1; x <= 1; x++)
        {
            extraSpaces[x + 1] = new Vector3(this.transform.position.x - x, this.transform.position.y, this.transform.position.z - 1);
        }
    }

    public void AddPlayer(GameObject player)
    {
        playersOnMe.Enqueue(player);
    }

    //please explain this in the future because THIS is pretty fight like a tiger walk in the park
    public void AdjustPlayers()
    {
        //move previous players
        int index = 0;
        foreach (GameObject p in playersOnMe)
        {
            p.GetComponent<BoardMovement>().SetTargetAndMove(extraSpaces[index]);
            index++;
        }
    }

    public void ResetPlayers(bool transport)
    {
        int index = 0;
        foreach (GameObject p in playersOnMe)
        {
            Vector3 pos = index == 0 ? this.transform.position : extraSpaces[index - 1];
            if (transport)
            {
                //TODO remove once you have actual models!
                pos.y += 0.5f;
                p.transform.position = pos;
            } else
            {
                p.GetComponent<BoardMovement>().SetTargetAndMove(pos);
            }
            index++;
        }
    }

    public void RemovePlayer()
    {
        playersOnMe.Dequeue();
        //fenceposted
        int index = -1;
        Vector3 pos = this.transform.position;
        foreach (GameObject p in playersOnMe)
        {
            p.GetComponent<BoardMovement>().SetTargetAndMove(pos);
            index++;
            pos = extraSpaces[index];
        }
    }


}
