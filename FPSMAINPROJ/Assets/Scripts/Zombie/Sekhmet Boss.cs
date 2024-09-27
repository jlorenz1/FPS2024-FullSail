using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SekhmetBoss : EnemyAI
{
    bool Berserk;
    bool AddativeDamage;
    float AblityCoolDown = 10;
    float nextAbilityTime;
    GameObject mUserkare;
    float PlayerStartHP;
    bool canattack;
    float AttackRange;
    bool inAbilityRange;
    bool nextbuff;
    [SerializeField] Transform launchPoint;
    [SerializeField] Transform meleeTip;
    [SerializeField] GameObject ProjectilePrefab;
    [SerializeField] Zombiemeeleattacks MeleeWeapon;
    [SerializeField] GameObject Melee;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool isBoss;

    [Header("Audio")]
    
    [SerializeField] public AudioClip burn;
    [SerializeField] public AudioClip[] partnerDeath;
    public Transform rightHandTarget; 
    
    public float weight = 1.0f;

    bool playedClip;

    IEnemyDamage Partner;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canattack = true;
        Berserk = false;
        AddativeDamage = true;
        mUserkare = gameManager.gameInstance.UserKare;
        AttackRange = 2;
        animator.SetFloat("AttackSpeed", AttackSpeed);
        gameManager.gameInstance.SpawnSekhmet();
        nextbuff = true;
        MeleeWeapon.SetDamage(damage);


        playedClip = false;

        AlwaysSeePlayer = true;

        ressitKnockBack = true;

        startSpeed = agent.speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        if (rightHandTarget != null)
        {
          
            Vector3 directionToPlayer = gameManager.gameInstance.player.transform.position - meleeTip.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

           
            Melee.transform.rotation = Quaternion.Slerp(meleeTip.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (gameManager.gameInstance.SekhmetisBerserk && nextbuff)
        {
           StartCoroutine( GoBerserk());
        }

        if (PlayerinAttackRange && canattack)
        {

            StartCoroutine(BaseAAttack());
        }


        if (Time.time >= nextAbilityTime && canattack)
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

    IEnumerator GoBerserk( )
    {
        nextbuff = false;
        Armor = 450;
        MaxHealth *= 1;
        CurrentHealth *= 1;
        yield return new WaitForSeconds(4);


            nextbuff = true;
    }

    protected override void Die()
    {

        if (isBoss == true)
        {
            gameManager.gameInstance.BossKilled();
        }
      
        DieWithoutDrops();

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
        yield return new WaitForSeconds(2f);

        animator.SetBool("isMoving", true);

        // Wait for 3 seconds before stopping the bleeding effect
        yield return new WaitForSeconds(10f);

        MeleeWeapon.SetBleed(); 
    }


    public void BlinkingJab()
    {
        // teleports infront of the player and attacks them for 20% of their hp;
        canattack = false;

        animator.SetTrigger("JumpBack");


        StartCoroutine(BlinkJab());
     

    }

    IEnumerator BlinkJab()
    {

    

        Vector3 PlayerPosition = gameManager.gameInstance.player.transform.position;
        Vector3 forwardDirection = gameManager.gameInstance.player.transform.forward;

        Vector3 teleportPosition = PlayerPosition + forwardDirection * 3;

        transform.position = teleportPosition;


        animator.SetTrigger("Blink");

        yield return new WaitForSeconds(2);

        canattack = true;

        animator.SetBool("isMoving", true);
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
                  
                }
            }
        }
    }



   


    void UseSpecialAbility()
    {
       canattack = false;  // Disable normal attacks while using special ability

        int chance = UnityEngine.Random.Range(1, 5);  // Randomly select an ability (1-4)

        switch (chance)
        {
            case 1:
               // vacume();
                break;
            case 2:
                reinforce();
                break;
            case 3:
                if (PlayerinAttackRange)
                {
                    BleedingJab();
                }
                else
                {
                    BlinkingJab();
                }
                break;
        }

      canattack = true;
    }

    public void AttackDone()
    {
        canattack = true;
    }

    IEnumerator BaseAAttack()
    {
        canattack = false;
       
        animator.SetTrigger("Slow jab");

        yield return new WaitForSeconds(2.14f);

        animator.SetBool("isMoving", true);
     

        yield return new WaitForSeconds(2.86f);

        canattack = true;
    }

    public void StopMoving()
    {
        animator.SetBool("isMoving", false);

    }


    public void ToggelCollider()
    {
        MeleeWeapon.ToggleColider();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Set the position and rotation of the right hand to the target
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

            // Control the weight of the IK
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
        }
    }

 
}
