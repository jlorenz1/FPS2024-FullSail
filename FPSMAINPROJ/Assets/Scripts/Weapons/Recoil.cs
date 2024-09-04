using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Vector3 currentRot;
    private Vector3 targetRot; //where you want the recoil to end at 

    [SerializeFeild] public float recoilX;
    [SerializeFeild] public float recoilY;
    [SerializeFeild] public float recoilZ;

    [SerializeFeild] public float snapping;
    [SerializeFeild] public float returnSpeed; //takes for the gun to go back down

    private bool isFiring;

    private void Update()
    {
        if(isFiring)
        {
            targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
            currentRot = Vector3.Slerp(currentRot, targetRot, snapping * Time.deltaTime);

            transform.localRotation = Quaternion.Euler(currentRot);
        }
      
    }

    public void RecoilFire()
    {
        if(!isFiring)
        {
            isFiring = true;
        }
        targetRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
