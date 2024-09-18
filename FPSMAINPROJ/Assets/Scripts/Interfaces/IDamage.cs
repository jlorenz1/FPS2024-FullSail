using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{ 
    // Interface to assign damage to an object
    void takeDamage(float amountOfDamageTaken);

    float GetHealth();

    void CutSpeed(float duration, float strength);

    void TickDamage(float duration, float amountpertick, float tickrate);
}
