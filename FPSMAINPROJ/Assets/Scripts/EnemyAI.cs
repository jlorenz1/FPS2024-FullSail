using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HitPoints;



    [SerializeField] GameObject Player;
    [SerializeField] int AttackRange;
    [SerializeField] int AttackRate;
    [SerializeField] int AttackDamage;
    [SerializeField] NavMeshAgent agent;
 
    
    bool isAttacking;



    Color colorOriginal;

 
    void Start()
    {
        colorOriginal = model.material.color;


        agent.SetDestination(Player.transform.position);

        agent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {

        agent.SetDestination(Player.transform.position);

    }


    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            takeDamage(1);

            if (HitPoints <= 0)
            {
                Destroy(gameObject);
            }
        }

    }

    public void takeDamage(int amountOfDamageTaken)
    {
        HitPoints -= amountOfDamageTaken;
        StartCoroutine(flashRed());
        if (HitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
