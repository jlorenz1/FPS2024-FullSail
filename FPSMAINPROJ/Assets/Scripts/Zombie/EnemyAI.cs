using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.ProBuilder.MeshOperations;

public class EnemyAI : MonoBehaviour, IEnemyDamage
{

    protected int round;
    [Header("-----Audio-----")]
    [SerializeField] protected AudioSource Zombie;
    [SerializeField] protected AudioClip[] ZombieFootSteps;
    [Range(0, 1)][SerializeField] protected float ZombieFootStepsVol;
    [SerializeField] protected AudioClip[] ZombieGrowl;
    [Range(0, 1)][SerializeField] protected float ZombieGrowlVol;
    [SerializeField] protected AudioClip[] ZombieHit;
    [Range(0, 1)][SerializeField] protected float ZombieHitVol;
    [SerializeField] protected AudioClip[] ZombieDeath;
    [Range(0, 1)][SerializeField] protected float ZombieDeathVol;

    [Range(0, 1)][SerializeField] protected float VoiceLineVolume;


    [Header("-----Model/Animations-----")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Renderer model;
    [SerializeField] protected Animator animator;
    [SerializeField] protected int animatorspeedtrans;
    [SerializeField] protected int FacePlayerSpeed;
    [SerializeField] protected int ViewAngle;
    [SerializeField] protected Transform HeadPos;
    [Header("-----HitBoxes-----")]
    [SerializeField] Collider Body;
    [SerializeField] Collider Head;
    [SerializeField] Collider[] Legs;
    [SerializeField] Collider[] Arms;

    [Header("-----Stats-----")]
    [SerializeField] protected float CurrentHealth;
    [SerializeField] public float MaxHealth;
     float MaxArmor = 500;
    [SerializeField] protected float Armor;
    [SerializeField] protected float Range;
    [SerializeField] protected float damage;
    [SerializeField] protected float AttackSpeed = 0.75f;
    [SerializeField] protected float castSpeed = 0.85f;
    [SerializeField] protected float AttentionSpan;
    [SerializeField] protected float sight = 25;
    [SerializeField] protected float DetectionRange = 10;


  //  [SerializeField] float flockRange = 20;
    float startsight;
    float stoppingDistance;
    [Header("-----Armor-----")]
    [SerializeField] GameObject Helmate;
    [SerializeFeild] GameObject ChestPlate;
    [SerializeFeild] GameObject Leggings;
    [SerializeFeild] GameObject boots;

    [Header("-----Other-----")]
    [SerializeField] protected List<GameObject> Drops;
    [SerializeField] float DropChance;
    [SerializeField] GameObject Gems;
    [SerializeField] int GemDropCount;
    [SerializeField] private List<GameObject> models;
    [SerializeField] LayerMask obstacleMask;

    bool isBlind;

    Rigidbody rb;

    float sfxvolume;
    float mastervolume;


    protected bool ChasingPLayer;
    protected bool PlayerinAttackRange;
    int BoostRange = 30;
    bool RangeBoosted;
    protected float startSpeed;
    Vector3 PlayerDrr;
    float AngleToPlayer;
    bool canGroan;
    Color colorOriginal;

    bool HasHealthBuffed;
    bool HasStrengthBuffed;
    bool HasSpeedBuffed;
    bool speednerfed;
    bool damagenerfed;
   protected bool AttackSpeedBuffed;

    float HitPoints;
    float legdamage;
    Vector3 currentVelocity;

    private GameObject currentModel;
    float sfxVolume;
    protected bool PlayerInSIte;
    bool roaming;
    protected bool ressitKnockBack;
    protected bool AlwaysSeePlayer;

    protected virtual void Start()
    {

        AlwaysSeePlayer = false;

        ressitKnockBack = false;

        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;

         startsight = sight;

       
        ChasingPLayer = false;
        speednerfed = false;
        damagenerfed = false;
        Body.gameObject.tag = "Zombie Body";
        Head.gameObject.tag = "Zombie Head";



        /*   Transform bodyAttatch = transform.Find("mixamorig5:Hips");
           Transform hekaOutting = new GameObject("HekaOutting").transform;
           hekaOutting.SetParent(bodyAttatch);
           hekaOutting.localPosition = new Vector3(0, .79f, 0);*/

        rb.isKinematic = true;

        for (int i = 0; i < Legs.Length; i++)
        {
            Legs[i].gameObject.tag = "Zombie Legs";
        }


        for (int i = 0; i < Arms.Length; i++)
        {
            Arms[i].gameObject.tag = "Zombie Arms";
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
          
        }

        //   AssignRandomModel();
        colorOriginal = model.material.color;
        gameManager.gameInstance.UpdateGameGoal(1);


        startSpeed = agent.speed;

        HasHealthBuffed = false;
        HasStrengthBuffed = false;
        HasSpeedBuffed = false;
        AttackSpeedBuffed = false;

        SetTagRecursively(gameObject, "Zombie");
        /*   roam();*/

    }

    protected virtual void Update()
    {
        if (agent.enabled == true)
        {
            FacePlayer();

            CheckRange();

            if (Range / 2 >= 2)
            {
                agent.stoppingDistance = Range / 2;
            }
            else
                agent.stoppingDistance = 2;


            agent.SetDestination(gameManager.gameInstance.player.transform.position);


            sfxVolume = AudioManager.audioInstance.GetSFXAudioVolume();

            // ApplySeparationAndRandomMovement();
            // OutOfRangeBoost();
            currentVelocity = agent.velocity;
            float maxSpeed = agent.speed;
            float currentSpeed = currentVelocity.magnitude;

            float normalizedSpeed = Mathf.Clamp(currentSpeed / maxSpeed, 0, 1);
            animator.SetFloat("Speed", normalizedSpeed);

            CanSeePlayer();
            ApplyGravity();


            rb.velocity = Vector3.zero;

            if (legdamage >= MaxHealth / 2)
            {
                Cripple();
            }

            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }

            if (Armor > MaxArmor)
            {
                Armor = MaxArmor;
            }


            if (ChasingPLayer)
            {
                roaming = false;
            }
         
        }
    }

