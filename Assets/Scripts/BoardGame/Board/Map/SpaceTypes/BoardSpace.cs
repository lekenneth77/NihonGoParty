using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardSpace : MonoBehaviour
{
    public Sprite Sprite { get; set; }
    public string typeName;

    public abstract void Action();

    public static event Action ActionFinish;

    public static event Action<string, bool> TriggerLoad;

    public static void InvokeFinish()
    {
        ActionFinish?.Invoke();
    }

    public static void InvokeLoad(string sceneName, bool additive)
    {
        TriggerLoad?.Invoke(sceneName, additive);
    }

    //all the stuff from the original spaceinfo class put into here!
    private Queue<GameObject> playersOnMe;
    private Vector3[] extraSpaces; //spaces used for when multiple players are on the same space
    public BoardSpace chosenPath;
    public BoardSpace nextWP;
    public BoardSpace prevWP;

    public virtual void Start()
    {
        chosenPath = nextWP;
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
            p.GetComponent<MoveObject>().SetTargetAndMove(extraSpaces[index]);
            StartCoroutine(HandleQuickRotation(p));
            index++;
        }
    }

    //holyshit i hate this function if everything hits the fucking shit fan change all of the boardspaces settargetandmove to STAMNAR
    private IEnumerator HandleQuickRotation(GameObject p) {
        yield return new WaitForSeconds(1f); //TODO really jank just consider not rotating at all!
        p.GetComponent<MoveObject>().RotateToIdentity();
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
                p.transform.rotation = Quaternion.identity;
            }
            else
            {
                p.GetComponent<MoveObject>().SetTargetAndMove(pos);
                StartCoroutine(HandleQuickRotation(p));
            }
            index++;
        }
    }

    public void RemovePlayer()
    {
        if (playersOnMe.Count == 0)
        {
            return;
        }
        playersOnMe.Dequeue();
        //fenceposted
        int index = -1;
        Vector3 pos = this.transform.position;
        foreach (GameObject p in playersOnMe)
        {
            p.GetComponent<MoveObject>().SetTargetAndMove(pos);
            StartCoroutine(HandleQuickRotation(p));
            index++;
            pos = extraSpaces[index];
        }
    }

}
