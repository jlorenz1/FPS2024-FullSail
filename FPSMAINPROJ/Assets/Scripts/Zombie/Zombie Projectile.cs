using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Projectile : MonoBehaviour
{
   [SerializeField] float speed;
   [SerializeField] float lifetime;
   [SerializeField] bool IsNormal;
   [SerializeField] bool IsSlow;
   [SerializeField] bool IsGround;
   [SerializeField] bool IsBossAttack;
   [SerializeField] int projectileDamage;
    [SerializeField] float followtime;
    [SerializeField] AudioSource ProjectileAudio;

    [SerializeField] AudioClip ZombieProjectileAudio;
    [SerializeField, Range(0f, 1f)] float ZombieProjectileAudioVol;
    [SerializeField] bool SekhmetAttack;
    [SerializeField] bool UserKareBaseAttack;
    [SerializeField] bool UserKareSpecialAttack1;
    [SerializeField] bool UserKareSpecialAttack2;
    Rigidbody rb;
    Transform playerTransform;

    bool followPlayer;
    float OriginalSpeed;
    int OriginalJumpCount;
    Vector3 currentDirection;
    float Nerf;
    GameObject player;
    float NerfTimer;
    int round;
    float StartSpeed;

    public GameObject aoePrefab;    // Assign your AoE slow effect prefab here
    public float aoeRadius = 5f;    // Radius of the AoE slow effect
    public float slowDuration = 5f; // How long the slow effect lasts on the player
    public float slowEffectDuration = 3f; // How long the player stays slowed


    void Start()
    {
        followPlayer = true;
        int projectileLayer = gameObject.layer;
        int zombieLayer = LayerMask.NameToLayer("Zombie");
        StartSpeed = gameManager.gameInstance.playerScript.GetSpeed();
        Physics.IgnoreLayerCollision(projectileLayer, zombieLayer);
        Nerf = 3.00f;
        NerfTimer = 5.00f;
        if (ProjectileAudio != null)
        {
            ProjectileAudio.clip = ZombieProjectileAudio;
            ProjectileAudio.volume = ZombieProjectileAudioVol;
            ProjectileAudio.loop = true; // Enable looping
            ProjectileAudio.Play(); // Start playing the audio
        }

        OriginalSpeed = gameManager.gameInstance.playerScript.GetSpeed();
        OriginalJumpCount = gameManager.gameInstance.playerScript.GetJumpCount();
        // Destroy the projectile after a certain amount of time
        Destroy(gameObject, lifetime);



        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Ensure gravity does not affect the projectile
        }

        // Find the player object and initialize playerTransform
        player = gameManager.gameInstance.playerScript.gameObject;
        if (player != null)
        {
            playerTransform = player.transform;
        }

        StartCoroutine(FollowPlayer(followtime));

    }
        void Update()
    {
        round = gameManager.gameInstance.GetGameRound();
       // SetNerfTimer();
       // SetBuffStrength();
        if (followPlayer && player != null)
        {
            // During the first second, follow the player
            currentDirection = (player.transform.position - transform.position).normalized;
        }

        // Move the projectile in the current direction
        if (rb != null)
        {
            rb.velocity = currentDirection * speed;
        }
        else
        {
            transform.Translate(currentDirection * speed * Time.deltaTime);
        }
    }

    IEnumerator FollowPlayer(float seconds)
    {
        // Follow the player for 1 second
        yield return new WaitForSeconds(seconds);

        // Stop following the player and continue in the last known direction
        followPlayer = false;
    }

    void OnDestroy()
    {
        // Stop the audio when the object is destroyed
        if (ProjectileAudio != null)
        {
            ProjectileAudio.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
{
    if (other.isTrigger || other.CompareTag("Zombie"))
    {
        return;
    }

        if (other.isTrigger || other.CompareTag("Zombie"))
        {
            return;
        }

        if (SekhmetAttack)
        {
            HandelSekhmetAttack(other);
        }
       
        else if (UserKareSpecialAttack1)
        {
            HandelUskareSpecialAttack(other);
        }

        else
        {
            HandleNonBossAttack(other);
        }

    }
    void HandelUskareSpecialAttack(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.gameInstance.playerScript.CutSpeed(5, 100f);
            if(gameManager.gameInstance.isSekhmetDead == false)
            {
                gameManager.gameInstance.BlinkingJab = true;
            }
            Destroy(gameObject);
        }

    }
    void HandelSekhmetAttack(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            CreateAoESlow(other.transform.position);
        }
        else if(other.CompareTag("Player"))
        {
            gameManager.gameInstance.playerScript.takeDamage(5);
            gameManager.gameInstance.playerScript.CutSpeed(5, 1.5f);
            if (gameManager.gameInstance.isUserKareDead == false)
            {
              
               gameManager.gameInstance.LightGautlening = true;
            }

            Destroy(gameObject);
        }

    }


    void CreateAoESlow(Vector3 position)
    {
        // Instantiate the AoE prefab at the position passed (the player's position or other)
        GameObject aoe = Instantiate(aoePrefab, position, Quaternion.identity);

        // Set the AoE effect to destroy itself after the slow duration ends
        Destroy(aoe, slowDuration);
    }

  

    void HandleNonBossAttack(Collider other)
{
    if (!other.CompareTag("Player"))
    {
        Destroy(gameObject);
        return;
    }
        gameManager.gameInstance.playerScript.takeDamage(projectileDamage);
    if (IsNormal)
    {
        Destroy(gameObject);
    }
    else
    {
        StickToPlayer(other);
    }
}

void StickToPlayer(Collider other)
{
    player = other.gameObject;
    transform.parent = player.transform;
        if (ProjectileAudio != null)
        {
            ProjectileAudio.Stop();
        }
        ApplyDebufs();
    
}



    void ApplyDebufs()
    {
        if(IsSlow)
        {
            gameManager.gameInstance.playerScript.CutSpeed(5, 1.5f);
        }

        if (IsGround)
        {
            gameManager.gameInstance.playerScript.SetJumpCount(0);
        }

      
    }

   
    public void SetBuffStrength()
    {
        Nerf = round * 2;
    }

    public void SetNerfTimer()
    {
        if (round * 3 >= 10)
        {
            NerfTimer = round * 3;
        }
        else if(round * 3 >= 60)
        {
            NerfTimer = 60;
        }
        else if(round * 3 < 10)
        {
            NerfTimer = 10;
        }

    }

   
}
