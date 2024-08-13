using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour , IPickup
{
    [SerializeField] int pickupDis;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Camera cam;
    private GameObject pickUp; //item to pickup
    private GameObject player;
    private PlayerController playerController;
    public ArrayList basicInventory = new ArrayList();

    bool isPickedUp;
    bool inProcess;

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
    }

    public void pickUpItem(GameObject item)
    {
        if (item.tag == "pickup")
        {
            Debug.Log("hit");
            int pickupLayer = item.layer;
            string recived = LayerMask.LayerToName(pickupLayer);
            Debug.Log(recived);
            basicInventory.Add(item);
        }
        Destroy(item);
    }

    IEnumerator Interact()
    {
        RaycastHit hit;
    
        if (Physics.Raycast(gameManager.gameInstance.MainCam.transform.position, gameManager.gameInstance.MainCam.transform.forward, out hit, pickupDis, ~ignoreMask))
        {
            IPickup pickup = hit.collider.GetComponent<IPickup>();
            Debug.Log(hit.collider.name);

            if(pickup != null)
            {
                pickup.pickUpItem(hit.collider.gameObject);
            }
        }
        yield return new WaitForSeconds(1);

    }
}
