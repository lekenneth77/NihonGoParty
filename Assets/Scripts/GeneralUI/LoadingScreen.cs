using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    private Sprite[] loadingImages;
    public GameObject screen;
    public Image loadingBarFill;


    void Awake()
    {
        if (Instance != null && Instance != this)  {
            Destroy(this);
            return;
        } else  { 
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        loadingImages = Resources.LoadAll<Sprite>("Images/loadingScreens/");
        BoardSpace.TriggerLoad += LoadScene;
        BoardSpace.TriggerUnload += UnloadScene;
    }

    private void OnDestroy() {
        BoardSpace.TriggerLoad -= LoadScene;
        BoardSpace.TriggerUnload -= UnloadScene;
    }

    public void LoadScene(string sceneName, bool additive)
    {
        StartCoroutine(LoadSceneAsync(sceneName, additive));
    }

    IEnumerator LoadSceneAsync(string sceneName, bool additive)
    {
        loadingBarFill.fillAmount = 0;
        screen.GetComponent<Image>().sprite = loadingImages[Random.Range(0, loadingImages.Length)];
        screen.SetActive(true);
        LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
        op.completed += Bruh;
        while (!op.isDone)
        {
            float progressValue = Mathf.Clamp01(op.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }
        //SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
    }

    public void Bruh(AsyncOperation op) {
        screen.SetActive(false);

    }

    public void UnloadScene(int index, bool triggerBoard) {
        StartCoroutine(UnloadSceneAsync(index, triggerBoard));
    }

    IEnumerator UnloadSceneAsync(int index, bool triggerBoard) {
        if (SceneManager.sceneCount <= index) {
            yield break;
        }
        loadingBarFill.fillAmount = 0;
        screen.SetActive(true);
        AsyncOperation op = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
        while (!op.isDone)
        {
            float progressValue = Mathf.Clamp01(op.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }
        screen.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        if (triggerBoard) {
            BoardSpace.InvokeFinish();
        }
    }

}
