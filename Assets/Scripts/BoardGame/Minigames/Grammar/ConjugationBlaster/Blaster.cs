using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : MonoBehaviour
{
    public GameObject defBullet;
    public Transform bulletFolder;
    
    // Start is called before the first frame update
    void Start()
    {

        defBullet.GetComponent<Bullet>().StopBullet();
    }

    public void Blast(int bulletType) {
        GameObject bullet = Instantiate(defBullet, transform.position, Quaternion.identity, bulletFolder);
        bullet.GetComponent<Bullet>().bulletType = bulletType;
    }

    public void Swap() { 

    }

}
