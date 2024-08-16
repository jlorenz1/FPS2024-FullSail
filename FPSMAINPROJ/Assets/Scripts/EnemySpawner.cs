using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour

{
    [SerializeField] GameObject prefab;
    [SerializeField] int ZombiesPerRound;
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] Vector3 spawnAreaCenter;
    [SerializeField] Vector3 spawnAreaSize;
    [SerializeField] float spawnRadius = 1f;
    [SerializeField] int maxAttempts = 10;
    [SerializeField] bool ScalingDamage;
    [SerializeField] bool ScalingHealth;
    [SerializeField] bool IsBufferSpawn;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] bool UsingFloorToSpawn;
   
    void Start()
    {

        if (floorPrefab != null)
        {
            // Set the spawn area center based on the floor prefab's position
            spawnAreaCenter = floorPrefab.transform.position;
            spawnAreaCenter.y += 1f; // Adjust Y position if needed to spawn above the floor
        }

    }

   public void PopulateSpawnPoints()
    {
        spawnPoints.Clear(); // Clear any existing points in the list

        // Find all GameObjects with the "SpawnPoint" tag and add them to the list
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject obj in spawnPointObjects)
        {
            if (obj.transform != null)
            {
                spawnPoints.Add(obj.transform);
            }
        }
    }

    public void ZombieSpawner()
    {

        int Round = gameManager.gameInstance.GetGameRound();
        int targetCount = Round * ZombiesPerRound;
        

      for(int i =0;i < targetCount; i++)
        {
            if (!UsingFloorToSpawn)
            {
                SpawnAtRandomPoint();
            }
            else
            {
                SpawnAtRandomNavMeshPoint();
            }
           
        }
    }


    void SpawnAtRandomPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Vector3 randomPoint = GetRandomPointOnNavMesh(randomSpawnPoint.position, spawnRadius);
        if (randomPoint != Vector3.zero)
        {
            GameObject newZombie = Instantiate(prefab, randomPoint, Quaternion.identity);
            SetupZombie(newZombie, gameManager.gameInstance.GetGameRound());
        }
    }

    void SpawnAtRandomNavMeshPoint()
    {
        Vector3 randomPoint = GetRandomPointOnNavMesh(spawnAreaCenter, spawnRadius);
        if (randomPoint != Vector3.zero)
        {
            GameObject newZombie = Instantiate(prefab, randomPoint, Quaternion.identity);
            SetupZombie(newZombie, gameManager.gameInstance.GetGameRound());
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