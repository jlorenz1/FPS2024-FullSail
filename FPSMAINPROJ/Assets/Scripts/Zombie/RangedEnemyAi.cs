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

    [SerializeField] float castSpeed = 1f;
    float castDelay = 1;
    [SerializeField] float castRange;
    bool canAttack;
    float basespeed;


    protected override void Start()
    {
        base.Start();
        agent.stoppingDistance = castRange / 2;
        canAttack = true;
    }

  public  void ToggleCastPortal()
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

        if (PlayerinAttackRange && canAttack)
        {
            StartCoroutine(CastAttackRoutine());
        }
    }

    IEnumerator CastAttackRoutine()
    {
        canAttack = false;
        animator.SetFloat("CastSpeed", castSpeed); // New: Set animator speed to match cast speed
        animator.SetTrigger("Shoot");

        // Wait based on the cast speed
        yield return new WaitForSeconds(1f / castSpeed);

        // Perform the ranged attack
        

        nextFireTime = Time.time + (1f / castSpeed); // Adjust next fire time based on cast speed
        canAttack = true;
    }

    public void CastAttack()
    {
        // Ranged attack logic
        Debug.Log("Ranged attack");
        Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
    }

    protected override void Die()
    {
        // Additional ranged-specific death logic (if any)
        base.Die();
    }
}
