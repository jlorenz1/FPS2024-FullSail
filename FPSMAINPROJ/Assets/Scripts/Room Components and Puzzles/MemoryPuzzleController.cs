using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPuzzleController : MonoBehaviour
{
    public GameObject door;

    //pattern options for sequence player needs to complete
    int[] pattern1 = { 3, 6, 11, 15, 20 };
    int[] pattern2 = { 2, 5, 10, 15, 18 };
    int[] pattern3 = { 4, 7, 11, 14, 17 };
    int[] pattern4 = { 1, 5, 10, 15, 18 };

    int[] pattern; //chosen pattern

    ArrayList sequence = new ArrayList(); //the sequence the player steps in

    private static MemoryPuzzleController _memPuzzleInstance;
    public static MemoryPuzzleController memPuzzleInstance
    {
        get
        {
            if (_memPuzzleInstance == null)
            {
                Debug.LogError("MemoryPuzzleController is null");
            }
            return _memPuzzleInstance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_memPuzzleInstance != null && _memPuzzleInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _memPuzzleInstance = this;
        }

        //randomly choosing which sequence is required this run
        int randVal = Random.Range(0, 4);
        if (randVal == 0)
        {
            pattern = pattern1;
        } else if (randVal == 1)
        {
            pattern = pattern2;
        } else if (randVal == 2)
        {
            pattern = pattern3;
        } else if (randVal == 3)
        {
            pattern = pattern4;
        }

        //!delete later, testing purposes only
        pattern = pattern1;
    }

    //updates sequence array and handles when player steps on incorrect tile
    public void UpdateSequence(int id)
    {
        Debug.Log(id);

        int index = sequence.Count;

        Debug.Log("ID: " + id + ", Pattern Num: " + pattern[index]);

        if (pattern[index] == id) //player stepped on correct tile
        {
            sequence.Add(id);
            Debug.Log("ID added to sequence. Sequence.Count: " + sequence.Count);
        } else //player stepped on incorrect tile
        {
            sequence.Clear(); //player must restart
        }

        



        if (sequence.Count == 5) //sequence is completed, and already checked for accuracy
        {
            door.GetComponent<doorScript>().slide(); //opening door
        }
    }
}
