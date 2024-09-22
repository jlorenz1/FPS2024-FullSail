using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altarinteract : MonoBehaviour
{
    [SerializeField] bossInteraction BossThing;
    [SerializeField] Transform EffigyPosition;
    public bool HasObject;
public void PlaceObject(GameObject effigy)
    {

        Instantiate(effigy, EffigyPosition.position, EffigyPosition.rotation);

        BossThing.EffigiesPlaced();

        HasObject = true;

    }
    public void takeObject()
    {

        BossThing.EffigiesTaken();
        HasObject = false;

    }
}
