using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyAI
{

    [SerializeField] Zombiemeeleattacks weapon;

    [Header("-----Melee Attack Stats -----")]
  
    bool canAttack = true;
    [SerializeField] Collider MeeleColider;
    [SerializeField] float rotationSpeed;
    public Transform rightHandTarget;
    public float weight = 1.0f;


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



        if (rightHandTarget != null)
        {

            Vector3 directionToPlayer = gameManager.gameInstance.player.transform.position - weapon.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);


            weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }


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


    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Set the position and rotation of the right hand to the target
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

            // Control the weight of the IK
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
        }
    }





}