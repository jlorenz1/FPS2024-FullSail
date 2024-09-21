using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSway : MonoBehaviour
{

    public float intensity;
    public float smoothing;

    private Quaternion originRot;
    private WeaponController weapon;
    // Start is called before the first frame update
    void Start()
    {
        //original rotation
        originRot = transform.localRotation;
     
    }

    // Update is called once per frame
    void Update()
    {
        updateSway();
    }

    private void updateSway()
    {
        //using mouse input like camera contoller.
        float yAxis = Input.GetAxis("Mouse Y");
        float xAxis = Input.GetAxis("Mouse X");

        //get both targets on the x and y to rotate
        Quaternion targetX = Quaternion.AngleAxis(intensity * xAxis, Vector3.up); //up down 
        Quaternion targetY = Quaternion.AngleAxis(intensity * yAxis, Vector3.right); //left right
        Quaternion targetRot = originRot * targetX * targetY; //multiply found targets to originalrot.

        //lerp using the current rotation of the gun and target rot
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, Time.deltaTime * smoothing);
    }
}
