using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] public int sens;
    [SerializeField] int vertMax, vertMin; //Max up and down camera can rotate
    [SerializeField] bool invert;

    private Vector3 currentRot;
    private Vector3 targetRot; //where you want the recoil to end at 

    [Header("RECOIL VARIABLES")]
    [SerializeFeild] public float recoilX; //offset for x
    [SerializeFeild] public float recoilY; //offset for y
    [SerializeFeild] public float recoilZ; //offset for z
    [SerializeFeild] public float snapping; //fast it goes up
    [SerializeFeild] public float returnSpeed; //takes for the gun to go back down
    private bool isFiring;

    float rotX;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime; 
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;


        //Option for invert up/down
        if(invert)
        {
            rotX += mouseY;
        }
        else
        {
            rotX -= mouseY;
        }
           
        //Camera rotation 
        rotX = Mathf.Clamp(rotX, vertMin, vertMax);

        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);


        //RECOIL CODE
        //checks if the recoil is happening or not, player is shooting.
        if (isFiring)
        {
            //using lerp to go up to the target location, going back to the original pos at a certian speed 
            targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
            //random between the target and current rotation, going back at the disered speed, Slerp allows for smooth motion
            currentRot = Vector3.Slerp(currentRot, targetRot, snapping * Time.deltaTime);
            //apply the current rotation of recoil to the camera rotation 
            transform.localRotation = Quaternion.Euler(currentRot);
        }
        //this allows if the recoil effected the x rotation of the player moving and the recoil moving it at the same time. 
        //I was stuck for a while with the camera being locked up and this is what I found to be the solution. 
        transform.localRotation = Quaternion.Euler(rotX + currentRot.x, 0, 0);
    }

    public void RecoilFire()
    {
        if (!isFiring)
        {
            isFiring = true;
        }

        //adding the recoil specific effects creating the pattern for the gun
        //using random for the y and z to get a more natural recoil effect. 
        targetRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
