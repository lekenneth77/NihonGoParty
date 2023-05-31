using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public GameObject screen;
    public Image loadingBarFill;
    void Start()
    {
        BoardSpace.TriggerLoad += LoadScene;
    }

    public void LoadScene(string sceneName, bool additive)
    {
        StartCoroutine(LoadSceneAsync(sceneName, additive));
    }

    IEnumerator LoadSceneAsync(string sceneName, bool additive)
    {
        loadingBarFill.fillAmount = 0;
        screen.SetActive(true);
        LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!op.isDone)
        {
            float progressValue = Mathf.Clamp01(op.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }
        screen.SetActive(false);

    }

}
