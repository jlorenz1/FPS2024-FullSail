using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class ElectricityBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f; // Speed of the bullet
    [SerializeField] private float destroyTime = 5f; // Time before the bullet is destroyed
    [SerializeField] private int damage = 10; // Damage dealt by the bullet
    [SerializeField] private float maxDist;
    [SerializeFeild] public int bounceMax;
    [SerializeFeild] public float bounceDelay;
    [SerializeField] private LayerMask damageableLayer;

    private Rigidbody rb;
    int currentBounces = 0;
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
        if (other.isTrigger || other.CompareTag("Weapon") || other.CompareTag("Player"))
        {
            return;
        }

        IEnemyDamage damageable = other.GetComponent<IEnemyDamage>();
        GameObject zombieHit = getTopLevelParent(other);
        Transform spawnBounce = zombieHit.transform.Find("mixamorig5:Hips/HekaOutting");
        GameObject closestZombie = getClosestZombie(zombieHit);
        if (damageable != null)
        {

            damageable.takeDamage(damage);
            if(currentBounces < bounceMax)
            {
                
                currentBounces++;
                StartCoroutine(startNextBounce(zombieHit, spawnBounce, closestZombie));
                //Transform spawnBounce = zombieHit.transform.Find("HekaOutting");
                //if (spawnBounce != null)
                //{
                //    Debug.Log("no spawn");
                //}
                //shootSecond(spawnBounce, closestZombie);
                //currentBounces++;
            }
        }
        else
        {
            Debug.Log("no IDMG");
        }

        
    }

    IEnumerator startNextBounce(GameObject zombieHit, Transform spawnBounce, GameObject closestZombie)
    {
        Debug.Log("starting bounce");

        yield return new WaitForSeconds(bounceDelay);
        
        Debug.Log("Attempting to instantiate bounce projectile...");
        spawnBounce.LookAt(closestZombie.transform.Find("mixamorig5:Hips"));
        GameObject projectile = Instantiate(gameManager.gameInstance.playerWeapon.hekaAbility, spawnBounce.position, spawnBounce.rotation);

        if(projectile == null)
        {
         Debug.Log("wont spawn");
        }
        
        ElectricityBullet newBullet = projectile.GetComponent<ElectricityBullet>();
        if (newBullet != null)
        {
            newBullet.currentBounces = currentBounces;
        }
        
        Destroy(gameObject);
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
        float MaxDistance = maxDist;
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
