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

        AdjustColliderRange();
    }


    void AdjustColliderRange()
    {

        if (MeeleColider is CapsuleCollider Melee)
        {
            // Get the original Y-center position before changing the height
            float originalHeight = Melee.height;
            Vector3 originalCenter = Melee.center;

            // Adjust the collider height based on the range
            Melee.height = Range;

            // Maintain the radius proportion to the height (range divided by 5 for example)
            Melee.radius = Mathf.Max(Range / 5, 0.1f);  // Avoid very small radius

            // Adjust the center so the bottom of the capsule stays in the same place
            float heightDifference = (Melee.height - originalHeight) / 2f;
            Melee.center = new Vector3(originalCenter.x, originalCenter.y + heightDifference, originalCenter.z);
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