using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryTileScript : MonoBehaviour
{
    public int id; //id of the tile stepped on
    public GameObject tile;

    private void OnTriggerEnter(Collider collide)
    {
        if (collide.gameObject.CompareTag("Player"))
        {
            MemoryPuzzleController.memPuzzleInstance.UpdateSequence(id, tile);
        }
    }
}
