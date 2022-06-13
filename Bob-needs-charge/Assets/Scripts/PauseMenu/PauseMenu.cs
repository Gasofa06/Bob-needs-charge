using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private LoadFade fadeAnimation;

    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;

    void OnPause()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Options()
    {
        Debug.Log("Options...");
    }

    public void LoadMenu()
    {
        fadeAnimation.LoadSceneFade("Menu");
        Time.timeScale = 1f;
    }
}
