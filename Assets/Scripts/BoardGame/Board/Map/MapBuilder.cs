using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    //parent folders
    public Transform waypointFolder;
    public Transform lineFolder;

    public MinigameSelector selector;

    //templates
    public GameObject blankObj;
    public GameObject crossObj;
    public GameObject finishObj;

    public GameObject[] minigameObjs; //kata, gram, kanji, vocab, duel, multi
    public Sprite[] spaceArts;

    //spawn position/rotation
    public Quaternion rotation;
    public Vector3 spawnPoint;
    public bool useYourSpawnPoint; //not used yet

    //tree info
    public BoardSpace prev; //if map is uninitialized, this should be as well
    public BoardSpace root; //please be consistent, the map is essentially a tree, so if this is changed or janked up oh god

    public string changeToThis; //blank katakana finish (does not work for crossroad)

    private GameObject BuildSpace(GameObject obj)
    {
        if (prev)
        {
            spawnPoint = prev.transform.position;
        }
        GameObject newObj = Instantiate(obj, spawnPoint, rotation);
        newObj.transform.parent = waypointFolder;
        newObj.name = "wp";
        BoardSpace info = newObj.GetComponent<BoardSpace>();
        //lol jank TODO REQUIRES ROOT OF TREE TO BE A BLANK SPACE! prob change in future
        if (!prev)
        {
            root = info;
        }
        else
        {
            info.prevWP = prev;
            prev.nextWP = info;
        }
        prev = info;
        return newObj;
    }

    public GameObject BuildBlankSpace()
    {
        GameObject newObj = BuildSpace(blankObj);
        return newObj;
    }

    public GameObject BuildCrossroad()
    {
        CrossroadSpace crossroad = BuildSpace(crossObj).GetComponent<CrossroadSpace>();
        //alt path
        BoardSpace altPath = BuildSpace(blankObj).GetComponent<BoardSpace>();
        altPath.transform.position = new Vector3(altPath.transform.position.x - 5, altPath.transform.position.y, altPath.transform.position.z);
        crossroad.alternateWP = altPath;
        prev = crossroad;
        //next path
        BoardSpace nextPath = BuildSpace(blankObj).GetComponent<BoardSpace>();
        nextPath.transform.position = new Vector3(nextPath.transform.position.x + 5, nextPath.transform.position.y, nextPath.transform.position.z);
        return crossroad.gameObject;
    }

    public GameObject BuildFinishLine()
    {
        GameObject newObj = BuildSpace(finishObj);
        return newObj;
    }
    
    public GameObject BuildMinigame(int minigameIndex) {
        GameObject newObj = BuildSpace(minigameObjs[minigameIndex]);
        return newObj;
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
        line.transform.position = new Vector3(start.x, start.y - 0.1f, start.z);
        line.AddComponent<LineRenderer>();
        LineRenderer renderer = line.GetComponent<LineRenderer>();
        renderer.sharedMaterial = new Material(Shader.Find("UI/Default"));
        renderer.sharedMaterial.SetColor("_Color", Color.white);
        renderer.startWidth = 0.3f;
        renderer.endWidth = 0.3f;
        renderer.SetPosition(0, start);
        renderer.SetPosition(1, end);
    }


    public void RenameWPs()
    {
        int count = 0;
        //typename doesn't work for some reason?
        foreach (Transform child in waypointFolder)
        {

            child.name = "wp" + child.gameObject.GetComponent<BoardSpace>().typeName + count;
            count++;
        }
    }

    public void ChangeSelectedTo(GameObject selected)
    {
        if (!selected.GetComponent<BoardSpace>() || selected.GetComponent<BoardSpace>() is CrossroadSpace) return;
        BoardSpace prev = selected.GetComponent<BoardSpace>().prevWP;
        BoardSpace next = selected.GetComponent<BoardSpace>().nextWP;
        DestroyImmediate(selected.GetComponent<BoardSpace>());
        BoardSpace changed = null;
        switch(changeToThis.ToLower())
        {
            case "katakana":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[0];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<MinigameSpace>();
                ((MinigameSpace)changed).category = "KATAKANA";
                ((MinigameSpace)changed).selector = selector;
                break;
            case "grammar":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[1];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<MinigameSpace>();
                ((MinigameSpace)changed).category = "GRAMMAR";
                ((MinigameSpace)changed).selector = selector;
                break;
            case "kanji":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[2];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<MinigameSpace>();
                ((MinigameSpace)changed).category = "KANJI";
                ((MinigameSpace)changed).selector = selector;
                break;
            case "vocab":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[3];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<MinigameSpace>();
                ((MinigameSpace)changed).category = "VOCAB";
                ((MinigameSpace)changed).selector = selector;
                break;
            case "duel":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[4];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<MinigameSpace>();
                ((MinigameSpace)changed).category = "DUEL";
                ((MinigameSpace)changed).selector = selector;
                break;
            case "multi":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[5];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<MinigameSpace>();
                ((MinigameSpace)changed).category = "MULTI";
                ((MinigameSpace)changed).selector = selector;
                break;
            case "blank":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[6];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<BlankSpace>();
                break;
            case "finish":
                selected.GetComponent<SpriteRenderer>().color = Color.white;
                selected.GetComponent<SpriteRenderer>().sprite = spaceArts[7];
                selected.transform.eulerAngles = new Vector3(270f, 0f, 0f);
                changed = selected.AddComponent<FinishSpace>();
                break;
            default:
                return;
        }
        changed.typeName = changeToThis.ToLower();
        prev.nextWP = changed;
        next.prevWP = changed;
        changed.nextWP = next;
        changed.prevWP = prev;
    }

    //write a function to remap the sprites to the template once the template sprite changes

}
