using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zombiemeeleattacks : MonoBehaviour
{


   public float damage;
    bool causesBleed;
    float duration;
    
    private void Start()
    {
        causesBleed = false;

        gameObject.GetComponent<Collider>().enabled = false;
    }

   public void ToggleColider()
    {
        gameObject.GetComponent<Collider>().enabled = !gameObject.GetComponent<Collider>().enabled;
    }





    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           IDamage player = other.GetComponent<IDamage>();

            player.takeDamage(damage);

            if (causesBleed )
            {
                player.TickDamage(duration, 10, 1f);
            }
        }
        else
            return;
    }




    public void SetDamage(float amount)
    {
        damage = amount;
    }


    public void SetBleed()
    {
        causesBleed = !causesBleed;
    }

  
}
