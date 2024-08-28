using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maxAmmo : MonoBehaviour
{
    Weapon weapon;
    int magazineCapacity;
    int ammoInMagazine;

    // Start is called before the first frame update
    void Start()
    {
        weapon = gameManager.gameInstance.playerScript.weapon;
        //magazineCapacity = weapon.getMaxAmmoCount();
        //ammoInMagazine = weapon.getMaxAmmoCount();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            //Weapon.Magazine newMagazine = new Weapon.Magazine(magazineCapacity, ammoInMagazine);

            //weapon.addMagazine(newMagazine, weapon.maxMagazines);
            Destroy(gameObject);
        }
    }
}
