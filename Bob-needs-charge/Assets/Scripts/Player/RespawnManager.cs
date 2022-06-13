using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    [SerializeField] private Animator respawnTransition;


    public float transitionTime = 1f;
    public Transform respawn;
    public Transform player;

    public void RespawnPlayer()
    {
        StartCoroutine(IFadeRespawn());
    }

    private IEnumerator IFadeRespawn()
    {
        respawnTransition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        player.position = respawn.position;
        respawnTransition.SetTrigger("End");

        respawnTransition.SetTrigger("Start");
        respawnTransition.SetTrigger("End");
    }
}
