using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class weaponStats : ScriptableObject
{
    [Header("----- Gun Model -----")]
    public GameObject gunModel;

    [Header("----- Gun Attributes -----")]
    public int shootDamage;
    public int shootingDistance;
    public float shootRate;

    [Header("----- Gun Ammo/Magazines -----")]
    public float reloadTime;
    [SerializeField] public Magazines[] magazines; // Array of magazines used by the weapon
    public int currentMagazineIndex = 0;

    [Header("----- Gun SFX/FX -----")]
    public ParticleSystem hitEffect;
    public ParticleSystem zombieHitEffect;
    public AudioClip[] shootSound;
    public float shootVol;

}

[System.Serializable]
public class Magazines
{
    public int magazineCapacity = 30;   // Capacity of the magazine
    public int currentAmmoCount = 30;  // Current number of bullets within magazine

    public Magazines(int _magazineCapacity, int _currentAmmoCount)
    {
        magazineCapacity = _magazineCapacity;
        currentAmmoCount = _currentAmmoCount;
    }

}
