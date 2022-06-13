using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogVillager : MonoBehaviour
{
    // All the villagers needs to be inside of the dialog manager.
    // Because dialog manager will send a message to all here childs (InputSystem - BoardcastMessagge) when cross is pressed

    [SerializeField] private DialogManager _dialogManager;
    [SerializeField] private string[] sentences;

    private bool isNear;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNear = false;
        }
    }

    // Input System function
    private void OnTalk()
    {
        if (isNear & _dialogManager.ConversationFinished())
        {
            _dialogManager.StartDialog(sentences);
        }
    }
}
