using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public weaponStats[] allWeaponsInScene;
    void Start()
    {
        resetAllMags();
    }

    private void resetAllMags()
    {
        for(int i = 0; i < allWeaponsInScene.Length; i++)
        {
            allWeaponsInScene[i].currentMagazineIndex = 0;
            for(int magIndex = 0; magIndex < allWeaponsInScene[i].magazines.Length; magIndex++)
            {
                allWeaponsInScene[i].magazines[magIndex].currentAmmoCount = allWeaponsInScene[i].magazines[magIndex].magazineCapacity;
            }
        }
    }
}
