using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Flashlight Pickup", menuName = "Inventory/pickups/Flashlight")]
public class flashlightPickup : pickupObject
{
    // Start is called before the first frame update
    void Start()
    {
        type = itemType.flashlight;
        triggerType = false;
    }
}
