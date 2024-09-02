using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager _WeaponsInstance;
    public static WeaponManager WeaponsInstance
    {
        get
        {
            if (_WeaponsInstance == null)
            {
                Debug.LogError("weaponinstance is null");
            }
            return _WeaponsInstance;
        }
    }
    public weaponStats[] allWeaponsInScene;
    private void Awake()
    {
        if (_WeaponsInstance != null && _WeaponsInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _WeaponsInstance = this;
        }
    }
    void Start()
    {
        resetAllMags();
    }

    public void resetAllMags()
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

    public void resetAllPlayerWeapons()
    {
        List<weaponStats> playerWeapons = gameManager.gameInstance.playerScript.gunList;
        for(int i = 0; i < playerWeapons.Count; i++)
        {
            playerWeapons[i].currentMagazineIndex = 0;
            for (int magIndex = 0; magIndex < playerWeapons[i].magazines.Length; magIndex++)
            {
                playerWeapons[i].magazines[magIndex].currentAmmoCount = playerWeapons[i].magazines[magIndex].magazineCapacity;
            }
        }
    }
}