    // Death and Damage mechanics 
    void SetTagRecursively(GameObject obj, string tag)
    {
        // Set the tag for the parent object
        obj.tag = tag;

        
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }
    public virtual void takeDamage(float amount)
    {
      
     

        float damageReduced = amount * Armor / 500;
        float TotalDamage = amount - damageReduced;
        //  PlaySFX(ZombieHit[Random.Range(0, ZombieHit.Length)], ZombieHitVol);
        CurrentHealth -= TotalDamage;
        if (CurrentHealth <= 0)
        {
            Die();
        }


    }

    protected virtual void Die()
    {
        // Common death logic
        StopAllCoroutines();
        RagDoll();

        if (Drops.Count > 0)
        {
            LootRoll(DropChance);
        }
       gameManager.gameInstance.UpdateGameGoal(-1);
      
        Destroy(gameObject, 4);
    }

    public void DieWithoutDrops()
    {
        gameManager.gameInstance.UpdateGameGoal(-1);
        Destroy(gameObject);
    }


    void RagDoll()
    {

        animator.enabled = false;


        agent.enabled = false;

        Rigidbody Fall = agent.GetComponent<Rigidbody>();
        if (Fall == null)
        {
            Fall = agent.gameObject.AddComponent<Rigidbody>();
        }

        if (Fall != null)
        {
            Fall.isKinematic = false;
            Fall.useGravity = true;
            Fall.constraints = RigidbodyConstraints.FreezePositionY;
            Fall.collisionDetectionMode = CollisionDetectionMode.Continuous;
           
           
        }



    }

    void LootRoll(float DropChance)
    {

        int chance = UnityEngine.Random.Range(0, 100);
        int DropItem = UnityEngine.Random.Range(0, Drops.Count);
        float yOffset = 1.0f;
        int GemsDropped = 0;
      
        
        if(chance < DropChance && Drops != null) {

            Instantiate(Drops[DropItem], new Vector3(agent.transform.position.x, agent.transform.position.y + yOffset, agent.transform.position.z), agent.transform.rotation);

        }

// Handeling Gem Drops 
        if (Gems != null)
        {
            for (int i = 0; i < 2; i++)
            {
                Instantiate(Gems, new Vector3(agent.transform.position.x, agent.transform.position.y + 1f, agent.transform.position.z), agent.transform.rotation);
                GemsDropped++;
            }
        }

        while (GemsDropped < GemDropCount && Gems != null)
        {

            if (chance < DropChance) 
            {

              //  int randomIndex = UnityEngine.Random.Range(0, Drops.Count);

                Instantiate(Gems, new Vector3(agent.transform.position.x, agent.transform.position.y + yOffset, agent.transform.position.z), agent.transform.rotation);

                GemsDropped++;



           
            }
            else
                break;
                break;
        }

    }


