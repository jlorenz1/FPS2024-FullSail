using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HitPoints;
 
    [SerializeField] int AttackRange;
    [SerializeField] int AttackRate;
    [SerializeField] int AttackDamage;
    [SerializeField] NavMeshAgent agent;
 

    bool isAttacking;



    Color colorOriginal;

 
    void Start()
    {
        colorOriginal = model.material.color;


    }

    // Update is called once per frame
    void Update()
    {
        
            agent.SetDestination(gameManager.gameInstance.player.transform.position);  
        
    }

    public void takeDamage(int amountOfDamageTaken)
    {
        HitPoints -= amountOfDamageTaken;
        StartCoroutine(flashRed());
        if (HitPoints <= 0 )
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }




}
