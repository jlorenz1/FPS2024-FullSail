using System;
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

    public void quitToMainMenu()
    {
        StartCoroutine(loadNextSceneAsync(0));
        gameManager.gameInstance.playerScript.inventory.clearInventory();
    }

    IEnumerator loadNextSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        gameManager.gameInstance.loadingScreen.SetActive(true);


        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); //progress goes to .9 due to 2 sets of thing happening while loading in unity, this will let the bar fill full making the progress go to 1

            gameManager.gameInstance.loadingBar.fillAmount = progress;

            yield return null;
        }

        gameManager.gameInstance.loadingBar.fillAmount = 1;
    }
}
