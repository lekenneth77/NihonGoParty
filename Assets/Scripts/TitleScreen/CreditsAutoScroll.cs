using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsAutoScroll : MonoBehaviour
{
    public float speed = 100f;
    float beginPos = 0f;
    float endPos = 5000f;

    public RectTransform container;
    // Start is called before the first frame update
    void Start()
    {
        container.localPosition = new Vector2(0, beginPos);
        StartCoroutine(AutoScrollText());
    }

    IEnumerator AutoScrollText()
    {
        while (container.localPosition.y < endPos)
        {
            container.Translate(Vector3.up * speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        //SceneManager.LoadSceneAsync("TitleScreen");
    }
}
