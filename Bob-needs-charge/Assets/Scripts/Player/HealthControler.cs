using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthControler : MonoBehaviour
{
    private int maxPlayerHealth;
    private int currentPlayerHealth;

    [SerializeField] private Image[] hearts;

    [SerializeField] private RespawnManager _respawnManager;

    private void Start()
    {
        maxPlayerHealth = hearts.Length;
        currentPlayerHealth = maxPlayerHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < maxPlayerHealth; i++)
        {
            if (i < currentPlayerHealth)
            {
                hearts[i].color = Color.red;
            } 
            else
            {
                hearts[i].color = Color.black;
            }
        }
    }

    public void Damage(int _damage)
    {
        currentPlayerHealth -= _damage;
        UpdateHealthUI();
        IsDead();
    }

    public void Heal(int _heal)
    {
        currentPlayerHealth += _heal;
        if (currentPlayerHealth < maxPlayerHealth)
        {
            currentPlayerHealth = maxPlayerHealth;
        }

        UpdateHealthUI();
    }

    private void IsDead()
    {
        if(currentPlayerHealth <= 0)
        {
            currentPlayerHealth = maxPlayerHealth;
            UpdateHealthUI();
            _respawnManager.RespawnPlayer();
        }
    }

    public int CurrentHealth()
    {
        return currentPlayerHealth;
    }
}
