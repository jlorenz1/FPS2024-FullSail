using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f; // Speed of the bullet
    [SerializeField] private float destroyTime = 5f; // Time before the bullet is destroyed
    [SerializeField] private int damage = 10; // Damage dealt by the bullet
    [SerializeField] private LayerMask damageableLayer;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.CompareTag("Player"))
        {
            return;
        }

        IEnemyDamage damageable = other.GetComponentInParent<IEnemyDamage>();
        GameObject zombieHit = getTopLevelParent(other);
        GameObject closestZombie = getClosestZombie(zombieHit);
        if (damageable != null)
        {

            damageable.takeDamage(damage);
            Transform spawnBounce = zombieHit.transform.Find("HekaOutting");
            if (spawnBounce != null)
            {
                Debug.Log("no spawn");
            }
            shootSecond(spawnBounce, closestZombie);
        }
        else
        {
            Debug.Log("no IDMG");
        }

        Destroy(gameObject); // Destroy the bullet on collision
    }

    void shootSecond(Transform shootPoint, GameObject closestZombie)
    {
        shootPoint.LookAt(closestZombie.transform.position);
        GameObject projectile = Instantiate(gameManager.gameInstance.playerWeapon.hekaAbility, shootPoint.position, shootPoint.rotation);

    }


    GameObject getTopLevelParent(Collider collider)
    {
        if (collider == null)
        {
            return null;
        }
        Transform currentTrans = collider.transform;

        while (currentTrans.parent != null)
        {
            currentTrans = currentTrans.parent;
        }

        return currentTrans.gameObject;
    }

    GameObject getClosestZombie(GameObject hitZombie)
    {
        float distance = 0;
        float MaxDistance = float.MaxValue;
        GameObject closestZombie = null;
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");


        foreach (GameObject zombie in zombies)
        {
            if (zombie == hitZombie)
            {
                continue; //skip hit zombie
            }

            distance = Vector3.Distance(hitZombie.transform.position, zombie.transform.position);

            if (distance < MaxDistance)
            {
                MaxDistance = distance;
                closestZombie = zombie;
            }

        }

        return closestZombie;

    }
}
