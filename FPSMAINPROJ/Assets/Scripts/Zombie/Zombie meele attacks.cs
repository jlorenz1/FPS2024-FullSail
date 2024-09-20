using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombiemeeleattacks : MonoBehaviour
{


   public float damage;
    bool causesBleed;
    float duration;
    
    private void Start()
    {
        causesBleed = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           IDamage player = other.GetComponent<IDamage>();

            player.takeDamage(damage);

            if (causesBleed )
            {
                player.TickDamage(duration, 0.5f, 3f);
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

    IEnumerator Bleed()
    {
        yield return new WaitForSeconds(duration); float bleedTick = 0.5f; // How often to apply bleed damage (every 0.5 seconds)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            gameManager.gameInstance.playerScript.takeDamage(3f); // Apply 2% damage
            elapsed += bleedTick;
            yield return new WaitForSeconds(bleedTick);
        }
    }
}
