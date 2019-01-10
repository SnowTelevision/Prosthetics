using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arm tip detect item trigger and collision
/// </summary>
public class ArmTipDetectCollision : DetectCollision
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        // If armTip is touching an item
        if (collidingCollider != null && collidingCollider.GetComponent<ItemInfo>())
        {

        }
    }

    public override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
    }

    public override void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
