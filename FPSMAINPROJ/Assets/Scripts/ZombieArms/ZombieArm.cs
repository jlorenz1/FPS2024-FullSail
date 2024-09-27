using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieArm : MonoBehaviour, IEnemyDamage
{
    public float HP = 0;
    public float HPMax = 0;
    public int damage = 0;

    void Start()
    {
        HP = HPMax; 
        //play animation
    }
    public void OnTriggerEnter(Collider other)
    {
        var dmg = other.gameObject.GetComponent<IDamage>();
        if(dmg != null)
        {
            dmg.takeDamage(damage);
        }
    }

    public void takeDamage(float damage)
    {
        
        if (HP > 0)
        {
            HP -= damage;
            if(HP >= 0)
            {
                Destroy(gameObject);
               
            }
        }
    }

   public void TakeTrueDamage(float amountOfDamageTaken)
    {

    }

    public void AddHP(float amount)
    {

    }

    public void AddMaxHp(float amount)
    {

    }

    public void AddDamage(float amount) 
    {

    }

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

    public void knockback(Vector3 hitPoint, float distance, float Duration) { }

    public float GetMaxHP()
    {
        return 0;
    }
    public bool isKnockBackRessitant()
    {

        return true;

    }
}
