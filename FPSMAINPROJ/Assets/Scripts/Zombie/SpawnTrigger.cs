using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnLocation;
    [SerializeFeild] int Difficulty;
    [SerializeField] bool isEndless;
    bool isActivated;
    bool wiped;
    int ZombieSpawnCount;
    int Count;
    bool BossIsSpawned;
    private void Start()
    {
        wiped = false;
        BossIsSpawned = false;
    }
    private void Update()
    {
        Count = gameManager.gameInstance.GetEnemyCount(); 
        if(Count == 0 && isActivated && isEndless)
        {
            SpawnEnemies(ZombieSpawnCount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (wiped == false) { 
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            foreach (GameObject zombie in zombies)
            {
                    IEnemyDamage Zombies = zombie.GetComponent<IEnemyDamage>();

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


            SpawnEnemies(ZombieSpawnCount);


            if (!BossIsSpawned)
            {
                gameManager.gameInstance.enemySpawner.SpawnBoss();

                BossIsSpawned = true;
            }
        }
        else
            return;

    }


   void SpawnEnemies(int Amount)
    {

        gameManager.gameInstance.enemySpawner.ZombieSpawner(Amount);



    }
}
