using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieArm : MonoBehaviour, IDamage
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

    public float GetHealth()
    {
        return 0f;
    }

}
