using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private LoadFade fadeAnimation;
    
    public void PlayGame()
    {
        fadeAnimation.LoadSceneFade("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
