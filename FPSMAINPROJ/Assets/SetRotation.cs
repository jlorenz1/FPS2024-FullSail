using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetRotation : MonoBehaviour
{
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = Quaternion.Euler(x, y, z); // Convert Vector3 to Quaternion
    }

    void Update()
    {
        if (transform.rotation != targetRotation)
        {
            transform.rotation = targetRotation; // Set the rotation to the target
        }
    }
}
