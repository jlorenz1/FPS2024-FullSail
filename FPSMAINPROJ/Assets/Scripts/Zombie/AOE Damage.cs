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
    [SerializeField] public GameObject Area;
    [SerializeField] bool IsHeal;
    IDamage Playerdamage;

    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, Duration);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        
        if ( other.CompareTag("Player") && !IsHeal)
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

        else if (IsHeal)
        {
            if(other.GetComponent<IEnemyDamage>() != null)
            {
                IEnemyDamage enemyDamage = other.GetComponent<IEnemyDamage>();

                enemyDamage.AddHP(Strength);

            }
        }



        else
            return;
    }

   public void expand(float strength, float radius)
    {
        Strength = strength;


        StartCoroutine(ExpandOverTime(radius));


    }


    IEnumerator ExpandOverTime(float targetRadius)
    {
        Vector3 originalScale = transform.localScale; // Get the initial scale
        Vector3 targetScale = new Vector3(targetRadius, targetRadius, targetRadius); // Target scale based on the radius

        float elapsedTime = 0f; // Keep track of the time elapsed

        while (elapsedTime < Duration)
        { 
            
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / Duration);

            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        // Ensure the scale is set to the exact target scale at the end
        transform.localScale = targetScale;
    }


    public void SetStats(float duration, float strength, float radius, AOETYPE Type)
    {
        Duration = duration;
        type = Type;
        Strength = strength;
        Radius = radius;
    }
}
