using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    inventoryObject inventory;
    PlayerController playerScript;

    [Header("Interact")]
    [SerializeField] int pickupDis;
    [SerializeField] LayerMask ignoreMask;
    private Ray interactRay;
    private RaycastHit interactHit;
    bool isPickedUp;
    bool inProcess;
    public bool hasItems;

    private void Start()
    {
        inventory = gameManager.gameInstance.playerScript.inventory;
        playerScript = gameManager.gameInstance.playerScript;
    }
    void Update()
    {
      interact();
    }

    public void interact()
    {
        RaycastHit hit;
        bool isInteractable = false;
        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            if (hit.collider.gameObject.CompareTag("pickup"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(startInteract());
                }
                isInteractable = true;
            }
            else if (hit.collider.gameObject.CompareTag("interactable"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                isInteractable = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.collider != null)
                    {
                        var door = hit.collider.GetComponent<doorScript>();
                        if (door != null)
                        {
                            if (inventory.hasItem(itemType.Key))
                            {
                                inventory.useItem(itemType.Key);
                            }
                            else
                            {
                                StartCoroutine(gameManager.gameInstance.requiredItemsUI("You need an effigy to open the door!", 3f));
                            }

                        }
                        else
                        {
                            var bossInteraction = hit.collider.GetComponent<bossInteraction>();
                            if (bossInteraction != null)
                            {
                                if (hasItems)
                                {
                                    bossInteraction.spawnBoss();
                                    Destroy(hit.collider);
                                }
                                else
                                {
                                    StartCoroutine(gameManager.gameInstance.requiredItemsUI("Do not have required items!", 3f));
                                }
                            }
                            else
                            {
                                UnityEngine.Debug.Log("not spawning");
                            }
                        }

                        //tutorial tile puzzle, for each tile in the puzzle
                        var tilePuzzle = hit.collider.GetComponent<TutorialTileScript>();
                        if (tilePuzzle != null)
                        {
                            hit.collider.GetComponent<TutorialTileScript>().CheckArea();
                        }
                    }
                }
            }
            else if(hit.collider.gameObject.CompareTag("Weapon"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    if(hit.collider != null)
                    {
                        var weapon = hit.collider.GetComponent<weaponPickup>();
                        if(weapon != null)
                        {
                            gameManager.gameInstance.playerWeapon.getWeaponStats(weapon.gun);
                            if(weapon.gun.hekaSchool != null)
                            {
                                gameManager.gameInstance.playerWeapon.hasHeka = true;
                            }
                            Destroy(hit.collider.gameObject);
                        }
                        else
                        {
                            Debug.Log("weapon not being handled correctly");
                        }
                    }
                    else
                    {
                        Debug.Log("No pickup found.");
                    }
                }
                isInteractable = true;
            }
            else if(hit.collider.gameObject.CompareTag("AlterShop"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    if(hit.collider != null)
                    {
                       gameManager.gameInstance.gameAlterMenu.SetActive(true);

                        gameManager.gameInstance.pausePlayerControls();

                    }
                }
                isInteractable = true;
            }
            else
            {
                gameManager.gameInstance.playerInteract.SetActive(false);
            }
        }

        if (!isInteractable)
        {
            gameManager.gameInstance.playerInteract.SetActive(false);
        }
    }

    public IEnumerator startInteract()
    {
        RaycastHit hit;
        AudioManager.audioInstance.playSFXAudio(playerScript.interactSounds[Random.Range(0, playerScript.interactSounds.Length)], playerScript.interactVol);

        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            pickup pickup = hit.collider.GetComponent<pickup>();
            if (pickup != null)
            {
                inventory.AddItem(pickup.item, 1);
                inventory.updateInventoryUI();
                Destroy(hit.collider.gameObject);
            }
            if (pickup.item.type == itemType.Default || pickup.item.type == itemType.Rune)
            {
                checkForRequiredItems();

            }
            else if (pickup.item.type == itemType.Key)
            {
                gameManager.gameInstance.displayRequiredIemsUI("Collected effigy!", 3f);
            }
            else if (pickup.item.type == itemType.flashlight)
            {
                gameManager.gameInstance.displayRequiredIemsUI("'F' to use flashlight.", 3f);

            }
        }

        yield return new WaitForSeconds(1);
        gameManager.gameInstance.playerInteract.SetActive(false);
    }

    public void checkForRequiredItems()
    {
        bool hasRunes = false;
        bool hasLighter = false;
        bool hasLighterOnce = false;
        bool runeMessageShown = false;
        bool lighterMessageShown = false;

        for (int i = 0; i < inventory.containerForInv.Count; i++)
        {
            if (inventory.containerForInv[i].pickup.type == itemType.Rune)
            {


                if (inventory.containerForInv[i].amount >= 4)
                {
                    hasRunes = true;
                    if (!runeMessageShown)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI("Collected all Runes!", 3f);

                        runeMessageShown = true;
                    }

                    if (hasLighter && !lighterMessageShown)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI("Collected all Runes and Lighter, Find the ritual sight to spawn the boss!", 3f);
                    }
                    UnityEngine.Debug.Log(hasRunes);
                }
                else
                {
                    UnityEngine.Debug.Log("Not enough runes");
                    if (!runeMessageShown)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI(inventory.containerForInv[i].amount.ToString() + " of 4 Runes collected!", 3f);

                        runeMessageShown = true;
                    }
                    //set UI active coroutine 
                }
            }
            else if (inventory.containerForInv[i].pickup.type == itemType.Default)
            {

                if (inventory.containerForInv[i].amount >= 1)
                {

                    hasLighter = true;
                    if (!hasLighterOnce)
                    {
                        gameManager.gameInstance.displayRequiredIemsUI("Collected Lighter!", 3f);

                        hasLighterOnce = true;
                    }
                }
            }
        }
        if (hasRunes && hasLighter)
        {
            UnityEngine.Debug.Log("can now spawn boss");
            gameManager.gameInstance.itemsCompleteText.gameObject.SetActive(true);
            hasItems = true;
        }
        else
        {
            UnityEngine.Debug.Log("required items missing");

            hasItems = false;
        }
    }

    public void OnApplicationQuit()
    {
        //set inventory back to the original starting loadout.
        for (int i = 0; i < inventory.containerForInv.Count; i++)
        {
            inventory.containerForInv.Clear();
        }
    }
}
