using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombiemeeleattacks : MonoBehaviour
{


    float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.gameInstance.playerScript.takeDamage(damage);

        }
        else
            return;
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }
}
