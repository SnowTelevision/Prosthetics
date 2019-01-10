using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Copy an object's transform
/// </summary>
public class CopyTransform : MonoBehaviour
{
    public bool copyRotation;
    public bool copyPosition;
    public Transform copyFrom;
    public Vector3 localPositionOffset;
    public Vector3 localEulerOffset;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(copyRotation)
        {
            transform.rotation = copyFrom.rotation;
            transform.localEulerAngles += localEulerOffset;
        }
        if(copyPosition)
        {
            transform.position = copyFrom.position;
            transform.localPosition += localPositionOffset;
        }
    }
}
