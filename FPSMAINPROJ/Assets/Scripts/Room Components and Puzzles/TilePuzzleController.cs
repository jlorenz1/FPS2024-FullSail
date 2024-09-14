//made by Emily Underwood
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TilePuzzleController : MonoBehaviour
{
    public GameObject topTile1;
    public GameObject leftTile2;
    public GameObject bottomTile3;
    public GameObject rightTile4;

    public GameObject blankTile1;
    public GameObject blankTile2;
    public GameObject emptyTile; //center tile, not randomly placed. still movable.
    public GameObject blankTile3;
    public GameObject blankTile4;

    public GameObject door; //door to open once puzzle is solved

    Vector3 pos1; //top left
    Vector3 pos2; //top middle
    Vector3 pos3; //top right
    Vector3 pos4; //middle left
    Vector3 pos5; //middle middle
    Vector3 pos6; //middle right
    Vector3 pos7; //bottom left
    Vector3 pos8; //bottom middle
    Vector3 pos9; //bottom right

    public int[,] posTiles = new int[3,3];
    public Vector3[,] positions = new Vector3[3,3];

    private static TilePuzzleController _tilePuzzleInstance;

    public static TilePuzzleController tilePuzzleInstance
    {
        get
        {
            if (_tilePuzzleInstance == null)
            {
                Debug.LogError("TilePuzzleController is null");
            }
            return _tilePuzzleInstance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_tilePuzzleInstance != null && _tilePuzzleInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _tilePuzzleInstance = this;
        }

        //setting positions
        pos5 = emptyTile.transform.position;

        pos1 = pos5 + new Vector3(-0.25f, 0.25f, 0);
        pos2 = pos5 + new Vector3(0, 0.25f, 0);
        pos3 = pos5 + new Vector3(0.25f, 0.25f, 0);
        pos4 = pos5 + new Vector3(-0.25f, 0, 0);
        pos6 = pos5 + new Vector3(0.25f, 0, 0);
        pos7 = pos5 + new Vector3(-0.25f, -0.25f, 0);
        pos8 = pos5 + new Vector3(0, -0.25f, 0);
        pos9 = pos5 + new Vector3(0.25f, -0.25f, 0);

        //array of positions
        positions[0, 0] = pos1;
        positions[0, 1] = pos2;
        positions[0, 2] = pos3;
        positions[1, 0] = pos4;
        positions[1, 1] = pos5;
        positions[1, 2] = pos6;
        positions[2, 0] = pos7;
        positions[2, 1] = pos8;
        positions[2, 2] = pos9;

        //used to make sure the same tile isnt placed multiple times
        ArrayList tileArr = new ArrayList();
        tileArr.Add(1);
        tileArr.Add(2);
        tileArr.Add(3);
        tileArr.Add(4);
        //blank tiles
        tileArr.Add(-1);
        tileArr.Add(-2);
        tileArr.Add(-3);
        tileArr.Add(-4);

        //randomly populating posArray with tiles
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0;  j < 3; j++)
            {
                if (i == 1 && j == 1)
                {
                    posTiles[i, j] = 0; //empty tile
                } else
                {
                    //randomly choosing tile and putting it in position array
                    int randVal = Random.Range(0, tileArr.Count);
                    posTiles[i, j] = (int) tileArr[randVal];

                    //removing tile from arr
                    tileArr.RemoveAt(randVal);
                }
            }
        }

        //placing tiles
        //posTiles[i,j] contains the number id of the tile to place
        //positions[i+j] is the location to put it at
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0;j < 3; j++)
            {
                if (!(i == 1 && j == 1))
                {
                    if (posTiles[i, j] == 1)
                    {
                        topTile1.transform.position = positions[i, j];
                    }
                    else if (posTiles[i, j] == 2)
                    {
                        leftTile2.transform.position = positions[i, j];
                    }
                    else if (posTiles[i, j] == 3)
                    {
                        bottomTile3.transform.position = positions[i, j];
                    }
                    else if (posTiles[i, j] == 4)
                    {
                        rightTile4.transform.position = positions[i, j];
                    }
                    else if (posTiles[i, j] == -1)
                    {
                        blankTile1.transform.position = positions[i, j];
                    }
                    else if (posTiles[i, j] == -2)
                    {
                        blankTile2.transform.position = positions[i, j];
                    }
                    else if (posTiles[i, j] == -3)
                    {
                        blankTile3.transform.position = positions[i, j];
                    }
                    else if (posTiles[i, j] == -4)
                    {
                        blankTile4.transform.position = positions[i, j];
                    }
                }
               
            }
        }

        
    }
    
    //moves tile from point startPos to endPos
    public IEnumerator moveTile(GameObject tile, Vector3 startPos, Vector3 endPos, int[] posIndices)
    {
        //speed to slide
        float slidespeed = 1f;
        //time to slide
        float timeToSlide = 0f;

        {
            string debugStr = "";
            for (int i = 0; i < 3; i++)
            {
                debugStr += "Row " + i + ": ";
                for (int j = 0; j < 3; j++)
                {
                    debugStr += posTiles[i, j] + " ";
                }
                debugStr += "\n";
            }
            Debug.Log(debugStr);
        }

        //move tile smoothly
        while (timeToSlide < slidespeed)
        {
            tile.transform.position = Vector3.Lerp(startPos, endPos, (timeToSlide/slidespeed));
            timeToSlide += Time.deltaTime;
            yield return null;
        }

        tile.transform.position = endPos;
        emptyTile.transform.position = startPos;

        //updating posTiles
        int tileID = posTiles[posIndices[0],posIndices[1]];
        posTiles[posIndices[0], posIndices[1]] = 0; //that spot is now empty
        posTiles[posIndices[2], posIndices[3]] = tileID; //this tile is now where empty spot was

        {
            string debugStr = "";
            for (int i = 0; i < 3; i++)
            {
                debugStr += "Row " + i + ": ";
                for (int j = 0; j < 3; j++)
                {
                    debugStr += posTiles[i, j] + " ";
                }
                debugStr += "\n";
            }
            Debug.Log(debugStr);
        }

        Debug.Log("(" + posIndices[0] + ", " + posIndices[1] + ") (" + posIndices[2] + ", " + posIndices[3] + ")");

        checkPattern();
    }
    
    //checks if the new pattern is the winning pattern or not
    void checkPattern()
    {
        if (posTiles[0,1] == 1 && posTiles[1,0] == 2 && posTiles[2,1] == 3 && posTiles[1,2] == 4)
        {
            door.GetComponent<doorScript>().slide(); //opening door
        }
    }
}
