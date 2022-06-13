using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDamage : MonoBehaviour
{
    public int damage = 1;
    [SerializeField] private HealthControler _healthControler;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _healthControler.Damage(damage);
        }
    }

}
