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
    float AttackSpeed;
    bool canattack;
    float AttackRange;
    bool inAbilityRange;
    [SerializeField] Transform launchPoint;
    [SerializeField] GameObject ProjectilePrefab;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canattack = true;
        Berserk = false;
        AddativeDamage = true;
        mUserkare = GameObject.FindGameObjectWithTag("Userkare");
        AttackRange = 30;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base .Update();

        animator.SetFloat("AttackSpeed", AttackSpeed);

        if (Time.time >= nextAbilityTime)
        {
            UseSpecialAbility();
          
            nextAbilityTime = Time.time + AblityCoolDown;
        }


        if (Berserk && AddativeDamage)
        {
            StartCoroutine(RampingDamage());
        }

        if (PlayerinAttackRange)
        {
            StartCoroutine(BaseAAttack());
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
        gameManager.gameInstance.SekhmetDead();
        gameManager.gameInstance.SekhmetDeathLocation(agent.transform);
        base.Die();
    }

    void BleedingJab()
    {
        // mid ranged melee special attack does base damage then 2% damage over time to player
        animator.SetTrigger("Quick jab");
        // Define the range and sphere radius
        float jabRange = 5f;         // Adjust the range of the jab
        float sphereRadius = 1f;     // Adjust the radius of your sphere

        // Calculate the position of the sphere's tip
        Vector3 sphereTipPosition = transform.position + transform.forward * sphereRadius;

        RaycastHit hit;

        // Cast a ray from the tip of the sphere forward
        if (Physics.Raycast(sphereTipPosition, transform.forward, out hit, jabRange))
        {
            // Check if the ray hit the player
            if (hit.collider.CompareTag("Player"))
            {
                // Apply damage to the player
                IDamage player = hit.collider.GetComponent<IDamage>();
                if (player != null)
                {
                    player.takeDamage(5);
                    StartCoroutine(ApplyBleed(player, 5)); // Apply bleeding effect for 5 seconds
                }
            }
        }
    }
    IEnumerator ApplyBleed(IDamage player, float duration)
    {
        float bleedTick = 0.5f; // How often to apply bleed damage (every 0.5 seconds)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            player.takeDamage(player.GetHealth() * 0.02f); // Apply 2% damage
            elapsed += bleedTick;
            yield return new WaitForSeconds(bleedTick);
        }
    }

    public void BlinkingJab()
    {
        // teleports infront of the player and attacks them for 20% of their hp;

        Vector3 PlayerPosition = gameManager.gameInstance.player.transform.position;
        Vector3 forwardDirection = gameManager.gameInstance.player.transform.forward;

        Vector3 teleportPosition = PlayerPosition + forwardDirection * AttackRange;

        transform.position = teleportPosition;

        animator.SetTrigger("Quick jab");

        

    }

    

    IEnumerator CastAttackRoutine()
    {
   
        animator.SetFloat("AttackSpeed", AttackSpeed); // New: Set animator speed to match cast speed
        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(1f / AttackSpeed);

      
    }

    public void CastAttack()
    {
        // Ranged attack logic
        Debug.Log("Ranged attack");
        Instantiate(ProjectilePrefab, launchPoint.position, Quaternion.identity);
    }



    void reinforce()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        foreach (GameObject zombie in zombies) {

            IEnemyDamage FellowZombie = zombie.GetComponent<IEnemyDamage>();
            IEnemyDamage Partner = mUserkare.GetComponent<IEnemyDamage>();

            if (!Berserk)
            {
                // increases Her armor and Uskares armor by other enemies by 2

                FellowZombie.AddArmor(2);
                Partner.AddArmor(5);
                Armor += 5;
            }

            else if (Berserk)
            {
                Armor += 10;
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
       
        animator.SetTrigger("Slow jab");

        yield return new WaitForSeconds(1/AttackSpeed);

        canattack = true;

    }



}
