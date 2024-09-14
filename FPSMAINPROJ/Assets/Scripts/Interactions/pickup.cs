using JetBrains.Annotations;
using System;
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

    private void OnTriggerEnter(Collider other)
    {
        if(item.triggerType == true)
        {
            //type = getType();

            if(type == itemType.Bandage)
            {
                if(other.gameObject.CompareTag("Player"))// && (gameManager.gameInstance.playerScript.playerHP < gameManager.gameInstance.playerScript.HPorig))
                {
                    useItem();
                    Destroy(gameObject);
                }
            }
            if (type == itemType.Gem)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    useItem();
                    Destroy(gameObject);
                }
            }
        }
    }

    public itemType getType()
        { return type; }
    
}
