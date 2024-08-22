using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Secondary Weapon Pickup", menuName = "Inventory/pickups/Secondary Weapon")]
public class secondaryPickup : pickupObject, IPickup
{
    public void Awake()
    {
        type = itemType.Secondary;
    }

    public secondaryPickup GetPistol()
    {
        return this;
    }
    public void useItem()
    {
        PlayerController controller = gameManager.gameInstance.playerScript;
        controller.equipWeapon(prefab, type);
        //equip weapon from player controller

    }
}
