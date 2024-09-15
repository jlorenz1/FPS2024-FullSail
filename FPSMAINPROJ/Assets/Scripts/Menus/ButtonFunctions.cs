using System.Collections;
using System.Collections.Generic;
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

    //altershop
    public void hekaTempest()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() > 35) //2 for testing
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasTempest)
            {
                gameManager.gameInstance.playerWeapon.getWeaponStats(gameManager.gameInstance.weaponManager.getHekaBasedWeapon("Electricity"));
            }
        }
        
    }

    public void pharoahsEclipse()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() > 20) //2 for testing
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasEclipse)
            {
                gameManager.gameInstance.playerWeapon.getWeaponStats(gameManager.gameInstance.weaponManager.getHekaBasedWeapon("Darkness"));
            }
        }
            
    }

    public void nilesWrath()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() > 25) //2 for testing
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasFloods)
            {
                gameManager.gameInstance.playerWeapon.getWeaponStats(gameManager.gameInstance.weaponManager.getHekaBasedWeapon("Floods"));
                gameManager.gameInstance.playerScript.inventory.takeGems(2);
            }
        }
    }

    public void healPlayer()
    {
        if(gameManager.gameInstance.playerScript.inventory.gemCount() > 5) //2 for testing
        {
            float healthAmount = 25;

            float currentHP = gameManager.gameInstance.playerScript.playerHP;
            float maxHp = gameManager.gameInstance.playerScript.HPorig;

            if (maxHp - currentHP < healthAmount)
            {

                healthAmount = maxHp - currentHP;
            }

            gameManager.gameInstance.playerScript.recieveHP(healthAmount);
            gameManager.gameInstance.playerScript.inventory.takeGems(2);
        }
    }

    public void increaseArmor()
    {
        //if we implement
    }
}
