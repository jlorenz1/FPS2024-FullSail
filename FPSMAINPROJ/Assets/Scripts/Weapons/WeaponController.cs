using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] LayerMask ignoreMask;

    [Header("WEAPON MODEL/POSTIONS")]
    [SerializeField] public List<weaponStats> gunList = new List<weaponStats>();

    [SerializeFeild] public List<Vector3> RecoilPattern;

    [SerializeFeild] GameObject currentWeaponInstance;
    [SerializeFeild] public Transform gunModel;
    [SerializeField] public Transform muzzleFlashTransform;
    [SerializeField] public Transform casingSpawnTransform;
    [SerializeField] Transform gunTransform;

    [Header("WEAPON SPECIALTIES")]
    [SerializeField] public float shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    [SerializeFeild] string fireMode;

    [Header("WEAPON VFX")]
    [SerializeField] public GameObject muzzleFlash;
    [SerializeField] public GameObject casingEffect;
    [SerializeFeild] public TrailRenderer bulletTrail;

    private cameraController cameraScript;
    int selectedGun;
    public bool isReloading = false;
    public bool isShooting;
    private weaponStats currGun;
    int currentPatternIndex = 0;
    public bool sprayPattern = false;
   
    void Start()
    {
        cameraScript = FindObjectOfType<cameraController>();
    }

    void Update()
    {
        if (!gameManager.gameInstance.gameIsPaused)
        {
            selectGun();
        }


        if (Input.GetKeyDown(KeyCode.R) && gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount < gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].magazineCapacity)
        {
            if (!isReloading)
            {
                isReloading = true;
                StartCoroutine(reload());
            }
        }

        if (gunList.Count >= 1)
        {
            displayAmmo();
        }

        if (gunList.Count > 0)
        {
            if (gunList[selectedGun].fireMode == "Full-Auto")
            {
                handleFullAuto();
            }
            else if (gunList[selectedGun].fireMode == "Semi-Auto")
            {
                handleSemiAuto();
            }
        }

        if(!isShooting)
        {
            currentPatternIndex = 0;
        }

    }

    void handleFullAuto()
    {
        if (Input.GetButton("Fire1") && gunList.Count > 0 && !isShooting)
        {
            StartCoroutine(shoot());
        }
    }

    void handleSemiAuto()
    {
        if (Input.GetButtonDown("Fire1") && gunList.Count > 0 && !isShooting)
        {
            StartCoroutine(shoot());
        }
    }


    IEnumerator shoot()
    {

        if (selectedGun >= 0 && selectedGun < gunList.Count)
        {
            if (gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount >= 1 && !isReloading)
            {
                UnityEngine.Debug.Log(gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount);
                gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount--;
                isShooting = true;
                //StartCoroutine(flashMuzzel());
                AudioManager.audioInstance.playAudio(gunList[selectedGun].shootSound[UnityEngine.Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVol);
                var muzzleFlashObj = Instantiate(muzzleFlash, muzzleFlashTransform.position, Quaternion.identity);
                muzzleFlashObj.gameObject.transform.SetParent(muzzleFlashTransform);
                Instantiate(casingEffect, casingSpawnTransform.position, casingSpawnTransform.rotation);
                cameraScript.RecoilFire();
                RaycastHit hit;

                Vector3 direction = getDirection();


                if (Physics.Raycast(Camera.main.transform.position, direction, out hit, shootDistance, ~ignoreMask))
                {
                    TrailRenderer trail = Instantiate(bulletTrail, muzzleFlashTransform.position, Quaternion.identity);
                    StartCoroutine(spawnTrail(trail, hit));
                    Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
                    IEnemyDamage dmg = hit.collider.GetComponentInParent<IEnemyDamage>();
                    if (dmg != null)
                    {
                        float actualDamage = shootDamage; // Start with the base damage

                        // Example condition to modify the damage
                        if (hit.collider.CompareTag("Zombie Head"))
                        {
                            actualDamage *= 2.0f; // Double damage for headshots
                            Debug.Log("crit shot");
                        }
                        else if (hit.collider.CompareTag("Zombie Body"))
                        {
                            actualDamage *= 1.0f; // Normal damage for body shots
                            Debug.Log("body shot");
                        }
                        else if (hit.collider.CompareTag("Zombie Legs"))
                        {
                            actualDamage *= 0.25f; // Reduced damage for leg shots
                            Debug.Log("leg shot");
                            dmg.cutspeed(2, actualDamage);
                        }
                        else if (hit.collider.CompareTag("Zombie Arms"))
                        {
                            actualDamage *= 0.25f;
                            dmg.cutdamage(2);
                        }

                        // Apply the modified damage
                        dmg.takeDamage(actualDamage);
                        Instantiate(gunList[selectedGun].zombieHitEffect, hit.point, Quaternion.identity);
                    }
                    else
                    {
                        Debug.Log("Hit Tag: " + hit.collider.tag);
                        Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
                    }

                }
                yield return new WaitForSeconds(shootRate);

                isShooting = false;
            }
            else
            {
                UnityEngine.Debug.Log("need to reload");

            }
            
        }
        else
        {
            UnityEngine.Debug.LogError("selectedGun index out of range");
        }

    }

    Vector3 getDirection()
    {

        Vector3 direction = Camera.main.transform.forward;

        if (sprayPattern)
        {

            direction += new Vector3(
            RecoilPattern[currentPatternIndex].x,
            RecoilPattern[currentPatternIndex].y,
            RecoilPattern[currentPatternIndex].z
            );
            direction.Normalize();
        }
        if(isShooting)
        {
            currentPatternIndex = (currentPatternIndex + 1) % RecoilPattern.Count;
        }
        return direction;
    }

    IEnumerator reload()
    {

        AudioManager.audioInstance.playAudio(gunList[selectedGun].reloadSound, gunList[selectedGun].reloadVol);
        yield return new WaitForSeconds(gunList[selectedGun].reloadTime);
        //StartCoroutine(fillWhileReloading());
        //checks if there are mags to reload with

        if (gunList[selectedGun].currentMagazineIndex + 1 < gunList[selectedGun].magazines.Length)
        {
            gunList[selectedGun].currentMagazineIndex++;
            gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount = gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].magazineCapacity;
        }
        else
        {
            UnityEngine.Debug.Log("No more mags!");
        }
        isReloading = false;

    }

    IEnumerator spawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition,  hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }

    public void displayAmmo()
    {
        int amountToDisplay = 0;
        gameManager.gameInstance.ammoCount.text = gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount.ToString("F0");
        for (int i = 0; i < gunList[selectedGun].magazines.Length; i++)
        {
            amountToDisplay += gunList[selectedGun].magazines[i].currentAmmoCount;
        }
        //UnityEngine.Debug.Log(amountToDisplay);
        gameManager.gameInstance.maxAmmoCount.text = amountToDisplay.ToString("F0");

    }

    public void getWeaponStats(weaponStats gun)
    {
    
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
        currentWeaponInstance = Instantiate(gun.gunModel);
        gunModel = currentWeaponInstance.transform;

        gunModel.SetParent(gunTransform);
        gunModel.localPosition = Vector3.zero;
        gunModel.localRotation = Quaternion.identity;
        muzzleFlashTransform = gunModel.Find("MuzzleTransform");
        muzzleFlash = gun.muzzleFlash;
        gunList.Add(gun);

        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        shootDistance = gun.shootingDistance;
        shootRate = gun.shootRate;
        fireMode = gun.fireMode;
        //recoil
        cameraScript.recoilX = gun.recoilX;
        cameraScript.recoilY = gun.recoilY;
        cameraScript.recoilZ = gun.recoilZ;
        cameraScript.returnSpeed = gun.returnSpeed;
        cameraScript.snapping = gun.snapping;

        RecoilPattern = new List<Vector3>(gun.RecoilPattern);
       
        //gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        //gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {

            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {

            selectedGun--;
            changeGun();
        }

    }
    void changeGun()
    {
        currGun = gunList[selectedGun];
        shootDamage = currGun.shootDamage;
        shootDistance = currGun.shootingDistance;
        shootRate = currGun.shootRate;
        fireMode = currGun.fireMode;

        cameraScript.recoilX = currGun.recoilX;
        cameraScript.recoilY = currGun.recoilY;
        cameraScript.recoilZ = currGun.recoilZ;
        cameraScript.returnSpeed = currGun.returnSpeed;
        cameraScript.snapping = currGun.snapping;

        RecoilPattern = new List<Vector3>(currGun.RecoilPattern);

        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }
        currentWeaponInstance = Instantiate(currGun.gunModel);
        gunModel = currentWeaponInstance.transform;
        muzzleFlashTransform = gunModel.Find("MuzzleTransform");
        muzzleFlash = currGun.muzzleFlash;
        gunModel.SetParent(gunTransform);
        gunModel.localPosition = Vector3.zero;
        gunModel.localRotation = Quaternion.identity;

    }
}