    //movement mechanics 
    void ApplySeparationAndRandomMovement()
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
    }

    void OutOfRangeBoost()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position);

        if (BoostRange <= distanceToPlayer && !RangeBoosted)
        {
            agent.speed = startSpeed + 30;
            RangeBoosted = true;
        }
        else if (RangeBoosted && BoostRange >= distanceToPlayer)
        {

            agent.speed = startSpeed;
            RangeBoosted = false;
        }

        else
            return;

    }


    //Player interaction 
    void CanSeePlayer()
    {
        PlayerDrr = gameManager.gameInstance.player.transform.position - HeadPos.position;
        AngleToPlayer = Vector3.Angle(PlayerDrr, transform.forward);

        // Draw a ray for debugging
        Debug.DrawRay(HeadPos.position, PlayerDrr, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(HeadPos.position, PlayerDrr, out hit, sight, ~obstacleMask))
        {
            if (hit.collider.CompareTag("Player") && AngleToPlayer <= ViewAngle)
            {
                PlayerInSIte = true;
              
            }
            else
            {
                PlayerInSIte = false;
              
            }
        }
 

    }


    protected void FacePlayer()
    {
        Vector3 direction = (gameManager.gameInstance.player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));  // Ignore y-axis for rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);  // Adjust the rotation speed
    }

    void OnAnimatorIK(int layerIndex)
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        }
    }


    void CheckRange()
    {
        float Distance = Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position);

        {
            if (Range >= Distance)
            {
                PlayerinAttackRange = true;
            }
            else
                PlayerinAttackRange = false;

       
        }
    }


