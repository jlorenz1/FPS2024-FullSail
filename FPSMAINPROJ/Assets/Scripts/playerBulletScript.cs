
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBulletScript : MonoBehaviour
{

    [SerializeField] int DamageAmount;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("hit" + collision.gameObject.name);

            Destroy(gameObject);
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage ding = other.GetComponent<IDamage>();
        if (ding != null)
        {
            ding.takeDamage(DamageAmount);
        }
        Destroy(gameObject);
    }

}
