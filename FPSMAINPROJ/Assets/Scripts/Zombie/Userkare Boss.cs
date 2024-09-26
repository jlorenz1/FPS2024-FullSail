using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Userkare : EnemyAI
{
    bool canattack;
    float AttackRange;
    bool inAbilityRange;
    [SerializeField] Transform launchPoint;
    [SerializeField] GameObject ProjectilePrefab;
    [SerializeField] Zombiemeeleattacks MeleeWeapon;
    [SerializeField] GameObject Melee;
    bool AttackDone;
    [SerializeField] EnemySheilds sheilds;
    [SerializeField] GameObject Sheild;
    [SerializeField] Transform SheildPosition;
    [SerializeField] float sheildHealth; 


    [SerializeField] float BuffRange;
    float PushBackRadius;
    bool UnCapped;
    bool AddativeAP;
    float AblityCoolDown = 5;
    float nextAbilityTime = 8;
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
    bool IsSpecialAttacking;
    public bool SheildActive;

    bool nextbuff;
    
    [SerializeField] int castAmount;
    int DefCastAmount;
    [SerializeField] float ProjectileSpeed;
    float DefProjectSpeed;
    [SerializeField] float ProjectileLifeTime;
    float DefLifeTime;
    [SerializeField] float ProjectileDamage;
    float DefDamage;
    [SerializeField] float ProjectileFollowTime;
    float DefFollowTime;
    [SerializeField] ProjectileType Type;
    ProjectileType DefType;
    [SerializeField] ProjectileAblity projectileAblity;
    ProjectileAblity DefAblility;
    [SerializeField] float AbilityStrength;
    float DefStrength;
    [SerializeField] float AbilityDuration;
    float DefDuration;
    [SerializeField] float effectDuration;
    [SerializeField] float AoeStrength;
    [SerializeField] float radius;
    [SerializeField] AOETYPE type;
    [SerializeField] AOEDamage Heal;
    bool isStun;
    ProjectileType placeHolder;
    ProjectileAblity placeHolderAbility;
    Caster caster;
    GameObject sheild;

   [SerializeField] Color BulletColor;
    [SerializeField] Material BulletMaterial;
    float LazerSpeed;


    [Header("Audio")]
    [SerializeField]public AudioClip Spawn;
    [SerializeField]public AudioClip stun;
    [SerializeField]public AudioClip burn;
    [SerializeField]public AudioClip DuoCall;
    [SerializeField]public AudioClip[] partnerDeath;
    [SerializeField] public AudioClip SummonVoiceLine;
    [SerializeField] public AudioClip SheildOrHealVoiceLine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnVoiceLine());
        mSekhmet = gameManager.gameInstance.SekhMet;
        UnCapped = false;
        BaseMaxHealth = MaxHealth;
        BaseAttackSpeed = attackSpeed;
        player = gameManager.gameInstance.playerScript;
        playerRb = player.GetComponent<Rigidbody>();  // Get the player's Rigidbody
        PlayerStartHP = gameManager.gameInstance.playerScript.GetHealth();
        damage = 5;
        canattack = true;
        gameManager.gameInstance.SpawnUserkare();
        nextbuff = true;

        caster = Caster.Userkare;
        DefCastAmount = castAmount;
        DefAblility = projectileAblity;
        DefDamage = ProjectileDamage;
        DefDuration = AbilityDuration;
        DefStrength = AbilityStrength;
        DefType = Type;
        DefProjectSpeed = ProjectileSpeed;
        DefFollowTime = ProjectileFollowTime;
        DefLifeTime = ProjectileLifeTime;
        SheildActive = false;
        isStun = false;

         IsSpecialAttacking = false;
        AlwaysSeePlayer = true;
        ressitKnockBack = true;

        sheild = Instantiate(Sheild, SheildPosition.position, Quaternion.identity);
        sheild.transform.SetParent(transform);


    }

    // Update is called once per frame
    protected override void Update()
    {
      
        base.Update();


        agent.SetDestination(gameManager.gameInstance.player.transform.position);



        if (Time.time >= nextAbilityTime && canattack)
        {
            UseSpecialAbility();

            nextAbilityTime = Time.time + AblityCoolDown;

            canattack = false;
        }



        if (PlayerinAttackRange && canattack)
        {

            animator.SetTrigger("Shoot");

            canattack = false;
        }



        if (sheild != null && sheild.activeInHierarchy == true)
        {
            float SheildHealth = sheild.GetComponent<EnemySheilds>().GetHitPoints();

            if (SheildHealth <= 0)
            {
                Destroy(sheild);
            }
        }
        else
            return;

        if (MaxHealth > 1000)
        {
            MaxHealth = 1000;
        }

  

      

       

      


   

       
    }
  

    IEnumerator SpawnVoiceLine()
    {
        canattack = false;
        yield return new WaitForSeconds(2);

        PlayVoice(Spawn);

        canattack = true;
    }



    protected override void Die()
    {
        gameManager.gameInstance.BossKilled();
        gameManager.gameInstance.UserkareDead();
        DieWithoutDrops();

    }


    void UseSpecialAbility()
    {
        IsSpecialAttacking = true;
        canattack = false;  // Disable normal attacks while using special ability

        int chance = UnityEngine.Random.Range(1, 3);  // Randomly select an ability (1-4)

        switch (chance)
        {
            case 1:
                animator.SetTrigger("Stun");
                break;

            case 2:
                BurstSHot();
                break;

            case 3:
                if (!sheild.activeInHierarchy)
                {
                    animator.SetTrigger("Sheild");
                }
                else
                    animator.SetTrigger("Heal");
                break;

         
        }
    }
    //Attacks 
    public void TrappingLight()
    {
        PlayVoice(stun);
        CastAttack(ProjectileAblity.Stun);
    }


    void BurstSHot()
    {
        animator.SetTrigger("BurstShot");
    }


    public void LightGautling()
    {
        StartCoroutine(RapidFire());
       
    }
    IEnumerator RapidFire()
    {

        for (int i = 0; i < castAmount; i++)
        {
            CastAttack(ProjectileAblity.Normal);
            yield return new WaitForSeconds(0.5f);
        }

        animator.SetTrigger("Finsih Attack");
        canattack = true;
    }
  public  void PerfectDefence()
    {
        if (sheild == null)
        {
            sheild = Instantiate(Sheild, SheildPosition.position, Quaternion.identity);
            sheild.transform.SetParent(transform);
            sheilds.SetHitPoints(sheildHealth);

            PlayVoice(SheildOrHealVoiceLine);
        }
        else return;
     
    }

    public void EnableAttack()
    {
        canattack = true;
    }



    public void CreateHeal()
    {
        // Instantiate the HealArea and place it at the launchPoint position
        GameObject HealArea = Instantiate(Heal.gameObject, launchPoint.position, Quaternion.identity);

        // Get the AOEDamage component from the instantiated object
        AOEDamage aoeDamage = HealArea.GetComponent<AOEDamage>();

        if (aoeDamage != null)
        {
            aoeDamage.expand(AoeStrength, BuffRange); // Call expand on the AOEDamage script
        }
    }




    public void CastBasic()
    {
        CastAttack(ProjectileAblity.Burn);
    }

    public void CastAttack(ProjectileAblity ABILITY)
    {
            GameObject projectile = Instantiate(ProjectilePrefab, launchPoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript == null)
            {
                projectileScript = projectile.AddComponent<Projectile>();
            }
            if (projectileScript != null)
            {
                projectileScript.SetStats(ProjectileSpeed, ProjectileLifeTime, ProjectileDamage, ProjectileFollowTime, Type, ABILITY, AbilityStrength, 1f, caster);

                projectileScript.SetColor(BulletColor, BulletMaterial);
            }
      
    }




}
