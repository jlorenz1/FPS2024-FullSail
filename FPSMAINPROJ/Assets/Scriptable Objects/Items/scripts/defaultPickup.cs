using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Pickup", menuName = "Inventory/pickups/default")]
public class defaultPickup : pickupObject
{
    public void Awake()
    {
        type = itemType.Default;
        amount = 0;
    }
}
