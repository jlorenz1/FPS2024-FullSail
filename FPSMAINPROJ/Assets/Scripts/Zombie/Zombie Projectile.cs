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

    Rigidbody rb;
    Transform playerTransform;

    float OriginalSpeed;
    int OriginalJumpCount;
    float Nerf;
    GameObject player;
    int NerfTimer;
    int round;
    void Start()
    {

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
    }
        void Update()
    {
        round = gameManager.gameInstance.GetGameRound();
        SetNerfTimer();
        SetBuffStrength();
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
            else
            {
                transform.Translate(direction * speed * Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger || other.CompareTag("Zombie"))
        {
            return;
        }
        else
        {
            if (IsNormal)
            {
                if (other.CompareTag("Player"))
                {
                    gameManager.gameInstance.playerScript.takeDamage(projectileDamage);
                    Destroy(gameObject);
                }
            }
            else if (other.CompareTag("Player"))
            {
                // Stick to the player
                player = other.gameObject;
                transform.parent = player.transform;

                // Apply debuffs immediately
                ApplyDebufs();

                // Start the coroutine to reset stats and destroy the projectile
                StartCoroutine(StatReset());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }



    void ApplyDebufs()
    {
        if(IsSlow)
        {
            if (OriginalSpeed / Nerf >= 1)
            {
                gameManager.gameInstance.playerScript.SetSpeed(OriginalSpeed / Nerf);
            }
            else
                gameManager.gameInstance.playerScript.SetSpeed(1);
        }

        if (IsGround)
        {
            gameManager.gameInstance.playerScript.SetJumpCount(0);
        }
       
       if(IsBossAttack)
        {
            if (OriginalSpeed / Nerf >= 1)
            {
                gameManager.gameInstance.playerScript.SetSpeed(OriginalSpeed / Nerf);
            }
            else
                gameManager.gameInstance.playerScript.SetSpeed(1);

            gameManager.gameInstance.playerScript.SetJumpCount(0);

        }
    }

    IEnumerator StatReset()
    {

        yield return new WaitForSeconds(NerfTimer);

        gameManager.gameInstance.playerScript.SetSpeed(OriginalSpeed);
        gameManager.gameInstance.playerScript.SetJumpCount(OriginalJumpCount);
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
