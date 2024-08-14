using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage, IHitPoints
{
    [SerializeField] Renderer model;
    [SerializeField] int BaseHitPoints;


    [SerializeField] int maxHeight;
    [SerializeField] int AttackRange;
    [SerializeField] int AttackDelay;
    [SerializeField] int BaseAttackDamage;
    [SerializeField] NavMeshAgent agent;



    [SerializeField] bool isStrengthBuffer;
    [SerializeField] bool isHealthBuffer;
    [SerializeField] bool isSpeedBuffer;
    [SerializeField] int DamageBuff;
    [SerializeField] int SpeedBuff;
    [SerializeField] int HealthBuff;
    [SerializeField] int BuffRange;

    [SerializeField] bool isDebuffer;

   public int HitPoints;

    int AttackDamage;

    int EnemyAmount;

    bool isAttacking;

    bool PlayerinRange;

    bool canAttack = true;

    Color colorOriginal;

    bool isBuffer;

    bool HasHealthBuffed;

    bool HasStrengthBuffed;

    bool HasSpeedBuffed;

    void Start()
    {

        colorOriginal = model.material.color;

        HitPoints = BaseHitPoints;

        AttackDamage = BaseAttackDamage;

        agent.SetDestination(gameManager.gameInstance.player.transform.position);

        agent = GetComponent<NavMeshAgent>();

        gameManager.gameInstance.UpdateGameGoal(1);

        int round;

        if ( gameManager.gameInstance.GetGameRound() > 0)
        {
            round = gameManager.gameInstance.GetGameRound();
        }
        else
            round = 1;

        HasHealthBuffed = false;
        HasSpeedBuffed = false;
        HasStrengthBuffed = false;



    }

    // Update is called once per frame
    void Update()
    {

        agent.SetDestination(gameManager.gameInstance.player.transform.position);
        DestroyOutOfBounds(10);
        CheckRange();
        if (PlayerinRange && canAttack)
        {
            StartCoroutine(AttackWithDelay());
        }
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
        if(!isSpeedBuffer && !isHealthBuffer && !isStrengthBuffer)
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


    /*  private void OnCollisionEnter(Collision collision)
      {
          if (collision.gameObject.CompareTag("Bullet"))
          {
              takeDamage(1);

              if (HitPoints <= 0)
              {
                  Destroy(gameObject);
              }
          }

      }*/

    public void takeDamage(int amountOfDamageTaken)
    {
        HitPoints -= amountOfDamageTaken;
        StartCoroutine(flashRed());
        if (HitPoints <= 0)
        {
            gameManager.gameInstance.UpdateGameGoal(-1);
            Destroy(gameObject);
          
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
        canAttack = false; // Prevent immediate re-attack

        // Perform the attack
        AttackPlayer();

        // Wait for the specified attack delay
        yield return new WaitForSeconds(AttackDelay);

        canAttack = true; // Allow the next attack
    }




    void AttackPlayer()
    {
        if (PlayerinRange == true)
        {
            IDamage DMG = gameManager.gameInstance.player.GetComponent<IDamage>();
            DMG.takeDamage(AttackDamage);
        }
        else
            return;

    }

    void CheckRange()
    {
        if (AttackRange >= Vector3.Distance(transform.position, gameManager.gameInstance.player.transform.position))
        {
            PlayerinRange = true;
        }
        else
            PlayerinRange = false;
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

    void OnTriggerEnter(Collider other)
    {

     

    
        
        int TotalBuff;
        IHitPoints FellowZombie = other.GetComponent<IHitPoints>();

        if (FellowZombie != null)
        {
            Debug.Log("A zombie has entered the trigger zone.");




            if (isHealthBuffer == true && HasHealthBuffed == false)
            {
                TotalBuff = HealthBuff * gameManager.gameInstance.GetGameRound();
                FellowZombie.AddHP(TotalBuff);
                HasHealthBuffed = true;
                Debug.Log("ZombieBuffed");
            }
            if (isStrengthBuffer == true && HasStrengthBuffed == false)
            {
                TotalBuff = DamageBuff * gameManager.gameInstance.GetGameRound();
                FellowZombie.AddHP(TotalBuff);
                HasStrengthBuffed = true;
                Debug.Log("ZombieBuffed");

            }
            if (isSpeedBuffer == true && HasSpeedBuffed == false)
            {
                TotalBuff = SpeedBuff * gameManager.gameInstance.GetGameRound();
            }
        }

        else
        {
            Debug.Log("The object does not implement IHitPoints.");
        }

    }



   public void AddHP(int amount)
    {
        HitPoints += amount;
    }

   public void AddDamage(int amount)
    {

        AttackDamage += amount;

    }

  public  void AddSpeed(int amount)
    {


    }







}





