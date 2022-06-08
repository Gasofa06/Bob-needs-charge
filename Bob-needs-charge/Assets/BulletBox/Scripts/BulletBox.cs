using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBox : MonoBehaviour
{

    public GameObject bulletPrefab;
    public int numBulletsToInstantiate = 10;

    void Start()
    {
        for (int i = 0; i < numBulletsToInstantiate; i++)
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
        }
    }

    public void InstantiateOneMoreBullet()
    {
        Instantiate(bulletPrefab);
    }
}
