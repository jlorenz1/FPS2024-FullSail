using UnityEngine;

/// <summary>
/// Class defining the specific characteristics and behaviors for a pistol.
/// </summary>
public class Pistol : MonoBehaviour, IGun
{
    [Header("Fire Mode Settings")]
    [SerializeField] private IGun.FireMode[] availableFireModes = new IGun.FireMode[] { IGun.FireMode.Single };
    [SerializeField] private IGun.FireMode currentFireMode = IGun.FireMode.Single;

    [Header("Shooting Settings")]
    [SerializeField] private float shootingDistance = 50f;
    [SerializeField] private float rateOfFire = 0.5f;

    [Header("Magazine Settings")]
    [Tooltip("Capacity of the magazine")]
    [SerializeField] private int magazineCapacity = 30;
    [Tooltip("Current number of bullets within the magazine")]
    [SerializeField] private int currentAmmoCount = 30;
    [Tooltip("Maximum number of magazines")]
    [SerializeField] private int maxMagazines = 5;
    private int startingAmmo = 18;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilIntensity = 0.1f;
    [SerializeField] private float recoilSpeed = 5f;

    [Header("Durability Settings")]
    [SerializeField] private float maxDurability = 500f;
    [SerializeField] private float durabilityLossPerShot = 0.05f;
    [SerializeField] private bool canJam = true;

    [Header("Burst Mode Settings")]
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstRate = 0.1f;

    [Header("Particle Systems and Effects")]
    [SerializeField] private ParticleSystem muzzleFlashSystem;
    [SerializeField] private ParticleSystem bulletImpactEffectSystem;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private Light muzzleFlashLight;

    [Header("Animations")]
    [SerializeField] private AnimationClip reloadAnimation;
    [SerializeField] private AnimationClip shootingAnimation;
    [SerializeField] private AnimationClip idleAnimation;

    public IGun.FireMode[] AvailableFireModes => availableFireModes;
    public IGun.FireMode CurrentFireMode { get => currentFireMode; set => currentFireMode = value; }
    public float ShootingDistance => shootingDistance;
    public float RateOfFire => rateOfFire;
    public int MagazineCapacity => magazineCapacity;
    public int CurrentAmmoCount { get; set; } = 30;
    public int MaxMagazines => maxMagazines;
    public float ReloadTime => reloadAnimation.length;
    public float RecoilIntensity => recoilIntensity;
    public float RecoilSpeed => recoilSpeed;
    public float MaxDurability => maxDurability;
    public float DurabilityLossPerShot => durabilityLossPerShot;
    public bool CanJam => canJam;
    public int BurstCount => burstCount;
    public float BurstRate => burstRate;
    public int StartingAmmo => startingAmmo;
    public ParticleSystem ShootingSystem => muzzleFlashSystem;
    public ParticleSystem ShootImpactSystem => bulletImpactEffectSystem;
    public TrailRenderer BulletTrail => bulletTrail;
    public Light MuzzleFlash => muzzleFlashLight;
    public AnimationClip ReloadAnimation => reloadAnimation;
    public AnimationClip ShootAnimation => shootingAnimation;
    public AnimationClip IdleAnimation => idleAnimation;

    public void InitializeGun(Weapon weapon)
    {
        weapon.SetGunConfiguration(this);
    }

    public void SwitchFireMode()
    {
        int currentIndex = System.Array.IndexOf(availableFireModes, currentFireMode);
        currentFireMode = availableFireModes[(currentIndex + 1) % availableFireModes.Length];
    }
}
