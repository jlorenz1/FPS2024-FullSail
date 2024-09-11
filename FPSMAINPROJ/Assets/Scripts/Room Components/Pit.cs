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
            PlayerController.playerInstance.takeDamage(PlayerController.playerInstance.playerHP); //kills player by removing all HP
        }
    }
}
