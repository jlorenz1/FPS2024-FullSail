using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPuzzleController : MonoBehaviour
{
    public GameObject door;
    public GameObject placingDisplaysBit; //used for placing display tiles

    //display tiles
    public GameObject display1;
    public GameObject display2;
    public GameObject display3;
    public GameObject display4;
    public GameObject display5;
    public GameObject display6;
    public GameObject display7;
    public GameObject display10;
    public GameObject display11;
    public GameObject display14;
    public GameObject display15;
    public GameObject display17;
    public GameObject display18;
    public GameObject display20;

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

        //displaying the sequence tiles
        //placingDisplaysBit is in there to make it easier to traverse the array
        GameObject[] displayTiles = {display1,  display2, display3, display4, display5, display6, display7, 
        placingDisplaysBit, placingDisplaysBit, display10, display11, placingDisplaysBit, placingDisplaysBit, 
        display14, display15, placingDisplaysBit, display17, display18, placingDisplaysBit, display20};

        Vector3 location = placingDisplaysBit.transform.position;
        for (int i = 0; i < pattern.Length; i++) //moving chosen tiles so the player can see them
        {
            displayTiles[pattern[i]-1].transform.position = location;
            location += new Vector3(-1.25f, 0, 0);
        }
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
