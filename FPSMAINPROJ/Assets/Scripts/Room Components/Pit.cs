//made by Emily Underwood
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.gameInstance.loseScreen(); //kills player
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
