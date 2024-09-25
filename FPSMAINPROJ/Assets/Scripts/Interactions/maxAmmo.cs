using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maxAmmo : MonoBehaviour
{
    //Weapon weapon;
    [SerializeFeild] public AudioClip pickupSound;
    [Range(0, 1)][SerializeFeild] public float pickupVol;


    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player") && gameManager.gameInstance.playerWeapon.gunList.Count > 0)
        {
            WeaponManager.WeaponsInstance.resetAllPlayerWeapons();
            AudioManager.audioInstance.playSFXAudio(pickupSound, pickupVol);
            Destroy(gameObject);
        }
    }
}
