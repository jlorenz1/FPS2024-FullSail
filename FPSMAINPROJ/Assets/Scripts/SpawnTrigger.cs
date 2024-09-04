using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnLocation;

  
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < SpawnLocation.Length; i++)
            {
                SpawnLocation[i].SetActive(true);
            }
            gameManager.gameInstance.enemySpawner.RefeshSpawnPoints();
        }
        else
            return;

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < SpawnLocation.Length; i++)
            {
                SpawnLocation[i].SetActive(false);
            }
            gameManager.gameInstance.enemySpawner.RefeshSpawnPoints();
        }
        else
            return;
    }
}
