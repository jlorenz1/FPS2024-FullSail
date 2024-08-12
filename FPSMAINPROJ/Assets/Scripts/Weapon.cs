using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    // The prefab of the bullet that will be instantiated and fired
    public GameObject bulletPrefab;

    // The point from which the bullet will be spawned (typically the muzzle of the weapon)
    public Transform bulletSpawn;

    // The velocity at which the bullet will be fired
    public float bulletVelocity = 30f;

    // The lifetime of the bullet before it is automatically destroyed
    public float bulletPrefabLifeTime = 3.0f;

    // Boolean flag to determine if the weapon is ready to shoot again
    private bool weaponCanShoot = true;

    // The rate of fire (time delay between shots)
    public float weaponFireRate = 0.1f;

    // Update is called once per frame to check for player input
    void Update()
    {
        // Check if the player is holding down the left mouse button (Mouse0) and if the weapon is ready to shoot
        if (Input.GetButton("Fire1") && weaponCanShoot)
        {
            // Call the method to handle single-shot firing
            FireSingleShot();
        }
    }

    // Handles firing a single shot
    private void FireSingleShot()
    {
        // Set the weapon to not be able to shoot until the delay passes
        weaponCanShoot = false;

        // Call the method to instantiate and shoot the bullet
        ShootBullet();

        // Start the coroutine to reset the shooting ability after the specified delay
        StartCoroutine(ResetShoot(weaponFireRate));
    }

    // Method to handle bullet instantiation, force application, and destruction
    private void ShootBullet()
    {
        // Offset the bullet's spawn position slightly forward from the spawn point
        Vector3 spawnPosition = bulletSpawn.position + bulletSpawn.forward * 0.2f; // Adjust the 0.2f value as needed

        // Instantiate the bullet prefab at the adjusted position and rotation
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, bulletSpawn.rotation);

        // Get the Rigidbody component attached to the bullet for physics calculations
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Apply a force to the bullet in the forward direction to simulate shooting
            rb.AddForce(bulletSpawn.forward * bulletVelocity, ForceMode.Impulse);
        }

        // Start a coroutine to destroy the bullet after its lifetime has elapsed
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    // Coroutine to destroy the bullet after a set amount of time
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Destroy the bullet GameObject to free up resources
        Destroy(bullet);
    }

    // Coroutine to reset the shooting ability after the fire rate delay has passed
    private IEnumerator ResetShoot(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Allow the weapon to shoot again
        weaponCanShoot = true;
    }
}
