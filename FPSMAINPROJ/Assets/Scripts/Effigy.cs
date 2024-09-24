using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffigyScript :  pickup
{
    [SerializeField] public GameObject effigyObject;
    int EffigyNumber;

    public void SetNumber(int label)
    {
        EffigyNumber = label;
    }
    public int GetNumber()
    {
        return EffigyNumber;
    }

    public void SetModel(GameObject NewModel)
    {
        effigyObject = NewModel;

    }

    public GameObject GetModel()
    {
        return effigyObject;
    }

   
}
