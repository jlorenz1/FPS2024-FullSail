using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altarinteract : MonoBehaviour
{
    [SerializeField] bossInteraction BossThing;
    [SerializeField] Transform EffigyPosition;
    public bool HasObject;
    public int AltarNumber;
public void PlaceObject(GameObject effigy)
    {

        Instantiate(effigy, EffigyPosition.position, EffigyPosition.rotation);

        BossThing.EffigiesPlaced(AltarNumber);

        HasObject = true;

    }
    public void takeObject()
    {

        BossThing.EffigiesTaken(AltarNumber);
        HasObject = false;

    }

   public void setAltarNumber(int altarNumber)
    {
        AltarNumber = altarNumber;
    }




}
