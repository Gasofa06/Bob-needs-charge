using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFade : MonoBehaviour
{

    public Animator loadSceneTransition;


    public float transitionTime = 1f;

    public void LoadSceneFade(string loadScene)
    {
        StartCoroutine(IFade(loadScene));
    }

    private IEnumerator IFade(string _loadScene)
    {
        loadSceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(_loadScene);
    }
}
