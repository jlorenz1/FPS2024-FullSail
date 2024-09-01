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
    bool canCast;

    protected override void Update()
    {
        base.Update();

        if (Vector3.Distance(gameManager.gameInstance.player.transform.position, transform.position) <= castRange && canCast)
        {
            delayCast();
           
        }
    }


    IEnumerator delayCast()
    {
        canCast = false;
        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(castDelay);
        canCast = true;
    }

    private void Cast()
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
