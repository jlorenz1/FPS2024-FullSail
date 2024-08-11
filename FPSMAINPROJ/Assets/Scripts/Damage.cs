
   using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary }
    [SerializeField] damageType Type;
    [SerializeField] Rigidbody rb;


    [SerializeField] int DamageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        if (Type == damageType.bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }


}

