using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyDamage
{
    // Interface to assign damage to an object

    void takeDamage(float amountOfDamageTaken);

    void TakeTrueDamage(float amountOfDamageTaken);

    void AddHP(int amount);

    void AddDamage(int amount);

    void AddSpeed(int amount);

    void cutspeed(float amount, float damagetaken);

    void cutdamage(float amount);

    void DieWithoutDrops();

    void AddArmor(float amount);

    void RemoveArmor(float amount);

    void TempRemoveArmor(float reduction, float Duration);

    void bounceHeka(RaycastHit hit)
    {
        Vector3 origin = hit.point;

        Debug.Log("entering bounce");
        RaycastHit newRay;
        float requiredDist = 100f;
        ////get nearest enemy

        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        Vector3 closetestTrans = Vector3.zero;

        foreach(GameObject zombie in zombies)
        {
            float distance = Vector3.Distance(hit.collider.gameObject.transform.position, zombie.transform.position);

            
            
                if (distance < requiredDist)
                {
                    Debug.Log(zombie.transform.position);
                    closetestTrans = zombie.transform.position;
                    break;
                }
            
        }

        
        if (Physics.Raycast(hit.transform.position, closetestTrans, out newRay))
        {
            Debug.DrawRay(hit.transform.position, closetestTrans, Color.red);
            IEnemyDamage dmg = newRay.transform.GetComponent<IEnemyDamage>();
            if(dmg != null)
            {
                Debug.Log("Hit other enemey");
                dmg.takeDamage(1f);
            }
        }
        
        
        //if (Physics.Raycast(hit.transform.position, enemy.gameObject.transform.position, out newRay, requiredDist))
        //{
        //    var dmg = newRay.collider.gameObject.GetComponent<IDamage>();
        //    if (dmg != null)
        //    {
        //        //takedamage
        //        Debug.Log("bounced and hit");
        //    }
        //}
    }
}
