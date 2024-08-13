using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    // Bullet-related variables
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float bulletVelocity = 30f;
    [SerializeField] private float bulletPrefabLifeTime = 3.0f;

    // Weapon firing mode and rate of fire
    public enum FireMode { Single, Burst, FullAuto, Sniper }
    [SerializeField] private FireMode fireMode = FireMode.Single;
    [SerializeField] private float weaponFireRate = 0.1f;
    private bool weaponCanShoot = true;

    // Magazine and ammo management
    [System.Serializable]
    public class Magazine
    {
        public int magazineCapacity = 30;
        public int currentAmmoCount = 30;
    }

    [SerializeField] private Magazine[] magazines;
    private int currentMagazineIndex;

    // Reloading mechanics
    [SerializeField] private float reloadTime = 2f;
    private bool isReloading = false;
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

    private void Start()
    {
        currentDurability = maxDurability;
        ValidateMagazines();
    }

    private void Update()
    {
        if (isReloading || isJammed) return;

        HandleFiring();
        HandleReloading();
        HandleFireModeSwitching();
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

        OnFire?.Invoke();

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

    private void ShootBullet()
    {
        Vector3 spawnPosition = bulletSpawn.position + bulletSpawn.forward * 0.2f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, bulletSpawn.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(bulletSpawn.forward * bulletVelocity, ForceMode.Impulse);
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
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = magazines[currentMagazineIndex].magazineCapacity - magazines[currentMagazineIndex].currentAmmoCount;
        magazines[currentMagazineIndex].currentAmmoCount += ammoToReload;

        OnReload?.Invoke();

        isReloading = false;
        weaponCanShoot = true;
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
}
