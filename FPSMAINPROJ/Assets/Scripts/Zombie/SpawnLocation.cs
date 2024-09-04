using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] GameObject[] trigeers;




    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        gameObject.SetActive(true);
        
    }



    private void OnTriggerExit(Collider other)
    {
        gameObject.SetActive(true);
    }

}
