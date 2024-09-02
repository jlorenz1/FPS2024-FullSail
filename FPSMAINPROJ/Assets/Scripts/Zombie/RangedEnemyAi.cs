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
   
    [SerializeField] float castDelay;
    [SerializeField] float castRange;
    bool canAttack;



    protected override void Start()
    {
        base.Start();
        agent.stoppingDistance = castRange / 2;
        canAttack = true;
    }



    protected override void Update()
    {
        base.Update();

        if (PlayerinAttackRange && canAttack)
        {
            StartCoroutine(delayCast());
        }
    }


    IEnumerator delayCast()
    {
        canAttack = false;
        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(castDelay);
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
