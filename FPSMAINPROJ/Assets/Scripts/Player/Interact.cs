using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interact : MonoBehaviour
{
    inventoryObject inventory;
    PlayerController playerScript;

    [Header("Interact")]
    [SerializeField] int pickupDis;
    [SerializeField] AudioSource PlayerAudio;
    [SerializeField] AudioClip PickUp;
    [SerializeField] AudioClip Place;
    [SerializeField] LayerMask ignoreMask;
    private Ray interactRay;
    private RaycastHit interactHit;
    bool isPickedUp;
    bool inProcess;
    public bool hasItems;
    List<GameObject> Effgies = new List<GameObject>();
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
                                    //  bossInteraction.spawnBoss();
                                    Destroy(hit.collider);
                                    StartCoroutine(gameManager.gameInstance.requiredItemsUI("Has Items", 3f));
                                }
                                else
                                {
                                    StartCoroutine(gameManager.gameInstance.requiredItemsUI("Do not have required items!", 3f));
                                }
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
            else if (hit.collider.gameObject.CompareTag("Weapon"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.collider != null)
                    {
                        var weapon = hit.collider.GetComponent<weaponPickup>();
                        if (weapon != null)
                        {
                            gameManager.gameInstance.playerWeapon.getWeaponStats(weapon.gun);
                            if (weapon.gun.hekaSchool != null)
                            {
                                gameManager.gameInstance.playerWeapon.hasHeka = true;
                            }
                            Destroy(hit.collider.gameObject);
                        }
                     
                    }
                  
                }
                isInteractable = true;
            }
            else if (hit.collider.gameObject.CompareTag("AlterShop"))
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.collider != null)
                    {
                        gameManager.gameInstance.gameAlterMenu.SetActive(true);

                        gameManager.gameInstance.pausePlayerControls();

                    }
                }
                isInteractable = true;
            }

            else if (hit.collider.gameObject.CompareTag("Altar"))
            {
                Altarinteract alter = hit.collider.gameObject.GetComponent<Altarinteract>();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (alter.HasObject == false && Effgies[0] != null)
                    {

                        alter.PlaceObject(Effgies[0]);
                        Effgies.Remove(Effgies[0]);
                        PlayerAudio.PlayOneShot(Place, 1f);
                    }
                    else if (alter.HasObject)
                    {
                        alter.takeObject();
                    }
                    else
                        return;
                }
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
            GameObject model = hit.collider.gameObject;
            if (pickup != null)
            {
                inventory.AddItem(pickup.item, 1);
                Effgies.Add(model);
               PlayerAudio.PlayOneShot(PickUp, 1f);
                inventory.updateInventoryUI();
                hit.collider.gameObject.transform.position = new Vector3(10000, 10000, 10000);
            }
          
       
            else if (pickup.item.type == itemType.flashlight)
            {
                gameManager.gameInstance.displayRequiredIemsUI("'F' to use flashlight.", 3f);

            }
        }

        yield return new WaitForSeconds(1);
        gameManager.gameInstance.playerInteract.SetActive(false);
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
