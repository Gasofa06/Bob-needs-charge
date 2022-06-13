using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinPicker : MonoBehaviour
{
    private int coins;

    public TextMeshProUGUI coinstText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Coin"))
        {
            coins++;
            UpdateCoinsUI();
            Destroy(collision.gameObject);
        }
    }

    private void UpdateCoinsUI()
    {
        coinstText.text = coins.ToString();
    }
}
