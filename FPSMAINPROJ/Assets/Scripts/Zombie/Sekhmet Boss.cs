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
    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Berserk = false;
        AddativeDamage = true;
        mUserkare = GameObject.FindGameObjectWithTag("Userkare");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base .Update();



        if (Time.time >= nextAbilityTime)
        {
            UseSpecialAbility();
          
            nextAbilityTime = Time.time + AblityCoolDown;
        }


        if (Berserk && AddativeDamage)
        {
            StartCoroutine(RampingDamage());
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


    }

   public void BlinkingJab()
    {
        // teleports infront of the player and attacks them for 20% of their hp;



    }

    void liquidPrison()
    {
        // Ranged Special attack slows the player 



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





    }

    void UseSpecialAbility() {

     


    }

        



}
