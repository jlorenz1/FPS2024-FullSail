using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Ammo Pickup", menuName = "Inventory/pickups/Ammo")]

public class ammoPickup : pickupObject
{
    public int amountOfAmmo;

    public void Awake()
    {
        type = itemType.Ammo;
        amountOfAmmo = 0;
    }
}
