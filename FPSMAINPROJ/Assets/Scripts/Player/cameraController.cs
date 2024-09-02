using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int vertMax, vertMin; //Max up and down camera can rotate
    [SerializeField] bool invert;

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
    }
}
