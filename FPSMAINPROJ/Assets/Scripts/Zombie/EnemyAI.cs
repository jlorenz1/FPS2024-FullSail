using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    protected int round;
    [SerializeField] protected AudioSource Zombie;
[SerializeField] protected AudioClip[] ZombieFootSteps;
[SerializeField] protected float ZombieFootStepsVol;
[SerializeField] protected AudioClip[] ZombieGrowl;
[SerializeField] protected float ZombieGrowlVol;
[SerializeField] protected AudioClip[] ZombieHit;
[SerializeField] protected float ZombieHitVol;
[SerializeField] protected AudioClip[] ZombieDeath;
[SerializeField] protected float ZombieDeathVol;
[SerializeField] protected NavMeshAgent agent;
[SerializeField] public float health = 100f;
[SerializeField] protected Renderer model;
[SerializeField] protected int BaseHitPoints;
[SerializeField] protected Animator animator;
[SerializeField] protected int animatorspeedtrans;
[SerializeField] protected int FacePlayerSpeed;
[SerializeField] protected int ViewAngle;
[SerializeField] protected Transform HeadPos;
[SerializeField] protected List<GameObject> Drops;
  protected  bool PlayerinAttackRange;
    int BoostRange = 30;
    bool RangeBoosted;
    float startSpeed;
    Vector3 PlayerDrr;
    float AngleToPlayer;
    bool canGroan;
    Color colorOriginal;
 [SerializeField] Collider Body;
[SerializeField] Collider Head;
[SerializeField] Collider[] Legs;
[SerializeField] Collider[] Arms;
    bool HasHealthBuffed;
    bool HasStrengthBuffed;
    bool HasSpeedBuffed;

  [ SerializeField] protected float damage;
    float HitPoints;

    [SerializeField] private List<GameObject> models;
    private GameObject currentModel;

    [SerializeField] float Range;

    protected virtual void Start()
    {

        Body.gameObject.tag = "Zombie Body";
        Head.gameObject.tag = "Zombie Head";
        for(int i = 0; i < Legs.Length; i++)
        {
            Legs[i].gameObject.tag = "Zombie Legs";
        }


        for (int i = 0; i < Arms.Length; i++)
        {
            Arms[i].gameObject.tag = "Zombie Arms";
        }

        agent = GetComponent<NavMeshAgent>();
        if(agent == null)
        {
            Debug.LogError("NO Nav MESH");
        }

        AssignRandomModel();
        colorOriginal = model.material.color;
        gameManager.gameInstance.UpdateGameGoal(1);
      
     
        startSpeed = agent.speed;

        HasHealthBuffed = false;
        HasStrengthBuffed = false;
        HasSpeedBuffed = false;


    }

    protected virtual void Update()
    {
        agent.SetDestination(gameManager.gameInstance.player.transform.position);
        ApplySeparationAndRandomMovement();
        OutOfRangeBoost();
       CanSeePlayer();
        ApplyGravity();

        CheckRange();
        if (canGroan == true)
        {
            StartCoroutine(Groan());
        }
    }

    // Death and Damage mechanics 

    public virtual void takeDamage(float amount)
    {
       // StartCoroutine(flashRed());
      //  PlayAudio(ZombieHit[Random.Range(0, ZombieHit.Length)], ZombieHitVol);
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Common death logic
        LootRoll(5);
        gameManager.gameInstance.UpdateGameGoal(-1);
        Destroy(gameObject);
    }

    void LootRoll(int DropChance)
    {

        if (Drops.Count > 0)
        {

            int chance = UnityEngine.Random.Range(0, 100);
            float yOffset = 1.0f;

            if (chance < DropChance)
            {

                int randomIndex = UnityEngine.Random.Range(0, Drops.Count);

                Instantiate(Drops[randomIndex], new Vector3(agent.transform.position.x, agent.transform.position.y + yOffset, agent.transform.position.z), agent.transform.rotation);
            }
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


   protected void PlayAudio(AudioClip audio, float volume)
    {
        Zombie.PlayOneShot(audio, volume);
    }

    protected IEnumerator Groan()
    {
        canGroan = false;
        int index = Random.Range(0, ZombieGrowl.Length);
        AudioClip Hit = ZombieGrowl[index];


        PlayAudio(Hit, ZombieGrowlVol);
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
            Zombie.PlayOneShot(footstep, ZombieFootStepsVol);
        }
    }

    // Model and Animations 
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


    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

  //  static modifiers;

    public void AddHP(int amount)
    {
        if (!HasHealthBuffed)
        {
            health += amount;
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
            damage += amount;
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


    public void SetDamage(float amount)
    {
        damage = amount;
    }
    public float GetDamage()
    {
        return damage; 
    }

 public void  ScalingDamage(float amount)
    {
        damage += amount*1.334f;
    }

  public void IncreaseHitPoints(float amount)
    {
        health += amount * 1.334f;
    }
       
}
