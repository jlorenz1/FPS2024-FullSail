
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBulletScript : MonoBehaviour
{

    [SerializeField] int DamageAmount;

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with the weapon
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Ignored collision with Weapon");
            return;
        }

        if (collision.gameObject.CompareTag("Zombie"))
        {
            Debug.Log("hit" + collision.gameObject.name);

            // Apply damage if the object implements damage interface
            IDamage  dmgDetect = collision.gameObject.GetComponent<IDamage>();
            if (dmgDetect != null )
            {
                dmgDetect.takeDamage(DamageAmount);
                gameManager.gameInstance.PointCount += 1;
            }
            // Destroy the game object that was hit
            Destroy(gameObject);
        }
        
    }


  /*  private void OnTriggerEnter(Collider other)
    {

        //ignores other triggers
        if (other.isTrigger)
        {
            return;
        }
        // determines if the object has the takeDamage function
        IDamage dmgDetect = other.GetComponent<IDamage>();
        if (dmgDetect != null)
        {
            dmgDetect.takeDamage(DamageAmount);
        }

        IHitPoints hitPointsComponent = other.GetComponent<IHitPoints>();
        if (hitPointsComponent != null)
        {
            hitPointsComponent.DisplayHitPoints();
        }

        else
        {
            Debug.Log("The object does not implement IHitPoints.");
        }



        Destroy(gameObject);


    }*/

}
