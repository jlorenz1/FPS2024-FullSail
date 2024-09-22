using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponKickup : MonoBehaviour
{
   
    [SerializeField] public float kickPosAmount;
    [SerializeField] public float kickPosSnapping;
    public Vector3 targetPos;
    public Vector3 currentPos;
    bool isKicking;

    private void Start()
    {
        currentPos = transform.localPosition;
        targetPos = currentPos;

    }
    private void Update()
    {
        if (isKicking)
        {
            

            //potition
            //targetPos = Vector3.Lerp(targetPos, Vector3.zero, kickPosAmount * Time.deltaTime);
            currentPos = Vector3.Slerp(currentPos, targetPos, kickPosSnapping * Time.deltaTime);
            transform.localPosition = currentPos;

            // Check if close enough to stop kicking
            if (Vector3.Distance(currentPos, targetPos) < 0.01f)
            {
                isKicking = false; // Stop kicking once close to target
                targetPos = currentPos; // Reset target position
            }
        }

    }

    public void addKick()
    {
        if (!isKicking)
        {
            isKicking = true;
        }

       
        targetPos += new Vector3(transform.localPosition.x, transform.localPosition.y, kickPosAmount);

    }
}
