using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Banadage Pickup", menuName = "Inventory/pickups/Banadage")]
public class bandagePickup : pickupObject, IPickup
{
    public int hpToRestore;
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
        gameManager.gameInstance.playerScript.recieveHP(hpToRestore);
        Destroy(prefab);
    }
}
