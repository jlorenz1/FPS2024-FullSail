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
                if (pickupComponent != null)
                {
                    pickupComponent.useItem();
                 
                    if (containerForInv[i].pickup.destroyAfterUse)
                    {
                        containerForInv[i].addAmount(-1);

                        if (containerForInv[i].amount <= 0)
                        {
                            containerForInv.RemoveAt(i);
                            
                        }
                    }
                   
                    break;
                }
              
                
            }
        }
    }

    public bool hasItem(itemType type )
    {
        for(int i = 0; i < containerForInv.Count; i++)
        {
            if (containerForInv[i].pickup.type == type)
            {
                return true;
            }
            
        }
        return false;
    }

    public int gemCount()
    {
        int amount = 0;
        for (int i = 0; i < containerForInv.Count; i++)
        {
            if (containerForInv[i].pickup.type == itemType.Gem)
            {
               amount = containerForInv[i].amount;
            }

        }

        return amount;
    }

    public void takeGems(int amount)
    {
        for (int i = 0; i < containerForInv.Count; i++)
        {
            if (containerForInv[i].pickup.type == itemType.Gem)
            {
                containerForInv[i].amount -= amount;
            }
        }
    }

    public pickupObject getItem(itemType type)
    {
        for (int i = 0; i < containerForInv.Count; i++)
        {
            if (containerForInv[i].pickup.type == type)
            {
                return containerForInv[i].pickup;
            }

        }
        return null;
    }

    public void updateInventoryUI()
    {
        for(int i = 0; i < containerForInv.Count;i++)
        {
            if (containerForInv[i].pickup.type == itemType.Key)
            {
                gameManager.gameInstance.keyCount.text = containerForInv[i].amount.ToString();
            }
            else if(containerForInv[i].pickup.type == itemType.Default)
            {
                gameManager.gameInstance.lighterCount.text = containerForInv[i].amount.ToString();
            }
            else if (containerForInv[i].pickup.type == itemType.Rune)
            {
                gameManager.gameInstance.runesCount.text = containerForInv[i].amount.ToString();
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
