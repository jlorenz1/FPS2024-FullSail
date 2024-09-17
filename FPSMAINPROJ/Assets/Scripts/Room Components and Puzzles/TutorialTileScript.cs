using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTileScript : MonoBehaviour {
    public int id; //id of the tile
    public GameObject tile;


    //checks around tile to see if an empty spot is in range
    public void CheckArea()
    {
        Debug.Log("CheckArea() has been called");

        //index of tile
        int rowIndex = 0;
        int colIndex = 0;

        //finding where in posTiles this tile is
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (TilePuzzleController.tilePuzzleInstance.posTiles[i,j] == id)
                {
                    rowIndex = i;
                    colIndex = j;
                }
            }
        }

        Vector3 tilePos = TilePuzzleController.tilePuzzleInstance.positions[rowIndex,colIndex]; //tiles current position

        //passed into moveTile so it can update posTile and positions more efficiently
        // [0] and [1] are the indices for the starting position, [2] and [3] are the indices for the ending position
        int[] indexes = new int[4];
        indexes[0] = rowIndex;
        indexes[1] = colIndex;

        
        if (rowIndex - 1 >= 0) //checking above tile
        {
            if (TilePuzzleController.tilePuzzleInstance.posTiles[rowIndex-1, colIndex] == 0) //empty tile above curr tile
            {
                Debug.Log("moving tile up");

                indexes[2] = rowIndex - 1;
                indexes[3] = colIndex;
                StartCoroutine(TilePuzzleController.tilePuzzleInstance.moveTile(tile, tilePos, TilePuzzleController.tilePuzzleInstance.positions[rowIndex - 1, colIndex], indexes));
            }
        } 

        if (rowIndex + 1 <= 2) //checking below tile
        {
            if (TilePuzzleController.tilePuzzleInstance.posTiles[rowIndex + 1, colIndex] == 0) //empty tile below curr tile
            {
                Debug.Log("moving tile down");

                indexes[2] = rowIndex + 1;
                indexes[3] = colIndex;
                StartCoroutine(TilePuzzleController.tilePuzzleInstance.moveTile(tile, tilePos, TilePuzzleController.tilePuzzleInstance.positions[rowIndex + 1, colIndex], indexes));
            }
        }

        if (colIndex - 1 >= 0) //checking to the left of tile
        {
            if (TilePuzzleController.tilePuzzleInstance.posTiles[rowIndex, colIndex - 1] == 0) //empty tile to left of curr tile
            {
                Debug.Log("moving tile left");

                indexes[2] = rowIndex;
                indexes[3] = colIndex - 1;
                StartCoroutine(TilePuzzleController.tilePuzzleInstance.moveTile(tile, tilePos, TilePuzzleController.tilePuzzleInstance.positions[rowIndex, colIndex - 1], indexes));
            }
        }

        if (colIndex + 1 <= 2) //checking to the right of tile
        {
            if (TilePuzzleController.tilePuzzleInstance.posTiles[rowIndex, colIndex + 1] == 0) //empty tile to right of curr tile
            {
                Debug.Log("moving tile right");

                indexes[2] = rowIndex;
                indexes[3] = colIndex + 1;
                StartCoroutine(TilePuzzleController.tilePuzzleInstance.moveTile(tile, tilePos, TilePuzzleController.tilePuzzleInstance.positions[rowIndex, colIndex + 1], indexes));
            }
        }
    }
}
