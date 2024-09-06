using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{ 
    // Interface to assign damage to an object
    void takeDamage(float amountOfDamageTaken);


    void cutspeed(float amount, float damagetaken);


    void cutdamage(float amount);


    void DieWithoutDrops();  
}
