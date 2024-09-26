using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnLocation;
    [SerializeFeild] int Difficulty;
    [SerializeField] bool isEndless;
    [SerializeField] int roomlevel;
    [SerializeField] bool isSpecific;
    [SerializeField] bool isSet;
    [SerializeField] bool BossSpawner;

    [SerializeField] bool TypeMelee;
    [SerializeField] bool TypeRanged;
    [SerializeField] bool TypeSpecial;


    [SerializeField] int CountMelee;
    [SerializeField] int CountRanged;
    [SerializeField] int CountSpecial;
    [SerializeField] int ZombieSpawnCount;


    GameObject[] PrimaryList;
    [SerializeField] GameObject[] RangedEnemy;
    [SerializeField] GameObject[] MeleeEnemy;
    [SerializeField] GameObject[] SpecialEnemy;
    [SerializeField] GameObject SpecifcEnemy;

    bool isActivated;
   public bool wiped;
  public  bool TriggerEntered;
    int Count;
  public  bool BossIsSpawned;
    int range;
    int chance;
    private void Start()
    {
        TriggerEntered = false;
        wiped = false;
        BossIsSpawned = false;
    }
    private void Update()
    {
        Count = gameManager.gameInstance.GetEnemyCount();
        if (Count == 0 && isActivated && isEndless)
        {
            SpawnEnemies(ZombieSpawnCount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (wiped == false)
            {
                DespawnZombies();
            }

            if (!TriggerEntered)
            {
                gameManager.gameInstance.enemySpawner.RefeshSpawnPoints();

                for (int i = 0; i < SpawnLocation.Length; i++)
                {
                    SpawnLocation[i].SetActive(true);
                }

                gameManager.gameInstance.enemySpawner.PopulateSpawnPoints();

                if (!isSpecific && !isSet)
                    SpawnEnemies(ZombieSpawnCount);
                else if (isSpecific)
                {
                    for (int i = 0; i < ZombieSpawnCount; i++)
                    {
                        GetWhichType();
                        SpawnSpecifc(PrimaryList[chance], roomlevel);
                    }
                }
                else if (isSet)
                {
                    SpawnRandomSet(CountMelee, CountSpecial, CountRanged);
                }

                if (!BossIsSpawned && BossSpawner)
                {
                    gameManager.gameInstance.enemySpawner.SpawnBoss();

                    BossIsSpawned = true;
                }

                TriggerEntered = true;
            }
        }
        else
            return;

    }

    public void DisableSpawn()
    {
        for (int i = 0; i < SpawnLocation.Length; i++)
        {
            SpawnLocation[i].SetActive(false);
        }
    }
    void SpawnEnemies(int Amount)
    {

        gameManager.gameInstance.enemySpawner.ZombieSpawner(Amount);

    }

    public void DespawnZombies()
    {


        
            gameManager.gameInstance.playerRespawned = true;
            StartCoroutine(delay());
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            foreach (GameObject zombie in zombies)
            {
                IEnemyDamage Zombies = zombie.GetComponent<IEnemyDamage>();

                if (Zombies != null)
                {

                    Zombies.DieWithoutDrops();

                }

            }
         


        
    }




    void GetWhichType()
    {
        if (isSpecific)
        {
            if (TypeMelee)
            {
                PrimaryList = MeleeEnemy;
            }

            else if (TypeRanged)
            {
                PrimaryList = RangedEnemy;
            }

            else if (TypeSpecial)
            {
                PrimaryList = SpecialEnemy;
            }
        }

        chance = (int)(Random.Range(0, PrimaryList.Length));
    }
    void SpawnSpecifc(GameObject Zombie, int round)
    {
        gameManager.gameInstance.enemySpawner.SpawnSpecficAtRandomPoint(Zombie, round);
    }


    void SpawnRandomSet(int meelecount, int specialcount, int rangedcount)
    {
        if (meelecount > 0)
        {
            for (int i = 0; i < meelecount; i++)
            {
                SpawnSpecifc(MeleeEnemy[(int)(Random.Range(0, MeleeEnemy.Length))], roomlevel);
            }
        }
        if (specialcount > 0)
        {
            for (int i = 0; i < specialcount; i++)
            {
                SpawnSpecifc(SpecialEnemy[(int)(Random.Range(0, SpecialEnemy.Length))], roomlevel);
            }
        }

        if (rangedcount > 0)
        {
            for (int i = 0; i < rangedcount; i++)
            {

                SpawnSpecifc(RangedEnemy[(int)(Random.Range(0, RangedEnemy.Length))], roomlevel);
            }

        }
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(5);

        gameManager.gameInstance.playerRespawned = false ;
      
       
    }
}
