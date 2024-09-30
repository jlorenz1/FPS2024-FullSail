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

    [Header("AUDIO")]
    public AudioClip[] blockSounds;
    [Range(0, 1)][SerializeField] public float blockVol;

    private static TilePuzzleController _tilePuzzleInstance;
    bool isTileMoving = false;
    public static TilePuzzleController tilePuzzleInstance
    {
        get
        {
            if (_tilePuzzleInstance == null)
            {
                //do nothing
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

        

        //patterns the puzzle can spawn in with, must be done here to be initialized efficiently
        Vector3[] pattern1 = { pos8, pos3, pos4, pos2, pos1, pos6, pos7, pos9 };
        int[,] posTiles1 = { { -1, 4, 2 }, { 3, 0, -2 }, { -3, 1, -4 } };
        Vector3[] pattern3 = { pos8, pos3, pos6, pos1, pos2, pos4, pos7, pos9 };
        int[,] posTiles3 = { { 4, -1, 2 }, { -2, 0, 3 }, { -3, 1, -4 } };
        Vector3[] pattern5 = { pos9, pos1, pos8, pos6, pos2, pos3, pos4, pos7 };
        int[,] posTiles5 = { { 2, -1, -2}, { -3, 0, 4}, { -4, 3, 1 } };
        Vector3[] pattern6 = { pos3, pos7, pos6, pos9, pos1, pos2, pos4, pos8 };
        int[,] posTiles6 = { { -1, -2, 1}, { -3, 0, 3}, { 2, -4, 4 } };
        Vector3[] pattern7 = { pos3, pos4, pos1, pos8, pos2, pos6, pos7, pos9 };
        int[,] posTiles7 = { { 3, -1, 1}, { 2, 0, -2 }, { -3, 4, -4 } };
        Vector3[] pattern8 = { pos6, pos8, pos9, pos4, pos1, pos2, pos3, pos7 };
        int[,] posTiles8 = { { -1, -2, -3 }, { 4, 0, 1 }, { -4, 2, 3 } };
        Vector3[] pattern9 = { pos8, pos3, pos9, pos7, pos1, pos2, pos4, pos6 };
        int[,] posTiles9 = { { -1, -2, 2 }, { -3, 0, -4 }, { 4, 1, 3 } };
        Vector3[] pattern10 = { pos9, pos2, pos4, pos3, pos1, pos6, pos7, pos8 };
        int[,] posTiles10 = { { -1,2,4}, { 3,0,-2}, { -3,-4,1 } };

        //randomly choosing a pattern to display
        Vector3[][] patterns = { pattern1, pattern3, pattern5, pattern6, pattern7, pattern8, pattern9, pattern10 };
        int[][,] posTilesPatterns = { posTiles1, posTiles3, posTiles5, posTiles6, posTiles7, posTiles8, posTiles9, posTiles10 };
        int randVal = Random.Range(0, patterns.Count());

        Place(patterns[randVal], posTilesPatterns[randVal]);
}

    //places where tiles spawn in at
    void Place(Vector3[] pattern, int[,] positions)
    {
        posTiles = positions;

        topTile1.transform.position = pattern[0];
        leftTile2.transform.position = pattern[1];
        bottomTile3.transform.position = pattern[2];
        rightTile4.transform.position = pattern[3];
        blankTile1.transform.position = pattern[4];
        blankTile2.transform.position = pattern[5];
        blankTile3.transform.position = pattern[6];
        blankTile4.transform.position = pattern[7];
    }
    

    //moves tile from point startPos to endPos
    public IEnumerator moveTile(GameObject tile, Vector3 startPos, Vector3 endPos, int[] posIndices)
    {

        if(isTileMoving)
        {
            yield break;
        }

        isTileMoving = true;
        AudioManager.audioInstance.playSFXAudio(blockSounds[Random.Range(0, blockSounds.Length)], blockVol);
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
        }

        isTileMoving = false;
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

    //DO NOT DELETE!!
    //commented out because i couldnt figure out a randomization algorithm that wouldnt generate unsolvable puzzles, but i will come back to it
    /*
    //populates the puzzle with tiles
    void Populate()
    {
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
            for (int j = 0; j < 3; j++)
            {
                if (i == 1 && j == 1)
                {
                    posTiles[i, j] = 0; //empty tile
                }
                else
                {
                    //randomly choosing tile and putting it in position array
                    int randVal = Random.Range(0, tileArr.Count);
                    posTiles[i, j] = (int)tileArr[randVal];

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
            for (int j = 0; j < 3; j++)
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


    }*/
}
