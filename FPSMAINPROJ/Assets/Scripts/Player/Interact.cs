using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interact : MonoBehaviour
{
    inventoryObject inventory;
    PlayerController playerScript;

    [Header("Interact")]
    [SerializeField] float pickupDis;
    [SerializeField] AudioSource PlayerAudio;
    [SerializeField] AudioClip PickUp;
    [SerializeField] AudioClip Place;
    [SerializeField] LayerMask ignoreMask;
    bool DisplayActive;
    private Ray interactRay;
    private RaycastHit interactHit;
    bool isPickedUp;
    bool inProcess;
    public bool hasItems;
    List<EffigyScript> Effgies = new List<EffigyScript>();
    private void Start()
    {
        inventory = gameManager.gameInstance.playerScript.inventory;
        playerScript = gameManager.gameInstance.playerScript;
        DisplayActive = gameManager.gameInstance.EffigyImage.gameObject.activeInHierarchy;
    }
    void Update()
    {
      interact();

        if(Effgies.Count > 1 && Input.GetKeyDown("t"))
        {
            EffigyScript currentEffigy = Effgies[0];
            Effgies.RemoveAt(0);
            Effgies.Add(currentEffigy);
           
        }
        if (Effgies != null && Effgies.Count > 0)
        {
            if (!DisplayActive)
            {
                gameManager.gameInstance.ActivateEffigiDisplay();
                DisplayActive = true;
            }

            gameManager.gameInstance.SetInHand(Effgies[0].gameObject.name);
        }
        else if (Effgies == null || Effgies.Count <= 0)
        {
            if (DisplayActive)
            {
                gameManager.gameInstance.SetInHand("None");
                StartCoroutine(HideDisplay());
            }
        }
        else if(Effgies == null || Effgies.Count <= 0 && DisplayActive == false)
        {
            return;
        }
    }

    IEnumerator HideDisplay()
    {

        yield return new WaitForSeconds(4);

        if (Effgies == null || Effgies.Count <= 0)
        {
            gameManager.gameInstance.DeactivateEffigyDisplay();
            DisplayActive = false;
        }


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
                                var arm = weapon.transform.parent;
                                var altar = arm.GetComponentInParent<AlterShopController>();
                                altar.pickedWeaponUp = true;
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
                if (Effgies.Count > 0)
                {
                    gameManager.gameInstance.displayRequiredIemsUI("('E')", 5);
                    Altarinteract alter = hit.collider.gameObject.GetComponent<Altarinteract>();

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (alter.HasObject == false && Effgies[0] != null)
                        {

                            alter.PlaceObject(Effgies[0]);

                            Effgies.Remove(Effgies[0]);
                            AudioManager.audioInstance.playSFXAudio(Place, 1f);
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
                    return;
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
            EffigyScript model = hit.collider.GetComponent<EffigyScript>();
            if (pickup != null)
            {
                if (pickup.item.type == itemType.Rune)
                {
                    inventory.AddItem(pickup.item, 1);
                    if (model != null)
                    {

                        GameObject newEffigyObject = Instantiate(model.GetModel(), Vector3.zero, Quaternion.identity);

                        EffigyScript newModel = newEffigyObject.GetComponent<EffigyScript>();
                        if (newModel == null)
                        {
                            newModel = newEffigyObject.AddComponent<EffigyScript>();
                        }
                        newModel.SetNumber(model.GetNumber());
                        newModel.SetModel(newEffigyObject);
                        newModel.setItem(model.GetItem());
                        newModel.SetGameObject(newEffigyObject);
                        newModel.SetType(model.getType());
                        
                        Effgies.Add(newModel);

                    }
                    //PlayerAudio.PlayOneShot(PickUp, 1f);
                    AudioManager.audioInstance.playSFXAudio(PickUp, gameManager.gameInstance.playerScript.interactVol);
                    inventory.updateInventoryUI();
                    Destroy(hit.collider.gameObject);
                }
                else if (pickup.item.type == itemType.flashlight)
                {
                    gameManager.gameInstance.displayRequiredIemsUI("'F' to use flashlight.", 3f);
                    inventory.AddItem(pickup.item, 1);
                    Destroy(hit.collider.gameObject);
                }
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
