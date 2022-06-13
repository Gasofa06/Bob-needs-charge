using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public float typingSpeed;
    private bool conversationFinished = true; 

    [SerializeField] private Animator dailogAnimatonControler;
    [SerializeField] private float waitForAnimation = 0.5f;

    private string[] sentences;
    private int index;
    private bool finishTyping;

    public void StartDialog(string[] _sentences)
    {
        conversationFinished = false;
        Time.timeScale = 0f;
        dailogAnimatonControler.SetBool("pop", true);
        sentences = _sentences;
        StartCoroutine(WaitForAnimation());
    }

    // Waits some second until start typing
    IEnumerator WaitForAnimation()
    {
        yield return new WaitForSecondsRealtime(waitForAnimation);
        StartCoroutine(Type());
    }

    // Animation typing
    IEnumerator Type()
    {
        finishTyping = false;
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        finishTyping = true;
    }

    //Input Systemfunction
    private void OnNextSentence()
    {
        if (finishTyping)
        {
            if (index < sentences.Length - 1)
            {
                index++;
                textDisplay.text = "";
                StartCoroutine(Type());
            }
            else
            {
                index = 0;
                finishTyping = false;
                conversationFinished = true;
                textDisplay.text = "";
                dailogAnimatonControler.SetBool("pop", false);
                Time.timeScale = 1f;
            }
        }
    }

    // Know if all the conversation has finished
    public bool ConversationFinished()
    {
        return conversationFinished;
    }
}
