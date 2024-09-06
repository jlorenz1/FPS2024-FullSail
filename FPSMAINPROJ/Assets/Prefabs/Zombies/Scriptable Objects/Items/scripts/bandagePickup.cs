using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Banadage Pickup", menuName = "Inventory/pickups/Banadage")]
public class bandagePickup : pickupObject, IPickup
{
    public void Awake()
    {
        type = itemType.Bandage;
        triggerType = true;
    }
    
    private void onTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("HP pickup");
            useItem();
        }
    }
    public void useItem()
    {
        float currentHP = gameManager.gameInstance.playerScript.playerHP;
        float maxHp = gameManager.gameInstance.playerScript.HPorig;

        float hpNeeded = maxHp - currentHP;


        if(hpNeeded > 0)
        {
            gameManager.gameInstance.playerScript.recieveHP(hpNeeded);
            Destroy(prefab);
        }
    }
}
