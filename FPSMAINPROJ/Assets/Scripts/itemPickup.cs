using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class itemPickup : MonoBehaviour , IPickup
{
    [SerializeField] int pickupDis;
    [SerializeField] LayerMask ignoreMask;
    private GameObject pickUp; //item to pickup
    private GameObject player;
    private PlayerController playerController;
    private Ray interactRay;
    private RaycastHit interactHit;
    bool isPickedUp;
    bool inProcess;
    bool doorOpen = false;
      
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Interact());
        }

        interactRay = Camera.main.ScreenPointToRay(Camera.main.transform.position);
        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out interactHit, pickupDis, ~ignoreMask))
        {

            if (interactHit.collider.GetComponent<IPickup>() != null)
            {
                gameManager.gameInstance.playerInteract.SetActive(true);
            }
            else
            {
                gameManager.gameInstance.playerInteract.SetActive(false);
            }
        }
    }

    public void pickUpItem(GameObject item)
    {
        int pickupLayer = item.layer;
        string recived = LayerMask.LayerToName(pickupLayer);
        if (recived == "Key")
        {
            Debug.Log("hit");
            Debug.Log(recived);
            inventoryManager.Instance.AddItem(recived);
            Destroy(item);
        }
        else if (recived == "Door")
        {
            Debug.Log(inventoryManager.Instance.checkInventory("Key"));

            if (inventoryManager.Instance.checkInventory("Key"))
            {
                StartCoroutine(slideDoor(item.transform));
            }
            else
            {
                Debug.Log("no key found in inv");
            }

        }
        else if (recived == "Ammo")
        {
            if (playerController.weapon != null)
            {
                if(playerController.weapon.getAmmoCount() < playerController.weapon.getMaxAmmoCount())
                {
                    playerController.weapon.HandleAmmoDrop();
                    Destroy(item);
                }
                else { return; }
            }
        }
    }

    IEnumerator Interact()
    {
        RaycastHit hit;
    
        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            IPickup pickup = hit.collider.GetComponent<IPickup>();
            if (pickup != null)
            {
                pickup.pickUpItem(hit.collider.gameObject);
            }
        }
        yield return new WaitForSeconds(1);

    }

    IEnumerator slideDoor(Transform transform)
    {
        if (doorOpen == false)
        {
            //get door pos
            Vector3 doorPos = transform.position;
            //get desired endPos
            Vector3 endPos = doorPos - new Vector3(0, 4, 0);
            //speed to slide
            float slidespeed = 1f;
            //time it takes
            float timeToOpen = 0f;

            while (timeToOpen < slidespeed)
            {
                //lerp over the positions smoothly
                transform.position = Vector3.Lerp(doorPos, endPos, (timeToOpen / slidespeed));
                timeToOpen += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
            doorOpen = true;
        }
        else
        {
            Debug.Log("doors open!");
            yield return null;
        }
    }
}
