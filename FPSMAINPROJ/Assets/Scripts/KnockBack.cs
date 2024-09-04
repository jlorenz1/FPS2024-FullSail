using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [SerializeField] float Y;
    [SerializeField] float X;
    [SerializeField] float Z;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {


        Debug.Log("collision active");

        if (collision.rigidbody != null)
        {
            // Combine the forces into one Vector3
            Vector3 force = Vector3.up * Y + Vector3.back * X + Vector3.left * Z;

            // Apply the force to the Rigidbody
            collision.rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }





}
