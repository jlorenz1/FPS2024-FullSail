using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MenuZombies : MonoBehaviour
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] int roamDist;
    [SerializeField] int roamTimer;

    Vector3 startingPos;

    bool isRoaming;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 randPos = Random.insideUnitSphere * roamDist;

        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    // Update is called once per frame
    void Update()
    {

        if (!isRoaming && agent.remainingDistance <= 0.5)
            StartCoroutine(roam());
        
    }

    IEnumerator roam()
    {
        isRoaming = true;

        Vector3 randPos = Random.insideUnitSphere * roamDist;
        //randPos = startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        yield return new WaitForSeconds(roamTimer);
        isRoaming = false;

    }

}
