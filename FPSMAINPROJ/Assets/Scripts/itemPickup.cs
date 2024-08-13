using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class itemPickup : MonoBehaviour , IPickup
{
    [SerializeField] int pickupDis;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Camera cam;
    private GameObject pickUp; //item to pickup
    private GameObject player;
    private PlayerController playerController;
    public List<string> basicInventory;

    bool isPickedUp;
    bool inProcess;
   
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        basicInventory = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Interact());
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
            basicInventory.Add(recived);
            Destroy(item);
        }
        else if (recived == "Door")
        {
            Debug.Log(checkInventory("Key"));
            if (checkInventory("Key"))
            { 
                Destroy(item);
            }
            else
            {
                Debug.Log("no key found in inv");
            }
            
        }
    }

    IEnumerator Interact()
    {
        RaycastHit hit;
    
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            IPickup pickup = hit.collider.GetComponent<IPickup>();
            if (pickup != null)
            {
                pickup.pickUpItem(hit.collider.gameObject);
            }
        }
        yield return new WaitForSeconds(1);

    }

    bool checkInventory(string item)
    {
        Debug.Log(basicInventory.Count);
        foreach(string invItem in basicInventory)
        {
            Debug.Log("item in inv: " + invItem);
            if(item == invItem)
            {
                return true;
            }
        }
        return false;
    }
}
