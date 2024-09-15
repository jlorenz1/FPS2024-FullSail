using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Userkare : EnemyAI
{
    bool canattack;
    float AttackRange;
    bool inAbilityRange;
    [SerializeField] Transform launchPoint;
    [SerializeField] GameObject ProjectilePrefab;
    [SerializeField] GameObject rootProjectilePrefab;
    [SerializeField] GameObject buffedProjectilePrefab;
    [SerializeField] Zombiemeeleattacks MeleeWeapon;
    [SerializeField] GameObject Melee;
    IEnemyDamage FellowZombie;
    IEnemyDamage Partner;
    [SerializeField] GameObject summon;
    [SerializeField] GameObject summonpartner;
    float PushBackRadius;
    bool UnCapped;
    bool AddativeAP;
    float AblityCoolDown = 3;
    float nextAbilityTime = 1 ;
    float attackSpeed;
    int SummonCount;
    GameObject[] Summons;
    PlayerController player;
    IDamage playerDamage;
    Rigidbody playerRb;
    GameObject mSekhmet;
    float BaseMaxHealth;
    float BaseAttackSpeed;

    float PlayerStartHP;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
       
        mSekhmet = gameManager.gameInstance.SekhMet;
        UnCapped = false;
        BaseMaxHealth = MaxHealth;
        BaseAttackSpeed = attackSpeed;
        player = gameManager.gameInstance.playerScript;
        playerRb = player.GetComponent<Rigidbody>();  // Get the player's Rigidbody
        PlayerStartHP = gameManager.gameInstance.playerScript.GetHealth();
        damage = 5;
        canattack = true;
        gameManager.gameInstance.isUserKareDead = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(PlayerinAttackRange && canattack)
        {
            StartCoroutine(CastAttackRoutine());
        }

        Summons = GameObject.FindGameObjectsWithTag("Summon");

        SummonCount = Summons.Length;

        if (SummonCount > 30)
        {
            RespawnSekmet();
        }
      

        if (Time.time >= nextAbilityTime)
        {
            UseSpecialAbility();

            nextAbilityTime = Time.time + AblityCoolDown;
        }


        if (UnCapped && AddativeAP)
        {
            damage = PlayerStartHP - (PlayerStartHP * 0.1f);
            StartCoroutine(RampingAbilites());
        }
    }
  IEnumerator  RampingAbilites()
    {
        AddativeAP = false;
                damage++;
        attackSpeed += 0.01f;
        yield return new WaitForSeconds(4);
        AddativeAP = true;
    }

    public  bool SetUncaped(bool capped)
    {
        Armor = 0;
        MaxHealth *= 4;
        CurrentHealth *= 4;
        return capped;

    }


    protected override void Die()
    {
        gameManager.gameInstance.UserkareDead();
        base.Die();
    }


    void UseSpecialAbility()
    {

        canattack = false;  // Disable normal attacks while using special ability

        int chance = UnityEngine.Random.Range(1, 4);  // Randomly select an ability (1-4)

        switch (chance)
        {
            case 1:
                repulse();
                break;
            case 2:
                Heal();
                break;
            case 3:
                StartCoroutine(CastStunRoutine());
                break;
            case 4:
                if (UnCapped)
                {
                    Summon();
                }
                else
                    Heal();
                break;
        }

        canattack = true;


    }

   public void TrappingLight()
    {
        //roots the player in place 
        Debug.Log("casted stun");
        StartCoroutine(CastStunRoutine());

    }

    IEnumerator CastAttackRoutine()
    {
        canattack = false;
        animator.SetFloat("AttackSpeed", AttackSpeed); // New: Set animator speed to match cast speed
        animator.SetTrigger("Base");
        yield return new WaitForSeconds(1f / AttackSpeed);
        canattack = true;

    }

    public void CastAttack()
    {
        // Ranged attack logic
        Debug.Log("Ranged attack");
        Instantiate(ProjectilePrefab, launchPoint.position, Quaternion.identity);

    }


    IEnumerator CastStunRoutine()
    {

        animator.SetFloat("AttackSpeed", AttackSpeed); // New: Set animator speed to match cast speed
        animator.SetTrigger("Stun");
        yield return new WaitForSeconds(1f / AttackSpeed);


    }


    public void CastStun()
    {

        Instantiate(rootProjectilePrefab, launchPoint.position, Quaternion.identity);

    }

    void PerfectDefence()
    {
       // refelcts all ranged damage


    }

    public void LightGautling()
    {
        // fires buffed base attacks at the player at 5 times the base speed 
        canattack = false;
        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LightG());
        }
        canattack = true;
    }

    IEnumerator LightG()
    {
        canattack = false;
        Debug.Log("Buffed Attack");
        animator.SetFloat("AttackSpeed", (5)); // New: Set animator speed to match cast speed
        animator.SetBool("Buff 0", true);
        animator.SetTrigger("Buffed");
        yield return new WaitForSeconds(1f / 5);
        animator.SetBool("Buff 0", false);
        canattack = true;
    }

  public  void CastBuffSpell()
    {
        Instantiate(buffedProjectilePrefab, launchPoint.position, Quaternion.identity);
    }
    public void Summon()
    {
        // summons weaker version of the base mummy that will increase his health and try to revevie Sekhemt



    }

    public void Heal()
    {
       // heals all allies up to 70% of thier Hp









    }

    public void repulse()
    {
        Debug.Log("repulse called");
        //Knocks the player back if they are too close 
        int strength;
        
        if (!UnCapped)
        {
            strength = 5;
        }
        else
        {
            strength = 7;
            PushBackRadius *= 1.5f;
            
        }

          
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);  // Calculate distance to the player

                // Check if the player is within the pushback radius
                if (distance < PushBackRadius)
                {
            
                  
                    if (playerRb != null)
                    {
                        Vector3 knockBackDirection = (player.transform.position - transform.position).normalized;  // Calculate direction
                        playerRb.AddForce(knockBackDirection * strength, ForceMode.Impulse);  // Apply force to player
                    }

                player.takeDamage(damage);
                }

                }
            }



        


   void RespawnSekmet() {

        Instantiate(summonpartner, gameManager.gameInstance.SekhmetRespawn);
        UnCapped = false;
        ResetStats();


    }
    private void ResetStats()
    {
        MaxHealth = BaseMaxHealth;
        attackSpeed = BaseAttackSpeed;
        
    }
}
