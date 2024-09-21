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

    [SerializeField] EnemySheilds sheilds;
    [SerializeField] GameObject Sheild;
    [SerializeField] Transform SheildPosition;
    [SerializeField] float sheildHealth; 

    IEnemyDamage FellowZombie;
    IEnemyDamage Partner;
    [SerializeField] GameObject SummoningMob;
    [SerializeField] GameObject summonpartner;
    [SerializeField] int SummonAmount;
    GameObject[] zombies;
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
    bool AttackDone;
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
    bool isStun;
    ProjectileType placeHolder;
    ProjectileAblity placeHolderAbility;
    Caster caster;


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
    }

    // Update is called once per frame
    protected override void Update()
    {
      
        base.Update();

        agent.SetDestination(gameManager.gameInstance.player.transform.position);

        if (PlayerinAttackRange && canattack &&  !IsSpecialAttacking)
        {
      
            StartCoroutine(CastAttackRoutine());
        }

        Summons = GameObject.FindGameObjectsWithTag("Summon");

        zombies = GameObject.FindGameObjectsWithTag("Zombie");

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


        if (gameManager.gameInstance.LightGautlening == true)
        {
            LightGautling();
            gameManager.gameInstance.LightGautlening = false;
        }

        if (gameManager.gameInstance.UserkareIsUncaped && nextbuff)
        {
            StartCoroutine(SetUncaped());
        }

    }
    IEnumerator RampingAbilites()
    {
        AddativeAP = false;
        damage++;
        attackSpeed += 0.01f;
        yield return new WaitForSeconds(4);
        AddativeAP = true;
    }

    IEnumerator SpawnVoiceLine()
    {

        yield return new WaitForSeconds(2);

        PlayVoice(Spawn);
    }


    IEnumerator SetUncaped()
    {
        nextbuff = false;

        Armor = 0;
        MaxHealth *= 2;
        CurrentHealth *= 1;

        yield return new WaitForSeconds(4);

        nextbuff = true;

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

        int chance = UnityEngine.Random.Range(1, 4);  // Randomly select an ability (1-4)

        switch (chance)
        {
            case 1:
                if (!SheildActive)
                {
                    PerfectDefence();
                }
                else
                    Heal();

                break;
            case 2:
                Heal();
                break;
            case 3:
                animator.SetTrigger("Stun");
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
       AbilitySetTo(ProjectileAblity.Stun);
        isStun = true;
        StartCoroutine(CastStunRoutine());
        PlayVoice(stun);

        if (gameManager.gameInstance.isSekhmetDead == false)
        {
            PLAYDUOCALL();
            gameManager.gameInstance.BlinkingJab = true;
        }

        IsSpecialAttacking = false;
    }

    IEnumerator CastAttackRoutine()
    {
        canattack = false;
        for (int i = 0; i < castAmount; i++)
        {
            GameObject projectile = Instantiate(ProjectilePrefab, launchPoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript == null)
            {
                projectileScript = projectile.AddComponent<Projectile>();
            }
            if (projectileScript != null)
            {
                projectileScript.SetStats(ProjectileSpeed, ProjectileLifeTime, ProjectileDamage, ProjectileFollowTime, Type, projectileAblity, AbilityStrength, 1f, caster);
              
                projectileScript.SetColor(BulletColor, BulletMaterial);
                if (Type == ProjectileType.AOE)
                {
                    projectileScript.AoeStats(effectDuration, AoeStrength, radius, type);
                }
                else
                    projectileScript.AoeStats(0, 0, 0, AOETYPE.Damage);
            }


            yield return new WaitForSeconds(1 / castSpeed);
        }
        yield return new WaitForSeconds(1);
        AttackDone = true;

        if (!isStun)
            animator.SetTrigger("Shoot");
        else
            isStun = false;
        canattack = true;
        //ResetToDefault();
    }


    IEnumerator CastStunRoutine()
    {
        canattack = false;
      
            GameObject projectile = Instantiate(ProjectilePrefab, launchPoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript == null)
            {
                projectileScript = projectile.AddComponent<Projectile>();
            }
            if (projectileScript != null)
            {
                projectileScript.SetStats(ProjectileSpeed, ProjectileLifeTime, ProjectileDamage, ProjectileFollowTime, Type, ProjectileAblity.Stun, AbilityStrength, 1f, caster);

                projectileScript.SetColor(BulletColor, BulletMaterial);
            
          
            yield return new WaitForSeconds(1 / castSpeed);
        }
        yield return new WaitForSeconds(1);
        AttackDone = true;
    }




    public void CastAttack()
    {
        // Ranged attack logic
        
        AttackDone = false;
        StartCoroutine(CastAttackRoutine());

    }

    private void ResetToDefault()
    {
        castAmount = DefCastAmount;
        projectileAblity = DefAblility;
        ProjectileDamage = DefDamage;
        AbilityDuration = DefDuration;
        AbilityStrength = DefStrength;
        Type = DefType;
        ProjectileSpeed = DefProjectSpeed;
        ProjectileFollowTime = DefFollowTime;
        ProjectileLifeTime = DefLifeTime;





    }

    void PerfectDefence()
    {
        // refelcts all ranged damage
        if (!SheildActive)
        {
            GameObject sheild = Instantiate(Sheild, SheildPosition.position, Quaternion.identity);
            EnemySheilds sheildScript = sheild.GetComponent<EnemySheilds>();


            sheild.transform.SetParent(transform);

            if (sheildScript == null)
            {
                sheildScript = sheild.AddComponent<EnemySheilds>();
            }
            sheildScript.SetHitPoints(sheildHealth);

            PlayVoice(SheildOrHealVoiceLine);
            SheildActive = true;
        }

         IsSpecialAttacking = false;
    }

    

    public void LightGautling()
    {
        // fires buffed base attacks at the player at 5 times the base speed 
        castAmount = 5;
        ProjectileSpeed *= 5;
        ProjectileDamage *= 1.5f;
        Type = ProjectileType.Lazer;
        StartCoroutine(CastAttackRoutine());
        IsSpecialAttacking = false;
    }




    public void Summon()
    {
        // summons weaker version of the base mummy that will increase his health and try to revevie Sekhemt

     

        PlayVoice(SummonVoiceLine);

        float radiusStep = 2f; // Distance between sets of 4 minions
        float radius = 1f; // Initial spawn radius
        int minionsPerCircle = 4;

        for (int i = 0; i < SummonAmount; i++)
        {
            // Calculate the angle in radians for more precision
            float angle = (i % minionsPerCircle) * (360f / minionsPerCircle) * Mathf.Deg2Rad;

            // Set the spawn position using cosine and sine
            Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            // Instantiate the minion
            Instantiate(SummoningMob, spawnPosition, Quaternion.identity);

            // Increase the radius after spawning every 4 minions
            if ((i + 1) % minionsPerCircle == 0)
            {
                radius += radiusStep;
            }
        }
        IsSpecialAttacking = false;
    }

    public void Heal()
    {
        // heals all allies up to 70% of thier Hp


        foreach (GameObject zombie in zombies)
        {
            float distance = Vector3.Distance(transform.position, zombie.transform.position);
            if (distance < BuffRange)
            {

                IEnemyDamage FellowZombie = zombie.GetComponent<IEnemyDamage>();
                if (FellowZombie != null)
                { 
                   float Healing = FellowZombie.GetMaxHP() * 0.2f;

                        FellowZombie.AddHP(Healing);
                    AddHP(5);
                }
            }
        }
        PlayVoice(SheildOrHealVoiceLine);
    }





    public void PLAYDUOCALL()
    {
        PlayVoice(DuoCall);
    }




    void RespawnSekmet()
    {

        Instantiate(summonpartner, gameManager.gameInstance.SekhmetRespawn);
        UnCapped = false;
        ResetStats();


    }
    private void ResetStats()
    {
        MaxHealth = BaseMaxHealth;
        attackSpeed = BaseAttackSpeed;

    }

   ProjectileType typeSetTo(ProjectileType newType)
    {

        placeHolder = newType;


        return placeHolder;
    }



    ProjectileAblity AbilitySetTo(ProjectileAblity newType)
    {

        placeHolderAbility = newType;


        return placeHolderAbility;
    }

}
