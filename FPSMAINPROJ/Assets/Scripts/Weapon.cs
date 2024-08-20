using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Weapon class is responsible for handling gun behaviours like shooting, reloading, and animation.
/// It interacts with the IGun interface to get specific gun settings.
/// </summary>

public class Weapon : MonoBehaviour
{
    #region Serialized Fields

    [Header("Weapon Settings")]
    [Tooltip("Animator responsible for controlling weapon animations.")]
    [SerializeField] private Animator weaponAnimator;
    [Tooltip("The LayerMask used to determine which objects the bullets can hit.")]
    [SerializeField] private LayerMask canBeShotMask;

    [Header("Shooting System")]
    [Tooltip("Particle system for gun muzzle flash.")]
    [SerializeField] private ParticleSystem shootingSystem;
    [Tooltip("Particle system for when a bullet impacts a target")]
    [SerializeField] private ParticleSystem shootImpactSystem;
    [Tooltip("Trail renderer for bullet trails.")]
    [SerializeField] private TrailRenderer bulletTrail;
    [Tooltip("Light to simulate muzzle flash lighting.")]
    [SerializeField] private Light muzzleFlashLight;

    [Header("Reload Settings")]
    [Tooltip("Additional logic for reload mechanic.")]
    [SerializeField] float delayBeforeQTE; // Delay before the QTE starts
    [SerializeField] float windowForQTE; // Time window for the QTE

    #endregion

    #region Private Fields

    private IGun.FireMode[] availableFireModes;
    private float shootDistance;
    private float weaponFireRate;
    private float recoilIntensity;
    private float recoilSpeed;
    private float maxDurability;
    private float durabilityLossPerShot;
    private float burstRate;
    private int burstCount;
    private bool canJam;
    private bool weaponCanShoot = true;             // Indicates if the weapon is ready to shoot
    private bool isReloading = false;              // Indicates if the weapon is currently reloading
    private bool isJammed = false;                // Indicates if the weapon is jammed
    private int currentMagazineIndex = 0;        // Tracks the current magazine being used
    private float currentDurability;            // Tracks the current durability of the weapon
    private IGun currentGun;                   // Reference to the current gun configuration
    private float lastShotTime;               // Tracks the last time a shot was fired
    private float shootDelay = 0.5f;         // Prevents weapon from being immediately fired
    private int ammoToReload;              // Amount of ammo to reload
    private bool qteSuccess = false;      // Tracks success of Quick Time Event (QTE)
    private float reloadTime;

    [Serializable]
    public class Magazine
    {
        public int magazineCapacity = 30;   // Capacity of the magazine
        public int currentAmmoCount = 30;  // Current number of bullets within magazine

        public Magazine(int _magazineCapacity, int _currentAmmoCount)
        {
            magazineCapacity = _magazineCapacity;
            currentAmmoCount = _currentAmmoCount;
        }
    }
    [SerializeField] private Magazine[] magazines; // Array of magazines used by the weapon
    [SerializeField] public int maxMagazines = 10;
    #endregion

    #region Unity Methods


    // Serialized fields for Inspector Visibility

    // Events
    public event Action OnFire;
    public event Action OnReload;
    private Vector3 originalWeaponPosition;
    float origReloadTime;
    private void Awake()
    {
        // Initialize gun configuration
        currentGun = GetComponent<IGun>();
        if (currentGun != null)
        {
            currentGun.InitializeGun(this);
        }
        originalWeaponPosition = transform.localPosition;

        weaponAnimator = GetComponent<Animator>();
        weaponAnimator.SetBool("IsIdle", true); // Ensure IsIdle is true on start
        weaponAnimator.Play("Idle");

        currentDurability = currentGun.MaxDurability;
        ValidateMagazines();
        origReloadTime = currentGun.ReloadTime;
    }

    private void Update()
    {

        if (isReloading || isJammed) return;

        HandleFiring();
        HandleReloading();
        HandleFireModeSwitching();
        HandleIdleState();
        displayAmmo();
    }

    #endregion

    private void HandleIdleState()
    {
        // Check if the player is moving
        bool isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;

        // Check if the player is shooting, aiming, or reloading
        bool isIdle = !isMoving && !Input.GetButton("Fire1") && !Input.GetMouseButton(1) && !isReloading;

        // Set the animation param
        weaponAnimator.SetBool("IsIdle", isIdle);

        if (isIdle)
        {
            weaponAnimator.ResetTrigger("ReloadPistol");
        }

    }

    /// <summary>
    /// Validates the magazine count.
    /// </summary>
    private void ValidateMagazines()
    {
        if (magazines.Length == 0)
        {
            Debug.LogError("No magazines assigned to the weapon!");
        }
    }

    /// <summary>
    /// Handles input for firing the weapon.
    /// </summary>
    private void HandleFiring()
    {
        if (Input.GetButtonDown("Fire1") && weaponCanShoot)
        {
            if (magazines[currentMagazineIndex].currentAmmoCount > 0)
            {
                FireWeapon();
            }
            else
            {
                Debug.Log("Magazines empty! Reload or switch Magazines.");
            }
        }
    }

