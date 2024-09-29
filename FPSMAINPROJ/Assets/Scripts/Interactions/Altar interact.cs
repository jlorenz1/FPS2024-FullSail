using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altarinteract : MonoBehaviour
{
    [SerializeField] bossInteraction BossThing;
    [SerializeField] Transform EffigyPosition;
    public bool HasObject;
    public int AltarNumber;
    public GameObject GlowAura;

    private void Start()
    {
        GlowAura.SetActive(false);
    }
    public void PlaceObject(EffigyScript effigy)
    {

        if (HasObject == false)
        {

            HasObject = true;

            effigy.transform.position = EffigyPosition.position;

            effigy.transform.rotation = Quaternion.Euler(-90, 0, 0);

            if (AltarNumber == effigy.GetNumber())
            {
                GlowAura.SetActive(true);
                BossThing.EffigiesPlaced();
            }


           
        }

        else
            takeObject();

    }
    public void takeObject()
    {
        if (GlowAura.activeInHierarchy)
        {
            GlowAura.SetActive(false);
            BossThing.EffigiesTaken();
        }

        HasObject = false;

    }

   public void setAltarNumber(int altarNumber)
    {
        AltarNumber = altarNumber;
    }

    public void SetAura()
    {
        GlowAura.SetActive(!GlowAura.activeSelf);
    }
}
