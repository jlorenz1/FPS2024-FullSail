using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TerrainDestroyer"))
        {
            Destroy(other.gameObject);
        }
    }
}
