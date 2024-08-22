using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Banadage Pickup", menuName = "Inventory/pickups/Banadage")]
public class bandagePickup : pickupObject, IPickup
{
    public int hpToRestore;
    public void Awake()
    {
        type = itemType.Bandage;
        hpToRestore = 0;
    }

    public void useItem()
    {
        gameManager.gameInstance.playerScript.recieveHP(hpToRestore);
        Debug.Log("used");
    }
}
