using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlasterEnemy : MonoBehaviour
{
    public Transform enemyFolder;
    public Transform[] enemySpawns;
    public int enemyType;
    public float moveSpeed = 4f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(-moveSpeed, 0, 0);
    }

    public void StopEnemy() {
        rb.velocity = Vector3.zero;
    }

    

    public void Spawn(int enemyType, string text) {
        int random = Random.Range(0, enemySpawns.Length);
        GameObject enemy = Instantiate(gameObject, enemySpawns[random].position, Quaternion.identity, enemyFolder);
        enemy.GetComponent<BlasterEnemy>().enemyType = enemyType;
        enemy.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }





}
