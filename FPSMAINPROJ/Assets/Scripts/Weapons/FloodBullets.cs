using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodBullets : MonoBehaviour
{

    bool FirstEnemyHit;

    List<GameObject> EnemiesHit;

    private Rigidbody rb;
    [SerializeField] private float speed = 5f; 
    [SerializeField] private float destroyTime = 2f; 
    [SerializeField] private float damage = 0;
    int AmountHit;
    float knockBackDistance;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        AmountHit = 0;

        Destroy(gameObject, destroyTime);

        knockBackDistance = speed * destroyTime;
    }

   

    private void OnTriggerEnter(Collider other)
    {
        IEnemyDamage Damageable = other.GetComponent<IEnemyDamage>();

        if (other.CompareTag("Player") || other.isTrigger || other.CompareTag("Weapon"))
        {
            return;
        }

        if (Damageable != null)
        {
            damage += AmountHit * 4;
            Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
            Damageable.knockback(contactPoint, knockBackDistance, destroyTime);
            Damageable.takeDamage(damage);
            AmountHit++;
            return;
        }
      
        }
    


   


  


}
