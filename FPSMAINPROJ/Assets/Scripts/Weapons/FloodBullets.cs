using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodBullets : MonoBehaviour
{

    bool FirstEnemyHit;

    List<GameObject> EnemiesHit;

    private Rigidbody rb;
    [SerializeField] private float speed = 10f; // Speed of the bullet
    [SerializeField] private float destroyTime = 5f; // Time before the bullet is destroyed
    [SerializeField] private float damage = 5; // Damage dealt by the bullet
    float amount;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        Destroy(gameObject, destroyTime);

        FirstEnemyHit = false;
        EnemiesHit = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {


        IEnemyDamage Damageable = other.GetComponent<IEnemyDamage>();
        IEnemyDamage ChildDamage = other.GetComponentInChildren<IEnemyDamage>();

        if (other.CompareTag("Player") || other.isTrigger || other.CompareTag("Weapon"))
        {
            return;
        }

    
        else if(Damageable != null)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Damageable.knockback(hitPoint, 25);
            

            if (FirstEnemyHit == true)
            {
                gameObject.GetComponent<SphereCollider>().radius *= 5;
                FirstEnemyHit = true;

            }

            Damageable.takeDamage(damage);
            EnemiesHit.Add(other.gameObject);
         
        }

        else if (Damageable == null && ChildDamage != null)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            ChildDamage.knockback(hitPoint, 25);


            if (FirstEnemyHit == true)
            {
                gameObject.GetComponent<SphereCollider>().radius *= 5;
                FirstEnemyHit = true;

            }

            ChildDamage.takeDamage(damage);
            EnemiesHit.Add(other.gameObject);

        }

        else if (other.CompareTag("Untagged"))
        {
            if(EnemiesHit.Count <= 0)
            {

                Destroy(gameObject);

            }

            else {

                foreach(GameObject obj in EnemiesHit)
                {


               IEnemyDamage Damage = obj.GetComponent<IEnemyDamage>();
               IEnemyDamage CDamage = obj.GetComponentInChildren<IEnemyDamage>();
                    if (Damage != null) {
                         amount = Damage.GetMaxHP() * 0.5f;

                        Damage.TakeTrueDamage(amount);

                     }
                    else if (Damage == null &&  CDamage != null)
                    {

                         amount = CDamage.GetMaxHP() * 0.5f;

                        CDamage.TakeTrueDamage(amount);
                    }
                  

                }

            }
        }


    }



}
