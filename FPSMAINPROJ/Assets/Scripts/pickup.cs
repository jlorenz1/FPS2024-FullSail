using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour, IPickup
{
    public pickupObject item;
    internal itemType type;

    public void useItem()
    {
        if (item is IPickup pickupItem)
        {
            pickupItem.useItem();
        }
    }

    public itemType getType()
        { return type; }
    
}
