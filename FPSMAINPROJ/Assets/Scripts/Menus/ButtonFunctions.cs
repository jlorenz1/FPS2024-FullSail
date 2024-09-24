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
        gameManager.gameInstance.UnpauseGame();
    }

    public void respawn()
    {
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
    }

    public void quitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //altershop
    public void hekaTempest()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() >= 35) //2 for testing
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasTempest)
            {
                gameManager.gameInstance.playerWeapon.getWeaponStats(gameManager.gameInstance.weaponManager.getHekaBasedWeapon("Electricity"));
                gameManager.gameInstance.playerScript.inventory.takeGems(35);
                gameManager.gameInstance.PointCount -= 35;
            }
        }
        else
            StartCoroutine(FlashNoGems());
        
    }

    public void pharoahsEclipse()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() >= 20) //2 for testing
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasEclipse)
            {
                gameManager.gameInstance.playerWeapon.getWeaponStats(gameManager.gameInstance.weaponManager.getHekaBasedWeapon("Darkness"));
                gameManager.gameInstance.playerScript.inventory.takeGems(20);
                gameManager.gameInstance.PointCount -= 20
                    ;
            }
        }
        else
            StartCoroutine(FlashNoGems());

    }

    public void nilesWrath()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() >= 25) //2 for testing
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasFloods)
            {
                gameManager.gameInstance.playerWeapon.getWeaponStats(gameManager.gameInstance.weaponManager.getHekaBasedWeapon("Floods"));
                gameManager.gameInstance.playerScript.inventory.takeGems(25);
                gameManager.gameInstance.PointCount -= 25;
            }
        }
        else
            StartCoroutine(FlashNoGems());
    }

    public void healPlayer()
    {
        if(gameManager.gameInstance.playerScript.inventory.gemCount() >= 5 && gameManager.gameInstance.playerScript.playerHP != gameManager.gameInstance.playerScript.HPorig) //2 for testing
        {
            float healthAmount = 25;

            float currentHP = gameManager.gameInstance.playerScript.playerHP;
            float maxHp = gameManager.gameInstance.playerScript.HPorig;

            if (maxHp - currentHP < healthAmount)
            {

                healthAmount = maxHp - currentHP;
            }

            gameManager.gameInstance.playerScript.recieveHP(healthAmount);
            gameManager.gameInstance.playerScript.inventory.takeGems(5);
            gameManager.gameInstance.PointCount -= 5;
        }
        else
            StartCoroutine(FlashNoGems());
    }

    IEnumerator FlashNoGems()
    {
        gameManager.gameInstance.NoGems.GameObject().SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.gameInstance.NoGems.GameObject().SetActive(false);
    }

    public void increaseArmor()
    {
        //if we implement
    }
}
