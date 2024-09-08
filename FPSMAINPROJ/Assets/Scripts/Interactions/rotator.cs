using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float bobbingAmount = 1f;
    public float bobbingSpeed = 1f;

    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //rotating the object
        float rotation  = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotation);

        //bobbing the object (up and down)
        //staying at the objects y, going from 1 to -1 creating the bobbing effect * how much it moves 
        float objectY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, objectY, transform.position.z);
    }
}
