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
    [SerializeFeild] public int currentAmmo;
    [SerializeFeild] public int currentMaxAmmo;
    [SerializeField] public int magMaxAmmount;
    [SerializeFeild] public int maxReserve;


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


