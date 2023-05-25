using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public Transform waypointFolder;
    public Transform lineFolder;
    public GameObject blankObj;
    public Quaternion rotation;
    public Vector3 spawnPoint;
    public bool useYourSpawnPoint;

    public SpaceInfo prev; //if map is uninitialized, this should be as well
    public SpaceInfo root; //please be consistent, the map is essentially a tree, so if this is changed or janked up oh god

    public GameObject BuildBlankSpace()
    {
        if (prev)
        {
            spawnPoint = prev.transform.position;
        }
        GameObject newObj = Instantiate(blankObj, spawnPoint, rotation);
        newObj.transform.parent = waypointFolder;
        newObj.name = "wp";
        SpaceInfo info = newObj.GetComponent<SpaceInfo>();
        //lol jank 
        if (!prev)
        {
            root = info;
        } else
        {
            prev.nextWP = info;
        }
        prev = info;
        return newObj;
    }
    
    public void BuildCrossroad()
    {
        //for now it has to build to two blanks but maybe change it in the future
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
        traverseTree(root);
    }

    private void traverseTree(SpaceInfo node)
    {
        if (!node.nextWP)
        {
            return;
        }
        createSingleLine(node.transform.position, node.nextWP.transform.position);
        traverseTree(node.nextWP);
        if (node.amCrossroad)
        {
            createSingleLine(node.transform.position, node.alternateWP.transform.position);
            traverseTree(node.alternateWP);
        }
    }

    private void createSingleLine(Vector3 start, Vector3 end)
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

}
