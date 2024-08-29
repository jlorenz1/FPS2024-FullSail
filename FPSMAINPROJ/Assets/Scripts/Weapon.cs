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
    #region Weapon Fire Mode
    public enum FireMode
    {
        Single,
        Burst,
        FullAuto
    }
    #endregion

    #region Serialized Fields

    [Header("----- Weapon Settings -----")]
    [Tooltip("The LayerMask used to determine which objects the bullets can hit.")]
    [SerializeField] private LayerMask canBeShotMask;

    [Header("----- Gun Stats -----")]
    [Tooltip("Gun stats scriptable object containing weapon configuration.")]
    [SerializeField] private GunStats gunStats;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootLocation;
    #endregion

    #region Private Fields

    private FireMode[] availableFireModes;
    private float shootDistance, weaponFireRate, recoilIntensity, recoilSpeed, lastShotTime, maxDurability, currentDurability, durabilityLossPerShot, burstRate, reloadTime;
    private int shootDamage, burstCount, ammoToReload;
    private bool canJam;
    private bool weaponCanShoot = true;             // Indicates if the weapon is ready to shoot
    private bool isReloading = false;              // Indicates if the weapon is currently reloading
    private bool isJammed = false;                // Indicates if the weapon is jammed
    private int currentMagazineIndex = 0;        // Tracks the current magazine being used
    private float shootDelay = 0.05f;         // Prevents weapon from being immediately fired
    private bool qteSuccess = false;      // Tracks success of Quick Time Event (QTE)
    private bool isShooting = false;
    private string gunName;
    private Animator weaponAnimator;
    private AnimationClip reloadAnimation;
    private AnimationClip shootAnimation;
    private AnimationClip idleAnimation;

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

    private Vector3 originalWeaponPosition;
    #endregion

    #region Unity Methods


    // Serialized fields for Inspector Visibility

    // Events
    public event Action OnFire;
    public event Action OnReload;


    private void Awake()
    {
        SetGunConfiguration(gunStats);
        originalWeaponPosition = transform.position;

        weaponAnimator = GetComponent<Animator>();

        if (gunStats != null)
        {
            SetGunConfiguration(gunStats);
        }

        ValidateMagazines();
        reloadTime = gunStats.reloadAnimation.length;
    }

    private void Update()
    {
        CanShoot();

        if (isReloading || isJammed) return;

        HandleWeaponStates();


    }

    #endregion

    #region Init Method
    public void SetGunConfiguration(GunStats gun)
    {
        gunStats = gun;

        gunName = gunStats.gunName;
        shootDamage = gunStats.shootDamage;
        weaponFireRate = gunStats.rateOfFire;
        recoilIntensity = gunStats.recoilIntensity;
        recoilSpeed = gunStats.recoilSpeed;
        maxDurability = gunStats.maxDurability;
        durabilityLossPerShot = gunStats.durabilityLossPerShot;
        canJam = gunStats.canJam;
        burstCount = gunStats.burstCount;
        
        availableFireModes = gunStats.availableFireModes;

        if (weaponAnimator != null && gunStats.animatorController != null)
        {
            weaponAnimator.runtimeAnimatorController = gunStats.animatorController;
        }
        reloadAnimation = gunStats.reloadAnimation;
        shootAnimation = gunStats.shootingAnimation;
        idleAnimation = gunStats.idleAnimation;
    }
    private void ValidateMagazines()
    {
        if (magazines.Length == 0)
        {
            Debug.LogError("No magazines assigned to the weapon!");
        }
    }
    #endregion

    #region State Detection Methods
    public bool CanShoot()
    {
        if (!isReloading && !isJammed && weaponCanShoot)
            return true;
        else
            return false;
    }

    private IEnumerator ResetShoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        weaponCanShoot = true;
    }

    private IEnumerator ResetShootingState()
    {
        StopAllCoroutines();
        yield return new WaitForSeconds(gunStats.rateOfFire);
        isShooting = false;
        weaponCanShoot = true;
    }

    private void ResetShootingStatetwo()
    {
       
        isShooting = false;
        weaponCanShoot = true;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    #endregion

    #region UpdateHandling Methods
    /// <summary>
    /// Handles input for firing the weapon.
    /// </summary>
    public void HandleFiring()
    {
        if (gameObject.activeSelf)
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

    private void HandleWeaponStates()
    {
        if (gunStats.gunName == "Pistol")
        {

            if (Input.GetButtonDown("Fire1") && !isShooting && !isReloading)
            {
                StartCoroutine(PlayShootAnimation("pistolShoot", 0.05f));
            }
            else if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                StartCoroutine(PlayReloadAnimation("pistolReload", 0.25f));
            }
            else if (!isShooting && !isReloading)
            {
                PlayIdleAnimation("pistolIdle", 0.15f);
            }
        }
        else if (gunStats.gunName == "AssaultRifle")
        {
            if (Input.GetButtonDown("Fire1") && !isShooting && !isReloading)
            {
                StartCoroutine(PlayShootAnimation("assaultRifleShoot", 0.05f));
            }
            else if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                StartCoroutine(PlayReloadAnimation("assaultRifleReload", 0.2f));
            }
            else if (!isShooting && !isReloading)
            {
                PlayIdleAnimation("assaultRifleIdle", 0.2f);
            }
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

    public void HandleFireModeSwitching()
    {
        
        
            int currentIndex = Array.IndexOf(availableFireModes, gunStats.currentFireMode);
            gunStats.currentFireMode = availableFireModes[(currentIndex + 1) % availableFireModes.Length];
            Debug.Log("Switched fire mode to: " + gunStats.currentFireMode);
        
    }

    public void HandleReloading()
    {
       
            Debug.Log("Reload Triggered");

            StartCoroutine(Reload());
        
    }

    #endregion

    #region Shooting Methods

    public void ShootRaycastBullet()
    {

        GameObject bullet = Instantiate(bulletPrefab, shootLocation.position, shootLocation.rotation);


        Ray reticleRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(reticleRay, out hit, gunStats.shootingDistance, canBeShotMask))
        {
            // Apply damage
            IDamage target = hit.collider.gameObject.GetComponent<IDamage>();
            if (target != null)
            {
                target.takeDamage(gunStats.shootDamage);
            }

            // VFX effects



        }

        // Trigger shooting system (muzzle flash, sounds, etc)

    }

    public void FireWeapon()
    {
        // Check if enough time has passed before last shot
        if (Time.time - lastShotTime < shootDelay) return;

        // Update the last fire time
        lastShotTime = Time.time;

        if (!weaponCanShoot || isReloading) return; // Prevent fire during reload

        if (magazines[currentMagazineIndex].currentAmmoCount > 0)
        {
            isShooting = true;
            weaponCanShoot = false; // Prevents immediate re-firing of weapon
            magazines[currentMagazineIndex].currentAmmoCount--; // reduce ammo acount

            DecreaseDurability();   // Apply durability loss
            ShootRaycastBullet();   // Perform raycast shooting
            StartCoroutine(ApplyRecoil());  // Apply recoil effect
            OnFire?.Invoke();   // Trigger the onfire event

            Debug.Log(magazines[currentMagazineIndex].currentAmmoCount);
        }
        ResetShootingStatetwo();


    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < gunStats.burstCount; i++)
        {
            if (magazines[currentMagazineIndex].currentAmmoCount > 0)
            {
                FireWeapon();
                magazines[currentMagazineIndex].currentAmmoCount--;
                yield return new WaitForSeconds(gunStats.burstRate);
            }
            else
            {
                break;
            }
        }
        StartCoroutine(ResetShoot(gunStats.rateOfFire));

        StartCoroutine(ResetShootingState());
    }

    #endregion

    #region Reload Methods

    private IEnumerator Reload()
    {
        isReloading = true;
        reloadTime = gunStats.reloadAnimation.length;

        if (isReloading)
        {
            Debug.Log("Reloading...");
            // play reload anim

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
                    yield return new WaitForSeconds(reloadTime);
                    magazines[currentMagazineIndex].currentAmmoCount = magazines[currentMagazineIndex].magazineCapacity;

                }

            }
            else
            {
                yield return new WaitForSeconds(reloadTime);
                magazines[currentMagazineIndex].currentAmmoCount = magazines[currentMagazineIndex].magazineCapacity;
            }

            isReloading = false;
            weaponCanShoot = true;
        }
    }

    public IEnumerator fillWhileReloading()
    {
        float elapsedTime = 0f;
        float startingFill = gameManager.gameInstance.ammoCircle.fillAmount;
        if (qteSuccess == false)
        {
            reloadTime = gunStats.reloadAnimation.length;
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
        yield return new WaitForSeconds(gunStats.qteDelay);

        float startQTE = Time.time; //start timer when event starts
        float endQTE = startQTE + gunStats.qteWindow;
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
        if (qteSuccess == false)
        {
            reloadTime = gunStats.reloadAnimation.length;
        }
        yield return qteSuccess;
    }

    public void completeReload()
    {
        OnReload?.Invoke();
        gameManager.gameInstance.ammoCircle.fillAmount = 1f;
    }


    #endregion

    #region Animation Methods

    private IEnumerator PlayShootAnimation(string animationName, float customBlendTime)
    {
        isShooting = true;
        PlayAnimation(animationName, customBlendTime);
        yield return new WaitForSeconds(gunStats.rateOfFire);
        isShooting = false;

    }

    private IEnumerator PlayReloadAnimation(string animationName, float customBlendTime)
    {
        isReloading = true;
        PlayAnimation(animationName, customBlendTime);
        yield return new WaitForSeconds(3f);
        isReloading = false;
    }

    private void PlayIdleAnimation(string animationName, float customBlendTime)
    {
        PlayAnimation(animationName, customBlendTime);
    }

    private void PlayAnimation(string animationName, float customBlendTime)
    {
        if (weaponAnimator != null)
        {
            weaponAnimator.CrossFadeInFixedTime(animationName, customBlendTime);
        }
    }

    #endregion

    #region Recoil Methods

    private IEnumerator ApplyRecoil()
    {
        Vector3 originalPos = transform.localPosition;

        transform.localPosition -= new Vector3(0, 0, gunStats.recoilIntensity);

        yield return new WaitForSeconds(0.05f);

        transform.localPosition = originalPos;
    }

    #endregion

    #region Durability/Jamming Methods
    private void DecreaseDurability()
    {
        currentDurability -= gunStats.durabilityLossPerShot;
        if (currentDurability < 0) currentDurability = 0;

        if (currentDurability == 0 && gunStats.canJam)
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

    #endregion

    #region Ammo Methods
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
    #endregion
}
