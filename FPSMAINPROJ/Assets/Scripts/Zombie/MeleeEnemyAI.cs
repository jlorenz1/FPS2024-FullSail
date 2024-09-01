using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyAI
{

    [SerializeField] Zombiemeeleattacks weapon;

    [Header("-----Melee Attack Stats -----")]

    [SerializeField] float attackDelay;
   
    bool canAttack = true;
    [SerializeField] Collider MeeleColider;


    protected override void Start()
    {
        base.Start();
        canAttack = true;
        weapon.SetDamage(damage);
    }


    protected override void Update()
    {
        base.Update();

        if (PlayerinAttackRange && canAttack)
        {
            StartCoroutine(DelayAttack());
        }
    }

    IEnumerator DelayAttack()
    {
        canAttack = false;
        AttackPlayer();
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }


    private void AttackPlayer()
    {
      
        animator.SetTrigger("Hit");

        Debug.Log("Melee attack");
      
    }

    public void ToggleColider()
    {
        MeeleColider.enabled = !MeeleColider.enabled;
    }

    protected override void Die()
    {
        base.Die();
    }
}