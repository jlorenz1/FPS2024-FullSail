using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum itemType
{
    Ammo,
    Bandage,
    Key,
    Default
}

public abstract class pickupObject : ScriptableObject
{
    public GameObject prefab; //item to display
    public itemType type; //what type of object
    public int amount;
    [TextArea(10, 10)]
    public string description;
}
