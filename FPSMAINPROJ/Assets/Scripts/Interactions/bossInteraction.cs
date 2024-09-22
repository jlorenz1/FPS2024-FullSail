using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossInteraction : MonoBehaviour
{
    [SerializeField] doorScript DoorToOpen;
    [SerializeField] GameObject Door;
    public GameObject bossRitualPrefab;
    [SerializeField] public AudioClip placingSound;
    [Range(0, 1)][SerializeFeild] public float placingVol;
    int Effigies;
    public void Update()
    {

        if (Effigies >= 4)
        {

            DoorToOpen.slide();
            AudioManager.audioInstance.playSFXAudio(placingSound, placingVol);
            
        }
        
      
    }

    public void EffigiesPlaced()
    {
        Effigies++;
    }

    public void EffigiesTaken()
    {
        Effigies--;
    }

}
