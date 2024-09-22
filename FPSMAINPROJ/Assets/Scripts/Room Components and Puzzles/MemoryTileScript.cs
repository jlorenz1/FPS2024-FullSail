using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.ShaderData;

public class MemoryTileScript : MonoBehaviour
{
    [SerializeField] Light[] roomLights;
    [SerializeField] AudioClip[] sounds;
    [Range(0, 1)][SerializeField] public float soundVol;
    Color origColor;

    public int id; //id of the tile stepped on
    public GameObject tile;

    private void Start()
    {origColor = roomLights[0].color;
        
    }

    private void OnTriggerEnter(Collider collide)
    {
        if (collide.gameObject.CompareTag("Player"))
        {
            if (!MemoryPuzzleController.memPuzzleInstance.getPassResults())
            {
                StartCoroutine(playSounds());

                MemoryPuzzleController.memPuzzleInstance.UpdateSequence(id, tile);

                if (MemoryPuzzleController.memPuzzleInstance.getFailResults() || MemoryPuzzleController.memPuzzleInstance.getPassResults())
                {
                    StartCoroutine(lightsIndicatior());

                }
            }

            

        }
    }

    IEnumerator lightsIndicatior()
    {
        if (MemoryPuzzleController.memPuzzleInstance.getFailResults())
        {
            for (int i = 0; i < roomLights.Length; i++)
            {
                roomLights[i].color = Color.red;
            }
        }
        else if (MemoryPuzzleController.memPuzzleInstance.getPassResults())
        {
            for (int i = 0; i < roomLights.Length; i++)
            {
                roomLights[i].color = Color.green;
            }
        }

        yield return new WaitForSeconds(.3f);

        for (int i = 0; i < roomLights.Length; i++)
        {
            roomLights[i].color = origColor;
        }

    }

    IEnumerator playSounds()
    {
        AudioManager.audioInstance.playSFXAudio(sounds[Random.Range(0, sounds.Length)], soundVol);
        yield return new WaitForSeconds(3);
    }

}
