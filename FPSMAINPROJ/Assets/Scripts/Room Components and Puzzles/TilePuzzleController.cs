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

    int[,] posTiles = new int[3,3];

    // Start is called before the first frame update
    void Start()
    {
        //setting positions
        pos5 = emptyTile.transform.position;

        pos1 = pos5 + new Vector3(-0.25f, -0.25f, 0);
        pos2 = pos5 + new Vector3(0, -0.25f, 0);
        pos3 = pos5 + new Vector3(0.25f, -0.25f, 0);
        pos4 = pos5 + new Vector3(-0.25f, 0, 0);
        pos6 = pos5 + new Vector3(0.25f, 0, 0);
        pos7 = pos5 + new Vector3(-0.25f, 0.25f, 0);
        pos8 = pos5 + new Vector3(0, 0.25f, 0);
        pos9 = pos5 + new Vector3(0.25f, 0.25f, 0);

        //array of positions
        Vector3[,] positions = { { pos1, pos2, pos3}, { pos4, pos5, pos6}, { pos7, pos8, pos9 } };

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

                    Debug.Log(posTiles[i,j]);
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
    
    void moveTile(Transform transform, Vector3 startPos, Vector3 endPos)
    {
        //speed to slide
        float slidespeed = 1f;
        //time to slide
        float timeToSlide = 0f;
    }
    
}
