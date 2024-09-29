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
    [SerializeField] GameObject SheildPrefab;
    [SerializeField] Transform SheildPosition;
    [SerializeField] float sheildHealth;


    [SerializeField] float BuffRange;
    float PushBackRadius;
    bool UnCapped;
    bool AddativeAP;
    float AblityCoolDown = 10;
    float nextAbilityTime = 0;
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

    private bool isForceAttackResetRunning = false;
    [Header("Audio")]
    [SerializeField] public AudioClip Spawn;
    [SerializeField] public AudioClip stun;
    [SerializeField] public AudioClip burn;
    [SerializeField] public AudioClip DuoCall;
    [SerializeField] public AudioClip[] partnerDeath;
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



        sheild = Instantiate(sheilds.Body, SheildPosition.position, Quaternion.identity);
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

           
        }



        if (PlayerinAttackRange && canattack)
        {
            canattack = false;

            animator.SetTrigger("Shoot");

          
        }



        if (sheild != null && sheild.activeInHierarchy == true)
        {
            float SheildHealth = sheild.GetComponent<EnemySheilds>().GetHitPoints();

            if (SheildHealth <= 0)
            {
                Destroy(sheild);
                sheild = null;
            }
        }
        else
            return;

        if (MaxHealth > 1000)
        {
            MaxHealth = 1000;
        }



          if (!canattack && !isForceAttackResetRunning)
        {
            StartCoroutine(ForceAttackReset());
        }









    }


    IEnumerator SpawnVoiceLine()
    {
      

        yield return new WaitForSeconds(2);

        PlayVoice(Spawn);

      
    }



    protected override void Die()
    {
        gameManager.gameInstance.BossKilled();
        gameManager.gameInstance.UserkareDead();
        DieWithoutDrops();

    }


    void UseSpecialAbility()
    {
       
        canattack = false;  

        int chance = UnityEngine.Random.Range(1, 4);  

        switch (chance)
        {
            case 1:
                stunAnimation();
                break;

            case 2:
                BurstSHot();
                break;

            case 3:
                if (sheild == null || !sheild.activeInHierarchy)
                {
                    DefenceAnimation();
                }
                else
                    HealAnimation();
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
    void DefenceAnimation()
    {
        animator.SetTrigger("Sheild");
    }

    void HealAnimation()
    {
        animator.SetTrigger("Heal");
    }


    void stunAnimation()
    {
        animator.SetTrigger("Stun");
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
    public void PerfectDefence()
    {
        if (sheild == null || !sheild.activeInHierarchy)
        {
            if (sheild != null)
            {
                Destroy(sheild);  // Cleanup if the old shield is still there
            }
            sheild = Instantiate(sheilds.Body, SheildPosition.position, Quaternion.identity);
            sheild.transform.SetParent(transform);
            sheilds.SetHitPoints(sheildHealth);
            PlayVoice(SheildOrHealVoiceLine);
        }
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
        EnableAttack();
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


    IEnumerator ForceAttackReset()
    {
        float timer = 0f;
        isForceAttackResetRunning = true;
        while (!canattack)
        {
            yield return new WaitForSeconds(0.5f);  
            timer += 0.5f;  

         
            if (timer >= 5f)
            {
                EnableAttack();
            }
        }
        isForceAttackResetRunning = false;
    }

    public void StartDelay()
    {
        StartCoroutine(ShootDelay());
    }

    IEnumerator ShootDelay()
    {


        yield return new WaitForSeconds(2);

        canattack = true;


    }




}