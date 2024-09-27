using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f; // Speed of the bullet
    [SerializeField] private float destroyTime = 5f; // Time before the bullet is destroyed
    [SerializeField] private int damage = 10; // Damage dealt by the bullet
    [SerializeFeild] private int armorAmountTaken = 40;
    [SerializeField] private float maxDist;
    [SerializeField] private float blindDuration;
    [SerializeField] private LayerMask damageableLayer;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Camera camera = Camera.main;

            //get middle of the screen
            Ray rayMiddle = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Vector3 targetPoint = rayMiddle.GetPoint(200);

            Vector3 direction = (targetPoint - transform.position).normalized;

            //face the bullet towards direction
            transform.forward = direction;

            rb.velocity = transform.forward * speed;
        }

        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.CompareTag("Weapon") || other.CompareTag("Player"))
        {
            return;
        }

        IEnemyDamage damageable = other.GetComponent<IEnemyDamage>();
        if(damageable != null)
        {
            damageable.takeDamage(damage);
            damageable.AddArmor(-armorAmountTaken);
            
        }

        Destroy(gameObject);
    }
}
