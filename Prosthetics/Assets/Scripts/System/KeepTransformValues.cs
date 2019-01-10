using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keep some of the transform values for the game object
/// </summary>
public class KeepTransformValues : MonoBehaviour
{
    public bool keepPosiX;
    public float posiX;
    public bool keepPosiY;
    public float posiY;
    public bool keepPosiZ;
    public float posiZ;
    public bool keepEulerX;
    public float eulerX;
    public bool keepEulerY;
    public float eulerY;
    public bool keepEulerZ;
    public float eulerZ;
    public bool keepFacingDirection; // Should this object keep facing where it is facing
    public Vector3 facingForward; // The forward direction the object should facing
    public Vector3 facingUp; // The up direction the object should facing

    public Vector3 localPositionToKeep;
    public Vector3 localEulerAnglesToKeep;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        localPositionToKeep = transform.localPosition;
        localEulerAnglesToKeep = transform.localEulerAngles;

        if (keepPosiX)
        {
            localPositionToKeep.x = posiX;
        }
        if (keepPosiY)
        {
            localPositionToKeep.y = posiY;
        }
        if (keepPosiZ)
        {
            localPositionToKeep.z = posiZ;
        }
        if (keepEulerX)
        {
            localEulerAnglesToKeep.x = eulerX;
        }
        if (keepEulerY)
        {
            localEulerAnglesToKeep.y = eulerY;
        }
        if (keepEulerZ)
        {
            localEulerAnglesToKeep.z = eulerZ;
        }

        transform.localPosition = localPositionToKeep;
        transform.localEulerAngles = localEulerAnglesToKeep;

        if (keepFacingDirection)
        {
            transform.LookAt(transform.position + facingForward, facingUp);
        }
    }
}
