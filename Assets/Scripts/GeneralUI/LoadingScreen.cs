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

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingBarFill.fillAmount = 0;
        screen.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            float progressValue = Mathf.Clamp01(op.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }
        screen.SetActive(false);

    }

}
