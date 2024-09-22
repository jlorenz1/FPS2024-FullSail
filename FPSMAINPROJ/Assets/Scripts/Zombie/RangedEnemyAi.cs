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

    protected override void Start()
    {
        base.Start();
        agent.stoppingDistance = castRange / 2;
        canAttack = true;
        AttackDone = true;
        inPosition = false;
        caster = Caster.NormalCaster;
        LazerSpeed = ProjectileSpeed * 10;

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
            StartCoroutine(CastAttackRoutine());
        }
    }


    IEnumerator CastAttackRoutine()
    {
        canAttack = false;
        animator.SetFloat("CastSpeed", castSpeed); // New: Set animator speed to match cast speed
        animator.SetTrigger("Shoot");
        agent.speed = 0;

        yield return new WaitUntil(() => AttackDone == true);

        agent.speed = startSpeed;
        canAttack = true;
    }


    IEnumerator Cast()
    {
    
        for (int i = 0; i < castAmount; i++)
        {
           


            yield return new WaitForSeconds(1 / castSpeed);
        }
        yield return new WaitForSeconds(1);
        AttackDone = true;
        animator.SetTrigger("Shoot");
    }

    public void CastAttack()
    {
        // Ranged attack logic
       
        AttackDone = false;
        StartCoroutine(Cast());
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
