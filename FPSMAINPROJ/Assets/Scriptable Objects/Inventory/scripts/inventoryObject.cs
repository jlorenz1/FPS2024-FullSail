using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]
public class inventoryObject : ScriptableObject
{
    public List<inventorySlot> containerForInv = new List<inventorySlot>();
    public void AddItem(pickupObject _pickup, int _amount)
    {
        bool hasPickup = false;
        for (int i = 0; i < containerForInv.Count; i++)
        {
            if (containerForInv[i].pickup == _pickup)
            {
                containerForInv[i].addAmount(_amount);
                hasPickup = true;
                break;
            }    
        }
        if(!hasPickup)
        {
            containerForInv.Add(new inventorySlot(_pickup, _amount));
        }
       
    }

    public void useItem(itemType type)
    {
        for (int i = 0;i < containerForInv.Count;i++)
        {
            if (containerForInv[i].pickup.type == type && containerForInv[i].amount > 0)
            {
                IPickup pickupComponent = containerForInv[i].pickup as IPickup; //CANT USE GETCOMPONENT FOR scriptableObjects 
                if(pickupComponent != null)
                {
                    Debug.Log("in use item inventory");
                    pickupComponent.useItem();
                    containerForInv[i].addAmount(-1);

                    if(containerForInv[i].amount <= 0)
                    {
                        containerForInv.RemoveAt(i);
                    }
                    break;
                }
                else
                {
                    Debug.Log("doesnt contain a IPICKUP");
                }
            }
        }
    }
}
//SO WE CAN HAVE SEPERATE SLOTS FOR EACH ITEM TYPE AND HAVE DIFFRENET TYPES OF EACH ITEM
[System.Serializable]
public class inventorySlot
{
    public pickupObject pickup;
    public int amount;
    public inventorySlot(pickupObject _pickup, int _amount)
    {
        pickup = _pickup;
        amount = _amount;
    }

    public void addAmount(int value)
    {
        amount += value;
    }

}
