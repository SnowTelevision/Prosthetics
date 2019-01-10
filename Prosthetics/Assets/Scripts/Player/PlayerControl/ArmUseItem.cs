using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Let the arm use items
/// </summary>
public class ArmUseItem : MonoBehaviour
{


    public GameObject currentlyHoldingItem; // The item that is currently holding by this arm
    public bool hasTriggerReleased; // Has the trigger being released since the last pushed down
    public UnityEvent setupItem; // The event to be triggered for setup the item when the player picked it up
    public UnityEvent useItem; // The event to be triggered for the holding item when the player start using it
    public UnityEvent stopUsingItem; // The event to be triggered for the holding item when the player stop using it
    public UnityEvent resetItem; // The event to be triggered for reset the item when the player droped it
    public delegate void SetupItemDelegateClass();
    public delegate void UseItemDelegateClass();
    public delegate void StopUsingItemDelegateClass();
    public delegate void ResetItemDelegateClass();
    public SetupItemDelegateClass setupItemDelegate;
    public UseItemDelegateClass useItemDelegate;
    public StopUsingItemDelegateClass stopUsingItemDelegate;
    public ResetItemDelegateClass resetItemDelegate;

    // Use this for initialization
    void Start()
    {
        hasTriggerReleased = true;
    }

    // Update is called once per frame
    void Update()
    {
        DetectIfUseItem();

        if (currentlyHoldingItem == null)
        {
            ClearUnremovedJoints();
        }
    }

    private void LateUpdate()
    {
        UpdateTriggerStatus();
    }

    /// <summary>
    /// Update if the trigger has been released
    /// </summary>
    public void UpdateTriggerStatus()
    {
        if (GetComponentInParent<ControlArm>().isLeftArm)
        {
            // If the left trigger is pressed down
            if (Input.GetAxis("LeftTrigger") >= GetComponentInParent<ControlArm>().triggerThreshold)
            {
                // If the player just pressed it down
                if (hasTriggerReleased)
                {
                    hasTriggerReleased = false;
                }
            }
            else
            {
                hasTriggerReleased = true;
            }
        }

        if (!GetComponentInParent<ControlArm>().isLeftArm)
        {
            // If the right armTip is not holding an item and the right trigger is pressed down
            if (Input.GetAxis("RightTrigger") >= GetComponentInParent<ControlArm>().triggerThreshold)
            {
                // If the player just pressed it down
                if (hasTriggerReleased)
                {
                    hasTriggerReleased = false;
                }
            }
            else
            {
                hasTriggerReleased = true;
            }
        }
    }

    /// <summary>
    /// Detect if the armTip is using an item (need to implement hold down to use continuously)
    /// </summary>
    public void DetectIfUseItem()
    {
        // If the arm tip is holding an usable item
        if (currentlyHoldingItem != null &&
            currentlyHoldingItem.GetComponent<ItemInfo>().canUse)
        {
            if (GetComponentInParent<ControlArm>().isLeftArm)
            {
                // If the left trigger is pressed down
                if (Input.GetAxis("LeftTrigger") >= GetComponentInParent<ControlArm>().triggerThreshold)
                {
                    // If the player just pressed it down
                    if (hasTriggerReleased)
                    {
                        TryUsingItem();
                    }
                }
                else
                {
                    StopUsingItem();
                }
            }

            if (!GetComponentInParent<ControlArm>().isLeftArm)
            {
                // If the right armTip is not holding an item and the right trigger is pressed down
                if (Input.GetAxis("RightTrigger") >= GetComponentInParent<ControlArm>().triggerThreshold)
                {
                    // If the player just pressed it down
                    if (hasTriggerReleased)
                    {
                        TryUsingItem();
                    }
                }
                else
                {
                    StopUsingItem();
                }
            }
        }
    }

    /// <summary>
    /// Invoke the start using event that is currently registered to this armTip
    /// </summary>
    public void TryUsingItem()
    {
        useItem.Invoke();
        useItemDelegate();
    }

    /// <summary>
    /// Invoke the stop using event that is currently registered to this armTip
    /// </summary>
    public void StopUsingItem()
    {
        stopUsingItem.Invoke();
        stopUsingItemDelegate();
    }

    /// <summary>
    /// Invoke the setup item event
    /// </summary>
    public void SetUpItem()
    {
        setupItem.Invoke();
        setupItemDelegate();
    }

    ///// <summary>
    ///// Invoke the start using event that is currently registered to this armTip when the item is setup
    ///// </summary>
    ///// <returns></returns>
    //public IEnumerator TryUsingItemOnSetup()
    //{
    //    // Wait until the next update
    //    yield return new WaitForEndOfFrame();
    //    yield return null;
    //    yield return new WaitForEndOfFrame();

    //    if (currentlyHoldingItem != null)
    //    {
    //        TryUsingItem();
    //    }
    //}

    /// <summary>
    /// If the item is forced to be dropped
    /// </summary>
    /// <param name="breakForce"></param>
    private void OnJointBreak(float breakForce)
    {
        currentlyHoldingItem.GetComponent<ItemInfo>().ForceDropItem();
    }

    /// <summary>
    /// If the armTip is not holding anything, check if there is any un-destroyed joints
    /// </summary>
    public void ClearUnremovedJoints()
    {
        foreach (Joint j in GetComponents<Joint>())
        {
            Destroy(j);
        }
    }
}
