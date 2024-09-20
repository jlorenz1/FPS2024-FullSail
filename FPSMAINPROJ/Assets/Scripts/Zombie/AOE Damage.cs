using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum AOETYPE
{
    Slow = 0,
    Damage = 1,
    Sekhmet = 3,
    UserKare = 4,
}

public class AOEDamage : MonoBehaviour
{
    AOETYPE type;
    float Duration = 5;
    float Strength = 5;
    float Radius;
    [SerializeFeild] GameObject Area;
    IDamage Playerdamage;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        
        if ( other.CompareTag("Player"))
        {
             Playerdamage = other.GetComponent<IDamage>();
            if (Playerdamage != null)
            {
              
                if (type == AOETYPE.Slow)
                {
                  
                    Playerdamage.CutSpeed(Duration, Strength);
         
                }

                if(type == AOETYPE.Damage)
                {


                    Playerdamage.takeDamage(Strength);

                }

                if (type == AOETYPE.Sekhmet)
                {
                    Playerdamage.CutSpeed(Duration, Strength);

                    if (gameManager.gameInstance.isUserKareDead == false)
                    {
                        gameManager.gameInstance.Userkare.LightGautling();
                    }
                }

            }
           
              
        }
        else
            return;
    }


   
    public void SetStats(float duration, float strength, float radius, AOETYPE Type)
    {
        Duration = duration;
        type = Type;
        Strength = strength;
        Radius = radius;
    }
}
