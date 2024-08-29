using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
 
    [SerializeFeild] public weaponStats gun;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            gameManager.gameInstance.playerScript.getWeaponStats(gun);
            Destroy(gameObject);
        }
    }
}
