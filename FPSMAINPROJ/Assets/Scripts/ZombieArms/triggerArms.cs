using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerArms : MonoBehaviour
{
    [SerializeField] public GameObject[] arms;
    public float spawnDuration = 3;
    public float disableTime = 3;

    private Collider triggerCollider;
    private void Start()
    {
        triggerCollider = GetComponent<Collider>();
    }
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            StartCoroutine(spawnArmsForTime(spawnDuration));
        }
         StartCoroutine(disableTriggerForTime(disableTime));
    }

    IEnumerator spawnArmsForTime(float time)
    {
        if(arms.Length >= 1)
        {
            foreach (GameObject arm in arms)
            {
                if(arm != null)
                {
                    arm.SetActive(true);
                    yield return new WaitForSeconds(1);
                }
                
            }

            yield return new WaitForSeconds(time);

            foreach (GameObject arm in arms)
            {
                if(arm != null)
                {
                    arm.SetActive(false);
                    yield return new WaitForSeconds(1);
                }
            }
        }
    }

    IEnumerator disableTriggerForTime(float time)
    {
        triggerCollider.enabled = false;

        yield return new WaitForSeconds(time);

        triggerCollider.enabled = true;
    }
}
