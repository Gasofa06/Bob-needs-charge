using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 15f;
    public float maxTime = 1;
    public float delayDestroy = 0.1f;

    private Animator bulletAnimationControler;
    private Rigidbody2D rb;

    float interval;


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        bulletAnimationControler = GetComponent<Animator>();
        rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        if(interval > maxTime)
        {
            bulletAnimationControler.SetBool("destroy", true);
            Destroy(gameObject, delayDestroy);
        } else
        {
            interval += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
        {
            bulletAnimationControler.SetBool("destroy", true);
            rb.velocity = transform.right * 0.05f;
            Destroy(gameObject, delayDestroy);
        }
    }
}
