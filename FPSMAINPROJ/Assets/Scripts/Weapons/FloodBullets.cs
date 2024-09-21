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
    [SerializeField] private float damage = 0; // Damage dealt by the bullet
    float amount;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
      //  StartCoroutine(Destroy());

        FirstEnemyHit = false;
        EnemiesHit = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(destroyTime);
        DestroyBullet();
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
     

            // Change sphere collider radius on the first hit
            if (!FirstEnemyHit && Damageable.isKnockBackRessitant() == false)
            {
                gameObject.GetComponent<SphereCollider>().radius *= 5;
                FirstEnemyHit = true;
            }

            Damageable.takeDamage(damage);


            if (Damageable.isKnockBackRessitant() == false) { 
                EnemiesHit.Add(other.gameObject);
                StickEnemyToBullet(other.gameObject);
            }
        }
        else if (!other.CompareTag("Zombie"))
        {
            if (EnemiesHit.Count <= 0)
            {
                DestroyBullet();
            }
            else
            {
                foreach (GameObject obj in EnemiesHit)
                {
                    IEnemyDamage Damage = obj.GetComponent<IEnemyDamage>();

                  /*  if (Damage != null)
                    {
                        amount = Damage.GetMaxHP() * 0.5f;
                        Damage.TakeTrueDamage(amount);
                        
                    }*/

                }
                DestroyBullet();
            }
        }
    }


    private void DestroyBullet()
    {
        // Detach all enemies before destroying the bullet
        foreach (GameObject enemy in EnemiesHit)
        {
            enemy.transform.SetParent(null); // Detach enemy from bullet
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null)
            {
                enemyCollider.enabled = true; // Re-enable the collider if needed
            }
        }

        Destroy(gameObject); // Destroy the bullet
    }


    private void StickEnemyToBullet(GameObject enemy)
    {
        // Disable the enemy's collider
        Collider enemyCollider = enemy.GetComponent<Collider>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false; // Disable collider to prevent further collisions
        }

        // Set the enemy as a child of the bullet
        enemy.transform.SetParent(transform);

        // Optionally, reset the enemy's position to the bullet's position to avoid visual glitches
        enemy.transform.localPosition = Vector3.zero; // This might need adjustment based on your bullet's visual
    }



}
