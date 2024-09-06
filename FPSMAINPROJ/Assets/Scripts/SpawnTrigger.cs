using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnLocation;
    [SerializeFeild] int Difficulty;
    bool isActivated;
    bool wiped;
    int ZombieCount;

    private void Start()
    {
        wiped = false;
    }
    private void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (wiped == false) { 
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            foreach (GameObject zombie in zombies)
            {
                IDamage Zombies = zombie.GetComponent<IDamage>();

                if (Zombies != null)
                {

                    Zombies.DieWithoutDrops();

                }

            }
            wiped = true;
        }

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
