using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Weapon Pickup", menuName = "Inventory/pickups/Weapon")]
public class weaponPickup : pickupObject, IPickup
{
    // Update is called once per frame
    public void useItem()
    {
        Weapon newWeapon = Instantiate(prefab).GetComponent<Weapon>();

        PlayerController controller = gameManager.gameInstance.GetComponent<PlayerController>();
        //equip weapon from player controller
    }
}
