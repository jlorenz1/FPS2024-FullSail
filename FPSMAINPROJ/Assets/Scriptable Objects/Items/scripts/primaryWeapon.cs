using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Primary Weapon Pickup", menuName = "Inventory/pickups/Primary Weapon")]

public class primaryWeapon : pickupObject, IPickup
{
    public void Awake()
    {
        type = itemType.Primary;
    }

    public void useItem()
    {
        PlayerController controller = gameManager.gameInstance.playerScript;
        controller.equipWeapon(prefab, type);
        //equip weapon from player controller
        
    }
}
