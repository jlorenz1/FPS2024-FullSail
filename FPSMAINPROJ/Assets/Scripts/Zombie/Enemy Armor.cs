using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmor : ScriptableObject
{
    public GameObject Model;
    enum ArmorType
    {
        Helmate,
        ChestPlate,
        Leggings,
        Boots
    }

    [Header("-----Stats-----")]
    public float ArmorBonus;
    public float SpeedAlter;
    public float MaxHealthBonus;
}
