using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour

{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 spawnAreaCenter;
    [SerializeField] Vector3 spawnAreaSize;
    [SerializeField] float spawnRadius = 1f;
    [SerializeField] int maxAttempts = 10;
    [SerializeField] bool ScalingDamage;
    [SerializeField] bool ScalingHealth;

    public void SpawnZombies(int round)
    {
        for (int i = 0; i < round * 3; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool positionFound = false;

            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                // Generate a random position within the spawn area
                spawnPosition = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                spawnAreaSize.y / 2,  // Start raycast from above
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
                );

                // Check if the position is free
                if (Physics.OverlapSphere(spawnPosition, spawnRadius).Length == 0)
                {
                    positionFound = true;
                    break;
                }
            }

            // Instantiate the zombie if a valid position was found
            if (positionFound)
            {
                RaycastHit hit;
                if (Physics.Raycast(spawnPosition, Vector3.down, out hit))
                {
                    spawnPosition.y = hit.point.y;  // Adjust Y position to the ground level
                    GameObject newZombie = Instantiate(prefab, spawnPosition, Quaternion.identity);

                    EnemyAI zombieScript = newZombie.GetComponent<EnemyAI>();
                    if (zombieScript != null)
                    {
                        if (ScalingDamage == true)
                        {
                            zombieScript.ScalingDamage(round);
                        }


                        if (ScalingHealth == true)
                        {
                            zombieScript.IncreaseHitPoints(round);
                        }



                    }



                }
            }
            else
            {
                Debug.LogWarning("Could not find a free position to spawn a zombie.");
            }
        }
    }
}
