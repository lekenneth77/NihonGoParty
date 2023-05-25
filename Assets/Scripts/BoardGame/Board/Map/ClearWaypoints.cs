using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearWaypoints : MonoBehaviour
{

    public Transform lineFolder;
    public void removeAll()
    {
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        foreach (Transform child in lineFolder)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
