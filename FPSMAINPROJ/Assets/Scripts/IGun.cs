using UnityEngine;

/// <summary>
/// Interface defining the properties and methods required for any gun.
/// </summary>
public interface IGun
{
    /// <summary>
    /// Enum defining the various firing modes available for guns.
    /// </summary>
    public enum FireMode
    {
        Single,
        Burst,
        FullAuto
    }

    /// <summary>
    /// Magazine related properties.
    /// </summary>
    int MagazineCapacity { get; }
    int CurrentAmmoCount { get; set; }
    int MaxMagazines { get; }

    /// <summary>
    /// Gets the available fire modes for the gun.
    /// </summary>
    FireMode[] AvailableFireModes { get; }

    /// <summary>
    /// Gets or sets the current fire mode of the gun.
    /// </summary>
    FireMode CurrentFireMode { get; set; }

    /// <summary>
    /// Gets the shooting distance of the gun.
    /// </summary>
    float ShootingDistance { get; }

    /// <summary>
    /// Gets the rate of fire of the gun.
    /// </summary>
    float RateOfFire { get; }

    /// <summary>
    /// Gets the reload time of the gun.
    /// </summary>
    float ReloadTime { get; }

    /// <summary>
    /// Gets the maximum durability of the gun.
    /// </summary>
    float MaxDurability { get; }

    /// <summary>
    /// Gets the durability loss per shot of the gun.
    /// </summary>
    float DurabilityLossPerShot { get; }

    /// <summary>
    /// Gets whether the gun can jam.
    /// </summary>
    bool CanJam { get; }

    /// <summary>
    /// Gets the burst count for burst fire mode.
    /// </summary>
    int BurstCount { get; }

    /// <summary>
    /// Gets the burst rate for burst fire mode.
    /// </summary>
    float BurstRate { get; }

    /// <summary>
    /// Gets the recoil intensity of the gun.
    /// </summary>
    float RecoilIntensity { get; }

    /// <summary>
    /// Gets the recoil speed of the gun.
    /// </summary>
    float RecoilSpeed { get; }

    /// <summary>
    /// Gets the starting ammo count of the gun.
    /// </summary>
    int StartingAmmo { get; }

    /// <summary>
    /// Gets the particle system for the gun's muzzle flash.
    /// </summary>
    ParticleSystem ShootingSystem { get; }

    /// <summary>
    /// Gets the particle system for the gun's impact effect.
    /// </summary>
    ParticleSystem ShootImpactSystem { get; }

    /// <summary>
    /// Gets the trail renderer for the gun's bullet trail.
    /// </summary>
    TrailRenderer BulletTrail { get; }

    /// <summary>
    /// Gets the light for the gun's muzzle flash.
    /// </summary>
    Light MuzzleFlash { get; }

    /// <summary>
    /// Gets the reload animation clip for the gun.
    /// </summary>
    AnimationClip ReloadAnimation { get; }

    /// <summary>
    /// Gets the shooting animation clip for the gun.
    /// </summary>
    AnimationClip ShootAnimation { get; }

    /// <summary>
    /// Gets the idle animation clip for the gun.
    /// </summary>
    AnimationClip IdleAnimation { get; }

    /// <summary>
    /// Initializes the gun configuration.
    /// </summary>
    /// <param name="weapon">The weapon to which this gun is attached.</param>
    void InitializeGun(Weapon weapon);

    /// <summary>
    /// Switches to the next available fire mode.
    /// </summary>
    void SwitchFireMode();
}
