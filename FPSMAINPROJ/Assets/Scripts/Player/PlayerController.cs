
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum InventoryPos //for inventory
{
    Slot1 = 0,
    Slot2 = 1,
    Slot3 = 2,
}
public class PlayerController : MonoBehaviour, IDamage
{
    private static PlayerController _playerInstance;

    [SerializeField] CharacterController controller;
    [SerializeField] Renderer model;

    [Header("Doging")]
    [SerializeField] Collider playerCollider;
    [SerializeField] float dodgeDuration;
    [SerializeField] float dodgeCooldown;
    [SerializeField] float dodgeDistance;
    private bool canDodge = true;
    private bool invincible = false;


    [Header("PLAYER VARIABLES")]
    [SerializeField] float speed;
    [SerializeField] int sprintMod;
    //[SerializeField] int crouchSpeed;
    [SerializeField] public WeaponController playerWeapon;
    float crouchSpeed;
    [SerializeField] float maxMana;
    [SerializeField] float maxSprintTimer;
    [SerializeField] float maxSprintWaitTimer;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] public float playerHP;
    float StartHP;
    [SerializeField] Light flashLight;
    private bool isLit = false;

    public float HPorig;
    public float currentMana;
    float Sprintorig;

   

    // climbing video variables
    [Header("Reference")]
    [SerializeField] LayerMask whatToClimb;

    [Header("Climbing")]
    [SerializeField] float climbSpeed;
    [SerializeField] float maxClimbTimer;
    private float climbTimer;

    private bool isClimbing;

    [Header("Dectection")]

    [SerializeField] float aboveDetectionLength;
    bool underObject;

    [SerializeField] float climbDetectionLength;
    [SerializeField] float wallCastRadius;
    [SerializeField] float aboveCastRadius;
    [SerializeField] float maxWallLookAngle;
    private float WallLookAngle;

    private RaycastHit objectHit;
    private bool wallFront;
    // End of climbiing video variables

    // Sliding video variables
    [Header("Sliding")]
    [SerializeField] float maxSlideTime;
    [SerializeField] float slideForce;
    float slideTimer;

    public float slideYScale;
    float startingYScale;
    int controllerHeightOrgi;
    float meeleDuration;
    bool isSliding;
    int NeededItems;
    [Header("Edge Hang")]
    bool isHanging;

    [Header("Input")]
    private KeyCode slideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;
    private float sprintTimer;


    public inventoryObject inventory;


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
    public AudioClip gemSound;
    [Range(0, 1)][SerializeField] public float gemVol;

    // Weapon Vars
    //public Weapon primaryWeapon;
    //public Weapon secondaryWeapon;
    //public Weapon activeWeapon;

    public Vector3 move;
    Vector3 playerVel;

    float currentSpeed;
    bool SpeedStateSlow;
    int jumpCount;
    int selectedGun;
    public bool isSprinting;
    public bool isShooting;
    bool onSprintCoolDown;
    bool isCrouching;
    bool canMelee;
    float originalSpeed;
    float slowStrength;
    public float damage;
    public bool hasItems;
    bool isPlayingSound;
    private Coroutine waitTime;
    private Coroutine manaTime;
    private Coroutine gunSprintCoroutine;
    public float speedDuringSprint = 0;
    bool timerStarted;

    public static PlayerController playerInstance
    {
        get
        {
            if (_playerInstance == null)
            {
            }
            return _playerInstance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If instance already exists and it is not the PlayerController, destroy this instance
        if (_playerInstance != null && _playerInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _playerInstance = this;
        }

        originalSpeed = speed;

        HPorig = playerHP;
        damage = playerWeapon.shootDamage;
        spawnPlayer();
        meeleDuration = 2;
        canMelee = true;
        flashLight.gameObject.SetActive(false);
        StartHP = playerHP;
        isSprinting = false;
        isShooting = false;
        isCrouching = false;
        speed = originalSpeed;
    }



    // Update is called once per frame
    void Update()
    {
        if (!isSliding)
            //gameManager.gameInstance.armsScript.NoGunMovement();
        //else
        //{
        //    gameManager.gameInstance.armsScript.StartAnimationSlide();
        //}

        

        if (!onSprintCoolDown && !isCrouching)
        {
            sprint();
        }


        Slow();

        sprintTimerUpdate();

        wallCheck();
        edgeHang();
        crouchCheck();
        stateMachine();

        if(!gameManager.gameInstance.gameIsPaused)
        {
            movement();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (manaTime != null)
            {
                StopCoroutine(manaTime);
                manaTime = null;
            }
            mana(1.5f);
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            manaTime = StartCoroutine(ManaTimer());
        }


    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
            climbTimer = maxClimbTimer;

        }

     

        if (isHanging)
            jumpCount = 0;

        if (isSliding)
            slideMovement();

        if (isClimbing)
            climbingMovement();

        if (isHanging)
        {
            move = Vector3.zero;
        }
        else {
            move = Input.GetAxis("Vertical") * transform.forward +
            Input.GetAxis("Horizontal") * transform.right;
        }


      
        
            controller.Move(move * speed * Time.deltaTime);
        
 

        horizontalInput = Input.GetAxis("Vertical");
        verticalInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            if (isHanging)
            {
                isHanging = false;
            }

            jumpCount++;
            playerVel.y = jumpSpeed;
            AudioManager.audioInstance.playSFXAudio(jumpSounds[Random.Range(0, jumpSounds.Length)], jumpVol);
        }

        if (!isHanging)
        {
            controller.Move(playerVel * Time.deltaTime);
            playerVel.y -= gravity * Time.deltaTime;
        }
        


        if (!isSprinting) {
            if (Input.GetKey(slideKey) || underObject)
                startCrouch();
            else if (!Input.GetKey(slideKey) && !underObject)
                stopCrouch();
        }


        if (controller.isGrounded && move.magnitude > 0.2f && !isPlayingSound && !isSliding)
            StartCoroutine(playStep());

        if (Input.GetButtonDown("Dodge") && canDodge)
        {
            StartCoroutine(PerformDodge());
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            if (inventory.hasItem(itemType.flashlight))
            {
                enableFlashLight();
            }
            else
            {
                StartCoroutine(gameManager.gameInstance.requiredItemsUI("Do not have flashlight!", 3.0f));
            }
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

        AudioManager.audioInstance.playSFXAudio(stepSounds[Random.Range(0, stepSounds.Length)], stepVol);

        if (!isSprinting && !isCrouching)
            yield return new WaitForSeconds(0.45f);
        else
            yield return new WaitForSeconds(0.3f);

        if (isCrouching)
            yield return new WaitForSeconds(0.5f);


        isPlayingSound = false;
    }

    public void mana(float manaUsageAmount)
    {
        if (manaTime != null)
        {
            StopCoroutine(manaTime);
            manaTime = null;
        }

        if (currentMana > 0) {
            currentMana -= manaUsageAmount;
            updatePlayerUI();
        }

        manaTime = StartCoroutine(ManaTimer());

    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            speedDuringSprint = speed;
            isSprinting = true;
            
        }
        else if (Input.GetButtonUp("Sprint") || sprintTimer <= 0 || Input.GetButtonDown("Dodge"))
        {
            if (sprintTimer <= 0)
                onSprintCoolDown = true;
            //speed /= sprintMod;
            speed = originalSpeed;
            isSprinting = false;
            return;
        }

    }

    void Slow()
    {
        if (SpeedStateSlow)
        {
            speed *= slowStrength;
            if (speed > 0)
            {
                gameManager.gameInstance.StatusSlow.enabled = true;
            }
            else if (speed <= 0)
                gameManager.gameInstance.StatusStun.enabled = true;

        }
        else if (!isSprinting)
        {
            speed = originalSpeed;
            gameManager.gameInstance.StatusSlow.enabled = false;
            gameManager.gameInstance.StatusStun.enabled = false;

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

    IEnumerator ManaTimer()
    {
        yield return new WaitForSeconds(1.5f);
        float startingFill = gameManager.gameInstance.playerManaBar.fillAmount;
        while (currentMana < maxMana)
        {
            
            currentMana += Time.deltaTime * 50;
            float fillAmount = Mathf.Lerp(startingFill, 1f, currentMana / maxMana);
            gameManager.gameInstance.playerManaBar.fillAmount = fillAmount;
            yield return null;
        }
        //gameManager.gameInstance.playerManaBar.fillAmount = 1f;

    }

    IEnumerator waitTimer()
    {
        if (!timerStarted)
        {
            timerStarted = true;
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
            timerStarted = false;

            yield return new WaitForSeconds(.3f);
            gameManager.gameInstance.SprintBarBoarder.transform.GameObject().SetActive(false);
        }
        else
        {
            yield return null;
        }



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
        wallFront = Physics.SphereCast(Camera.main.transform.position, wallCastRadius, transform.forward, out objectHit, climbDetectionLength, whatToClimb);
        WallLookAngle = Vector3.Angle(Camera.main.transform.forward, -objectHit.normal);

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
        //model.transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        invincible = true;
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
        AudioManager.audioInstance.playSFXAudio(slideSounds[Random.Range(0, slideSounds.Length)], slideVol);
        yield return new WaitForSeconds(.05f);
        isPlayingSound = false;
    }

    void stopSlide()
    {
        //gameManager.gameInstance.armsScript.StopAnimationSlide();
        controller.height = controllerHeightOrgi;
        model.transform.localScale = new Vector3(transform.localScale.x, startingYScale, transform.localScale.z);
        speed = originalSpeed;
        invincible = false;
        isSliding = false;
    }

    void startCrouch()
    {
        controller.height = slideYScale;
        //model.transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        isCrouching = true;
        speed = crouchSpeed;
    }

    void crouchCheck()
    {
        underObject = Physics.SphereCast(controller.transform.position, aboveCastRadius, controller.transform.up, out objectHit, aboveDetectionLength);
    }

    void stopCrouch()
    {
        if(!underObject)
        {
            controller.height = controllerHeightOrgi;
            model.transform.localScale = new Vector3(transform.localScale.x, startingYScale, transform.localScale.z);
            isCrouching = false;
            speed = originalSpeed;
        }
    }

    void edgeHang()
    {
        if(playerVel.y < -2 && !isHanging)
        {

            Physics.Linecast(Camera.main.transform.position + transform.forward, transform.position + transform.forward * .15f, out objectHit, LayerMask.GetMask("Hangable"));

            if (objectHit.collider != null)
            {
                RaycastHit forwardHit;
                Vector3 fowardStart = new Vector3(Camera.main.transform.position.x, objectHit.point.y, Camera.main.transform.position.z);
                Vector3 fowardEnd = new Vector3(Camera.main.transform.position.x, objectHit.point.y, Camera.main.transform.position.z) + transform.forward;
                Physics.Linecast(fowardStart, fowardEnd, out forwardHit, LayerMask.GetMask("Hangable"));

                if(forwardHit.collider != null)
                {

                    isHanging = true;

                    Vector3 handPOS = new Vector3(forwardHit.point.x, objectHit.point.y, forwardHit.point.z);
                    Vector3 offSet = transform.forward * 0.1f + transform.up * -1f;
                    handPOS += offSet;

                    transform.position = handPOS;
                    transform.forward = -forwardHit.normal;

                }

            }
        }

    }

    // IDamage Player Damage
    public void takeDamage(float amountOfDamageTaken)
    {
        if(!invincible)
        {
            // Subtract the amount of current damage from player HP
            playerHP -= amountOfDamageTaken;
            AudioManager.audioInstance.playSFXAudio(hurtSounds[Random.Range(0, hurtSounds.Length)], hurtVol);
            StartCoroutine(damageFeedback());
            updatePlayerUI();
            if (playerHP <= 0)
            {
                gameManager.gameInstance.loseScreen();
            }

        }
        

    }
    public void recieveHP(float amount)
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
        gameManager.gameInstance.playerHPBar.fillAmount = playerHP / HPorig;
        gameManager.gameInstance.playerSprintBar.fillAmount = sprintTimer / maxSprintTimer;
        gameManager.gameInstance.playerManaBar.fillAmount = currentMana / maxMana;
    }
    // Things Added by Jamauri 

   

   

    public void CutSpeed(float duration, float strength)
    {
       
       StartCoroutine( CutSpeedrunner(duration, strength));
    }

    IEnumerator CutSpeedrunner(float duration, float strength)
    {


        slowStrength = 1 - (strength / 100f);

       

        SpeedStateSlow = true;

        if(playerHP <= 0)
        {
            SpeedStateSlow = false;
           
            yield break;

        }


        yield return new WaitForSeconds(duration);


        SpeedStateSlow = false;
        slowStrength = 1f;
    }

   public void TickDamage(float duration, float amountpertick, float tickrate)
    {
        StartCoroutine(TakeTickDamage(duration, amountpertick, tickrate));
    }


    IEnumerator TakeTickDamage(float duration,  float amountpertick, float tickrate)
    {

        gameManager.gameInstance.StatusBurn.enabled = true; 
        int numberOfTicks = Mathf.CeilToInt(duration / tickrate);
        for (int i = 0; i < numberOfTicks; i++)
        {

            if(playerHP <= 0)
            {
                gameManager.gameInstance.StatusBurn.enabled = false;
                break;
            }



            takeDamage(amountpertick);
            yield return new WaitForSeconds(tickrate);


        }
        




        yield return new WaitForSeconds(duration);
        gameManager.gameInstance.StatusBurn.enabled = false;
    }

    private IEnumerator PerformDodge()
    {
        // Start the dodge
        canDodge = false;
        invincible = true;

        // Disable the collider
            //if (playerCollider != null)
            //{
            //    playerCollider.enabled = false;
            //}

        float elapsedTime = 0f;
        while (elapsedTime < dodgeDuration)
        {
            //Vector3 inputDir = transform.forward * verticalInput + transform.right * horizontalInput;
            speed += dodgeDistance;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    //    speed = originalSpeed;

        // Re-enable the collider
            //if (playerCollider != null)
            //{
            //    playerCollider.enabled = true;
            //}
        invincible = false;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(dodgeCooldown);

        // End the dodge cooldown
        canDodge = true;
    }



    void enableFlashLight()
    {
        isLit = !isLit;
        flashLight.gameObject.SetActive(isLit);
        AudioManager.audioInstance.playSFXAudio(flashlightSounds[Random.Range(0, flashlightSounds.Length)], flashlightVol);
    }


  public float GetHealth()
    {
        return StartHP;
    }

    public void RespawnPlayer()
    {
        gameManager.gameInstance.CurrentCheckPoint.ResetTrigger();
        spawnPlayer();
    }


    public void spawnPlayer()
    {

      
        playerHP = HPorig;
        sprintTimer = maxSprintTimer;
        StopCoroutine(TakeTickDamage(1,1,1));
        crouchSpeed = speed / 2;
        startingYScale = transform.localScale.y;
        controllerHeightOrgi = ((int)controller.height);
        currentMana = maxMana;
        updatePlayerUI();
        SpeedStateSlow = false;
        controller.enabled = false;
        transform.position = gameManager.gameInstance.playerSpawnPoint.transform.position;

        controller.enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Zombie") && !collision.collider.isTrigger)
        {

            Vector3 pushDirection = collision.contacts[0].normal;

            // Move the object in the opposite direction of the collision
            transform.position += pushDirection * Time.deltaTime * 5;

        }




    }

}

