using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;



public class EnemySheilds : MonoBehaviour, IEnemyDamage
{
    public float HitPoints;
    [SerializeFeild] GameObject Body;
    public bool IsActive;



    void Start()
    {
        gameObject.tag = "Zombie";
        IsActive = true;
    }

    public void takeDamage(float amountOfDamageTaken) {

        HitPoints -= amountOfDamageTaken;

        if(HitPoints < 0)
        {
            gameManager.gameInstance.Userkare.SheildActive = false;
            Destroy(gameObject);
           
        }


    }
   public void SetHitPoints(float hitPoints)
    {
        HitPoints = hitPoints;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            gameManager.gameInstance.pausePlayerControls();
            // Get the player's position
            Transform playerTransform = collision.transform;

            // Calculate the direction away from the collision
            Vector3 knockbackDirection = (playerTransform.position - transform.position).normalized;

            // Define the knockback distance
            float knockbackForce = 5f; // Adjust this value to change the intensity of the knockback

            // Apply knockback
           gameManager.gameInstance.player.transform.position += knockbackDirection * knockbackForce;

            gameManager.gameInstance.resumePlayerControls();
        }
    }



    public void TakeTrueDamage(float amountOfDamageTaken) { }

    public void AddHP(float amount) { }

    public void AddMaxHp(float amount) { }

    public void AddDamage(float amount) { }

    public void AddSpeed(float amount) { }


    public void AddAttackSpeed(float amount) { }



    public void cutspeed(float amount, float damagetaken) { }

    public void cutdamage(float amount) { }

    public void DieWithoutDrops() { }

    public void AddArmor(float amount) { }

    public void RemoveArmor(float amount) { }

    public void TempRemoveArmor(float reduction, float Duration) { }

    public void Blind(float duration) { }

    public void Stun(float duration) { }

    public void knockback(Vector3 hitPoint, float distance) { }

    public float GetMaxHP() {

        return 100;
    
    }
}
