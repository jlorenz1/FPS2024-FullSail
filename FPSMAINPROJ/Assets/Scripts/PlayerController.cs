using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] float maxSprintTimer;
    [SerializeField] float maxSprintWaitTimer;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] public int playerHP;
    int HPorig;

    // Weapon Variables for player
    [SerializeField] int shootDamage;
    [SerializeField] int shootRate;
    [SerializeField] int shootDistance;
    public Weapon weapon;

    // climbing video variables
    [Header("Reference")]
    [SerializeField] LayerMask whatIsWall;

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

    Vector3 move;
    Vector3 playerVel;

    int jumpCount;

    bool isSprinting;
    bool onSprintCoolDown;

    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        HPorig = playerHP;
        updatePlayerUI();
        damage  = shootDamage;
        sprintTimer = maxSprintTimer;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        if (!onSprintCoolDown)
            sprint();
        sprintTimerUpdate();

        wallCheck();
        stateMachine();
        
        interact();
        useItemFromInv();

    }
    
    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
            climbTimer = maxClimbTimer;

        }

        if (isClimbing) 
            climbingMovement();

        move = Input.GetAxis("Vertical") * transform.forward +
            Input.GetAxis("Horizontal") * transform.right;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

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
            speed /= sprintMod;
            isSprinting = false;
            return;
        }

    }
    void stateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && WallLookAngle < maxWallLookAngle)
        {
            if (!isClimbing && climbTimer > 0) startClimb();

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) endClimbing();

        }
        else
        {
            if (isClimbing) endClimbing();
        }

    }
    void wallCheck()
    {
        //wallFront tell if there is a wall in front
        //                            ( starting postion,       Radius,        Directions,    location of the infomation, sphereCast length,  the layerMask)
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out wallHit, detectionLength, whatIsWall);
        WallLookAngle = Vector3.Angle(transform.forward, -wallHit.normal);


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

    // IDamage Player Damage
    public void takeDamage(int amountOfDamageTaken)
    {
        // Subtract the amount of current damage from player HP
        playerHP -= amountOfDamageTaken;
        StartCoroutine(damageFeedback());
        updatePlayerUI();
        if(playerHP <= 0)
        {
            gameManager.gameInstance.loseScreen();
        }
        
    }
    public void recieveHP(int amount)
    {
        playerHP += amount;
    }

    IEnumerator damageFeedback()
    {
        gameManager.gameInstance.flashDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.gameInstance.flashDamage.SetActive(false);
    }
    void sprintTimerUpdate()
    {
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
        }

        if (onSprintCoolDown && Input.GetButtonUp("Sprint"))
        {
            StartCoroutine(waitTimer());
        }

        if(onSprintCoolDown && Input.GetButtonDown("Sprint"))
        {
            StopAllCoroutines();
            //StopCoroutine(waitTimer());
        }

    }

    IEnumerator waitTimer()
    {
        yield return new WaitForSeconds(maxSprintWaitTimer);
        sprintTimer = maxSprintTimer;
        onSprintCoolDown = false;
    }

    public void updatePlayerUI()
    {
        gameManager.gameInstance.playerHPBar.fillAmount = (float)playerHP / HPorig;
    }

    public void interact()
    {
        RaycastHit hit;

        if(Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            if (hit.collider.gameObject.CompareTag("pickup"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(startInteract());
                }
            }
            else
            {
                gameManager.gameInstance.playerInteract.SetActive(false);
            }
        }
    }

    public IEnumerator startInteract()
    {
        RaycastHit hit;

        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            var pickup = hit.collider.GetComponent<pickup>();
            if (pickup != null)
            {
                inventory.AddItem(pickup.item, 1);
                Destroy(hit.collider.gameObject);
            }
        }
        yield return new WaitForSeconds(1);
        gameManager.gameInstance.playerInteract.SetActive(false);
    }

    public void useItemFromInv()
    {
        RaycastHit hit;

        if(Input.GetKeyDown(KeyCode.Q))
        {
            //using heal
            inventory.useItem(itemType.Bandage);
        }
        else if(Input.GetKeyDown(KeyCode.E) && Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            UnityEngine.Debug.Log("hit");
            var door = hit.collider.GetComponent<doorScript>();
            if (door != null)
            {
                inventory.useItem(itemType.Key);
            }
        }
         
    }
    public void OnApplicationQuit()
    {
        inventory.containerForInv.Clear();
    }
}