/*   protected void TargetPlayer()
    {
      
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(gameManager.gameInstance.player.transform.position);
        FlockPlayer();
    }
*/

  /*  void roam()
    {
        roaming = true;
        float roamRange = 10f;
       
        // Get a random point within a short range around the enemy
        Vector3 randomPosition = GetRandomPositionWithinRange(transform.position, roamRange);

        // Move the enemy towards that random position
        agent.SetDestination(randomPosition);

        // If the enemy reaches the random point, pick another one after a short delay

        StartCoroutine(WaitThenRoamAgain());

    }

    IEnumerator WaitThenRoamAgain()
    {
        agent.stoppingDistance = 0;
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null; // Wait until the next frame
        }

        if (ChasingPLayer)
        {
            agent.stoppingDistance = Range;
            yield break;
        }

        yield return new WaitForSeconds(2);

        roam();
        agent.stoppingDistance = Range;
    }*/





    // Helper method to get a random position within a defined range
    Vector3 GetRandomPositionWithinRange(Vector3 origin, float range)
    {
        // Generate random values within a circular area around the origin
        float randomX = Random.Range(-range, range);
        float randomZ = Random.Range(-range, range);

        // Return a new position around the origin with the random offsets
        return new Vector3(origin.x + randomX, origin.y, origin.z + randomZ);
    }




    //world interaction
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
    //Genral audio


    protected void PlaySFX(AudioClip audio, float vol)
    {


      float currentVol =  adjustedVolume(vol);

        if (audio != null)
        {
            Zombie.PlayOneShot(audio, currentVol);
        }
      
    }


    protected void PlayVoice(AudioClip audio)
    {
        if (audio != null)
        {
            float currentVol = adjustedVolume(VoiceLineVolume);
            Zombie.PlayOneShot(audio, currentVol);
            
        }
    }


    protected IEnumerator Groan()
    {
        canGroan = false;
        int index = Random.Range(0, ZombieGrowl.Length);
        AudioClip Hit = ZombieGrowl[index];


        PlaySFX(Hit, ZombieGrowlVol);
        float delay = Random.Range(5f, 10f);
        //use a random delay so each instance doesnt do it at the same time.
        yield return new WaitForSeconds(delay);

        canGroan = true;
    }
    public void PlayFootstep()
    {
      
        if (ZombieFootSteps.Length > 0)
        {
            // Randomly select a footstep clip from the array
            int index = Random.Range(0, ZombieFootSteps.Length);
            AudioClip footstep = ZombieFootSteps[index];

            // Play the selected clip through the AudioSource
            PlaySFX(footstep, ZombieFootStepsVol);
           
        }
    }

    float adjustedVolume(float vol)
    {
        // Get the current SFX volume
       

        // Adjust the input volume based on the SFX volume percentage
        float adjustedVol = vol * sfxVolume;

        return adjustedVol;
    }

    /*   // Model and Animations 
       void AssignRandomModel()
       {
           if (models != null && models.Count > 0)
           {
               // Select a random model from the list
               int randomIndex = Random.Range(0, models.Count);
               GameObject selectedModel = models[randomIndex];

               // Destroy any existing model first
               if (currentModel != null)
               {
                   Destroy(currentModel);
               }

               // Instantiate the selected model as a child of the enemy
               currentModel = Instantiate(selectedModel, transform.position, transform.rotation, transform);

               // Update references to renderer and animator
               model = currentModel.GetComponent<Renderer>();
               animator = gameObject.GetComponent<Animator>();

               // Optionally, deactivate other models if they exist on the list object
               foreach (var model in models)
               {
                   if (model != selectedModel)
                   {
                       model.SetActive(false);
                   }
               }
           }
       }
   */

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }


    protected IEnumerator SmoothTransitionSpeed(float targetSpeed, float duration)
    {
        float startSpeed = agent.speed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / duration);
            agent.speed = newSpeed;
            if (animator != null)
            {
                animator.SetFloat("Speed", newSpeed);
            }

            yield return null; // Wait until the next frame
        }

        // Ensure final values are set correctly
        agent.speed = targetSpeed;
        if (animator != null)
        {
            animator.SetFloat("Speed", targetSpeed);
        }
    }




    //  static modifiers;

    public void AddHP(float amount)
    {
        CurrentHealth += amount;
    }


    public void AddMaxHp(float amount)
    {
      
        {
            MaxHealth += amount;
          
            HasHealthBuffed = true;
        }
    
    }




    public void AddDamage(float amount)
    {
       
        {
            damage += amount;
          
            HasStrengthBuffed = true;
        }
       
    }

    public void AddSpeed(float amount)
    {
      
        {
            agent.speed += amount;
         
            HasSpeedBuffed = true;
        }
       
    }


    public void SetDamage(float amount)
    {
        damage = amount;
    }
    public float GetDamage()
    {
        return damage;
    }

    public void ScalingDamage(float amount)
    {
        damage += amount * 1.334f;
    }

    public void IncreaseHitPoints(float amount)
    {
        MaxHealth += amount * 1.334f;
    }

    public void cutspeed(float amount, float damagetaken)
    {

        legdamage += damagetaken;
        if (!speednerfed)
        {

            agent.speed /= amount;

            speednerfed = true;
        }
        else
            return;
    }

    public void cutdamage(float amount)
    {

        if (!damagenerfed)
        {
            damage /= amount;

            damagenerfed = true;
        }
        else
            return;
    }


    public  void AddAttackSpeed(float amount)
    {
            AttackSpeed *= amount;
            castSpeed *= amount;
            
        if(AttackSpeed > 3)
        {
            AttackSpeed = 3;
        }
        if(castSpeed > 3)
        {
            castSpeed = 3;
        }

    }


    protected virtual void Cripple()
    {
        animator.SetBool("Cripple", true);

        if (agent.speed / 2 > 1)
        {
            agent.speed /= 2;
        }
        else
            agent.speed = 1;
    }

    public void AddArmor(float amount)
    {
        if (Armor + amount < 500)
        {
            Armor += amount;
        }
        else
            Armor = 500;
    }

    public void RemoveArmor(float amount)
    {
        if (Armor - amount > 0)
        {
            Armor += amount;
        }
        else
            Armor = 0;
    }

    public void TempRemoveArmor(float reduction, float Duration)
    {

        StartCoroutine(ArmorStrip(Duration, reduction));

    }

    IEnumerator ArmorStrip(float StripDurration, float reduction)
    {

        RemoveArmor(reduction);
        yield return new WaitForSeconds(StripDurration);
        AddArmor(reduction);
    }

    public void TakeTrueDamage(float amountOfDamageTaken)
    {

      


         PlaySFX(ZombieHit[Random.Range(0, ZombieHit.Length)], ZombieHitVol);
        CurrentHealth -= amountOfDamageTaken;
        if (MaxHealth <= 0)
        {
            Die();
        }

    }




    public void Blind(float duration)
    {

        StartCoroutine(blind(duration));


    }
    IEnumerator blind(float duration)
    {
        isBlind = true;
        sight = 0;
        ChasingPLayer = false;
     
        yield return new WaitForSeconds(duration);
        isBlind = false;
        sight = startsight;

    }

    public void Stun(float duration)
    {
        StartCoroutine(stun(duration));
    }

    IEnumerator stun(float duration)
    {
        agent.speed = 0;

        yield return new WaitForSeconds(duration);

        agent.speed = startSpeed;
    }



    public void knockback(Vector3 hitPoint, float distance, float duration)
    {
        if (!ressitKnockBack)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.enabled = false; // Disable NavMesh Agent during knockback
            Vector3 knockbackDirection = (transform.position - hitPoint).normalized;
            knockbackDirection.y = 0;

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(knockbackDirection * distance, ForceMode.Impulse); // Apply force for knockback

            StartCoroutine(ResetNavMeshAgent(agent, duration)); // Reset the agent after the knockback
        }
        else return;
    }

    private IEnumerator ResetNavMeshAgent(NavMeshAgent agent, float duration)
    {
        yield return new WaitForSeconds(duration);
        agent.enabled = true; // Re-enable NavMesh Agent
     
    }

    protected IEnumerator Stop(float duration)
    {
       
        agent.acceleration = 300;
        agent.speed = 0;
        yield return new WaitForSeconds(duration);
        agent.speed = startSpeed;
        agent.acceleration = 8;
    }

    
  public  float GetMaxHP()
    {
        return MaxHealth;
    }


   public bool isKnockBackRessitant()
    {

        return ressitKnockBack;

    }


}
