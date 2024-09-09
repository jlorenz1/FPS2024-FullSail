using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Userkare : EnemyAI
{

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

    float BaseMaxHealth;
    float BaseAttackSpeed;

    float PlayerStartHP;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        UnCapped = false;
        BaseMaxHealth = MaxHealth;
        BaseAttackSpeed = attackSpeed;
        player = gameManager.gameInstance.playerScript;
        playerRb = player.GetComponent<Rigidbody>();  // Get the player's Rigidbody
        PlayerStartHP = gameManager.gameInstance.playerScript.GetHealth();
        damage = 5;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

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

        repulse();
        
       

    }

    void TrappingLight()
    { 
        //roots the player in place 

    }


    void PerfectDefence()
    {
       // refelcts all ranged damage


    }

    public void LightGautling()
    {
        // fires buffed base attacks at the player at 5 times the base speed 



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
