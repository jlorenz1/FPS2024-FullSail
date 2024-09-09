using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Userkare : EnemyAI
{

    [SerializeField] GameObject summon;
    [SerializeField] GameObject summonpartner;
    bool UnCapped;
    bool AddativeAP;
    float AblityCoolDown = 30;
    float nextAbilityTime;
    float attackSpeed;
    int SummonCount;
    GameObject[] Summons;

    float BaseMaxHealth;
    float BaseAttackSpeed;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        UnCapped = false;
        BaseMaxHealth = MaxHealth;
        BaseAttackSpeed = attackSpeed;
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
        if(UnCapped)
        {

        }
        else
        {

        }

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
       //Knocks the player back if they are too close 
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
