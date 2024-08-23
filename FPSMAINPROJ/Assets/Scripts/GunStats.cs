using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    [Header("----- Gun Model -----")]
    public string gunName;
    public GameObject gunModel;

    [Header("----- Gun Animations -----")]
    public RuntimeAnimatorController animatorController;
    public AnimationClip reloadAnimation;
    public AnimationClip shootingAnimation;
    public AnimationClip idleAnimation;

    [Header("----- Gun Attributes -----")]
    public int shootDamage;
    public float shootingDistance;
    public float rateOfFire;
    public float recoilIntensity;
    public float recoilSpeed;

    [Header("----- Gun Ammo/Magazines -----")]
    public int magazineCapacity;
    public int currentAmmoCount;
    public int maxMagazines;
    public int startingAmmo;

    [Header("----- Gun Fire Modes -----")]
    public Weapon.FireMode currentFireMode;
    public Weapon.FireMode[] availableFireModes;
    public int burstCount;
    public float burstRate;

    [Header("----- Gun Effects -----")]
    //public ParticleSystem muzzleFlashSystem, bulletImpactEffectSystem;
    //public TrailRenderer bulletTrail;
    //public AudioClip[] shootSound;
    //public float shootVolume;

    [Header("----- Gun Durability -----")]
    public float maxDurability;
    public float durabilityLossPerShot;
    public bool canJam;

    [Header("----- Reload QTE Settings -----")]
    public float qteDelay; // Quick Time Event delay before start
    public float qteWindow; // Time window for QTE

}
