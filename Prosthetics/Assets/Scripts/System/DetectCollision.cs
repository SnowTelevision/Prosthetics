using UnityEngine;
using System.Linq;

/// <summary>
/// Detects collision
/// </summary>
public class DetectCollision : MonoBehaviour
{
    public bool ignoreTrigger; // If ignore trigger
    public Transform[] ignoredGameObjects; // Which game object(s) should this collider/trigger ignore (not in the Unity physics engine level, only in this script)
    public LayerMask ignoredLayers; // Which layer should this collider/trigger ignore (not in the Unity physics engine level, only in this script)
    public bool isArmTip; // Is this an armTip detector

    public bool isColliding; // If the collider is colliding something
    public GameObject collidingCollider; // The collider it is colliding with
    public bool isEnteringCollider; // If the trigger is entering a collider
    public GameObject enteringCollider; // The collider it is entering
    public bool isEnteringTrigger; // If the collider is entering a trigger
    public GameObject collidingTrigger; // The trigger it is entering
    public Vector3 collidingPoint; // The colliding position
    public Collision currentCollision; // The collision

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        //isColliding = false;
        //isEnteringCollider = false;
        //isEnteringTrigger = false;
        if (collidingCollider == null)
        {
            isColliding = false;
        }
        if (enteringCollider == null)
        {
            isEnteringCollider = false;
        }
        if (collidingTrigger == null)
        {
            isEnteringTrigger = false;
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        // If the collision should not be detected
        if (!VerifyCollision(false, collision.gameObject))
        {
            return;
        }

        currentCollision = collision;
        collidingCollider = collision.gameObject;
    }

    public virtual void OnCollisionStay(Collision collision)
    {
        // If the collision should not be detected
        if (!VerifyCollision(false, collision.gameObject))
        {
            return;
        }

        isColliding = true;
        collidingCollider = collision.gameObject;
        collidingPoint = collision.contacts[0].point;
    }

    public virtual void OnCollisionExit(Collision collision)
    {
        // If the collision should not be detected
        if (!VerifyCollision(false, collision.gameObject))
        {
            return;
        }

        currentCollision = null;
        collidingCollider = null;
        isColliding = false;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        // If the collision should not be detected
        if (!VerifyCollision(false, other.gameObject))
        {
            return;
        }

        // If ignoring trigger colliders
        if (ignoreTrigger && other.isTrigger)
        {
            return;
        }

        // If the entering collider is not a trigger
        if (!other.isTrigger)
        {
            enteringCollider = other.gameObject;
        }
        else
        {
            collidingTrigger = other.gameObject;
        }
    }

    public virtual void OnTriggerStay(Collider other)
    {
        // If the collision should not be detected
        if (!VerifyCollision(false, other.gameObject))
        {
            return;
        }

        // If ignoring trigger colliders
        if (ignoreTrigger && other.isTrigger)
        {
            return;
        }

        // If the entering collider is not a trigger
        if (!other.isTrigger)
        {
            enteringCollider = other.gameObject;
            collidingPoint = other.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            isEnteringCollider = true;
        }
        else
        {
            collidingTrigger = other.gameObject;
            collidingPoint = other.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            isEnteringTrigger = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        // If the collision should not be detected
        if (!VerifyCollision(false, other.gameObject))
        {
            return;
        }

        // If ignoring trigger colliders
        if (ignoreTrigger && other.isTrigger)
        {
            return;
        }

        // If the entering collider is not a trigger
        if (!other.isTrigger)
        {
            enteringCollider = null;
            isEnteringCollider = false;
        }
        else
        {
            collidingTrigger = null;
            isEnteringTrigger = false;
        }
    }

    /// <summary>
    /// Determine if the collider or trigger should be detected
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool VerifyCollision(bool trigger, GameObject other)
    {
        // If other's layer is in the ignored layer
        if (ignoredLayers == (ignoredLayers | (1 << other.layer)))
        {
            return false;
        }

        // Don't detect ignored colliders/triggers
        foreach (Transform t in ignoredGameObjects)
        {
            if (other == t || other.GetComponentsInChildren<Transform>().Contains(t))
            {
                return false;
            }
        }

        // Don't detect tutorial trigger box
        if (other.GetComponent<TriggerDetectStartEvent>())
        {
            return false;
        }

        // If this is the armTip and the collider is not an item
        if (isArmTip)
        {
            // If it is a child
            if (other.transform.parent != null)
            {
                // If it is not a touch range of an item or not an item itself
                if (other.name != "ArmTipTouchRange" && !other.GetComponent<ItemInfo>())
                {
                    return false;
                }
            }
            else
            {
                // If it is not an item
                if (!other.GetComponent<ItemInfo>())
                {
                    return false;
                }
            }
        }

        return true;
    }
}
