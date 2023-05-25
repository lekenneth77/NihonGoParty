using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitle : MonoBehaviour
{
    public void GoToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Title Screen");
    }
}
