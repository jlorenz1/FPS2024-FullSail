using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour

{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 spawnAreaCenter;
    [SerializeField] Vector3 spawnAreaSize;
    [SerializeField] float spawnRadius = 1f;
    [SerializeField] int maxAttempts = 10;
    [SerializeField] bool ScalingDamage;
    [SerializeField] bool ScalingHealth;
    [SerializeField] bool IsBufferSpawn;
    [SerializeField] private GameObject floorPrefab;

    int WaveMax;
    void Start()
    {
        if (floorPrefab != null)
        {
            // Set the spawn area center based on the floor prefab's position
            spawnAreaCenter = floorPrefab.transform.position;
            spawnAreaCenter.y += 1f; // Adjust Y position if needed to spawn above the floor
        }

        SetWaveMax(1); 
    }




    int SetWaveMax(int round)
    {

        WaveMax = round * 5;

        return WaveMax;

    }

    public void BaseSpawnZombies(int round)
    {
        for (int i = 0; i < WaveMax; i++)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(spawnAreaCenter, spawnRadius);
            if (randomPoint != Vector3.zero)
            {
                GameObject newZombie = Instantiate(prefab, randomPoint, Quaternion.identity);
                SetupZombie(newZombie, round);
            }
        }
    }

    void SetupZombie(GameObject zombie, int round)
    {
        EnemyAI zombieScript = zombie.GetComponent<EnemyAI>();
        if (zombieScript != null)
        {
            if (ScalingDamage)
                zombieScript.ScalingDamage(round);

            if (ScalingHealth)
                zombieScript.IncreaseHitPoints(round);
        }
    }


    Vector3 GetRandomPointOnNavMesh(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        NavMeshHit hit;

        randomDirection.x = Mathf.Clamp(randomDirection.x, center.x - radius, center.x + radius);
        randomDirection.z = Mathf.Clamp(randomDirection.z, center.z - radius, center.z + radius);


        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }

    public void BufferSpawnZombie(int round)
    {
        Debug.Log("Boss Wave");
        if (IsBufferSpawn == true)
        {
            for (int i = 0; i < WaveMax/5; i++)
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
}
