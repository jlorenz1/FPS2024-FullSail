using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour

{
    [SerializeField] GameObject prefab;
    [SerializeField] int ZombiesPerRound;
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] List<Transform> BossspawnPoints = new List<Transform>();
    [SerializeField] Vector3 spawnAreaCenter;
    [SerializeField] Vector3 spawnAreaSize;
    [SerializeField] float spawnRadius = 1f;
    
    [SerializeField] bool ScalingDamage;
    [SerializeField] bool ScalingHealth;
    [SerializeField] bool IsBufferSpawn;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] bool UsingFloorToSpawn;
    [SerializeField] List<GameObject> MeeleZombies;
    [SerializeField] List<GameObject> RangedZombies;
    [SerializeField] List<GameObject> SpecialZombies;
    [SerializeField] List<GameObject> BossZombie;

    int TypeSplit;
    int targetCount;

    private void Start()
    {
        PopulateSpawnPoints();
        targetCount = 0;
    }
    public void RefeshSpawnPoints()
    {
        if(spawnPoints.Count > 0)
        {
            foreach(Transform t in spawnPoints)
            {
                t.gameObject.SetActive(false);
            }
        }


        spawnPoints.Clear();
       
    }

    public void PopulateSpawnPoints()
    {
      

        // Find all GameObjects with the "SpawnPoint" tag and add them to the list
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject obj in spawnPointObjects)
        {
            if (obj.activeInHierarchy)
            {
                spawnPoints.Add(obj.transform);
            }
        
        }
    }

    public void ZombieSpawner()
    {


      for(int i =0;i < targetCount; i++)
        {
           
                SpawnAtRandomPoint();
           
        }
    }

    public void ZombieSpawner(int Amount)
    {
        targetCount += Amount;

        for (int i = 0; i < Amount; i++)
        {
            if (spawnPoints != null)
            {
                SpawnAtRandomPoint();
            }
        }


    }

    public void SpecialZombieSpawner(int Interval)
    {
        if (Interval > 0)
        {
            int Round = gameManager.gameInstance.GetGameRound();
            int targetCount = Round / Interval;


            for (int i = 0; i < targetCount; i++)
            {
                SpawnSpecialAtRandomPoint();
            }
        }
        else
            return;
    }

    public void SpawnBoss()
    {
        // Get a random spawn point from the list of boss spawn points
        Transform randomSpawnPoint = BossspawnPoints[Random.Range(0, BossspawnPoints.Count)];

        // Loop through the list of BossZombie prefabs
        for (int i = 0; i < BossZombie.Count; i++)
        {
            // Get a new random point on the NavMesh for each boss spawn
            Vector3 randomPoint = GetRandomPointOnNavMesh(randomSpawnPoint.position, spawnRadius);

            // Instantiate each boss zombie at a random point
            GameObject randomZombiePrefab = BossZombie[i];
            GameObject newZombie = Instantiate(randomZombiePrefab, randomPoint, Quaternion.identity);

            // Setup the zombie (possibly scaling with the game round)
            SetupZombie(newZombie, gameManager.gameInstance.GetGameRound());
        }
    }
    void SpawnAtRandomPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        // Select a random spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
       

        // Get a random point on the NavMesh near the selected spawn point
        Vector3 randomPoint = GetRandomPointOnNavMesh(randomSpawnPoint.position, spawnRadius);

        if (randomPoint != Vector3.zero)
        {
            // Determine whether to spawn a melee or ranged zombie based on the 80/20 split
            GameObject randomZombiePrefab;
            float chance = Random.Range(0f, 1f);

            if (chance <= 0.8f)
            {
                // 80% chance: Select a random melee zombie from the list
                randomZombiePrefab = MeeleZombies[Random.Range(0, MeeleZombies.Count)];
            }
            else
            {
                // 20% chance: Select a random ranged zombie from the list
                randomZombiePrefab = RangedZombies[Random.Range(0, RangedZombies.Count)];
            }

            // Instantiate the selected zombie at the random point
            GameObject newZombie = Instantiate(randomZombiePrefab, randomPoint, Quaternion.identity);

            // Set up the zombie's stats
            SetupZombie(newZombie, gameManager.gameInstance.GetGameRound());
        }
    }


   public void SpawnSpecficAtRandomPoint(GameObject ZombiePrfab, int roomlevel)
    {
        if (spawnPoints.Count == 0)
        {
          
            return;
        }

        // Select a random spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];


        // Get a random point on the NavMesh near the selected spawn point
        Vector3 randomPoint = GetRandomPointOnNavMesh(randomSpawnPoint.position, spawnRadius);

        if (randomPoint != Vector3.zero)
        {
            GameObject randomZombiePrefab;
            float chance = Random.Range(0f, 1f);

            randomZombiePrefab = MeeleZombies[Random.Range(0, MeeleZombies.Count)];
            
         
            // Instantiate the selected zombie at the random point
            GameObject newZombie = Instantiate(ZombiePrfab, randomPoint, Quaternion.identity);

            // Set up the zombie's stats
            SetupZombie(newZombie, roomlevel);
        }
    }



    public void SpawnSpecialAtRandomPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        // Select a random spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Get a random point on the NavMesh near the selected spawn point
        Vector3 randomPoint = GetRandomPointOnNavMesh(randomSpawnPoint.position, spawnRadius);

        if (randomPoint != Vector3.zero)
        {
            // Determine whether to spawn a melee or ranged zombie based on the 80/20 split
            GameObject randomZombiePrefab;
           

             randomZombiePrefab = SpecialZombies[Random.Range(0, SpecialZombies.Count)];

            // Instantiate the selected zombie at the random point
            GameObject newZombie = Instantiate(randomZombiePrefab, randomPoint, Quaternion.identity);

            // Set up the zombie's stats
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