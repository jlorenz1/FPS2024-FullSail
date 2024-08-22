using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Rune Pickup", menuName = "Inventory/pickups/Rune")]
public class runePickup : pickupObject, IPickup
{
    // Start is called before the first frame update
    void Start()
    {
        type = itemType.Rune;
        maxAmount = 3;
    }

    public void useItem()
    {

    }

}
