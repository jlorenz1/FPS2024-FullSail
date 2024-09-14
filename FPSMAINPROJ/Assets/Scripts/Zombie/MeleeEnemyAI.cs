using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyAI
{

    [SerializeField] Zombiemeeleattacks weapon;

    [Header("-----Melee Attack Stats -----")]
  
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
        yield return new WaitForSeconds(1/AttackSpeed);
        canAttack = true;
    }


    private void AttackPlayer()
    {

        animator.SetFloat("AttackSpeed", AttackSpeed);

        animator.SetTrigger("Hit");

        Debug.Log("Melee attack");
      
    }

    public void ToggleColider()
    {
        MeeleColider.enabled = !MeeleColider.enabled;
    }


    public override void takeDamage(float amount)
    {
        base.takeDamage(amount);
    }

    protected override void Die()
    {
        base.Die();
    }



 




}