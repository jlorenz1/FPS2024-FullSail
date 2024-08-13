using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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
    [SerializeField] int playerHP;

    // Weapon Variables for player
    [SerializeField] int shootDamage;
    [SerializeField] int shootRate;
    [SerializeField] int shootDistance;

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
        damage  = shootDamage;
}

    // Update is called once per frame
    void Update()
    {
        movement();
        //if (!onSprintCoolDown)
        //    sprint();
        sprint();
        //sprintTimerUpdate();

        wallCheck();
        stateMachine();


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
        else if (Input.GetButtonUp("Sprint"))// || sprintTimer == 0)
        {
            //if (sprintTimer == 0)
            //    onSprintCoolDown = true;
            speed /= sprintMod;
            isSprinting = false;
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
    }
    void sprintTimerUpdate()
    {
        if (isSprinting)
        {
            sprintTimer -= 0.5f;
        }

        if (sprintTimer == 0)
            StartCoroutine(waitTimer());

    }

    IEnumerator waitTimer()
    {
        yield return new WaitForSeconds(maxSprintWaitTimer);
        sprintTimer = maxSprintTimer;
        onSprintCoolDown = false;
    }

}
