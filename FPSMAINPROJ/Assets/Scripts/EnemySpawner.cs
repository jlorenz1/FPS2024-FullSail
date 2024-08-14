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




   public int SetWaveMax(int round)
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

}