using System.Collections;
using UnityEngine;

public class RangedEnemy : EnemyAI
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    [SerializeField] Transform launchPoint;
    [SerializeField] GameObject CastPortal1;
    [SerializeField] GameObject CastPortal2;

    [SerializeField] int castAmount;

    float castDelay = 1;
    [SerializeField] float castRange;
    bool canAttack;
    float basespeed;
    bool waitdone;
    bool AttackDone;
    bool inPosition;


    [SerializeField] bool HasRangedAttacks;
    
    [SerializeField] float ProjectileSpeed;
    [SerializeField] float ProjectileLifeTime;
    [SerializeField] float ProjectileDamage;
    [SerializeField] float ProjectileFollowTime;
    [SerializeField] ProjectileType Type;
    [SerializeField] ProjectileAblity projectileAblity;
    [SerializeField] float AbilityStrength;
    [SerializeField] float AbilityDuration;

    [SerializeField] float effectDuration;
    [SerializeField] float AoeStrength;
    [SerializeField] float radius;
    [SerializeField] AOETYPE type;


    [SerializeField] Color BulletColor;
    [SerializeField] Material BulletMaterial;
    float LazerSpeed;

    Caster caster;
    bool canattack;
    protected override void Start()
    {
        base.Start();
        agent.stoppingDistance = castRange / 2;
        canAttack = true;
        AttackDone = true;
        inPosition = false;
        caster = Caster.NormalCaster;
        LazerSpeed = ProjectileSpeed * 2;

    }

    public void ToggleCastPortal()
    {

        if (CastPortal1 != null)
        {
            CastPortal1.SetActive(!CastPortal1.activeSelf);
        }

        if (CastPortal2 != null)
        {
            CastPortal2.SetActive(!CastPortal2.activeSelf);
        }
    }

    protected override void Update()
    {
        base.Update();


        if (Type == ProjectileType.Lazer)
        {
            castAmount = 1;
            ProjectileSpeed = LazerSpeed;
        }


        if (PlayerinAttackRange && canAttack && ChasingPLayer)
        {
            canAttack = false;
            animator.SetTrigger("Shoot");

        }
    }


    IEnumerator CastAttackRoutine()
    {
      

        animator.SetFloat("CastSpeed", castSpeed);

        for (int i = 0; i < castAmount; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
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
            yield return new WaitForSeconds((1 / castSpeed)  );
        }

       
        animator.SetTrigger("Shoot");
        canAttack = true;
    }


   

    public void CastAttack()
    {
        // Ranged attack logic


           
            StartCoroutine(CastAttackRoutine());
      
    }


  

    protected override void Die()
    {
        // Additional ranged-specific death logic (if any)
        base.Die();
    }

    IEnumerator wait(float time)
    {
        waitdone = false;
        yield return new WaitForSeconds(time);
        waitdone = true;
    }
 


}
