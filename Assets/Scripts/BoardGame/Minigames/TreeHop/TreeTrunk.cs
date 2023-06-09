using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTrunk : MonoBehaviour
{
    public JumpObject jumper;
    public FollowOneAxis treeCamera;
    public Transform platformFolder;
    public GameObject platform;
    public StunStarSpin stars;

    public Transform nextLeftPlatform;
    public Transform nextRightPlatform;
    public Transform tippyTop;
    private float distanceBetween;

    public bool leftCorrect;
    public int maxDepth;

    public static string[] correctWords;
    public static string[] wrongWords;

    private int currentDepth;
    private bool onLeft;
    // Start is called before the first frame update
    void Start() {
        currentDepth = 0;
        distanceBetween = 2f;
        maxDepth = 10;
        int random = Random.Range(0, 2);
        //leftCorrect = random == 0 ? true : false;
        leftCorrect = false;
        onLeft = false;
    }

    public int Jump(bool left)
    {
        if (left && leftCorrect || !left && !leftCorrect)
        {
            //correct!
            Vector3 nextPlatform = left ? nextLeftPlatform.position : nextRightPlatform.position;
            jumper.SetupAndJump(nextPlatform + new Vector3(0, 0.63f, 0), 1f, 2f);

            onLeft = left;
            currentDepth++;
            if (currentDepth == maxDepth)
            {
                Debug.Log("Finished!");
                StartCoroutine("MoveCamera");
                return 1;
            } else
            {
                CreateNextPlatforms();
                return 0;
            }
        } else
        {
            if ((left && onLeft) || (!left && !onLeft))
            {
                //attempt jump to platform above self
                jumper.SetupAndJump(jumper.gameObject.transform.position, 1f, 1.5f);
            } else
            {
                //attempt to jump to other side
                float dist = left ? -distanceBetween : distanceBetween;
                jumper.SetupAndJump(new Vector3(jumper.gameObject.transform.position.x + dist, jumper.gameObject.transform.position.y, jumper.gameObject.transform.position.z), 1f, 1.5f);
                onLeft = left;
            }

            return -1;
        }
    }

    public void JumpToEnd() {
        jumper.SetupAndJump(tippyTop.position, 2f, 2f);
        StartCoroutine("MoveCamera");
    }

    private void CreateNextPlatforms()
    {
        StartCoroutine("MoveCamera");
        //create both new platforms
        GameObject newLeft = Instantiate(platform, nextLeftPlatform.position + new Vector3(0, 3f, 0), Quaternion.identity);
        GameObject newRight = Instantiate(platform, nextRightPlatform.position + new Vector3(0, 3f, 0), Quaternion.identity);
        newLeft.name = "plat" + currentDepth;
        newRight.name = "plat" + currentDepth;
        newLeft.transform.parent = platformFolder;
        newRight.transform.parent = platformFolder;
        if (platformFolder.childCount > 6) {
            Destroy(platformFolder.GetChild(0).gameObject);
            Destroy(platformFolder.GetChild(1).gameObject);
        }
        nextLeftPlatform = newLeft.transform;
        nextRightPlatform = newRight.transform;

        //setup words
        int random = Random.Range(0, 2);
        leftCorrect = random == 0 ? true : false;
        leftCorrect = false;
    }

    private IEnumerator MoveCamera()
    {
        //based on jumpers duration
        yield return new WaitForSeconds(1f);
        treeCamera.MoveCamera();
    }
   
}
