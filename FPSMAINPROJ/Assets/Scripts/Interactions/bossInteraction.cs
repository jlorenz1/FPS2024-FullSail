using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossInteraction : MonoBehaviour
{
    [SerializeField] doorScript DoorToOpen;
    [SerializeField] GameObject Door;
    public GameObject bossRitualPrefab;
    [SerializeField] public AudioClip placingSound;
    [Range(0, 1)][SerializeField] public float placingVol;
    [SerializeField] public AudioClip Incorrect;
    [SerializeField] List<Altarinteract> AlterNumber;
    [SerializeField] List<EffigyScript> EffigyNumber;
    List<int> Patter = new List<int>();
    List<int> input = new List<int>();
    int Effigies;
    public void Update()
    {

        if (Effigies >= AlterNumber.Count)
        {
            DoorToOpen.slide();
        }


    }

    private void Start()
    {
        Effigies = 0;
        AssignNumbers();
    }


    void AssignNumbers()
    {
        // Assign the numbers to each effigy
        for (int i = 1; i <= 4; i++)
        {
            EffigyNumber[i-1].SetNumber(i);
        }
        
    }




    public void EffigiesPlaced()
    {

        Effigies++;
    }

    public void EffigiesTaken()
    {

        {
            Effigies--;
        }

    }
}
