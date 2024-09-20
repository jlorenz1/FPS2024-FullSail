using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class weaponStats : ScriptableObject
{
    [Header("----- Gun Model -----")]
    public GameObject gunModel;
    public string gunName;
    public int animationLayer;

    [Header("----- Gun Attributes -----")]
    public int shootDamage;
    public int shootingDistance;
    public float shootRate;
    public string fireMode;

    [Header("-----Heka Attributes-----")]
    public string hekaSchool;
    [SerializeField] public GameObject hekaAbility;
    [SerializeFeild] public GameObject hekaMuzzleFlash;
    [SerializeFeild] public AudioClip[] hekaShootingSounds;
    public float hekaShootVol;
    public int hekaShootRate = 0;
    public float hekaManaAmount = 0;

    [Header("---- RECOIL VALUES ---")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public List<Vector3> RecoilPattern;
    public float snapping;
    public float returnSpeed;
    [Header("----- Gun Ammo/Magazines -----")]
    public float reloadTime;
    [SerializeField] public Magazines[] magazines; // Array of magazines used by the weapon
    public int currentMagazineIndex = 0;

    [Header("----- Gun SFX/FX -----")]
    public GameObject muzzleFlash;
    public ParticleSystem zombieHitEffect;
    [SerializeField] public GameObject[] bulletDecals;
    public ParticleSystem enviormentEffect;
    public AudioClip[] shootSound;
    public float shootVol;
    public AudioClip reloadSound;
    public float reloadVol;

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
