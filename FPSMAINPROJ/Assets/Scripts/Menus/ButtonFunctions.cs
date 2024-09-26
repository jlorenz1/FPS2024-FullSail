using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
   public void resume()
   {
        gameManager.gameInstance.UnpauseGame();
   }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.gameInstance.playerScript.inventory.clearInventory();
        gameManager.gameInstance.UnpauseGame();
    }

    public void respawn()
    {
        if (gameManager.gameInstance.CurrentCheckPoint != null)
        {
            gameManager.gameInstance.playerScript.RespawnPlayer();
        }
        else
            gameManager.gameInstance.playerScript.spawnPlayer();



        gameManager.gameInstance.UnpauseGame();
    }

    public void quit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
    Application.Quit();
    #endif
    }

    public void play()
    {
        SceneManager.LoadScene(1);
        gameManager.gameInstance.playerScript.inventory.clearInventory();
    }

    public void quitToMainMenu()
    {
        SceneManager.LoadScene(0);
        gameManager.gameInstance.playerScript.inventory.clearInventory();
    }
}
