using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Key Pickup", menuName = "Inventory/pickups/Key")]

public class keyPickup : pickupObject, IPickup
{
    
    public void Awake()
    {
        type = itemType.Key;
        triggerType = false;
        destroyAfterUse = true;
    }

    public void useItem()
    {
        RaycastHit hit;

        if(Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, 300, ~7))
        {
            doorScript door = hit.collider.GetComponent<doorScript>();
            if(door != null)
            {
                door.slide();
            }
        }
    }

}
