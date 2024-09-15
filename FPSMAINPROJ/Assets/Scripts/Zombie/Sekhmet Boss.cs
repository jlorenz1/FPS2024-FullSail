using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SekhmetBoss : EnemyAI
{
    bool Berserk;
    bool AddativeDamage;
    float AblityCoolDown = 30;
    float nextAbilityTime;
    GameObject mUserkare;
    float PlayerStartHP;
    bool canattack;
    float AttackRange;
    bool inAbilityRange;
    [SerializeField] Transform launchPoint;
    [SerializeField] GameObject ProjectilePrefab;
    [SerializeField] Zombiemeeleattacks MeleeWeapon;
    [SerializeField] GameObject Melee;
   
    IEnemyDamage Partner;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canattack = true;
        Berserk = false;
        AddativeDamage = true;
        mUserkare = gameManager.gameInstance.UserKare;
        AttackRange = 30;
        animator.SetFloat("AttackSpeed", AttackSpeed);
        gameManager.gameInstance.SpawnSekhmet();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

       

        if (PlayerinAttackRange && canattack)
        {
            Debug.Log("Did Base Attack");
            StartCoroutine(BaseAAttack());
        }


        if (Time.time >= nextAbilityTime)
        {
            UseSpecialAbility();
          
            nextAbilityTime = Time.time + AblityCoolDown;
        }


        if (Berserk && AddativeDamage)
        {
            StartCoroutine(RampingDamage());
        }

        if (gameManager.gameInstance.BlinkingJab == true)
        {
            BlinkingJab();
            gameManager.gameInstance.BlinkingJab = false;
        }
      

    }

    IEnumerator RampingDamage()
    {
        AddativeDamage = false;
        damage += 2;
        yield return new WaitForSeconds(4);
        AddativeDamage = true;
    }

    public bool GoBerserk(bool beserk)
    {
        Armor = 450;
        MaxHealth *= 2;
        CurrentHealth *= 2;

        return beserk;
    }

    protected override void Die()
    {
        base.Die();
        gameManager.gameInstance.SekhmetDead();
        gameManager.gameInstance.SekhmetDeathLocation(agent.transform);
       
    }

    void BleedingJab()
    {
        // Start the Bleeding Jab coroutine to handle the delay properly
        StartCoroutine(BleedingJabRoutine());
    }

    IEnumerator BleedingJabRoutine()
    {
        
        MeleeWeapon.SetBleed(); // Start bleeding effect
        animator.SetTrigger("Quick jab");

        // Wait for 3 seconds before stopping the bleeding effect
        yield return new WaitForSeconds(3f);

        MeleeWeapon.SetBleed(); 
    }


    public void BlinkingJab()
    {
        // teleports infront of the player and attacks them for 20% of their hp;

        Vector3 PlayerPosition = gameManager.gameInstance.player.transform.position;
        Vector3 forwardDirection = gameManager.gameInstance.player.transform.forward;

        Vector3 teleportPosition = PlayerPosition + forwardDirection * 3;

        transform.position = teleportPosition;

        animator.SetTrigger("Quick jab");

        

    }

    

    IEnumerator CastAttackRoutine()
    {
   
        animator.SetFloat("AttackSpeed", AttackSpeed); // New: Set animator speed to match cast speed
        animator.SetTrigger("Shoot");
        StartCoroutine(DisableWeapon());
        yield return new WaitForSeconds(1f / AttackSpeed);

      
    }

    public void CastAttack()
    {
        // Ranged attack logic
        Debug.Log("Ranged attack");
        Instantiate(ProjectilePrefab, launchPoint.position, Quaternion.identity);
        
    }

    IEnumerator DisableWeapon()
    {
        canattack = false;
        Melee.SetActive(false);
        yield return new WaitForSeconds(5);
        Melee.SetActive(true);
        canattack = true;
    }

    void reinforce()
    {
        Debug.Log("Reinforce Called");

        // Find all objects with the "Zombie" tag
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        foreach (GameObject zombie in zombies)
        {
            if (zombie != null)
            {
                // Get the IEnemyDamage component
                IEnemyDamage FellowZombie = zombie.GetComponent<IEnemyDamage>();

                if (FellowZombie != null) // Ensure the component exists
                {
                    if (!Berserk)
                    {
                        // Increases Her armor and Uskares armor by other enemies by 2
                        FellowZombie.AddArmor(2f);  // Add armor to other zombies
                        Armor += 5;                 // Increase self armor
                    }
                    else if (Berserk)
                    {
                        Armor += 10f;  // Increase more armor if Berserk
                    }
                }
                else
                {
                    Debug.LogWarning("Zombie does not have IEnemyDamage component.");
                }
            }
        }
    }


    void vacume()
    {
        //pulls the player closer in 

        if (inAbilityRange)
        {
            Vector3 newPosition = agent.transform.position + (agent.transform.forward * (AttackRange / 2));

            // Update player's position
            gameManager.gameInstance.playerScript.transform.position = newPosition;
        }



    }

    void UseSpecialAbility()
    {
       canattack = false;  // Disable normal attacks while using special ability

        int chance = UnityEngine.Random.Range(1, 5);  // Randomly select an ability (1-4)

        switch (chance)
        {
            case 1:
                vacume();
                break;
            case 2:
                reinforce();
                break;
            case 3:
                StartCoroutine(CastAttackRoutine());
                break;
            case 4:
                BleedingJab();
                break;
        }

      canattack = true;
    }

    IEnumerator BaseAAttack()
    {
        canattack = false;

        Debug.Log("slow jab");

        //animator.SetFloat("Speed", 0);
       
        animator.SetTrigger("Slow jab");

        yield return new WaitForSeconds(1/AttackSpeed);

        canattack = true;

    }



}
