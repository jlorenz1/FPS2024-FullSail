using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    Transform RightHandTransform;
    Transform LeftHandTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeHandLocation(Transform currentGun)
    {
        RightHandTransform = currentGun.transform.Find("Right Hand Grip");
        LeftHandTransform = currentGun.transform.Find("Left Hand Grip");
        if(RightHandTransform != null && LeftHandTransform != null)
        {

            gameManager.gameInstance.rightHandTarget.data.target = RightHandTransform;
            gameManager.gameInstance.leftHandTarget.data.target = LeftHandTransform;
        }

    }

}
