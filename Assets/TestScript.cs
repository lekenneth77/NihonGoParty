using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    public GameObject[] boys;
    public GameObject[] smallBoys; 

    public Light l;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-transform.right * 0.1f);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(transform.right * 0.1f);

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadSceneAsync("Title Screen");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            l.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            l.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (GameObject go in boys) {
                go.SetActive(false);
            }
            foreach (GameObject go in smallBoys)
            {
                go.SetActive(false);
            }

        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (GameObject go in boys)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in smallBoys)
            {
                go.SetActive(false);
            }

        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            foreach (GameObject go in smallBoys)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in boys)
            {
                go.SetActive(false);
            }
        }
    }
}
