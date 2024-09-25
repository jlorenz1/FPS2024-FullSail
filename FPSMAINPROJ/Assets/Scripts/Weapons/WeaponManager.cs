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
            allWeaponsInScene[i].currentAmmo = allWeaponsInScene[i].magMaxAmmount;
            allWeaponsInScene[i].currentMaxAmmo = allWeaponsInScene[i].maxReserve;
        }
    }

    public void resetAllPlayerWeapons()
    {
        List<weaponStats> playerWeapons = gameManager.gameInstance.playerWeapon.gunList;
        for (int i = 0; i < playerWeapons.Count; i++)
        {
            playerWeapons[i].currentMaxAmmo += playerWeapons[i].magMaxAmmount;

        }
        gameManager.gameInstance.playerWeapon.displayCurrentAmmo();
        gameManager.gameInstance.playerWeapon.displayMaxAmmo();
    }

    public weaponStats getHekaBasedWeapon(string hekaSchool)
    {
        weaponStats chosenWeapon = null; 

        for(int i = 0; i < allWeaponsInScene.Length; i++)
        {
            if (allWeaponsInScene[i].hekaSchool ==  hekaSchool)
            {
                chosenWeapon =  allWeaponsInScene[i];
            }
        }

        return chosenWeapon;
    }
}
