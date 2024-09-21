using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionFollower : MonoBehaviour
{
    public Transform targetTrans;
    public Vector3 offSet;
    public Vector3 originalOffset;


    void Start()
    {
        originalOffset = transform.position - targetTrans.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = targetTrans.position + originalOffset + offSet;
    }
}
