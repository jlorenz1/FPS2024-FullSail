using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryManager : MonoBehaviour
{
    public static inventoryManager Instance;
    List<string> basicInventory = new List<string>();
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
            
    }

    // Update is called once per frame
    public void AddItem(string item)
    {
        if(!basicInventory.Contains(item))
        {
            basicInventory.Add(item);
        }
    }

    public bool checkInventory(string item)
    {
        return basicInventory.Contains(item);
    }

    public void removeItem(string item)
    {
        if (basicInventory.Contains(item))
        {
            basicInventory.Remove(item);
        }
    }
}
