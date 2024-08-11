using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour

{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 spawnAreaCenter;
    [SerializeField] Vector3 spawnAreaSize;

    public void SpawnZombies(int round)
    {
        for (int i = 0; i < round * 3; i++)
        {
            // Generate a random position within the spawn area
            Vector3 randomPosition = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            // Instantiate the zombie at the random position
            GameObject newZombie = Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }
}
