using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{

    // Bullet-related variables
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletVelocity = 30f;
    [SerializeField] private float bulletPrefabLifeTime = 3.0f;
    [SerializeField] private float shootDistance;

    // Weapon firing mode and rate of fire
    public enum FireMode { Single, Burst, FullAuto, Sniper }
    [SerializeField] private FireMode fireMode = FireMode.Single;
    [SerializeField] private float weaponFireRate = 0.1f;
    private bool weaponCanShoot = true;
    private bool qteSuccess = false;
    [SerializeField] float delayBeforeQTE;
    [SerializeField] float windowForQTE;

    // Weapon ADS Mode variables
    [SerializeField] private float adsFOV = 35f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float adsSpeed = 10f;
    [SerializeField] private Transform adsWeaponPosition;
    [SerializeField] private Transform normalWeaponPosition;
    private bool isAiming = false;
    [SerializeField] private float ADSoffsetX = 0f;
    [SerializeField] private float ADSoffsetY = 0f;
    [SerializeField] private float ADSoffsetZ = 0f;

    // Weapon Recoil variables
    [SerializeField] private float recoilIntensity = 0.1f;
    [SerializeField] private float recoilSpeed = 5f;

    // Magazine and ammo management
    [System.Serializable]
    public class Magazine
    {
        public int magazineCapacity = 30;
        public int currentAmmoCount = 30;

    }

    [SerializeField] public Magazine[] magazines;
    public int currentMagazineIndex;

    // Reloading mechanics
    public Animation pistolReloadAnim;
    [SerializeField] private float reloadTime = 2f;
    private bool isReloading = false;
    int ammoToReload;
    // Add a timer feature to the reload mechanic if the user presses reload again within certain timeframe it achieves a perfect reload with no reload wait time

    // Burst fire specifics
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstRate = 0.1f;


    // Weapon durability
    [SerializeField] private float maxDurability = 100f;
    private float currentDurability;
    [SerializeField] private float durabilityLossPerShot = 1f;
    [SerializeField] private bool canJam = true;
    private bool isJammed = false;

    // Events
    public event Action OnFire;
    public event Action OnReload;
    private Vector3 originalWeaponPosition;
    float origReloadTime;
    private void Start()
    {
        currentDurability = maxDurability;
        ValidateMagazines();
        origReloadTime = reloadTime;
        // Store weapon pos
        originalWeaponPosition = transform.localPosition;


    }

    private void Update()
    {
        if (isReloading || isJammed) return;

        HandleFiring();
        HandleReloading();
        HandleFireModeSwitching();
        HandleAiming();
        displayAmmo();
    }

    private void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }

        // Smoothly transition FOV
        float targetFOV = isAiming ? adsFOV : normalFOV;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * adsSpeed);

        // Smoothly transition weapon position
        if (isAiming)
        {
            // Get target screen position
            Vector3 centerOfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            // Convert screen position to world position based on the weapon's dist
            float distanceFromCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
            Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(centerOfScreen.x, centerOfScreen.y, distanceFromCamera));

            // Convert world pos to a local pos relative to the weapon
            Vector3 targetLocalPos = transform.parent.InverseTransformPoint(targetWorldPos);

            // Apply the offsets
            targetLocalPos += new Vector3(ADSoffsetX, ADSoffsetY, ADSoffsetZ);
            targetLocalPos.z = ADSoffsetZ;

            // Lerp weapon pos to match target pos
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * adsSpeed);

        }
        else
        {
            // Return weapon to original pos
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalWeaponPosition, Time.deltaTime * adsSpeed);
        }
    }

    private void ValidateMagazines()
    {
        if (magazines.Length == 0)
        {
            Debug.LogError("No magazines assigned to the weapon!");
        }
    }

    private void HandleFiring()
    {
        if (Input.GetButton("Fire1") && weaponCanShoot)
        {
            if (magazines[currentMagazineIndex].currentAmmoCount > 0)
            {
                FireWeapon();
            }
            else
            {
                Debug.Log("Magazine empty! Reload or switch magazines.");
            }
        }
    }

    private void FireWeapon()
    {
        weaponCanShoot = false;
        magazines[currentMagazineIndex].currentAmmoCount--;
        DecreaseDurability();
        ShootBullet();
        Debug.Log(magazines[currentMagazineIndex].currentAmmoCount);
        StartCoroutine(ApplyRecoil());
        OnFire?.Invoke();

        // Apply recoil

        switch (fireMode)
        {
            case FireMode.Single:
                StartCoroutine(ResetShoot(weaponFireRate));
                break;
            case FireMode.Burst:
                StartCoroutine(FireBurst());
                break;
            case FireMode.FullAuto:
                StartCoroutine(ResetShoot(weaponFireRate));
                break;
            case FireMode.Sniper:
                StartCoroutine(ResetShoot(weaponFireRate * 3)); // Example: Sniper has a longer delay
                break;
        }
    }

    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    private void HandleFireModeSwitching()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            fireMode = (FireMode)(((int)fireMode + 1) % System.Enum.GetValues(typeof(FireMode)).Length);
            Debug.Log("Switched to fire mode: " + fireMode);
        }
    }

    private IEnumerator ApplyRecoil()
    {
        Vector3 originalPos = transform.localPosition;

        transform.localPosition -= new Vector3(0, 0, recoilIntensity);

        yield return new WaitForSeconds(0.05f);

        transform.localPosition = originalPos;
    }

    private void ShootBullet()
    {
        Ray reticleRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(reticleRay, out hit, shootDistance))
        {
            targetPoint = hit.point; // If you hit something within shoot distance aim at the hit point
        }
        else
        {
            targetPoint = reticleRay.GetPoint(shootDistance); // if nothing hit, aim at max distance
        }

        Vector3 shootDirection = (targetPoint - bulletSpawn.position).normalized;

        float distanceToTarget = Vector3.Distance(bulletSpawn.position, targetPoint);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletVelocity;

            // Calculate time needed for bullet to reach target based on velocity and distance
            float timeToTarget = distanceToTarget / bulletVelocity;

            StartCoroutine(DestroyBulletAfterTime(bullet, Mathf.Min(timeToTarget, bulletPrefabLifeTime)));
        }

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator ResetShoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        weaponCanShoot = true;
    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (magazines[currentMagazineIndex].currentAmmoCount > 0)
            {
                ShootBullet();
                magazines[currentMagazineIndex].currentAmmoCount--;
                yield return new WaitForSeconds(burstRate);
            }
            else
            {
                break;
            }
        }
        StartCoroutine(ResetShoot(weaponFireRate));
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
            Coroutine fillCoroutine = StartCoroutine(fillWhileReloading());
            if (magazines[currentMagazineIndex].currentAmmoCount == 0 && qteSuccess == false)
            {
                gameManager.gameInstance.quickTime.SetActive(true);
                yield return StartCoroutine(quickTimeEvent());

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
        }
    }
    private void DecreaseDurability()
    {
        currentDurability -= durabilityLossPerShot;
        if (currentDurability < 0) currentDurability = 0;

        if (currentDurability == 0 && canJam)
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
}
