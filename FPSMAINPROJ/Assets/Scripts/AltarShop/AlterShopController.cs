using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AlterShopController : MonoBehaviour
{
    [SerializeField] Transform armStartPoint;

    [SerializeField] GameObject emptyArm;
    [SerializeField] GameObject tempestArm;
    [SerializeField] GameObject darknessArm;
    [SerializeField] GameObject floodsArm;
    [SerializeField] float changeWaitTime;

    public GameObject activeArm;
    GameObject activeArmInstance;


    bool changingArm;
    public bool weaponArmActive = false;
    public bool pickedWeaponUp;
    bool isDisplayActive = false;
    void Start()
    {
        armStartPoint.localRotation = Quaternion.Euler(0f, -90f, -90f);
        activeArm = Instantiate(emptyArm, armStartPoint.position, armStartPoint.transform.localRotation);
        activeArm.transform.SetParent(armStartPoint.transform);

        weaponArmActive = false;
        pickedWeaponUp = true;
    }



    public IEnumerator changeArm(GameObject armToSpawn)
    {

        changingArm = true;

        Vector3 startPos = armStartPoint.localPosition;
        Vector3 lowerPos = startPos + new Vector3(0f, -1.3f, 0);

        float swapTime = changeWaitTime * 0.5f;
        float elapsed = 0f;

        while (elapsed < swapTime)
        {
            armStartPoint.localPosition = Vector3.Lerp(startPos, lowerPos, elapsed / swapTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        activeArm.transform.localPosition = lowerPos;


        if (armStartPoint.transform.childCount > 0)
        {
            Transform currentArm = armStartPoint.GetChild(0);
            Destroy(currentArm.gameObject);
        }


        GameObject newArm = Instantiate(armToSpawn, armStartPoint.position, Quaternion.Euler(0f, -90f, -90f));
        newArm.transform.SetParent(armStartPoint.transform);
        activeArm = newArm;
        activeArm.SetActive(true);

        elapsed = 0f;

        while (elapsed < swapTime)
        {
            armStartPoint.localPosition = Vector3.Lerp(armStartPoint.localPosition, startPos, elapsed / swapTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        changingArm = false;
    }




    public void hekaTempest()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() >= 35 && !gameManager.gameInstance.playerWeapon.hasTempest && !changingArm && pickedWeaponUp)
        {
            pickedWeaponUp = false;
            activeArm = tempestArm;
            StartCoroutine(changeArm(tempestArm));
            gameManager.gameInstance.playerScript.inventory.takeGems(35);
            gameManager.gameInstance.PointCount -= 35;
            weaponArmActive = true;

        }
        else if (gameManager.gameInstance.playerScript.inventory.gemCount() < 35)
        {
            //StartCoroutine(FlashNoGems());
            displayPromptText("Not enough gems!", 0.5f);
        }
        else if (!pickedWeaponUp)
        {
            
            displayPromptText("Pick up current weapon on altar!", 0.5f);
        }


    }

    public void pharoahsEclipse()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() >= 20 && !gameManager.gameInstance.playerWeapon.hasEclipse && !changingArm && pickedWeaponUp)
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasEclipse)
            {
                pickedWeaponUp = false;
                activeArm = darknessArm;
                StartCoroutine(changeArm(darknessArm));
                gameManager.gameInstance.playerScript.inventory.takeGems(20);
                gameManager.gameInstance.PointCount -= 20;
                weaponArmActive = true;
            }
        }
        else if (gameManager.gameInstance.playerScript.inventory.gemCount() < 20)
        {
            //StartCoroutine(FlashNoGems());
           
            displayPromptText("Not enough gems!", 0.5f);
        }
        else if (!pickedWeaponUp)
        {
          
            displayPromptText("Pick up current weapon on altar!", 0.5f);
        }

    }

    public void nilesWrath()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() >= 25 && !gameManager.gameInstance.playerWeapon.hasFloods && !changingArm && pickedWeaponUp)
        {
            //if player has gems amount
            if (!gameManager.gameInstance.playerWeapon.hasFloods)
            {
                pickedWeaponUp = false;
                activeArm = floodsArm;
                StartCoroutine(changeArm(floodsArm));
                gameManager.gameInstance.playerScript.inventory.takeGems(25);
                gameManager.gameInstance.PointCount -= 25;
                weaponArmActive = true;
            }
        }
        else if (gameManager.gameInstance.playerScript.inventory.gemCount() < 25)
        {
            //StartCoroutine(FlashNoGems());
            
            displayPromptText("Not enough gems!", 0.5f);
        }
        else if(!pickedWeaponUp)
        {
            
            displayPromptText("Pick up current weapon on altar!", 0.5f);
        }
    }

    public void healPlayer()
    {
        if (gameManager.gameInstance.playerScript.inventory.gemCount() >= 5 && gameManager.gameInstance.playerScript.playerHP != gameManager.gameInstance.playerScript.HPorig) //2 for testing
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
        else if(gameManager.gameInstance.playerScript.inventory.gemCount() < 5)
        {
            //StartCoroutine(FlashNoGems());
           
            displayPromptText("Not enough gems!", 0.5f);
        }
        else if (gameManager.gameInstance.playerScript.playerHP != gameManager.gameInstance.playerScript.HPorig)
        {
            //StartCoroutine(FlashNoGems());
            
            displayPromptText("Full HP", 0.5f);
        }


    }

    //    IEnumerator FlashNoGems()
    //    {
    //        gameManager.gameInstance.NoGems.GameObject().SetActive(true);
    //        yield return new WaitForSeconds(0.5f);
    //        gameManager.gameInstance.NoGems.GameObject().SetActive(false);
    //    }
     IEnumerator flashPrompt(string  prompt, float duration)
    {

        gameManager.gameInstance.altarPromtText.text = prompt;
        gameManager.gameInstance.NoGems.GameObject().SetActive(true);
        yield return new WaitForSeconds(duration);
        gameManager.gameInstance.NoGems.GameObject().SetActive(false);
        gameManager.gameInstance.altarPromtText.text = string.Empty;



    }

    void displayPromptText(string prompt, float duration)
    {
        if (isDisplayActive)
        {
            StopCoroutine("flashPrompt");
        }
        StartCoroutine(flashPrompt(prompt, duration));
    }

} 
