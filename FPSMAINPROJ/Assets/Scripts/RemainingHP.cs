using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitPoints
{
    // Interface to assign damage to an object
    void DisplayHitPoints();


    int CurrentHitPoints { get; }
}