    private void FireWeapon()
    {
        // Check if enough time has passed before last shot
        if (Time.time - lastShotTime < shootDelay) return;

        // Update the last fire time
        lastShotTime = Time.time;

        if (!weaponCanShoot || isReloading) return; // Prevent fire during reload

        if (magazines[currentMagazineIndex].currentAmmoCount > 0)
        {
            weaponCanShoot = false; // Prevents immediate re-firing of weapon
            magazines[currentMagazineIndex].currentAmmoCount--; // reduce ammo acount

            // Set IsShooting to true to trigger shooting anim
            weaponAnimator.SetBool("IsShooting", true);

            DecreaseDurability();   // Apply durability loss
            ShootRaycastBullet();   // Perform raycast shooting
            StartCoroutine(ApplyRecoil());  // Apply recoil effect
            OnFire?.Invoke();   // Trigger the onfire event

            Debug.Log(magazines[currentMagazineIndex].currentAmmoCount);
        }
        StartCoroutine(ResetShootingState(currentGun.RateOfFire));
    }

    private IEnumerator DisableMuzzleFlashLight()
    {
        yield return new WaitForSeconds(0.2f);
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = false;
        }
    }

    private IEnumerator ResetShootingState(float weaponFireRate)
    {
        yield return new WaitForSeconds(weaponFireRate);
        weaponCanShoot = true;
        weaponAnimator.SetBool("IsShooting", false);
    }

    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && !weaponAnimator.GetBool("IsShooting"))
        {
            Debug.Log("Reload Triggered");
            weaponAnimator.SetTrigger("ReloadPistol");
            StartCoroutine(Reload());
        }
    }

    private void HandleFireModeSwitching()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            currentGun.SwitchFireMode();
            Debug.Log("Switched to fire mode: " + currentGun.CurrentFireMode);
        }
    }

    private IEnumerator ApplyRecoil()
    {
        Vector3 originalPos = transform.localPosition;

        transform.localPosition -= new Vector3(0, 0, currentGun.RecoilIntensity);

        yield return new WaitForSeconds(0.05f);

        transform.localPosition = originalPos;
    }

    private void ShootRaycastBullet()
    {
        Ray reticleRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(reticleRay, out hit, currentGun.ShootingDistance, canBeShotMask))
        {
            // spawn bullet trail
            if (bulletTrail != null)
            {
                TrailRenderer trail = Instantiate(bulletTrail, transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
            }
            // trigger hit effect
            if (shootImpactSystem != null)
            {
                Instantiate(shootImpactSystem, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        // trigger muzzle flash
        if (shootingSystem != null)
        {
            shootingSystem.Play();
        }

        // Activate muzzle flash light
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            StartCoroutine(DisableMuzzleFlashLight());
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        Instantiate(shootImpactSystem, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);

        weaponAnimator.SetBool("IsShooting", false);
    }

    private IEnumerator ResetShoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        weaponCanShoot = true;
    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < currentGun.BurstCount; i++)
        {
            if (magazines[currentMagazineIndex].currentAmmoCount > 0)
            {
                FireWeapon();
                magazines[currentMagazineIndex].currentAmmoCount--;
                yield return new WaitForSeconds(currentGun.BurstRate);
            }
            else
            {
                break;
            }
        }
        StartCoroutine(ResetShoot(currentGun.RateOfFire));

        StartCoroutine(ResetShootingState(currentGun.BurstRate * currentGun.BurstCount));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        reloadTime = origReloadTime;

        if (isReloading)
        {
            Debug.Log("Reloading...");
            // play reload anim
            weaponAnimator.SetTrigger("ReloadPistol");
            Coroutine fillCoroutine = StartCoroutine(fillWhileReloading());

            if (magazines[currentMagazineIndex].currentAmmoCount == 0)
            {
                gameManager.gameInstance.quickTime.SetActive(true);
                yield return StartCoroutine(quickTimeEvent());
                Debug.Log(qteSuccess);
                if (qteSuccess == true)
                {
                    Debug.Log("Entering");
                    gameManager.gameInstance.quickTime.SetActive(false);
                    magazines[currentMagazineIndex].currentAmmoCount = magazines[currentMagazineIndex].magazineCapacity;
                    Debug.Log("qteSucces");
                    if (fillCoroutine != null)
                    {
                        StopCoroutine(fillCoroutine);
                    }
                    completeReload();
                }
                else
                {
                    gameManager.gameInstance.quickTime.SetActive(false);
                    StopCoroutine(fillCoroutine);
                    StartCoroutine(fillWhileReloading());
                    yield return new WaitForSeconds(origReloadTime);
                    magazines[currentMagazineIndex].currentAmmoCount = magazines[currentMagazineIndex].magazineCapacity;

                }

            }
            else
            {
                yield return new WaitForSeconds(origReloadTime);
                magazines[currentMagazineIndex].currentAmmoCount = magazines[currentMagazineIndex].magazineCapacity;
            }

            isReloading = false;
            weaponCanShoot = true;
            
            if (Input.GetButton("Fire1"))
            {
                weaponAnimator.SetBool("IsShooting", true);
                FireWeapon();
            }
            else
            {
                weaponAnimator.ResetTrigger("ReloadPistol");
            }
        }
    }
    private void DecreaseDurability()
    {
        currentDurability -= currentGun.DurabilityLossPerShot;
        if (currentDurability < 0) currentDurability = 0;

        if (currentDurability == 0 && currentGun.CanJam)
        {
            isJammed = true;
            Debug.Log("Weapon is jammed! Clear the jam to continue firing.");
        }
    }

    public void ClearJam()
    {
        if (isJammed)
        {
            isJammed = false;
            Debug.Log("Weapon jam cleared!");
        }
    }

    public void HandleAmmoDrop()
    {
        if (getAmmoCount() < getMaxAmmoCount())
        {
            int amountNeeded = magazines[currentMagazineIndex].magazineCapacity - magazines[currentMagazineIndex].currentAmmoCount;

            magazines[currentMagazineIndex].currentAmmoCount += amountNeeded;
        }
        Debug.Log("Ammo filled");

    }

    public int getAmmoCount()
    {
        return magazines[currentMagazineIndex].currentAmmoCount;
    }

    public int getMaxAmmoCount()
    {
        return magazines[currentMagazineIndex].magazineCapacity;
    }

    public void displayAmmo()
    {
        gameManager.gameInstance.ammoCount.text = getAmmoCount().ToString("F0");
        gameManager.gameInstance.maxAmmoCount.text = getMaxAmmoCount().ToString("F0");
        gameManager.gameInstance.ammoCircle.fillAmount = (float)magazines[currentMagazineIndex].currentAmmoCount / (float)magazines[currentMagazineIndex].magazineCapacity;
    }

    public IEnumerator fillWhileReloading()
    {
        float elapsedTime = 0f;
        float startingFill = gameManager.gameInstance.ammoCircle.fillAmount;
        if(qteSuccess == false)
        {
            reloadTime = origReloadTime;
        }
        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = Mathf.Lerp(startingFill, 1f, elapsedTime / reloadTime);
            gameManager.gameInstance.ammoCircle.fillAmount = fillAmount;
            yield return null;
        }
        gameManager.gameInstance.ammoCircle.fillAmount = 1f;
    }

    public IEnumerator quickTimeEvent()
    {
        yield return new WaitForSeconds(delayBeforeQTE);

        float startQTE = Time.time; //start timer when event starts
        float endQTE = startQTE + windowForQTE;
        qteSuccess = false;

        while (Time.time < endQTE) // the timed window for when the player can press
        {
            //Debug.Log("CAN QUICK TIME");
            if (Input.GetKeyDown(KeyCode.F))
            {
                qteSuccess = true;
                break;
            }
            yield return null;
        }
        reloadTime = Time.time - startQTE;
        if(qteSuccess == false)
        {
            reloadTime = origReloadTime;
        }
        yield return qteSuccess;
    }

    public void completeReload()
    {
        OnReload?.Invoke();
        gameManager.gameInstance.ammoCircle.fillAmount = 1f;
    }

    public void SetGunConfiguration(IGun currentGun)
    {
        // Assign gun properties from the IGun interface to local variables in Weapon class
        availableFireModes = currentGun.AvailableFireModes;
        shootDistance = currentGun.ShootingDistance;
        weaponFireRate = currentGun.RateOfFire;
        recoilIntensity = currentGun.RecoilIntensity;
        recoilSpeed = currentGun.RecoilSpeed;
        maxDurability = currentGun.MaxDurability;
        durabilityLossPerShot = currentGun.DurabilityLossPerShot;
        canJam = currentGun.CanJam;
        burstCount = currentGun.BurstCount;
        burstRate = currentGun.BurstRate;

        // Assign systems and effects
        shootingSystem = currentGun.ShootingSystem;
        shootImpactSystem = currentGun.ShootImpactSystem;
        bulletTrail = currentGun.BulletTrail;
        muzzleFlashLight = currentGun.MuzzleFlash;
    }

    public void addMagazine(Magazine magazine, int numberOfMagazines)
    {
        int currentMagazineCount = magazines.Length;
        int avaliableSpace = maxMagazines - currentMagazineCount;
        int magazinesToAdd = avaliableSpace;

        if(magazinesToAdd > 0)
        {
            Magazine[] newMagazines = new Magazine[currentMagazineCount + magazinesToAdd];
            //copying array to the new magazines 
            System.Array.Copy(magazines, newMagazines, currentMagazineCount);
            //make a new amount mag based on gun magazine and max magazine count
            for(int i = 0; i < magazinesToAdd; i++)
            {
                newMagazines[currentMagazineCount + i] = new Magazine(magazine.magazineCapacity, magazine.currentAmmoCount);
            }
            magazines = newMagazines;
        }
        else
        {
            Debug.Log("At max Ammo");
        }
    }
}
