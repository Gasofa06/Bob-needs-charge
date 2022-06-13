using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    private float startPos;
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffect;


    void Start()
    {
        startPos = transform.position.x;
    }

    void Update()
    {
        float distance = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
