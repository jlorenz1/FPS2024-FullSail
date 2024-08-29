using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeele : MonoBehaviour
{
    [SerializeField] int MeleeDamage;
    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger || other.CompareTag("Player"))
        {
            return;
        }

        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(MeleeDamage);
        }
    }
}
