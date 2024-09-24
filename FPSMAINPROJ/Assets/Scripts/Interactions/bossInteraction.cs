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
        AssignRandomNumbers();
        Effigies = 0;
    }


    void AssignRandomNumbers()
    {
        // Create a list of integers equal to the number of altars
        List<int> randomNumbers = new List<int>();
        for (int i = 0; i < AlterNumber.Count; i++)
        {
            randomNumbers.Add(i); // Add numbers from 0 to AlterNumber.Count - 1
        }

        // Shuffle the list using Fisher-Yates algorithm or Unity's random shuffle
        for (int i = randomNumbers.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = randomNumbers[i];
            randomNumbers[i] = randomNumbers[randomIndex];
            randomNumbers[randomIndex] = temp;
        }

        // Assign the shuffled numbers to each Altar
        for (int i = 0; i < AlterNumber.Count; i++)
        {
            AlterNumber[i].setAltarNumber(randomNumbers[i]);
            EffigyNumber[i].SetNumber(randomNumbers[i]);
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
