using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    //parent folders
    public Transform waypointFolder;
    public Transform lineFolder;

    //templates
    public GameObject blankObj;
    public GameObject crossObj;

    //spawn position/rotation
    public Quaternion rotation;
    public Vector3 spawnPoint;
    public bool useYourSpawnPoint; //not used yet

    //tree info
    public BoardSpace prev; //if map is uninitialized, this should be as well
    public BoardSpace root; //please be consistent, the map is essentially a tree, so if this is changed or janked up oh god

    private GameObject BuildSpace(GameObject obj)
    {
        GameObject newObj = Instantiate(obj, spawnPoint, rotation);
        newObj.transform.parent = waypointFolder;
        newObj.name = "wp";
        return newObj;
    }

    public GameObject BuildBlankSpace()
    {
        if (prev)
        {
            spawnPoint = prev.transform.position;
        }
        GameObject newObj = BuildSpace(blankObj);
        BoardSpace info = newObj.GetComponent<BoardSpace>();
        //lol jank TODO REQUIRES ROOT OF TREE TO BE A BLANK SPACE! prob change in future
        if (!prev)
        {
            root = info;
        } else
        {
            info.prevWP = prev;
            prev.nextWP = info;
        }
        prev = info;
        return newObj;
    }
    
    public void BuildCrossroad()
    {

    }

    public void BuildFinishLine()
    {
        //
    }

    public void DrawLines()
    {
        foreach (Transform child in lineFolder)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        //oh lmao do recursion
        TraverseTree(root);
    }

    private void TraverseTree(BoardSpace node)
    {
        if (!node.nextWP)
        {
            return;
        }
        CreateSingleLine(node.transform.position, node.nextWP.transform.position);
        TraverseTree(node.nextWP);
        if (node is CrossroadSpace)
        {
            CrossroadSpace crossNode = (CrossroadSpace)node;
            CreateSingleLine(node.transform.position, crossNode.alternateWP.transform.position);
            TraverseTree(crossNode.alternateWP);
        }
    }

    private void CreateSingleLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject();
        line.name = "Line";
        line.transform.parent = lineFolder;
        line.transform.position = start;
        line.AddComponent<LineRenderer>();
        LineRenderer renderer = line.GetComponent<LineRenderer>();
        renderer.sharedMaterial = new Material(Shader.Find("UI/Default"));
        renderer.sharedMaterial.SetColor("_Color", Color.white);
        renderer.startWidth = 0.1f;
        renderer.endWidth = 0.1f;
        renderer.SetPosition(0, start);
        renderer.SetPosition(1, end);
    }

    public void RenameWPs()
    {
        int count = 0;
        foreach (Transform child in waypointFolder)
        {
            child.name = "wp" + count;
            count++;
        }
    }

}
