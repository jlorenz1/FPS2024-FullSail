using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossInteraction : MonoBehaviour
{
    public Transform spawnLoaction;
    public GameObject bossRitualPrefab;
    [SerializeFeild] public AudioClip placingSound;
    [Range(0, 1)][SerializeFeild] public float placingVol;
    public void spawnBoss()
    {
        

        if (gameManager.gameInstance.playerScript.hasItems)
        {
            Instantiate(bossRitualPrefab, spawnLoaction.position, spawnLoaction.rotation);
            gameManager.gameInstance.hasStartedRitual = true;
            StartCoroutine(gameManager.gameInstance.requiredItemsUI("Boss is spawning, defeat to survive!", 6f));
            AudioManager.audioInstance.playSFXAudio(placingSound, placingVol);
            //SPAWN BOSS FUNTION FOR ENEMIES
            gameManager.gameInstance.enemySpawner.SpawnBoss();
        }
        else
        {
            Debug.Log("doesnt have items");
        }
    }
}
