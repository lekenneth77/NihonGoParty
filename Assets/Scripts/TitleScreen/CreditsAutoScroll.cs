using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsAutoScroll : MonoBehaviour
{
    public float speed = 150f;
    float beginPos = 1985f;
    float endPos = 9600f;

    public Image fadeIn;
    public RectTransform container;
    // Start is called before the first frame update
    void Start()
    {
        container.localPosition = new Vector2(0, beginPos);
        StartCoroutine(AutoScrollText());
    }

    IEnumerator AutoScrollText()
    {

        for (float i = 1f; i >= 0f; i -= 0.05f) {
            Color temp = fadeIn.color;
            temp.a = i;
            fadeIn.color = temp;
            yield return new WaitForSeconds(0.1f);
        }

        while (container.localPosition.y < endPos)
        {
            container.Translate(Vector3.up * speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(8f);
        SceneManager.LoadSceneAsync("Title Screen");
    }
}
