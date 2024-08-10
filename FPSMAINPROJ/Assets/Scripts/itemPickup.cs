using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour , IPickup
{
    private GameObject pickUp; //item to pickup
    private GameObject player;

    private PlayerController playerController;

    public ArrayList inventory = new ArrayList();

    bool isPickedUp;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        pickUp = this.gameObject;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && pickUp.tag == "Item1")
        {
            Debug.Log("hit");
            string gun = "gun";
            inventory.Add(gun);
            pickUp = GameObject.FindWithTag("Item1");
            isPickedUp = true;
            pickUpItem(pickUp);
            isPickedUp = false;
        }
    }

    public void pickUpItem(GameObject item)
    {
        if (isPickedUp && item.tag == "Item1")
        {
            Debug.Log(inventory.Count);
        }
        Destroy(item);
    }
}
