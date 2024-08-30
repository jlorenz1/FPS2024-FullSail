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
       if (other.CompareTag("Player"))
        {
            WeaponManager.WeaponsInstance.resetAllPlayerWeapons();
            AudioManager.audioInstance.playAudio(pickupSound, pickupVol);
            Destroy(gameObject);
        }
    }
}
