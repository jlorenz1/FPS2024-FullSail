using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.gameInstance.playerSpawnPoint.transform.position != this.transform.position)
        {
            gameManager.gameInstance.playerSpawnPoint.transform.position = transform.position;
            StartCoroutine(flashCheckpoint());
        }

    }

    IEnumerator flashCheckpoint()
    {
        gameManager.gameInstance.checkpoint.GameObject().SetActive(true);
        yield return new WaitForSeconds(1);
        gameManager.gameInstance.checkpoint.GameObject().SetActive(false);
    }

}
