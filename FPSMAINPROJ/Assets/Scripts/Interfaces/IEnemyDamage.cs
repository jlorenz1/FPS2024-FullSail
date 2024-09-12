using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyDamage
{
    // Interface to assign damage to an object

    void takeDamage(float amountOfDamageTaken);

    void TakeTrueDamage(float amountOfDamageTaken);

    void AddHP(int amount);

    void AddDamage(int amount);

    void AddSpeed(int amount);

    void cutspeed(float amount, float damagetaken);

    void cutdamage(float amount);

    void DieWithoutDrops();

    void AddArmor(float amount);

    void RemoveArmor(float amount);

    void TempRemoveArmor(float reduction, float Duration);

    void Blind(float duration);

    void Stun(float duration);
}
