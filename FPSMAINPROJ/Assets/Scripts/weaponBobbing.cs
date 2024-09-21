using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponBobbing : MonoBehaviour
{
    public Transform gunHolderTrans;
    public Vector3 offSet;

    public float intensity;
    public float intensityOnX;
    public float speed;

    private Vector3 originalPosition;
    private float sinTime;

    void Start()
    {
        originalPosition = gunHolderTrans.localPosition;
    }
    // Update is called once per frame
    void Update()
    {

        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));

        //get magnitude based on input axis
        if (inputVector.magnitude > 0f)
        {
            sinTime += Time.deltaTime * speed;
        }
        else
        {
            sinTime = 0f;
        }

        //using -sin for a wave that starts at 0,0. Sin makes waves giving smooth transitions.
        //sinAmountY represents the vertical displacement of the gun position.
        float sinAmountY = -Mathf.Abs(intensity * Mathf.Sin(sinTime));

        //use cos gives the displacement of the X axis of the gun position. 
        //this also gives a smooth transition side to side for the gun to move in a bobbing left to right
        Vector3 sinAmountX = gunHolderTrans.right * intensity * Mathf.Cos(sinTime) * intensityOnX;

        //using the position of the wave using a targ and lerp can smoothly move the gun, this reduces the snapping that was occuring
        Vector3 targPosition = originalPosition + new Vector3(sinAmountX.x, sinAmountY, sinAmountX.z);
        
        if(gameManager.gameInstance.playerScript.isSprinting)
        {
            float sprintSpeed = gameManager.gameInstance.playerScript.speedDuringSprint;
            gunHolderTrans.localPosition = Vector3.Lerp(gunHolderTrans.localPosition, targPosition, Time.deltaTime * sprintSpeed);
        }
        else
        {
            gunHolderTrans.localPosition = Vector3.Lerp(gunHolderTrans.localPosition, targPosition, Time.deltaTime * speed);
        }
        
    }
}
