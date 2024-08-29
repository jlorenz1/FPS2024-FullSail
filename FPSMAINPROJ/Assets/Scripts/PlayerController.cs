using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum InventoryPos //for inventory
{
    Slot1 = 0,
    Slot2 = 1,
    Slot3 = 2,
}
public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] Renderer model;

    [Header("Doging")]
    [SerializeField] Collider playerCollider;
    [SerializeField] float dodgeDuration;
    [SerializeField] float dodgeCooldown;
    [SerializeField] float dodgeDistance;
    private bool canDodge = true;

    [Header("PLAYER VARIABLES")]
    [SerializeField] float speed;
    [SerializeField] int sprintMod;
    //[SerializeField] int crouchSpeed;
    float crouchSpeed;
    [SerializeField] float maxSprintTimer;
    [SerializeField] float maxSprintWaitTimer;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] public int playerHP;
    [SerializeField] Light flashLight;
    private bool isLit = false;

    int HPorig;
    float Sprintorig;

    // Weapon Variables for player
    [Header("WEAPON VARIABLES")]
    [SerializeField] List<weaponStats> gunList = new List<weaponStats>();
    [SerializeFeild] public GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    [SerializeField] LayerMask canBeShotMask;
    [SerializeField] Transform gunTransform;

    //public Weapon weapon;
    //private float lastShotTime;
    //private bool weaponCanShoot;


    [SerializeField] Collider meleeWeapon;

    // climbing video variables
    [Header("Reference")]
    [SerializeField] LayerMask whatToClimb;

    [Header("Climbing")]
    [SerializeField] float climbSpeed;
    [SerializeField] float maxClimbTimer;
    private float climbTimer;

    private bool isClimbing;

    [Header("Dectection")]
    [SerializeField] float detectionLength;
    [SerializeField] float sphereCastRadius;
    [SerializeField] float maxWallLookAngle;
    private float WallLookAngle;

    private RaycastHit wallHit;
    private bool wallFront;
    // End of climbiing video variables


    // Sliding video variables
    [Header("Sliding")]
    [SerializeField] float maxSlideTime;
    [SerializeField] float slideForce;
    float slideTimer;

    public float slideYScale;
    float startingYScale;
    float startingYPOS;
    int controllerHeightOrgi;
    float meeleDuration;
    bool isSliding;

    [Header("Input")]
    private KeyCode slideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;
    // End of Sliding video variables


    [Header("Interact")]
    [SerializeField] int pickupDis;
    [SerializeField] LayerMask ignoreMask;
    private Ray interactRay;
    private RaycastHit interactHit;
    bool isPickedUp;
    bool inProcess;
    bool doorOpen = false;
    public inventoryObject inventory;
    private float sprintTimer;

    [Header("Sounds")]
    public AudioClip[] interactSounds;
    [Range(0, 1)][SerializeField] public float interactVol;
    public AudioClip[] flashlightSounds;
    [Range(0, 1)][SerializeField] public float flashlightVol;
    public AudioClip[] jumpSounds;
    [Range(0, 1)][SerializeField] public float jumpVol;
    public AudioClip[] stepSounds;
    [Range(0, 1)][SerializeField] public float stepVol;
    public AudioClip[] slideSounds;
    [Range(0, 1)][SerializeField] public float slideVol;
    public AudioClip[] hurtSounds;
    [Range(0, 1)][SerializeField] public float hurtVol;

    // Weapon Vars
    //public Weapon primaryWeapon;
    //public Weapon secondaryWeapon;
    //public Weapon activeWeapon;

    Vector3 move;
    Vector3 playerVel;

    int jumpCount;
    int selectedGun;
    public bool isSprinting;
    public bool isShooting;
    bool onSprintCoolDown;
    bool isCrouching;
    bool canMelee;
    float originalSpeed;
    public int damage;
    public bool hasItems;
    bool isPlayingSound;
    private Coroutine waitTime;
    // Start is called before the first frame update
    void Start()
    {
        HPorig = playerHP;
        damage = shootDamage;
        sprintTimer = maxSprintTimer;
        originalSpeed = speed;
        crouchSpeed = speed / 3;
        startingYScale = transform.localScale.y;
        controllerHeightOrgi = ((int)controller.height);
        updatePlayerUI();
        meeleDuration = 2;
        canMelee = true;
        flashLight.gameObject.SetActive(false);

        //lastShotTime = -shootRate; // Allows immediate shooting
        //weaponCanShoot = true;
        //activeWeapon = primaryWeapon;
        //EquipWeapon(activeWeapon);
    }



    // Update is called once per frame
    void Update()
    {
        if (!onSprintCoolDown && !isCrouching)
            sprint();
        sprintTimerUpdate();

        wallCheck();
        stateMachine();

        interact();
        //useItemFromInv();

        // Check for shooting input (left mouse button)

        //HandleWeaponSwitching();

        if(!gameManager.gameInstance.gameIsPaused)
        {
            movement();
            selectGun();
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~canBeShotMask))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
            else
            {
                Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
            }
            
        }
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    //void FireWeapon()
    //{
    //    // Check if enough time has passed before last shot
    //    if (Time.time - lastShotTime < shootRate) return;

    //    // Update the last fire time
    //    lastShotTime = Time.time;

    //    ShootRaycastBullet();

    //    StartCoroutine(ResetShootingState(shootRate));
    //}

    //void ShootRaycastBullet()
    //{
    //    Ray reticleRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
    //    RaycastHit hit;

    //    if (Physics.Raycast(reticleRay, out hit, shootDistance, canBeShotMask))
    //    {
    //        // Apply damage
    //        IDamage target = hit.collider.gameObject.GetComponent<IDamage>();
    //        if (target != null)
    //        {
    //            target.takeDamage(shootDamage);
    //        }

    //        // VFX effects


    //        UnityEngine.Debug.Log("Hit " + hit.collider.name + " for " + shootDamage + " damage.");
    //    }

    //    // Trigger shooting system (muzzle flash, sounds, etc)

    //}


    //IEnumerator ResetShootingState(float weaponFireRate)
    //{
    //    yield return new WaitForSeconds(weaponFireRate);
    //    isShooting = false;
    //    weaponCanShoot = true;
    //}

    //void EquipWeapon(Weapon activeWeapon)
    //{

    //    if (activeWeapon == null)
    //    {
    //        UnityEngine.Debug.LogWarning("Weapon to equip is null");
    //        return;
    //    }



    //    if (primaryWeapon != null)
    //        primaryWeapon.gameObject.SetActive(false);

    //    if (secondaryWeapon != null)
    //        secondaryWeapon.gameObject.SetActive(false);

    //    activeWeapon.gameObject.SetActive(true);
    //}

    //void HandleWeaponSwitching()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1) && activeWeapon != primaryWeapon)
    //    {
    //        activeWeapon = primaryWeapon;
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2) && activeWeapon != secondaryWeapon)
    //    {
    //        activeWeapon = secondaryWeapon;
    //    }

    //    EquipWeapon(activeWeapon);
    //}

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
            climbTimer = maxClimbTimer;

        }
        if (isSliding)
            slideMovement();

        if (isClimbing)
            climbingMovement();

        move = Input.GetAxis("Vertical") * transform.forward +
            Input.GetAxis("Horizontal") * transform.right;
        controller.Move(move * speed * Time.deltaTime);

        horizontalInput = Input.GetAxis("Vertical");
        verticalInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            AudioManager.audioInstance.playAudio(jumpSounds[Random.Range(0, jumpSounds.Length)], jumpVol);
        }
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;


        if (Input.GetKeyDown(slideKey) && !isSprinting)
        {
            startCrouch();
        }
        else if (Input.GetKeyUp(slideKey))
        {
            stopCrouch();
        }

        if (controller.isGrounded && move.magnitude > 0.2f && !isPlayingSound && !isSliding)
            StartCoroutine(playStep());

        if (Input.GetButtonDown("Dodge") && canDodge)
        {
            UnityEngine.Debug.Log("Dodge input detected");
            StartCoroutine(PerformDodge());
        }


        if (Input.GetButtonDown("Melee") && canMelee)
        {
            UnityEngine.Debug.Log("Melee input detected");
            StartCoroutine(PerformMelee());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (inventory.hasItem(itemType.flashlight))
            {
                enableFlashLight();
            }
            else
            {
                StartCoroutine(gameManager.gameInstance.requiredItemsUI("Do not have flashlight! Check your campsite for it!", 3.0f));
            }
        }

        if (Input.GetButton("Fire1") && gunList.Count > 0 && !isShooting)
        {
            StartCoroutine(shoot());
        }
        //if (Input.GetButtonDown("Reload"))

        //{
        //    activeWeapon.TempReload();
        //}


        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //   activeWeapon.HandleFireModeSwitching();
        //}

        //    //  activeWeapon.HandleFireModeSwitching();
        //    activeWeapon.displayAmmo();


    }

    IEnumerator playStep()
    {
        isPlayingSound = true;

        AudioManager.audioInstance.playAudio(stepSounds[Random.Range(0, stepSounds.Length)], stepVol);

        if (!isSprinting && !isCrouching)
            yield return new WaitForSeconds(0.45f);
        else
            yield return new WaitForSeconds(0.3f);

        if (isCrouching)
            yield return new WaitForSeconds(0.5f);


        isPlayingSound = false;
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;

        }
        else if (Input.GetButtonUp("Sprint") || sprintTimer <= 0)
        {
            if (sprintTimer <= 0)
                onSprintCoolDown = true;
            //speed /= sprintMod;
            speed = originalSpeed;
            isSprinting = false;
            return;
        }

    }
    void sprintTimerUpdate()
    {
        if (isSprinting)
            gameManager.gameInstance.SprintBarBoarder.transform.GameObject().SetActive(true);

        gameManager.gameInstance.playerSprintBar.color = Color.white;
        gameManager.gameInstance.playerSprintBar.fillAmount = sprintTimer / maxSprintTimer;
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
        }

        if (Input.GetButtonUp("Sprint"))
        {
            waitTime = StartCoroutine(waitTimer());
        }
        if (!onSprintCoolDown && Input.GetButtonDown("Sprint"))
        {
            if (waitTime != null)
            {
                StopCoroutine(waitTime);
                waitTime = null;
            }

        }

    }

    IEnumerator waitTimer()
    {
        float startingFill = gameManager.gameInstance.playerSprintBar.fillAmount;
        while (sprintTimer < maxSprintTimer)
        {
            if (!onSprintCoolDown)
                gameManager.gameInstance.playerSprintBar.color = Color.white;
            else
            {
                gameManager.gameInstance.playerSprintBar.color = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.3f);
            }



            sprintTimer += Time.deltaTime;
            float fillAmount = Mathf.Lerp(startingFill, 1f, sprintTimer / maxSprintTimer);
            gameManager.gameInstance.playerSprintBar.fillAmount = fillAmount;
            yield return null;
        }
        gameManager.gameInstance.playerSprintBar.fillAmount = 1f;
        onSprintCoolDown = false;

        yield return new WaitForSeconds(.3f);
        gameManager.gameInstance.SprintBarBoarder.transform.GameObject().SetActive(false);

    }

    void stateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && WallLookAngle < maxWallLookAngle)
        {
            if (isSliding)
                stopSlide();

            if (!isClimbing && climbTimer > 0) startClimb();

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) endClimbing();

        }
        else
        {
            if (isClimbing) endClimbing();
        }

        if (isSprinting)
        {
            if (Input.GetKeyDown(slideKey))
            {
                startingYPOS = transform.position.y;
                startSlide();
            }
            if (Input.GetKeyUp(slideKey) && isSliding)
            {
                stopSlide();
            }
        }

    }
    void wallCheck()
    {
        //wallFront tell if there is a wall in front
        //                            ( starting postion,                   Radius,              Directions,    location of the infomation, sphereCast length,  the layerMask)
        wallFront = Physics.SphereCast(Camera.main.transform.position, sphereCastRadius, Camera.main.transform.forward, out wallHit, detectionLength, whatToClimb);
        WallLookAngle = Vector3.Angle(Camera.main.transform.forward, -wallHit.normal);


    }
    void startClimb()
    {
        isClimbing = true;
    }
    void climbingMovement()
    {
        playerVel = new Vector3(playerVel.x, climbSpeed, playerVel.z);

    }

    void endClimbing()
    {
        isClimbing = false;
    }
    void startSlide()
    {
        controller.height = slideYScale;
        model.transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        isSliding = true;
        slideTimer = maxSlideTime;
    }

    void slideMovement()
    {
        Vector3 inputDir = transform.forward * verticalInput + transform.right * horizontalInput;
        if (!isPlayingSound)
            StartCoroutine(slideAud());
        speed += slideForce;

        slideTimer -= Time.deltaTime;

        if (slideTimer <= 0)
            stopSlide();

    }

    IEnumerator slideAud()
    {
        isPlayingSound = true;
        AudioManager.audioInstance.playAudio(slideSounds[Random.Range(0, slideSounds.Length)], slideVol);
        yield return new WaitForSeconds(.05f);
        isPlayingSound = false;
    }

    void stopSlide()
    {
        controller.height = controllerHeightOrgi;
        model.transform.localScale = new Vector3(transform.localScale.x, startingYScale, transform.localScale.z);
        speed = originalSpeed;
        isSliding = false;
    }

    void startCrouch()
    {
        controller.height = slideYScale;
        model.transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        isCrouching = true;
        speed = crouchSpeed;
    }
    void stopCrouch()
    {
        controller.height = controllerHeightOrgi;
        model.transform.localScale = new Vector3(transform.localScale.x, startingYScale, transform.localScale.z);
        isCrouching = false;
        speed = originalSpeed;
    }

    // IDamage Player Damage
    public void takeDamage(int amountOfDamageTaken)
    {
        // Subtract the amount of current damage from player HP
        playerHP -= amountOfDamageTaken;
        AudioManager.audioInstance.playAudio(hurtSounds[Random.Range(0, hurtSounds.Length)], hurtVol);
        StartCoroutine(damageFeedback());
        updatePlayerUI();
        if (playerHP <= 0)
        {
            gameManager.gameInstance.loseScreen();
        }

    }
    public void recieveHP(int amount)
    {
        playerHP += amount;
        updatePlayerUI();
    }

    IEnumerator damageFeedback()
    {
        gameManager.gameInstance.flashDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.gameInstance.flashDamage.SetActive(false);
    }

    public void updatePlayerUI()
    {
        gameManager.gameInstance.playerHPBar.fillAmount = (float)playerHP / HPorig;
        gameManager.gameInstance.playerSprintBar.fillAmount = sprintTimer / maxSprintTimer;
    }

    public void interact()
    {
        RaycastHit hit;
        bool isInteractable = false;
        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            if (hit.collider.gameObject.CompareTag("pickup"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(startInteract());
                }
                isInteractable = true;
            }
            else if (hit.collider.gameObject.CompareTag("interactable"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                isInteractable = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.collider != null)
                    {
                        var door = hit.collider.GetComponent<doorScript>();
                        if (door != null)
                        {
                            if (inventory.hasItem(itemType.Key))
                            {
                                inventory.useItem(itemType.Key);
                            }
                            else
                            {
                                StartCoroutine(gameManager.gameInstance.requiredItemsUI("Do not have a key!", 3f));
                            }

                        }
                        else
                        {
                            var bossInteraction = hit.collider.GetComponent<bossInteraction>();
                            if (bossInteraction != null)
                            {
                                if (hasItems)
                                {
                                    bossInteraction.spawnBoss();
                                    Destroy(hit.collider);
                                }
                                else
                                {
                                    StartCoroutine(gameManager.gameInstance.requiredItemsUI("Do not have required items!", 3f));
                                }
                            }
                            else
                            {
                                UnityEngine.Debug.Log("not spawning");
                            }
                        }
                    }
                }
            }
            else
            {
                gameManager.gameInstance.playerInteract.SetActive(false);
            }
        }

        if (!isInteractable)
        {
            gameManager.gameInstance.playerInteract.SetActive(false);
        }
    }

    public IEnumerator startInteract()
    {
        RaycastHit hit;
        AudioManager.audioInstance.playAudio(interactSounds[Random.Range(0, interactSounds.Length)], interactVol);

        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            pickup pickup = hit.collider.GetComponent<pickup>();
            if (pickup != null)
            {
                inventory.AddItem(pickup.item, 1);
                inventory.updateInventoryUI();
                Destroy(hit.collider.gameObject);
            }
            if (pickup.item.type == itemType.Default || pickup.item.type == itemType.Rune)
            {
                checkForRequiredItems();

            }
            else if (pickup.item.type == itemType.Key)
            {
                gameManager.gameInstance.displayRequiredIemsUI("Collected back cabin key!", 3f);
            }
            else if (pickup.item.type == itemType.flashlight)
            {
                gameManager.gameInstance.displayRequiredIemsUI("'F' to use flashlight.", 3f);

            }
        }

        yield return new WaitForSeconds(1);
        gameManager.gameInstance.playerInteract.SetActive(false);
    }



    public void checkForRequiredItems()
    {
        bool hasRunes = false;
        bool hasLighter = false;
        bool hasLighterOnce = false;
        bool runeMessageShown = false;
        bool lighterMessageShown = false;

        for (int i = 0; i < inventory.containerForInv.Count; i++)
        {
            if (inventory.containerForInv[i].pickup.type == itemType.Rune)
            {


                if (inventory.containerForInv[i].amount >= 4)
                {
                    hasRunes = true;
                    if (!runeMessageShown)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI("Collected all Runes!", 3f);

                        runeMessageShown = true;
                    }

                    if (hasLighter && !lighterMessageShown)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI("Collected all Runes and Lighter, Find the ritual sight to spawn the boss!", 3f);
                    }
                    UnityEngine.Debug.Log(hasRunes);
                }
                else
                {
                    UnityEngine.Debug.Log("Not enough runes");
                    if (!runeMessageShown)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI(inventory.containerForInv[i].amount.ToString() + " of 4 Runes collected!", 3f);

                        runeMessageShown = true;
                    }
                    //set UI active coroutine 
                }
            }
            else if (inventory.containerForInv[i].pickup.type == itemType.Default)
            {

                if (inventory.containerForInv[i].amount >= 1)
                {

                    hasLighter = true;
                    if (!hasLighterOnce)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI("Collected Lighter!", 3f);

                        hasLighterOnce = true;
                    }
                }
            }
        }
        if (hasRunes && hasLighter)
        {
            UnityEngine.Debug.Log("can now spawn boss");
            gameManager.gameInstance.itemsCompleteText.gameObject.SetActive(true);
            hasItems = true;
        }
        else
        {
            UnityEngine.Debug.Log("required items missing");

            hasItems = false;
        }
    }

    public void OnApplicationQuit()
    {
        //set inventory back to the original starting loadout.
        for (int i = 0; i < inventory.containerForInv.Count; i++)
        {
            inventory.containerForInv.Clear();
        }

    }
    // Things Added by Jamauri 
    public void SetSpeed(float Modifier)
    {
        speed = Modifier;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetJumpCount(int Modifier)
    {
        jumpMax = Modifier;
    }

    public int GetJumpCount()
    {
        return jumpMax;
    }


    private IEnumerator PerformDodge()
    {
        // Start the dodge
        canDodge = false;

        // Disable the collider
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        float elapsedTime = 0f;
        while (elapsedTime < dodgeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Re-enable the collider
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        // Wait for the cooldown duration
        yield return new WaitForSeconds(dodgeCooldown);

        // End the dodge cooldown
        canDodge = true;
    }


    private IEnumerator PerformMelee()
    {
        canMelee = false;


        meleeWeapon.enabled = true;
        float elapsedTime = 0f;
        while (elapsedTime < meeleDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        meleeWeapon.enabled = false;

        yield return new WaitForSeconds(1f);

        canMelee = true;
    }

    void enableFlashLight()
    {
        isLit = !isLit;
        flashLight.gameObject.SetActive(isLit);
        AudioManager.audioInstance.playAudio(flashlightSounds[Random.Range(0, flashlightSounds.Length)], flashlightVol);
    }

    public void getWeaponStats(weaponStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        shootDistance = gun.shootingDistance;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        } 
    }
    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootRate = gunList[selectedGun].shootRate;
        shootDistance = gunList[selectedGun].shootingDistance;
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

}
