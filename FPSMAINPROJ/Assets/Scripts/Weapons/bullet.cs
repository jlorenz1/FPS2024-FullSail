using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f; // Speed of the bullet
    [SerializeField] private float destroyTime = 5f; // Time before the bullet is destroyed
    [SerializeField] private int damage = 10; // Damage dealt by the bullet
    [SerializeField] private LayerMask damageableLayer; // Layer to determine what the bullet can hit

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        Destroy(gameObject, destroyTime); // Automatically destroy bullet after a certain time
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.CompareTag("Player"))
        {
            return;
        }
        IDamage damageable = other.GetComponent<IDamage>();
        if(damageable != null)
        {

            damageable.takeDamage(damage);

        }
          
        
        
           
            
        
        Destroy(gameObject); // Destroy the bullet on collision
    }
}