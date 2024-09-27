using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] SpawnTrigger trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.gameInstance.playerSpawnPoint.transform.position != this.transform.position)
        {
            gameManager.gameInstance.setSpawn(this);
            StartCoroutine(flashCheckpoint());
            GetComponent<Collider>().enabled = false;
        }

    }

    IEnumerator flashCheckpoint()
    {
        gameManager.gameInstance.checkpoint.GameObject().SetActive(true);
        yield return new WaitForSeconds(1);
        gameManager.gameInstance.checkpoint.GameObject().SetActive(false);
    }


    public void ResetTrigger()
    {

        trigger.wiped = false;
        trigger.TriggerEntered = false;
        trigger.BossIsSpawned = false;
        trigger.DisableSpawn();
        trigger.DespawnZombies();
    }



}
