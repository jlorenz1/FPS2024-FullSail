using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public interface IEnemyDamage 
{
    // Interface to assign damage to an object

    void takeDamage(float amountOfDamageTaken);

    void TakeTrueDamage(float amountOfDamageTaken);

    void AddHP(float amount);

    void AddMaxHp(float amount);

    void AddDamage(float amount);

    void AddSpeed(float amount);


    void AddAttackSpeed(float amount);



    void cutspeed(float amount, float damagetaken);

    void cutdamage(float amount);

    void DieWithoutDrops();

    void AddArmor(float amount);

    void RemoveArmor(float amount);

    void TempRemoveArmor(float reduction, float Duration);

    void Blind(float duration);

    void Stun(float duration);

    void knockback(Vector3 hitPoint, float distance, float Duration);

    float GetMaxHP();


    bool isKnockBackRessitant();
}
