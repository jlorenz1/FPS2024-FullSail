using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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
    [SerializeField] public Transform ArmTransform;
    [SerializeField] Transform gunTransform;
    [SerializeField] Transform emptyTransform;

    [Header("WEAPON SPECIALTIES")]
    [SerializeField] public float shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    [SerializeFeild] string fireMode;

    [Header("HEKA SPECIALTIES")]
    [SerializeField] public GameObject hekaAbility;
    GameObject hekaMuzzleFlash;
    int hekaShootRate = 0;
    float hekaManaAmount = 0;
    public bool hasHeka = false;

    [Header("WEAPON VFX")]
    [SerializeField] public GameObject muzzleFlash;
    [SerializeField] public GameObject casingEffect;
    [SerializeFeild] public TrailRenderer bulletTrail;

    [SerializeField] public AudioSource weaponSource;

    private cameraController cameraScript;
    int selectedGun;
    public bool isReloading = false;
    public bool isShooting;
    private weaponStats currGun;
    int currentPatternIndex = 0;
    public bool sprayPattern = false;
    private PlayerController playerController;
    public bool hasTempest = false;
    public bool hasEclipse = false;
    public bool hasFloods = false;
   
    void Start()
    {
        cameraScript = FindObjectOfType<cameraController>();
        playerController = gameManager.gameInstance.playerScript;

        if(hekaAbility != null)
        {
            hasHeka = true;
        }
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
            gameManager.gameInstance.AmmoHUD.gameObject.SetActive(true);
            
        }
        else if (gunList.Count == 0)
            gameManager.gameInstance.AmmoHUD.gameObject.SetActive(false);


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
            if (gunList[selectedGun].hekaSchool.Length > 0 && hekaAbility != null)
            {
                handleHeka();

            }
           

        }

        if(!isShooting)
        {
            currentPatternIndex = 0;
        }

    }

    void handleFullAuto()
    {
        if (Input.GetButton("Fire1") && gunList.Count > 0 && !isShooting && !playerController.isSprinting)
        {
            StartCoroutine(shoot());
        }
    }

    void handleSemiAuto()
    {
        if (Input.GetButtonDown("Fire1") && gunList.Count > 0 && !isShooting && !playerController.isSprinting)
        {
            StartCoroutine(shoot());
        }
    }

    void handleHeka()
    {
        if (Input.GetButtonDown("Fire2") && hasHeka && !isShooting && !playerController.isSprinting)
        {
            if (gunList[selectedGun].hekaSchool == "Electricity")
            {
                StartCoroutine(shootElectricity());
            }
            else if (gunList[selectedGun].hekaSchool == "Darkness")
            {
                StartCoroutine(shootDarkness());
            }
            else if (gunList[selectedGun].hekaSchool == "Floods")
            {
                StartCoroutine(shootFloods());
            }
        }
    }    

    IEnumerator shootElectricity()
    {
        if (Input.GetButtonDown("Fire2") && hasHeka && !isShooting && gameManager.gameInstance.playerScript.currentMana > hekaManaAmount) 
        {
            playHekaEffects();
            gameManager.gameInstance.playerScript.mana(hekaManaAmount);
        }
        yield return new WaitForSeconds(hekaShootRate);
    }
    IEnumerator shootDarkness()
    {
        if(Input.GetButtonDown("Fire2") && hasHeka && !isShooting && gameManager.gameInstance.playerScript.currentMana > hekaManaAmount)
        {
            playHekaEffects();
            gameManager.gameInstance.playerScript.mana(hekaManaAmount);
        }
        yield return new WaitForSeconds(hekaShootRate);
    }

    IEnumerator shootFloods()
    {
        if (Input.GetButtonDown("Fire2") && hasHeka && !isShooting && gameManager.gameInstance.playerScript.currentMana > hekaManaAmount)
        {
            playHekaEffects();
            gameManager.gameInstance.playerScript.mana(hekaManaAmount);
        }
        yield return new WaitForSeconds(hekaShootRate);
    }

    void playHekaEffects()
    {
        Vector3 targetPoint = getMiddleOfScreen();
        muzzleFlashTransform.LookAt(targetPoint);
        GameObject projectile = Instantiate(hekaAbility, muzzleFlashTransform.position, muzzleFlashTransform.rotation);
        var muzzleFlashObj = Instantiate(hekaMuzzleFlash, muzzleFlashTransform.position, Quaternion.identity);
        AudioManager.audioInstance.playSFXAudio(gunList[selectedGun].hekaShootingSounds[UnityEngine.Random.Range(0, gunList[selectedGun].hekaShootingSounds.Length)], gunList[selectedGun].hekaShootVol);
        muzzleFlashObj.gameObject.transform.SetParent(muzzleFlashTransform);
    }

    IEnumerator shoot()
    {

        if (selectedGun >= 0 && selectedGun < gunList.Count)
        {
            displayCurrentAmmo();
            if (gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount >= 1 && !isReloading)
            {
                UnityEngine.Debug.Log(gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount);
                gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount--;
                isShooting = true;
                //StartCoroutine(flashMuzzel());
                AudioManager.audioInstance.playSFXAudio(gunList[selectedGun].shootSound[UnityEngine.Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVol);
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
                    IEnemyDamage dmg = hit.collider.GetComponent<IEnemyDamage>();

                    IDamage armDmg = hit.collider.gameObject.GetComponent<IDamage>();
                    if (dmg != null)
                    {
                        float actualDamage = shootDamage; // Start with the base damage
/*
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
                        }*/

                        // Apply the modified damage
                        dmg.takeDamage(actualDamage);
                        ParticleSystem bloodEffect = Instantiate(gunList[selectedGun].zombieHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        bloodEffect.transform.SetParent(hit.collider.gameObject.transform);
                    }
                    else if(armDmg != null)
                    {
                        armDmg.takeDamage(shootDamage);
                    }
                    else
                    {
                        ParticleSystem enviormentEffect = Instantiate(gunList[selectedGun].enviormentEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        StartCoroutine(spawnBulletHole(hit));
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


    Vector3 getMiddleOfScreen()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(65);
        }

        return targetPoint;
    }
    Vector3 getDirection()
    {

        Vector3 direction = Camera.main.transform.forward;
        
        if (sprayPattern)
        {
            Vector3 Recoildirection = new Vector3(
            RecoilPattern[currentPatternIndex].x,
            RecoilPattern[currentPatternIndex].y,
            RecoilPattern[currentPatternIndex].z
            );
           direction += Camera.main.transform.TransformDirection(Recoildirection);
        }
        if (isShooting)
        {
            currentPatternIndex = (currentPatternIndex + 1) % RecoilPattern.Count;
        }
        return direction;
    }

    IEnumerator reload()
    {
        if (gunList[selectedGun].currentMagazineIndex + 1 < gunList[selectedGun].magazines.Length)
        {

            gunList[selectedGun].currentMagazineIndex++;
            gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount = gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].magazineCapacity;
        }
        else
        {
            UnityEngine.Debug.Log("No more mags!");
        }

        AudioManager.audioInstance.playSFXAudio(gunList[selectedGun].reloadSound, gunList[selectedGun].reloadVol);
        yield return new WaitForSeconds(gunList[selectedGun].reloadTime);
        //StartCoroutine(fillWhileReloading());
        //checks if there are mags to reload with

        isReloading = false;

        displayCurrentAmmo();
        displayMaxAmmo();
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

    IEnumerator spawnBulletHole(RaycastHit hit)
    {
        GameObject newBulletHole = Instantiate(gunList[selectedGun].bulletDecals[UnityEngine.Random.Range(0, gunList[selectedGun].bulletDecals.Length)], hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
        newBulletHole.transform.up = hit.normal;
        yield return new WaitForSeconds(2);
        Destroy(newBulletHole);
    }
    public void displayCurrentAmmo()
    {
        gameManager.gameInstance.ammoCount.text = gunList[selectedGun].magazines[gunList[selectedGun].currentMagazineIndex].currentAmmoCount.ToString("F0");
    }

    public void displayMaxAmmo()
    {
        int amountToDisplay = 0;
        for (int i = 0; i < gunList[selectedGun].magazines.Length; i++)
        {
            if(i < gunList[selectedGun].currentMagazineIndex)
            {
                continue;
            }
            amountToDisplay += gunList[selectedGun].magazines[i].currentAmmoCount;
        }

        if (gunList[selectedGun].currentMagazineIndex == gunList[selectedGun].magazines.Length - 1)
        {
            amountToDisplay = 0;
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

        Vector3 newPOS = new Vector3(ArmTransform.position.x + 0.05f, ArmTransform.position.y, ArmTransform.position.z);
        gunModel.SetParent(ArmTransform.transform.parent.transform.parent);
        gunModel.SetPositionAndRotation(newPOS, Quaternion.LookRotation(Camera.main.transform.forward));
        //gunModel.SetParent(gunTransform);
        //gunModel.localPosition = Vector3.zero;
        //gunModel.localRotation = Quaternion.identity;
        muzzleFlashTransform = gunModel.Find("MuzzleTransform");
        muzzleFlash = gun.muzzleFlash;
        gunList.Add(gun);
        gameManager.gameInstance.gunName.text = gun.gunName;

        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        shootDistance = gun.shootingDistance;
        shootRate = gun.shootRate;
        fireMode = gun.fireMode;
        gameManager.gameInstance.armsScript.ChangeGun(gun.animationLayer);

        //recoil
        cameraScript.recoilX = gun.recoilX;
        cameraScript.recoilY = gun.recoilY;
        cameraScript.recoilZ = gun.recoilZ;
        cameraScript.returnSpeed = gun.returnSpeed;
        cameraScript.snapping = gun.snapping;
        RecoilPattern = new List<Vector3>(gun.RecoilPattern);

        //heka
        if (gun.hekaAbility != null)
        {
            hekaAbility = gun.hekaAbility;
        }

        hekaManaAmount = gun.hekaManaAmount;
        hekaShootRate = gun.hekaShootRate;
        hekaMuzzleFlash = gun.hekaMuzzleFlash;
        if (gun.hekaSchool == "Electricity")
        {
            hasTempest = true;
        }
        else if (gun.hekaSchool == "Darkness")
        {
            hasEclipse = true;
        }
        else if (gun.hekaSchool == "Floods")
        {
            hasFloods = true;
        }

        displayMaxAmmo();
        displayCurrentAmmo();
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
        //currGun.gunModel.transform.parent = ArmTransform;
        currGun = gunList[selectedGun];
        gameManager.gameInstance.gunName.text = currGun.gunName;
        shootDamage = currGun.shootDamage;
        shootDistance = currGun.shootingDistance;
        shootRate = currGun.shootRate;
        fireMode = currGun.fireMode;
        gameManager.gameInstance.armsScript.ChangeGun(GetAnimationLayer());

        if (currGun.hekaAbility != null)
        {
            hekaAbility = currGun.hekaAbility;
        }
        hekaManaAmount = currGun.hekaManaAmount;
        hekaShootRate = currGun.hekaShootRate;
        hekaMuzzleFlash = currGun.hekaMuzzleFlash;

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
        Vector3 newPOS = new Vector3(ArmTransform.position.x + 0.05f, ArmTransform.position.y, ArmTransform.position.z);
        gunModel.SetParent(ArmTransform.transform.parent.transform.parent);
        gunModel.SetPositionAndRotation(newPOS, Quaternion.LookRotation(Camera.main.transform.forward));
        
        displayCurrentAmmo();
        displayMaxAmmo();

    }

    private int GetAnimationLayer()
    {
        return currGun.animationLayer;
    }

    //helper functions for chain effect
    GameObject getTopLevelParent(Collider collider)
    {
        if(collider == null)
        {
            return null;
        }
        Transform currentTrans = collider.transform;

        while (currentTrans.parent != null)
        {
            currentTrans = currentTrans.parent;
        }

        return currentTrans.gameObject;
    }

    GameObject getClosestZombie(GameObject hitZombie, RaycastHit hit)
    {
        float distance = 0;
        float MaxDistance = float.MaxValue;
        GameObject closestZombie = null;
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");


        foreach (GameObject zombie in zombies)
        {
            if (zombie == hitZombie)
            {
                continue; //skip hit zombie
            }

            distance = Vector3.Distance(hit.point, zombie.transform.position);

            if (distance < MaxDistance)
            {
                MaxDistance = distance;
                closestZombie = zombie;
            }

        }

        return closestZombie;

    }
}
