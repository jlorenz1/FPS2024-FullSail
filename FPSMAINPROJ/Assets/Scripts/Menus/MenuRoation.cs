using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoation : MonoBehaviour
{
    [SerializeField] float yRotationSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, yRotationSpeed * Time.deltaTime, 0);
    }
}
