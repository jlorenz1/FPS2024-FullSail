using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BufferSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    [SerializeField] GameObject prefab3;
    [SerializeField] Vector3 spawnAreaCenter;
    [SerializeField] Vector3 spawnAreaSize;
    [SerializeField] float spawnRadius = 2f;
    [SerializeField] bool ScalingDamage;
    [SerializeField] bool ScalingHealth;
    [SerializeField] bool IsBufferSpawn;
    [SerializeField] private GameObject floorPrefab;

    int WaveMax;

    int RoundType = 0;
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

        WaveMax = round / 5;

        return WaveMax;

    }

    public void BufferSpawnZombies(int round)
    {

       RoundType = RoundCycle(gameManager.gameInstance.GetGameRound());



        for (int i = 0; i < WaveMax; i++)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(spawnAreaCenter, spawnRadius);
            if (randomPoint != Vector3.zero)
            {
              /*  if (RoundType == 0)
                {
                    GameObject newZombie = Instantiate(prefab1, randomPoint, Quaternion.identity);
                    SetupZombie(newZombie, round);
                }
                if (RoundType == 2)
                {
                    GameObject newZombie = Instantiate(prefab2, randomPoint, Quaternion.identity);
                    SetupZombie(newZombie, round);
                }
                if (RoundType == 3)
                {
                    GameObject newZombie = Instantiate(prefab3, randomPoint, Quaternion.identity);
                    SetupZombie(newZombie, round);
                }
                if (RoundType == 4 || RoundType == 1)
                {*/
                    GameObject newZombie1 = Instantiate(prefab1, randomPoint, Quaternion.identity);
                    GameObject newZombie2 = Instantiate(prefab2, randomPoint, Quaternion.identity);
                    GameObject newZombie3 = Instantiate(prefab3, randomPoint, Quaternion.identity);
                    SetupZombie(newZombie1, round);
                    SetupZombie(newZombie2, round);
                    SetupZombie(newZombie3, round);
              //  }
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



    int RoundCycle(int round)
    {

        if (round / 5 == 1 || round / 5 == 4 || round / 5 == 7)
        {
            return 1;
        }
        if (round / 5 == 2 || round / 5 == 5 || round / 5 == 8)
        {
            return 2;
        }
        if (round / 5 == 3 || round / 5 == 6 || round / 5 == 9)
        {
            return 3;
        }
        if (round / 5 > 9)
        {
            return 4;
        }
        return 0;
    }



}
