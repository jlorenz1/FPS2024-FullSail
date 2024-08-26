using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossInteraction : MonoBehaviour
{
    public Transform spawnLoaction;
    public GameObject bossRitualPrefab;

    public void spawnBoss()
    {
        

        if (gameManager.gameInstance.playerScript.hasItems)
        {
            Instantiate(bossRitualPrefab, spawnLoaction.position, spawnLoaction.rotation);
            gameManager.gameInstance.hasStartedRitual = true;
            StartCoroutine(gameManager.gameInstance.requiredItemsUI("Boss is spawning, defeat to survive!", 6f));
            //SPAWN BOSS FUNTION FOR ENEMIES
        }
        else
        {
            Debug.Log("doesnt have items");
        }
    }
}
