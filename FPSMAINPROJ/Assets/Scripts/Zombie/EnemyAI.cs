using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage, IHitPoints
{



    [Header("-----Model/Animation-----")]
    [SerializeField] Renderer model;
    [SerializeField] int BaseHitPoints;
    [SerializeField] Animator animator;
    [SerializeField] int animatorspeedtrans;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int FacePlayerSpeed;
    [SerializeField] int ViewAngle;
    [SerializeField] Transform HeadPos;
    [SerializeField] Transform launchPoint;
    [SerializeField] GameObject CastPortal1;
    [SerializeField] GameObject CastPortal2;
    [SerializeField] Collider MeeleDamage;
    [SerializeField] Collider KickDamage;

    float AngleToPlayer;
    Color colorOriginal;

    [Header("-----Projectile Type-----")]
    [SerializeField] GameObject NoramalProjectilePrefab;
    [SerializeField] GameObject SlowProjectilePrefab;
    [SerializeField] GameObject GroundProjectilePrefab;
    [SerializeField] GameObject BossProjectilePrefab;

    [Header("-----Stats-----")]
    [SerializeField] int maxHeight;
    [SerializeField] int AttackRange;
    [SerializeField] int CastRange;
    [SerializeField] float AttackDelay;
    [SerializeField] int BaseAttackDamage;
    float startSpeed;
    public float Speed;
    public int HitPoints;
    public int AttackDamage;

    [Header("-----Type-----")]
    [SerializeField] bool isStrengthBuffer;
    [SerializeField] bool isHealthBuffer;
    [SerializeField] bool isSpeedBuffer;
    [SerializeField] bool isRanged;
    [SerializeField] bool IsSpecialCaster;
    [SerializeField] bool Slower;
    [SerializeField] bool Grounder;
    [SerializeField] bool IsBoss;

    bool isBuffer;
    [Header("-----Ability Stats-----")]
    [SerializeField] int DamageBuff;
    [SerializeField] int SpeedBuff;
    [SerializeField] int HealthBuff;
    [SerializeField] int BuffRange;

    [SerializeField] List<GameObject> Drops;

    // --------------------------------------------------------------------------------------------------\\
    int EnemyAmount;
    bool isAttacking;
    bool PlayerinAttackRange;
    bool PlayerinCastRange;
    bool canAttack = true;
    bool HasHealthBuffed;
    bool HasStrengthBuffed;
    bool HasSpeedBuffed;
    bool isGrounded;
    int AttackCount, BossAttackCount;

    // --------------------------------------------------------------------------------------------------\\
    int round;
    Vector3 PlayerDrr;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        startSpeed = agent.speed;


        colorOriginal = model.material.color;

        HitPoints = BaseHitPoints;

        AttackDamage = BaseAttackDamage;

        agent.SetDestination(gameManager.gameInstance.player.transform.position);

        gameManager.gameInstance.UpdateGameGoal(1);

        agent.baseOffset = 0;

        if (gameManager.gameInstance.GetGameRound() > 0)
        {
            round = gameManager.gameInstance.GetGameRound();
        }
        else
            round = 1;

        HasHealthBuffed = false;
        HasSpeedBuffed = false;
        HasStrengthBuffed = false;

        if (isHealthBuffer || isSpeedBuffer || isStrengthBuffer)
        {
            isBuffer = true;
        }

        Speed = agent.speed;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        ApplyGravity();

        if (isRanged && !IsBoss)
        {
            agent.stoppingDistance = AttackRange / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animatorspeedtrans));

      //  ApplySeparationAndRandomMovement();

        agent.SetDestination(gameManager.gameInstance.player.transform.position);
        DestroyOutOfBounds(10);
        CheckRange();
        if (PlayerinAttackRange || PlayerinCastRange && canAttack)
        {
            StartCoroutine(AttackWithDelay());
        }

        if (isBuffer)
        {
            ApplyBuffsToNearbyZombies();
        }

        UpdateSpeed();

        ApplyGravity();

        CanSeePlayer();





    }



    void OnValidate()
    {
        // Automatically set `isHealthBuffer` to false if `isStrengthBuffer` is false
        if (!isStrengthBuffer)
        {
            DamageBuff = 0;
        }

        if (!isHealthBuffer)
        {
            HealthBuff = 0;
        }

        if (!isSpeedBuffer)
        {
            SpeedBuff = 0;
        }
        if (!isSpeedBuffer && !isHealthBuffer && !isStrengthBuffer)
        {
            BuffRange = 0;
            isHealthBuffer = false;

        }

    }






    //Damage to zombie
    /*___________________________________________________________________________________________________*/
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    public void takeDamage(int amountOfDamageTaken)
    {
        HitPoints -= amountOfDamageTaken;
        StartCoroutine(flashRed());

        if (HitPoints <= 0)
        {
            gameManager.gameInstance.UpdateGameGoal(-1);
            Destroy(gameObject);
            gameManager.gameInstance.PointCount += 25;
            LootRoll();
        }
    }


    void DestroyOutOfBounds(int m_MaxHieght)
    {

        m_MaxHieght = maxHeight;
        if (gameObject.transform.position.y >= maxHeight)
        {
            Destroy(gameObject);
            gameManager.gameInstance.UpdateGameGoal(-1);
        }
    }




    // Damge to player 
    /*________________________________________________________________________________________________________________*/
    IEnumerator AttackWithDelay()
    {
        if (!IsBoss)
        {
            canAttack = false; // Prevent immediate re-attack
            if (!isRanged)
            {
                AttackPlayer();
            }
            else
            {
                CastAtPlayer();
            }

        }

        else if (IsBoss)
        {

            if (PlayerinCastRange)
            {
                CastAtPlayer();
            }
            else if (PlayerinAttackRange)
            {
                AttackPlayer();
            }

        }
        yield return new WaitForSeconds(AttackDelay);
        canAttack = true; // Allow the next attack
    }




    void AttackPlayer()
    {

        if (!IsBoss)
        {
            if (PlayerinAttackRange == true && CanSeePlayer() == true)
            {
                animator.SetTrigger("Hit");
            }
            else

                return;
        }
        else if (IsBoss)
        {

            if (PlayerinAttackRange == true && CanSeePlayer() == true)
            {
                int randomAction = Random.Range(0, 2); // Generates a random number 0 or 1

                if (randomAction == 0)
                {
                    animator.SetTrigger("Kick");
                }
                else
                {
                    animator.SetTrigger("Hit");
                }
            }
            else
                return;

        }

    }

    void CastAtPlayer()
    {
        if (PlayerinCastRange == true && CanSeePlayer() == true)
        {
            animator.SetTrigger("Shoot");
        }
        else
            return;

    }

    void Stop()
    {
        agent.speed = 0;
    }
    void Go()
    {
        agent.speed = startSpeed;
    }


    void CastAttack()
    {
        GameObject projectile;

        if(BossAttackCount == 10 && IsBoss)
        {
            projectile = Instantiate(BossProjectilePrefab, launchPoint.position, launchPoint.rotation);
            BossAttackCount = 0;


        }



        if (AttackCount == 5 && IsSpecialCaster)
        {

            if (Slower)
            {
                projectile = Instantiate(SlowProjectilePrefab, launchPoint.position, launchPoint.rotation);
                AttackCount = 0;
            }

            else if (Grounder)
            {
                projectile = Instantiate(GroundProjectilePrefab, launchPoint.position, launchPoint.rotation);
                AttackCount = 0;
            }

        }


        else
        {

            projectile = Instantiate(NoramalProjectilePrefab, launchPoint.position, launchPoint.rotation);
            AttackCount++;
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Zombie"))
        {
            return;
        }


        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            {
                damageable.takeDamage(AttackDamage);
            }
        }
    }

    public void SetColiderON()
    {
        MeeleDamage.enabled = true;

    }

    public void SetColiderOFF()
    {
        MeeleDamage.enabled = false;
    }

    public void SetKickColiderON()
    {
        KickDamage.enabled = true;

    }

    public void SetKickColiderOFF()
    {
        KickDamage.enabled = false;
    }

    void CheckRange()
    {
        if (!isRanged)
        {
            if (AttackRange >= Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position))
            {
                PlayerinAttackRange = true;
            }
            else
                PlayerinAttackRange = false;
        }

        if (isRanged && !IsBoss)
        {
            if (CastRange >= Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position))
            {
                PlayerinCastRange = true;
            }
            else
                PlayerinCastRange = false;
        }

        if (IsBoss)
        {
            if (AttackRange >= Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position))
            {
                PlayerinAttackRange = true;
                PlayerinCastRange = false;
            }
            else if (CastRange >= Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position) && AttackRange < Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position))
            {

                PlayerinCastRange = true;
                PlayerinAttackRange = false;
            }
            else
            {
                PlayerinCastRange = false;
                PlayerinAttackRange = false;
            }

        }




    }

    void TurnOnPortals()
    {
        if (CastPortal1 != null)
        {
            CastPortal1.SetActive(true);
        }

        if (CastPortal2 != null)
        {
            CastPortal2.SetActive(true);
        }


    }
    void TurnOffPortals()
    {

        if (CastPortal1 != null)
        {
            CastPortal1.SetActive(false);
        }

        if (CastPortal2 != null)
        {
            CastPortal2.SetActive(false);
        }



    }


    // Round Updates

    public void IncreaseHitPoints(int amount)
    {

        HitPoints += amount * 5;


    }


    public void ScalingDamage(int amount)
    {

        AttackDamage += amount * 5;

    }


    public int CurrentHitPoints
    {
        get { return HitPoints; }
    }

    public void DisplayHitPoints()
    {
        // Display the current hit points
        Debug.Log("Current HP: " + CurrentHitPoints);
    }

    // Zombie Varriant;

    void ApplyBuffsToNearbyZombies()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject zombie in zombies)
        {
            float distance = Vector3.Distance(transform.position, zombie.transform.position);
            if (distance < BuffRange)
            {
                IHitPoints FellowZombie = zombie.GetComponent<IHitPoints>();
                if (FellowZombie != null)
                {
                    Debug.Log("Zombie Entered buff range");
                    ApplyBuffs(FellowZombie);
                }
            }
        }
    }

    void ApplyBuffs(IHitPoints FellowZombie)
    {
        if (FellowZombie != null)
        {
            Debug.Log("A zombie has entered the trigger zone.");

            if (isHealthBuffer)
            {
                int totalHealthBuff = HealthBuff + round;
                FellowZombie.AddHP(totalHealthBuff);
            }

            if (isStrengthBuffer)
            {
                int totalDamageBuff = DamageBuff * gameManager.gameInstance.GetGameRound();
                FellowZombie.AddDamage(totalDamageBuff);
            }

            if (isSpeedBuffer)
            {
                int totalSpeedBuff = SpeedBuff * gameManager.gameInstance.GetGameRound();
                FellowZombie.AddSpeed(totalSpeedBuff);
            }
        }
        else
        {
            Debug.Log("The object does not implement IHitPoints.");
        }
    }

    public void AddHP(int amount)
    {
        if (!HasHealthBuffed)
        {
            HitPoints += amount;
            Debug.Log("Zombie health buffed by " + amount);
            HasHealthBuffed = true;
        }
        else
        {
            Debug.Log("Health already buffed");
        }
    }

    public void AddDamage(int amount)
    {
        if (!HasStrengthBuffed)
        {
            AttackDamage += amount;
            Debug.Log("Zombie damage buffed by " + amount);
            HasStrengthBuffed = true;
        }
        else
        {
            Debug.Log("Damage already buffed");
        }
    }

    public void AddSpeed(int amount)
    {
        if (!HasSpeedBuffed)
        {
            agent.speed += amount;
            Debug.Log("Zombie speed buffed by " + amount);
            HasSpeedBuffed = true;
        }
        else
        {
            Debug.Log("Speed already buffed");
        }
    }

    void UpdateSpeed()
    {

        Speed = agent.speed;

    }

    float GetSpeed()
    {

        return Speed;

    }
    private void ApplyGravity()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
        {
            float groundDistance = hit.distance;
            if (groundDistance > 0.1f)
            {
                agent.Move(Vector3.down * groundDistance * Time.deltaTime);
            }
        }
        else
        {
            agent.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }
    bool CanSeePlayer()
    {

        PlayerDrr = gameManager.gameInstance.player.transform.position - HeadPos.position;
        AngleToPlayer = Vector3.Angle(PlayerDrr, transform.forward);

        //Debug.Log(AngleToPlayer);
        Debug.DrawRay(HeadPos.position, PlayerDrr);



        RaycastHit hit;
        if (Physics.Raycast(HeadPos.position, PlayerDrr, out hit))
        {
            if (hit.collider.CompareTag("Player") && AngleToPlayer <= ViewAngle)
            {
                agent.SetDestination(gameManager.gameInstance.player.transform.position);


                return true;
            }


            if (agent.remainingDistance <= agent.stoppingDistance)
            {

                FacePlayer();
            }

        }

        return false;

    }

    void FacePlayer()
    {

        Quaternion Rot = Quaternion.LookRotation(PlayerDrr);
        transform.rotation = Quaternion.Lerp(transform.rotation, Rot, Time.deltaTime * FacePlayerSpeed);


    }

 /*   void ApplySeparationAndRandomMovement()
    {
        Vector3 separationForce = Vector3.zero;
        float separationRadius = 3f; // Radius within which zombies will try to separate
        float separationStrength = 1f; // How strongly zombies try to separate
        float randomMovementStrength = 1f; // Random movement intensity

        // Find all zombies in the scene
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        foreach (GameObject zombie in zombies)
        {
            if (zombie != gameObject)
            {
                float distance = Vector3.Distance(transform.position, zombie.transform.position);
                if (distance < separationRadius && distance > 0.1f)
                {
                    Vector3 directionAwayFromZombie = transform.position - zombie.transform.position;
                    separationForce += (directionAwayFromZombie.normalized / distance) * separationStrength;
                }
            }
        }

        // Calculate the direction towards the player with a slight random offset
        Vector3 directionToPlayer = (gameManager.gameInstance.player.transform.position - transform.position).normalized;
        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            0f, // Keep the offset on the XZ plane
            Random.Range(-1f, 1f)
        ) * randomMovementStrength;

        Vector3 finalDirection = directionToPlayer + randomOffset;

        // Combine separation force and the directed random movement towards the player
        Vector3 finalMovement = separationForce + finalDirection;

        // Apply the final movement to the NavMeshAgent
        if (finalMovement != Vector3.zero)
        {
            Vector3 newDestination = transform.position + finalMovement.normalized;
            agent.SetDestination(newDestination);
        }
    }*/

    void LootRoll()
    {

       if(Drops.Count > 0)
        {

            int chance = UnityEngine.Random.Range(0, 100);

            if (chance < 15)
            {

                int randomIndex = UnityEngine.Random.Range(0, Drops.Count);

                Instantiate(Drops[randomIndex], agent.transform.position, agent.transform.rotation);
            }
        }

    }

}





