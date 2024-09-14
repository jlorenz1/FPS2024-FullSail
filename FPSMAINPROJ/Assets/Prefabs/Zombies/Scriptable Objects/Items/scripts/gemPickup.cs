using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Gem Pickup", menuName = "Inventory/pickups/Gem")]
public class gemPickup : pickupObject, IPickup
{
    public void Awake()
    {
        type = itemType.Gem;
        triggerType = true;
    }

    private void onTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gem pickup");
            useItem();
        }
    }
    public void useItem()
    {
        gameManager.gameInstance.playerScript.inventory.AddItem(this, amount);
        gameManager.gameInstance.PointCount++;
        Destroy(prefab);
    }
}













//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "Gem Pickup", menuName = "Inventory/pickups/Gem")]

//public class gemPickup : pickupObject, IPickup
//{

//    public void Awake()
//    {
//        type = itemType.Gem;
//        triggerType = true;
//    }
//    private void onTriggerEnter(Collider other)
//    {

//        if (other.gameObject.CompareTag("Player"))
//        {
//            Debug.Log("Gem pickup");
//            useItem();
//        }
//    }

//    public void useItem()
//    {
//        gameManager.gameInstance.GemCountUpdate(1);
//        Destroy(prefab);
//    }

//}
