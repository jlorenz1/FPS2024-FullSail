using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEnemy : EnemyAI
{
 

    [Header("----- Type-----")]
    [SerializeField] float BuffRange = 30;
    [SerializeField] bool maxHealthBuffer;
    [SerializeField] bool AttackSpeedBuffer;
    [SerializeField] bool damageBUffer;
    [SerializeField] bool ArmorBUffer;
    [SerializeField] bool Healer;
    [SerializeField] bool Summoner;


    [Header("-----Ability Stats-----")]
    [SerializeField] float AttackSpeedBuff;
    [SerializeField] float AttackDamageBuff;
    [SerializeField] float HealthBuff;
    [SerializeField] float ArmorBuff;
    [SerializeField] float Healing;
    [SerializeField] GameObject SummoningMob;
    [SerializeField] int SummonAmount;
    [SerializeField] public float specialAbilityCooldown = 10f;
    private float nextAbilityTime = 5f;

    bool willharm = true;
    [SerializeField] AudioClip ZombieBuff;
    [SerializeField, Range(0f, 1f)] float ZombieBuffVol;

    float AuraRange = 2.5f;
    int SelfBuff;

    float StartMaxHealth;
    float startArmor;

    bool Buff1, Buff2, Buff3 = false;

    [SerializeField] Transform launchPoint;
    public GameObject projectilePrefab;

    [SerializeField] float ProjectileSpeed;
    [SerializeField] float ProjectileLifeTime;
    [SerializeField] float ProjectileDamage;
    [SerializeField] float ProjectileFollowTime;
    [SerializeField] ProjectileType Type;
    [SerializeField] ProjectileAblity projectileAblity;
    [SerializeField] float AbilityStrength;
    [SerializeField] float AbilityDuration;
    [SerializeField] float AttackDelay;





    [SerializeField] float effectDuration;
    [SerializeField] float AoeStrength;
    [SerializeField] float radius;
    [SerializeField] AOETYPE type;


    [SerializeField] Color BulletColor;
    [SerializeField] Material BulletMaterial;
    float LazerSpeed;

    Caster caster;


    bool canAttack;


    protected override void Start()
    {
        base.Start();

        StartMaxHealth = MaxHealth;
        startArmor = Armor;
        canAttack = true;


    }


    protected override void Update()
    {
        base.Update();




        if (Time.time >= nextAbilityTime && PlayerInSIte && canAttack)
        {

            animator.SetTrigger("Support");

            nextAbilityTime = Time.time + specialAbilityCooldown;

            canAttack = false;
        }




        if (PlayerinAttackRange && canAttack && PlayerInSIte)
        {
            animator.SetTrigger("Shoot");
            canAttack = false;
        }

      


        ZombiesInRange();



        if (SelfBuff < 10)
        {
            StartMaxHealth = MaxHealth;
            startArmor = Armor;
            Buff1 = false;  Buff2 = false; Buff3 = false;

        }

            if (SelfBuff >= 10 && SelfBuff < 20)
        {
            if (Buff1 == false)
            {
                MaxHealth = StartMaxHealth = 2;
                Armor = startArmor = 2;
                Buff1 = true;
                Buff2 = false; Buff3 = false;
            }
        }
        if (SelfBuff >= 20 && SelfBuff < 30)
        {
            if (Buff2 == false)
            {
                MaxHealth = StartMaxHealth = 4;
                Armor = startArmor = 2;
                Buff2 = true;
                Buff3 = false; Buff1 = false;
            }
        }
        if (SelfBuff >= 30)
        {
            if (Buff3 == false)
            {
                MaxHealth = StartMaxHealth = 8;
                Armor = startArmor = 2;
                Buff3 = true;
                Buff1 = false; Buff2 = false;
            }
        }




    }

    public void UseSpecialAbility()
    {

        StartCoroutine(Stop(3f));


        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");


        if (ZombieBuff != null)
        {

            PlaySFX(ZombieBuff, ZombieBuffVol);
        }

        if (Summoner)
        {
            SummonMinions();
        }

        foreach (GameObject zombie in zombies)
        {
            float distance = Vector3.Distance(transform.position, zombie.transform.position);
            if (distance < BuffRange)
            {

                IEnemyDamage FellowZombie = zombie.GetComponent<IEnemyDamage>();
                if (FellowZombie != null)
                {

                  

                    if (maxHealthBuffer)
                    {
                        FellowZombie.AddMaxHp(HealthBuff);
                    }

                    if (damageBUffer)
                    {

                        FellowZombie.AddDamage(AttackDamageBuff);
                    }

                    if (AttackSpeedBuffer)
                    {
                        FellowZombie.AddAttackSpeed(AttackSpeedBuff);
                    }

                    if (ArmorBUffer)
                    {
                        FellowZombie.AddArmor(ArmorBuff);
                    }


                    if (Healer)
                    {
                        FellowZombie.AddHP(Healing);
                    }


                }
            }
        }

    }


    void ZombiesInRange()
    {
        List<GameObject> ZombiesCloseEnough = new List<GameObject>();
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject zombie in zombies)
        {
            float distance = Vector3.Distance(transform.position, zombie.transform.position);
            if (distance < BuffRange)
            {

                ZombiesCloseEnough.Add(zombie);

            }
            if (distance > BuffRange)
            {
                ZombiesCloseEnough.Remove(zombie);
            }

        }
        SelfBuff = ZombiesCloseEnough.Count;

    }

    void SummonMinions()
    {

      

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


    protected override void Die()
    {
        // Additional special-specific death logic (if any)
        base.Die();
    }



    void SetTransparency(GameObject obj, float alpha)
    {
        // Get the Renderer component to access the material
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            // Get the material's current color
            Color color = renderer.material.color;

            // Set the alpha channel to the desired transparency level
            color.a = alpha;

            // Apply the color back to the material
            renderer.material.color = color;
        }
    }

    public void EnableAttack()
    {
        canAttack = true;
    }


    public void CastBaseAttack()
    {

        CastAttack(ProjectileAblity.Normal);

    }



    public void CastAttack(ProjectileAblity ABILITY)
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
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
