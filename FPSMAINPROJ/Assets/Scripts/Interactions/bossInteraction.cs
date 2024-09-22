using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossInteraction : MonoBehaviour
{
    [SerializeField] doorScript DoorToOpen;
    [SerializeField] GameObject Door;
    public GameObject bossRitualPrefab;
    [SerializeField] public AudioClip placingSound;
    [Range(0, 1)][SerializeFeild] public float placingVol;
    public void spawnBoss()
    {

        Debug.Log("door open");
        if (gameManager.gameInstance.playerScript.hasItems)
        {
            DoorToOpen.slide();
            AudioManager.audioInstance.playSFXAudio(placingSound, placingVol);
            Debug.Log("door open");
        }
        else
        {
            Debug.Log("doesnt have items");
        }
    }
}
