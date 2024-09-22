using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponKickBack : MonoBehaviour
{
    [SerializeFeild] public Transform gunTransform;
    [SerializeFeild] public float kickAmount;
    [SerializeFeild] public float kickSpeed;
    [SerializeFeild] public float kickSnapping;
   

    [SerializeFeild] public float kickPosAmount;
    [SerializeFeild] public float kickPosSnapping;

    private Vector3 currentRot;
    private Vector3 targetRot;

    public bool isKicking;
   
    private void Update()
    {
        if(isKicking)
        {
            //rotation
            targetRot = Vector3.Lerp(targetRot, Vector3.zero, kickSpeed * Time.deltaTime);
            //random between the target and current rotation, going back at the disered speed, Slerp allows for smooth motion
            currentRot = Vector3.Slerp(currentRot, targetRot, kickSnapping * Time.deltaTime);
            //apply the current rotation of recoil to the camera rotation 
            transform.localRotation = Quaternion.Euler(currentRot);

          
        }
        
    }

    public void addKick()
    {
        if(!isKicking)
        {
            isKicking = true;
        }

        targetRot += new Vector3(kickAmount, transform.localRotation.y, transform.localRotation.z);
       
        
    }
    
}
