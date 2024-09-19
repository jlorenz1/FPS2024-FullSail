using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    float nextAbilityTime = 1;
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

    Caster caster;


    [SerializeField] Color BulletColor;
    [SerializeField] Material BulletMaterial;
    float LazerSpeed;



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

        isStun = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (PlayerinAttackRange && canattack)
        {
            projectileAblity = ProjectileAblity.Normal;
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

        canattack = false;  // Disable normal attacks while using special ability

        int chance = UnityEngine.Random.Range(1, 4);  // Randomly select an ability (1-4)

        switch (chance)
        {
            case 1:
                PerfectDefence();

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
        projectileAblity = ProjectileAblity.Special;
        isStun = true;
        StartCoroutine(CastAttackRoutine());

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

            projectileScript.SetStats(ProjectileSpeed, ProjectileLifeTime, ProjectileDamage, ProjectileFollowTime, Type, projectileAblity, AbilityStrength, AbilityDuration, caster);
            projectileScript.SetColor(BulletColor, BulletMaterial);
            if (Type == ProjectileType.AOE)
            {
                projectileScript.AoeStats(effectDuration, AoeStrength, radius, type);
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
        ResetToDefault();
    }

    public void CastAttack()
    {
        // Ranged attack logic
        Debug.Log("Ranged attack");
        AttackDone = false;
        StartCoroutine(CastAttackRoutine());

    }


    IEnumerator CastStunRoutine()
    {

        animator.SetFloat("AttackSpeed", AttackSpeed); // New: Set animator speed to match cast speed

        projectileAblity = ProjectileAblity.Special;
        animator.SetTrigger("Stun");
      
        yield return new WaitForSeconds(1f / AttackSpeed);


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
        if (!sheilds.IsActive)
        {
            GameObject sheild = Instantiate(Sheild, SheildPosition.position, Quaternion.identity);
            EnemySheilds sheildScript = sheild.GetComponent<EnemySheilds>();


            sheild.transform.SetParent(transform);

            if (sheildScript == null)
            {
                sheildScript = sheild.AddComponent<EnemySheilds>();
            }
            sheildScript.SetHitPoints(sheildHealth);
        }
    }

    

    public void LightGautling()
    {
        // fires buffed base attacks at the player at 5 times the base speed 
        castAmount = 5;
        ProjectileSpeed *= 5;
        ProjectileDamage *= 1.5f;
        Type = ProjectileType.Lazer;

    }




    public void Summon()
    {
        // summons weaker version of the base mummy that will increase his health and try to revevie Sekhemt

        Debug.Log("Summon Called");

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
                    
                }
            }
        }

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
}
