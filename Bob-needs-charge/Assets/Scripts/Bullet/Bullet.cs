using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 15f;
    public float maxTime = 1;
    public Rigidbody2D rb;

    float delayDestroy = 0.1f;
    float interval;


    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        if(interval > maxTime)
        {
            Destroy(gameObject);
        } else
        {
            interval += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
        {
            Destroy(gameObject, delayDestroy);
        }
    }
}
