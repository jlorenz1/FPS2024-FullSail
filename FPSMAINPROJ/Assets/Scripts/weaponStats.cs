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
    public int ammoCur, ammoMax;

    [Header("----- Gun SFX/FX -----")]
    public ParticleSystem hitEffect;
    public ParticleSystem zombieHitEffect;
    public AudioClip[] shootSound;
    public float shootVol;

}
