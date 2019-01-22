using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the basic movement of the player arms
/// </summary>
public class ControlArm_UsingPhysics : ArmUseItem
{
    public bool isLeftArm; // Is this the left arm
    public Transform armTip; // The tip of the arm
    public Transform arm; // The actual arm that extends from the center of the body to the arm tip
    public GameObject armTip2D; // The 2D arm tip that is connected with the arm-body joint by a 2D slider joint
    public GameObject armBodyJoint2D; // The 2D arm-body joint that is connected to the body by a 2D hinge joint
    public float armBodyHingeJointMaxMotorSpeed; // The maximum motor speed on the 2D hinge joint on the 2D arm-body joint
    public float armBodyHingeJointMotorForce; // The motor torque on the 2D hinge joint on the 2D arm-body joint
    public float armMaxLength; // How long is the maximum arm length
    public float armTipSliderJointMaxMotorSpeed; // The maximum motor speed on the 2D slider joint on the 2D armTip
    public float armTipSliderJointMotorForce; // The motor torque speed on the 2D slider joint on the 2D armTip
    //public float armTipSliderJointMaxMotorSpeed; // The maximum motor speed on the 2D slider joint on the 2D armTip
    //public float armTipSliderJointMotorForce; // The maximum motor speed on the 2D slider joint on the 2D armTip
    public GameObject body2D; // The 2D body
    public Transform body; // The main body
    public LayerMask armCollidingLayer; // The object layers that the arm can collide with
    public LayerMask bodyCollidingLayer; // The object layers that the body can collide with
    public ControlArm_UsingPhysics otherArm_Physics; // The other arm
    public float triggerThreshold; // How much the trigger has to be pressed down to register
    public float armLiftingStrength; // How much weight the arm can lift? (The currently holding item's weight)
    public float armHoldingJointBreakForce; // How much force the fixed joint between the armTip and the currently holding item can bearing before break
    public float collisionRaycastOriginSetBackDistance; // How far should the raycast's origin move back from the pivot center when detecting collision of armTip/body
    public float armDefaultStretchForce; // How much force should be applied for the armTip to stretch and retract
    public float armStopThreshold; // How close the armTip has to be to the target position for it to stop being pushed
    public float joystickLengthThresholdForAngleBetweenTwoArms; // How long the joystick has to extend to be considered stretched out to calculate the angle
    public SpringJoint jointConnectingBody; // The SpringJoint in the first arm segment that is connected to the body
    public Transform lastArmSegment; // The arm segment that connects the armTip
    public ArmControlModeInfo[] armControlModeInfos; // Infos of different arm control modes
    //public Vector3 armConnectingBodyJointLocalPosition; // The local position of the joint the arm connected to the body relative to the body

    /// <summary>
    /// Motor speed function parameters for the function y = ( log_b((x / c / d) + 1) ) / d
    /// </summary>
    public float functionParamB; // The b parameter
    public float functionParamC; // The c parameter
    public float functionParamD; // The d parameter

    /// <summary>
    /// Arm flags
    /// </summary>
    public bool canGrabObject; // Can the armTip grab object or not
    public bool isUsingMechanicalArm; // Is this arm currently operating a mechanical extending arm or not
    public bool inWater; // Is this armTip currently in water
    public bool onLand; // Is this armTip currently touching ground
    public bool inAir; // Is this armTip currently in the air

    /// <summary>
    /// Audios
    /// </summary>
    public AudioClip armTipGrabbingGroundSFX; // The sfx for armTip grabbing onto ground
    public AudioClip armTipStopGrabbingGroundSFX; // The sfx for armTip grabbing move away from ground

    public bool isGrabbingFloor; // If the armTip is grabbing floor
    public float joyStickRotationAngle; // The rotation of the joystick
    public float joyStickRotationAngle360; // The rotation of the joystick in a 0-360 degree range, increasing counter-clockwise
    public float joyStickLength; // How much the joystick is pushed away
    public Vector3 joystickPosition; // Where is the joystick
    public Vector3 joystickLastPosition; // The joystick's position in the last frame
    public float angleBetweenTwoArms; // The angle between the two arms
    // The default connected mass scale on the spring joint of the last arm segment that connects to the armTip
    public float lastArmSegmentDefaultConnectedMassScale;
    // The connected mass scale on the spring joint of the last arm segment that connects to the armTip in the last frame
    public float lastArmSegmentLastConnectedMassScale;
    public float bodyRotation; // 2D body's world z rotation
    public float armBodyJointLocalRotation; // 2D Arm-body joint's rotation relative to the 2D body
    public float targetArmBodyJointLocalRotation; // The target local rotation for the 2D arm-body joint
    //public float targetArmBodyJointHingeAngle; // The target world z angle the 2D Arm-body joint should be
    //public JointAngleLimits2D targetArmBodyJointHingeAngleLimit; // The angle limit on the 2D Arm-body hinge joint should be
    public float unsignedTargetArmBodyHingeJointMotorSpeed; // The target 2D arm-body joint's hinge joint's motor speed (unsigned, absolute value)
    public JointMotor2D targetArmBodyHingeJointMotor; // The target 2D arm-body joint's hinge joint's motor speed
    public HingeJoint2D armBodyHingeJoint2D; // The 2D hinge joint on the 2D arm-body joint
    public float armBodyHingeJointAngle; // The 2D arm-body joint's hinge joint 2D's current rotation angle
    //public HingeJoint2D backUpArmBodyHingeJoint2D; // The "back up" hinge joint 2D which is used to replace the current one once the current one is out of 0 - 360 degree range
    public SliderJoint2D armTipSliderJoint2D; // The slider joint 2D on the 2D armTip
    public float armTipNormalizedDistance; // The normalized distance from the 2D armTip to the arm-body joint
    //public JointTranslationLimits2D targetArmTipSliderJointTranslationLimits; // The translation limit on the 2D armTip's slider joint should be
    public float unsignedTargetArmTipSliderJointMotorSpeed; // The target 2D armTip's slider joint's motor speed (unsigned, absolute value)
    public JointMotor2D targetArmTipSliderJointMotor; // The target 2D armTip's slider joint's motor speed

    //Test//
    public bool test; // Do we print test outputs
    public float bodySpeed; // The speed of the body
    public float bodyTopSpeed; // The top speed the body can reach when swimming
    public Vector3 testWindDirection; //
    public float testWindStrength; //
    public Vector3 testArmGlidingForce;
    public Transform realClamppedJoystickInput; // The target position for this arm tip based on the joystick input

    // Use this for initialization
    void Start()
    {
        // Set up the 2D arm
        armBodyHingeJoint2D = armBodyJoint2D.GetComponent<HingeJoint2D>(); // Get the initial hinge joint 2D on the 2D arm-body joint
        targetArmBodyHingeJointMotor.maxMotorTorque = armBodyHingeJointMotorForce; // Set the max torque of the motor on the 2D hinge joint on the 2D arm-body joint
        armBodyHingeJoint2D.useMotor = true;
        armBodyHingeJoint2D.useLimits = false;
        armTipSliderJoint2D = armTip2D.GetComponent<SliderJoint2D>(); // Get the initial slider joint 2D on the 2D armTip
        targetArmTipSliderJointMotor.maxMotorTorque = armTipSliderJointMotorForce; // Set the max torque of the motor on the 2D slider joint on the 2D armTip
        armBodyHingeJoint2D.useMotor = true;
        // Set the 2D armTip slider joint's translation limits
        JointTranslationLimits2D armTipSliderJointLimit = new JointTranslationLimits2D();
        armTipSliderJointLimit.min = 0;
        armTipSliderJointLimit.max = armMaxLength;
        armTipSliderJoint2D.limits = armTipSliderJointLimit;
        armTipSliderJoint2D.useLimits = true;

        //CreateBackUpHingeJoint2D();

        // Set up the flags
        canGrabObject = true;

        // Set up the arm stretch limiter
        if (isLeftArm)
        {
            //armTipStretchLimiter.localScale = armTipStretchLimiter.localScale * armMaxLength * 2;
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Collect 2D body info
        GetBodyRotation();

        // Collect player joystick input
        CalculateJoyStickRotation(isLeftArm);
        CalculateJoyStickLength(isLeftArm);
        GetClampedJoystickPosition();
        ConvertJoystickRotationAngleTo360();

        // Collect 2D arm info
        GetArmBodyJointLocalRotation();

        // Rotate arm-body joint
        CalculateSignedArmBodyHingeJointMotorSpeed();
        RotateArmBodyJoint();

        // Move armTip
        CalculateSignedArmTipSliderJointMotorSpeed();
        MoveArmTip2D();

        // Make sure the 2D arm-body hinge joint 2D doesn't rotate below 0 degree or above 360 degree
        //ClampArmBodyJointRotationAngle();

        if (!isGrabbingFloor)
        {

            if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null) // If the arm is not holding item
            {
                MoveArm(isLeftArm);

                if (canGrabObject)
                {
                    DetectIfPickingUpItem();
                }
            }
            else if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem != null) // If the arm is holding item
            {
                // If the player is not using a mechanical arm or an item that cannot be moved
                if (!armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().fixedPosition && !isUsingMechanicalArm)
                {
                    if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().itemWeight <= armLiftingStrength) // Is the item is not heavy
                    {
                        MoveArm(isLeftArm);
                    }
                }
                else
                {

                }
            }
        }

        // Test
        if (test)
        {
            //TestControllerInput();
            //bodySpeed = body.GetComponent<Rigidbody>().velocity.magnitude;
            //testArmGlidingForce = CalculateArmGlidingForce(testWindStrength, testWindDirection);
            DisplayJoystickPosition();
        }
    }

    private void LateUpdate()
    {

    }

    private void FixedUpdate()
    {
        LockArmBodyJoint(); // Stop the 2D arm-body joint from moving

        //if (test)
        //print(armBodyJointLocalRotation + ", " + targetArmBodyJointLocalRotation + ", " + targetArmBodyJointHingeAngle + ", " + armBodyHingeJointAngle);
    }

    /// <summary>
    /// Get the joystick's clamped (from 0 to 1) position
    /// </summary>
    public void GetClampedJoystickPosition()
    {
        joystickLastPosition = joystickPosition; // Assign the position in the last frame
        // Calculate the new position in this frame for the x-y plain
        joystickPosition = new Vector3(Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad), Mathf.Cos(joyStickRotationAngle * Mathf.Deg2Rad), 0) *
                                                 joyStickLength;
    }

    /// <summary>
    /// Updates different flag status for the arm
    /// </summary>
    public void UpdateArmFlags()
    {

        if (isGrabbingFloor) // If the arm is grabbing the floor then it cannot grab object
        {
            canGrabObject = false;
        }
        else
        {
            canGrabObject = true;
        }
    }

    /// <summary>
    /// Detect if the armTip is colliding with an item, and if the player want to pick up an item
    /// </summary>
    public void DetectIfPickingUpItem()
    {
        // If the trigger is released after the last item drop (no matter is forced drop or not)
        if (armTip.GetComponent<ArmUseItem>().hasTriggerReleased)
        {
            // If the arm tip is entering an item's trigger
            if (armTip.GetComponent<DetectCollision>().collidingTrigger != null &&
                armTip.GetComponent<DetectCollision>().collidingTrigger.GetComponentInParent<ItemInfo>())
            {
                if (isLeftArm)
                {
                    // If the left armTip is not holding an item and the left trigger is pressed down
                    if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null && Input.GetAxis("LeftTrigger") >= triggerThreshold)
                    {
                        armTip.GetComponent<ArmUseItem>().hasTriggerReleased = false;
                        StartCoroutine(PickUpItem(armTip.GetComponent<DetectCollision>().collidingTrigger.GetComponentInParent<ItemInfo>().gameObject));
                    }
                }

                if (!isLeftArm)
                {
                    // If the right armTip is not holding an item and the right trigger is pressed down
                    if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null && Input.GetAxis("RightTrigger") >= triggerThreshold)
                    {
                        armTip.GetComponent<ArmUseItem>().hasTriggerReleased = false;
                        StartCoroutine(PickUpItem(armTip.GetComponent<DetectCollision>().collidingTrigger.GetComponentInParent<ItemInfo>().gameObject));
                    }
                }
            }
            // If the arm tip is colliding with an item
            else if (armTip.GetComponent<DetectCollision>().collidingCollider != null &&
                     armTip.GetComponent<DetectCollision>().collidingCollider.GetComponentInParent<ItemInfo>())
            {
                if (isLeftArm)
                {
                    // If the left armTip is not holding an item and the left trigger is pressed down
                    if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null && Input.GetAxis("LeftTrigger") >= triggerThreshold)
                    {
                        armTip.GetComponent<ArmUseItem>().hasTriggerReleased = false;
                        StartCoroutine(PickUpItem(armTip.GetComponent<DetectCollision>().collidingCollider.GetComponentInParent<ItemInfo>().gameObject));
                    }
                }

                if (!isLeftArm)
                {
                    // If the right armTip is not holding an item and the right trigger is pressed down
                    if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null && Input.GetAxis("RightTrigger") >= triggerThreshold)
                    {
                        armTip.GetComponent<ArmUseItem>().hasTriggerReleased = false;
                        StartCoroutine(PickUpItem(armTip.GetComponent<DetectCollision>().collidingCollider.GetComponentInParent<ItemInfo>().gameObject));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Start picking up the item
    /// </summary>
    /// <param name="pickingItem"></param>
    /// <returns></returns>
    public IEnumerator PickUpItem(GameObject pickingItem)
    {
        // Assign the item that is currently colliding with the armTip to the armTip's current holding item
        armTip.GetComponent<ArmUseItem>().currentlyHoldingItem = pickingItem;
        // Prevent the player from using the item right when they pick up that item
        armTip.GetComponent<ArmUseItem>().hasTriggerReleased = false;
        // Assign the start and stop using item function to the UnityEvents
        //UnityAction startUsingAction = System.Delegate.CreateDelegate(typeof(UnityAction), pickingItem.GetComponent<ItemInfo>(), "StartUsing") as UnityAction;
        //UnityEventTools.AddPersistentListener(armTip.GetComponent<ArmUseItem>().useItem, startUsingAction);
        //armTip.GetComponent<ArmUseItem>().useItem.AddListener(pickingItem.GetComponent<ItemInfo>().StartUsing);
        //UnityAction stopUsingAction = System.Delegate.CreateDelegate(typeof(UnityAction), pickingItem.GetComponent<ItemInfo>(), "StopUsing") as UnityAction;
        //UnityEventTools.AddPersistentListener(armTip.GetComponent<ArmUseItem>().stopUsingItem, stopUsingAction);
        //armTip.GetComponent<ArmUseItem>().stopUsingItem.AddListener(pickingItem.GetComponent<ItemInfo>().StopUsing);
        armTip.GetComponent<ArmUseItem>().setupItemDelegate =
            System.Delegate.CreateDelegate(typeof(SetupItemDelegateClass), pickingItem.GetComponent<ItemInfo>(), "SetupItem") as SetupItemDelegateClass;
        armTip.GetComponent<ArmUseItem>().useItemDelegate =
            System.Delegate.CreateDelegate(typeof(UseItemDelegateClass), pickingItem.GetComponent<ItemInfo>(), "StartUsing") as UseItemDelegateClass;
        armTip.GetComponent<ArmUseItem>().stopUsingItemDelegate =
            System.Delegate.CreateDelegate(typeof(StopUsingItemDelegateClass), pickingItem.GetComponent<ItemInfo>(), "StopUsing") as StopUsingItemDelegateClass;
        armTip.GetComponent<ArmUseItem>().resetItemDelegate =
            System.Delegate.CreateDelegate(typeof(ResetItemDelegateClass), pickingItem.GetComponent<ItemInfo>(), "ResetItem") as ResetItemDelegateClass;

        // If the other armTip is currently holding the item which is going to be holding by this armTip, then let the other arm drop the item first
        if (otherArm_Physics.armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == pickingItem)
        {
            otherArm_Physics.DropDownItem(pickingItem);
        }

        yield return new WaitForEndOfFrame();

        // Turn off the gravity of the picking item
        if (!pickingItem.GetComponent<ItemInfo>().fixedPosition)
        {
            pickingItem.GetComponent<Rigidbody>().useGravity = false;
        }

        //// If the item can be lifted by the arm, then disable it's gravity and raise it to the arm's height
        if (pickingItem.GetComponent<ItemInfo>().itemWeight <= armLiftingStrength && !pickingItem.GetComponent<ItemInfo>().fixedPosition)
        {
            // Change the item's velocity to 0
            pickingItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
            pickingItem.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // If the item can be moved
            if (!pickingItem.GetComponent<ItemInfo>().fixedPosition)
            {
                // Raise the item to the arm's height
                pickingItem.transform.position = armTip.position;
            }

            // Using spring joint
            armTip.gameObject.AddComponent<SpringJoint>();
            armTip.GetComponent<SpringJoint>().connectedBody = pickingItem.GetComponent<Rigidbody>();
            armTip.GetComponent<SpringJoint>().breakForce = armHoldingJointBreakForce;

            // Turn on drag for better spring physics
            pickingItem.GetComponent<Rigidbody>().drag = 30;
            pickingItem.GetComponent<Rigidbody>().angularDrag = 30;

            if (pickingItem.GetComponent<ItemInfo>().canUse)
            {
                //// Turn off the collider
                //TurnOffColliders(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem);
                // Turn off mass
                pickingItem.GetComponent<Rigidbody>().mass = 0;
                //// Change the item to kinematic
                //pickingItem.GetComponent<Rigidbody>().isKinematic = true;
                // Rotate the item so the player can aim the usable item
                pickingItem.transform.rotation = armTip.rotation;

                // Setup spring joint
                armTip.GetComponent<SpringJoint>().spring = 0.0001f;
                armTip.GetComponent<SpringJoint>().damper = 0;
            }
            else
            {
                // Setup spring joint
                armTip.GetComponent<SpringJoint>().spring = pickingItem.GetComponent<Rigidbody>().mass * 1000f;
                armTip.GetComponent<SpringJoint>().damper = 0;
            }
        }
        else
        {
            // Using fixed joint
            armTip.gameObject.AddComponent<FixedJoint>();
            armTip.GetComponent<FixedJoint>().connectedBody = pickingItem.GetComponent<Rigidbody>();
            armTip.GetComponent<FixedJoint>().breakForce = armHoldingJointBreakForce;
        }

        // If the item cannot be moved
        if (pickingItem.GetComponent<ItemInfo>().fixedPosition && pickingItem.GetComponent<ItemInfo>().canUse)
        {
            // Change the status indicator color
            pickingItem.GetComponent<ItemInfo>().ChangeIndicatorColor(pickingItem.GetComponent<ItemInfo>().isUsingStatusColor, 1);
        }
        // Add a fixed joint to the holding item to attach it to the armTip
        //pickingItem.gameObject.AddComponent<FixedJoint>();
        //pickingItem.GetComponent<FixedJoint>().connectedBody = armTip.GetComponent<Rigidbody>();
        //pickingItem.GetComponent<FixedJoint>().autoConfigureConnectedAnchor = true;
        //pickingItem.GetComponent<FixedJoint>().breakForce = armHoldingJointBreakForce;

        // Using fixed joint
        //armTip.gameObject.AddComponent<FixedJoint>();
        //armTip.GetComponent<FixedJoint>().connectedBody = pickingItem.GetComponent<Rigidbody>();
        //armTip.GetComponent<FixedJoint>().breakForce = armHoldingJointBreakForce;

        pickingItem.GetComponent<ItemInfo>().isBeingHeld = true;
        pickingItem.GetComponent<ItemInfo>().holdingArmTip = armTip;

        // If the ItemInfo component is enabled on the item to let it be setup when picked
        if (pickingItem.GetComponent<ItemInfo>().enabled)
        {
            armTip.GetComponent<ArmUseItem>().SetUpItem(); // Invoke the setup item event
        }
    }

    /// <summary>
    /// Start drop down the item
    /// </summary>
    /// <param name="droppingItem"></param>
    /// <returns></returns>
    public void DropDownItem(GameObject droppingItem)
    {
        // Remove the start and stop using item function from the UnityEvents
        //armTip.GetComponent<ArmUseItem>().useItem.RemoveAllListeners();
        //armTip.GetComponent<ArmUseItem>().stopUsingItem.RemoveAllListeners();
        //UnityAction startUsingAction = System.Delegate.CreateDelegate(typeof(UnityAction), droppingItem.GetComponent<ItemInfo>(), "StartUsing") as UnityAction;
        //UnityEventTools.RemovePersistentListener(armTip.GetComponent<ArmUseItem>().useItem, startUsingAction);
        //UnityAction stopUsingAction = System.Delegate.CreateDelegate(typeof(UnityAction), droppingItem.GetComponent<ItemInfo>(), "StopUsing") as UnityAction;
        //UnityEventTools.RemovePersistentListener(armTip.GetComponent<ArmUseItem>().stopUsingItem, stopUsingAction);

        // Enable the gravity on the rigidbody of the dropping item if it normally has gravity
        if (!droppingItem.GetComponent<ItemInfo>().fixedPosition)
        {
            if (droppingItem.GetComponent<ItemInfo>().isUsingGravity)
            {
                droppingItem.GetComponent<Rigidbody>().useGravity = true;
            }

            // Rrestore the drag, angular drag, and mass of the dropping item
            //if (droppingItem.GetComponent<ItemInfo>().canUse)
            //{
            droppingItem.GetComponent<Rigidbody>().drag = droppingItem.GetComponent<ItemInfo>().normalDrag;
            droppingItem.GetComponent<Rigidbody>().angularDrag = droppingItem.GetComponent<ItemInfo>().normalAngularDrag;
            droppingItem.GetComponent<Rigidbody>().mass = droppingItem.GetComponent<ItemInfo>().normalMass;
            //// Turn on the collider
            //TurnOnColliders(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem);
            // Change the item to not kinematic
            //droppingItem.GetComponent<Rigidbody>().isKinematic = false;
            //}
            //else
            //{
        }

        armTip.GetComponent<ArmUseItem>().resetItemDelegate.Invoke(); // Reset the item when drop it
        armTip.GetComponent<ArmUseItem>().setupItemDelegate = null;
        armTip.GetComponent<ArmUseItem>().useItemDelegate = null;
        armTip.GetComponent<ArmUseItem>().stopUsingItemDelegate = null;
        armTip.GetComponent<ArmUseItem>().resetItemDelegate = null;

        // Destroy the fixed joint in the item that's currently holding by the armTip
        if (armTip.GetComponent<FixedJoint>())
        {
            //Destroy(droppingItem.GetComponent<FixedJoint>());
            Destroy(armTip.GetComponent<FixedJoint>());
        }
        //}

        // Destroy the spring joint in the item that's currently holding by the armTip
        if (armTip.GetComponent<SpringJoint>())
        {
            Destroy(armTip.GetComponent<SpringJoint>());
        }

        droppingItem.GetComponent<ItemInfo>().isBeingHeld = false;
        droppingItem.GetComponent<ItemInfo>().holdingArmTip = null;
        // Remove the dropping item from armTip's currentHoldingItem
        armTip.GetComponent<ArmUseItem>().currentlyHoldingItem = null;

        // If the item cannot be moved
        if (droppingItem.GetComponent<ItemInfo>().fixedPosition && droppingItem.GetComponent<ItemInfo>().canUse)
        {
            // Change the status indicator color
            droppingItem.GetComponent<ItemInfo>().ChangeIndicatorColor(droppingItem.GetComponent<ItemInfo>().defaultStatusColor, 1);
        }
    }


    /// <summary>
    /// Turn off colliders in a gameobject
    /// </summary>
    /// <param name="g"></param>
    public void TurnOffColliders(GameObject g)
    {
        foreach (Collider c in g.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }
    }

    /// <summary>
    /// Turn off colliders in a gameobject
    /// </summary>
    /// <param name="g"></param>
    public void TurnOnColliders(GameObject g)
    {
        foreach (Collider c in g.GetComponentsInChildren<Collider>())
        {
            c.enabled = true;
        }
    }

    /// <summary>
    /// Test the controller's input
    /// </summary>
    public void TestControllerInput()
    {
        //print("HorizontalLeft: " + Input.GetAxis("HorizontalLeft"));
        //print("VerticalLeft: " + Input.GetAxis("VerticalLeft"));
        //print("HorizontalRight: " + Input.GetAxis("HorizontalRight"));
        //print("VerticalRight: " + Input.GetAxis("VerticalRight"));

        //float rotation = Mathf.Atan(Input.GetAxis("HorizontalLeft") / Mathf.Abs(Input.GetAxis("VerticalLeft"))) * Mathf.Rad2Deg;
        //if (Input.GetAxis("VerticalLeft") < 0)
        //{
        //    rotation = Mathf.Sign(Input.GetAxis("HorizontalLeft")) * (180 - Mathf.Abs(rotation));
        //}
        //print(rotation);
        //print(Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalLeft"), 2) + Mathf.Pow(Input.GetAxis("VerticalLeft"), 2)));

        //print(Input.GetKey(KeyCode.JoystickButton4)); // LB
        //print(Input.GetKey(KeyCode.JoystickButton5)); // RB
        print("Left: " + Input.GetAxis("LeftTrigger"));
        print("Right: " + Input.GetAxis("RightTrigger"));
    }

    /// <summary>
    /// Calculate the euler angles of the arm relative to the world
    /// </summary>
    /// <param name="left"></param>
    public void CalculateJoyStickRotation(bool left)
    {
        // Calculate the euler angles of the arm relative to the world
        if (left)
        {
            joyStickRotationAngle = Mathf.Atan(Input.GetAxis("HorizontalLeft") / Mathf.Abs(Input.GetAxis("VerticalLeft"))) * Mathf.Rad2Deg;
            if (Input.GetAxis("VerticalLeft") < 0)
            {
                joyStickRotationAngle = Mathf.Sign(Input.GetAxis("HorizontalLeft")) * (180 - Mathf.Abs(joyStickRotationAngle));
            }
        }
        else
        {
            joyStickRotationAngle = Mathf.Atan(Input.GetAxis("HorizontalRight") / Mathf.Abs(Input.GetAxis("VerticalRight"))) * Mathf.Rad2Deg;
            if (Input.GetAxis("VerticalRight") < 0)
            {
                joyStickRotationAngle = Mathf.Sign(Input.GetAxis("HorizontalRight")) * (180 - Mathf.Abs(joyStickRotationAngle));
            }
        }
    }

    /// <summary>
    /// Converts the joystick angle from -180 to 180 to 0 to 360
    /// </summary>
    public void ConvertJoystickRotationAngleTo360()
    {
        joyStickRotationAngle360 = joyStickRotationAngle;

        // Convert -180 to 0 into 180 to 360
        if (joyStickRotationAngle < 0)
        {
            joyStickRotationAngle360 += 360;
        }

        joyStickRotationAngle360 = 360 - joyStickRotationAngle360; // Convert it to increasing counter-clockwise
    }

    /// <summary>
    /// Calculate how far the joy stick is away from the center
    /// </summary>
    /// <param name="left"></param>
    public void CalculateJoyStickLength(bool left)
    {
        // Calculate how far the joy stick is away from the center
        if (left)
        {
            joyStickLength = Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalLeft"), 2) + Mathf.Pow(Input.GetAxis("VerticalLeft"), 2)));
        }
        else
        {
            joyStickLength = Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(Input.GetAxis("HorizontalRight"), 2) + Mathf.Pow(Input.GetAxis("VerticalRight"), 2)));
        }
    }

    /// <summary>
    /// Get the 2D arm-body joint's local rotation relative to the 2D body
    /// </summary>
    public void GetArmBodyJointLocalRotation()
    {
        armBodyJointLocalRotation = armBodyJoint2D.transform.localEulerAngles.z; // Get the joint local transform angle
        armBodyHingeJointAngle = armBodyHingeJoint2D.jointAngle; // Get the current hinge joint angle
    }

    /// <summary>
    /// Calculate the arm-body hinge joint's target motor speed based on the joystick input and the joint's local rotation relative to the body
    /// </summary>
    public void CalculateSignedArmBodyHingeJointMotorSpeed()
    {
        // Get the target local z rotation for the arm-body joint
        // If the body's rotation is within the range of the target joint world rotation
        if (joyStickRotationAngle360 >= bodyRotation)
        {
            targetArmBodyJointLocalRotation = joyStickRotationAngle360 - bodyRotation;
        }
        else
        {
            targetArmBodyJointLocalRotation = 360 - (bodyRotation - joyStickRotationAngle360);
        }

        //// Update the back up hinge joint
        //UpdateBackUpHingeJoint2D();

        // Update target hinge joint motor speed based on the angle difference
        CalculateUnsignedArmBodyHingeJointMotorSpeed();

        // Get the direction of the motor
        targetArmBodyHingeJointMotor.motorSpeed =
            unsignedTargetArmBodyHingeJointMotorSpeed * Mathf.Sign(armBodyJointLocalRotation - targetArmBodyJointLocalRotation);

        // If the targeting angle is greater than the current angle (rotate counter-clockwise) but the joint can rotate in the other direction for a shorter travel distance
        // or the targeting angle is lesser than the current angle (rotate clockwise) but the joint can rotate in the other direction for a shorter travel distance
        if (targetArmBodyJointLocalRotation > armBodyJointLocalRotation + 180 || targetArmBodyJointLocalRotation < armBodyJointLocalRotation - 180)
        {
            targetArmBodyHingeJointMotor.motorSpeed *= -1;
        }

        //// If the targeting angle is lesser than the current angle (rotate clockwise) but the joint can rotate in the other direction for a shorter travel distance
        //if (targetArmBodyJointLocalRotation < armBodyJointLocalRotation - 180)
        //{
        //    targetArmBodyJointLocalRotation = 360 + targetArmBodyJointLocalRotation;
        //}

        // Get the target hinge angle limit
        //if (targetArmBodyJointLocalRotation != 0)
        //{
        //    targetArmBodyJointHingeAngle = 360 - targetArmBodyJointLocalRotation;
        //}
        //else
        //{
        //    targetArmBodyJointHingeAngle = 0;
        //}
    }

    /// <summary>
    /// Get the unsigned motor speed based on the angle difference between the current arm-body joint local angle and the target local angle
    /// </summary>
    public void CalculateUnsignedArmBodyHingeJointMotorSpeed()
    {
        float cycleDifference = Mathf.Abs(armBodyJointLocalRotation - targetArmBodyJointLocalRotation);

        // Motor function here, speed should be smaller as the difference get smaller
        if (cycleDifference <= 180)
        {
            unsignedTargetArmBodyHingeJointMotorSpeed =
                CalculateJointMotorSpeed(armBodyHingeJointMaxMotorSpeed, 180, cycleDifference);
        }
        else
        {
            unsignedTargetArmBodyHingeJointMotorSpeed =
                CalculateJointMotorSpeed(armBodyHingeJointMaxMotorSpeed, 180, 360 - cycleDifference);
        }


        //if (isLeftArm && test)
        //print("hinge: " + unsignedTargetArmBodyHingeJointMotorSpeed);
    }

    ///// <summary>
    ///// If the hinge joint rotates below 0 or above 360, then recreate the joint with an angle within 0 - 360
    ///// </summary>
    //public void ClampArmBodyJointRotationAngle()
    //{
    //    //// Re-enable the hinge joint 2D angle limits if it was disabled because of the clamping
    //    //if (!armBodyHingeJoint2D.useLimits)
    //    //{
    //    //    //armBodyHingeJoint2D.useLimits = true;
    //    //}

    //    armBodyHingeJointAngle = armBodyHingeJoint2D.jointAngle; // Get the current joint angle
    //    //return;

    //    // If the joint is rotating from 1 to -1 degrees
    //    if (armBodyHingeJoint2D.jointAngle < 0)
    //    {
    //        ChangeArmBodyHingeJoint2D(false);
    //        //ChangeArmBodyHingeJoint2D(true);
    //    }
    //    // If the joint is rotating from 359 to 361 degrees
    //    else if (armBodyHingeJoint2D.jointAngle > 360)
    //    {
    //        ChangeArmBodyHingeJoint2D(true);
    //    }
    //}

    ///// <summary>
    ///// Replace a hinge joint 2D to a new one
    ///// </summary>
    //public void ChangeArmBodyHingeJoint2D(bool clockwise)
    //{
    //    print("start create joint");

    //    Quaternion lastArmBodyJointLocalRotation = armBodyJoint2D.transform.localRotation; // Get the 2D arm-body joint's local rotation
    //    armBodyJoint2D.transform.localEulerAngles = Vector3.zero; // Temporarely reset the 2D arm-body joint's local rotation to 0 when creating the new joint

    //    if (clockwise)
    //    {
    //        HingeJoint2D newJoint = armBodyJoint2D.AddComponent<HingeJoint2D>(); // Create new joint

    //        // Copying joint parameters
    //        newJoint.autoConfigureConnectedAnchor = false;
    //        newJoint.connectedBody = armBodyHingeJoint2D.connectedBody;
    //        newJoint.anchor = armBodyHingeJoint2D.anchor;
    //        newJoint.connectedAnchor = armBodyHingeJoint2D.connectedAnchor;
    //        newJoint.useLimits = true;
    //        newJoint.useMotor = false;
    //        newJoint.limits = armBodyHingeJoint2D.limits;

    //        armBodyJoint2D.transform.localRotation = lastArmBodyJointLocalRotation; // Restore the 2D arm-body's last local rotation
    //        Destroy(armBodyHingeJoint2D); // Destroy the old joint
    //        armBodyHingeJoint2D = newJoint; // Update reference
    //    }
    //    else
    //    {
    //        //JointAngleLimits2D newLimits = armBodyHingeJoint2D.limits;
    //        //newLimits.min = armBodyHingeJoint2D.limits.min;
    //        //newLimits.max = armBodyHingeJoint2D.limits.max;
    //        //newJoint.limits = newLimits;
    //        backUpArmBodyHingeJoint2D.connectedBody = armBodyHingeJoint2D.connectedBody;
    //        backUpArmBodyHingeJoint2D.useLimits = true;
    //        backUpArmBodyHingeJoint2D.useMotor = false;
    //        backUpArmBodyHingeJoint2D.limits = armBodyHingeJoint2D.limits;

    //        armBodyJoint2D.transform.localRotation = lastArmBodyJointLocalRotation; // Restore the 2D arm-body's last local rotation
    //        Destroy(armBodyHingeJoint2D); // Destroy the old joint
    //        armBodyHingeJoint2D = backUpArmBodyHingeJoint2D; // Update reference
    //        //CreateBackUpHingeJoint2D();
    //    }

    //    print("finish create joint, " + armBodyHingeJoint2D.jointAngle);
    //}

    ///// <summary>
    ///// Create a "back up" joint in case the current joint needs to be replaced when rotate from 0 to -1
    ///// </summary>
    //public void CreateBackUpHingeJoint2D()
    //{
    //    backUpArmBodyHingeJoint2D = armBodyJoint2D.AddComponent<HingeJoint2D>();
    //    backUpArmBodyHingeJoint2D.autoConfigureConnectedAnchor = false;
    //    backUpArmBodyHingeJoint2D.connectedBody = null;
    //    backUpArmBodyHingeJoint2D.anchor = armBodyHingeJoint2D.anchor;
    //    backUpArmBodyHingeJoint2D.connectedAnchor = armBodyHingeJoint2D.connectedAnchor;
    //    backUpArmBodyHingeJoint2D.useLimits = false;
    //    backUpArmBodyHingeJoint2D.useMotor = false;
    //}

    ///// <summary>
    ///// Constantly update the back up hinge joint 2D to prepare to replace the current one
    ///// </summary>
    //public void UpdateBackUpHingeJoint2D()
    //{
    //    // Update the joint limit
    //    JointAngleLimits2D newLimit = new JointAngleLimits2D();
    //    newLimit.min = targetArmBodyJointLocalRotation;
    //    newLimit.max = targetArmBodyJointLocalRotation;
    //    backUpArmBodyHingeJoint2D.limits = newLimit;

    //    // Turn off the joint's limit
    //    backUpArmBodyHingeJoint2D.useLimits = false;
    //}

    /// <summary>
    /// Controls the 2D arm-body hinge joint's rotation
    /// </summary>
    public void RotateArmBodyJoint()
    {
        //// Set hinge joint 2D angle limits
        //targetArmBodyJointHingeAngleLimit.min = targetArmBodyJointHingeAngle;
        //targetArmBodyJointHingeAngleLimit.max = targetArmBodyJointHingeAngle;
        //armBodyJoint2D.GetComponent<HingeJoint2D>().limits = targetArmBodyJointHingeAngleLimit;

        // Update the hinge joint's motor to rotate the 2D arm-body joint
        armBodyHingeJoint2D.motor = targetArmBodyHingeJointMotor;

        // Change the 2D arm-body joint's transform
        transform.localEulerAngles = new Vector3(0, 0, targetArmBodyJointLocalRotation);
    }

    /// <summary>
    /// Stop the 2D arm-body joint from moving with the 2D armTip
    /// </summary>
    public void LockArmBodyJoint()
    {
        // Stop the 2D arm-body joint
        armBodyJoint2D.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        armBodyJoint2D.transform.localPosition = armBodyHingeJoint2D.connectedAnchor;
    }

    /// <summary>
    /// Calculate the armTip slider joint's target motor speed based on the joystick input and the joint's local distance relative to the body
    /// </summary>
    public void CalculateSignedArmTipSliderJointMotorSpeed()
    {
        // Get the normalized local distance from the 2D armTip to the 2D arm-body joint
        armTipNormalizedDistance = Vector3.Magnitude(armTip2D.transform.localPosition) / armMaxLength;

        // Update target slider joint motor speed based on the local distance from the 2D armTip to the 2D arm-body joint
        CalculateUnsignedArmTipSliderJointMotorSpeed();

        // If the armTip is closer to the body than the joystick length
        if (armTipNormalizedDistance < joyStickLength)
        {
            targetArmTipSliderJointMotor.motorSpeed = unsignedTargetArmTipSliderJointMotorSpeed;
        }
        else if (armTipNormalizedDistance > joyStickLength)
        {
            targetArmTipSliderJointMotor.motorSpeed = -unsignedTargetArmTipSliderJointMotorSpeed;
        }
    }

    /// <summary>
    /// Get the unsigned motor speed based on the local distance from the 2D armTip to the 2D arm-body joint
    /// </summary>
    public void CalculateUnsignedArmTipSliderJointMotorSpeed()
    {
        // Motor function here, speed should be smaller as the distance get shorter
        unsignedTargetArmTipSliderJointMotorSpeed =
            CalculateJointMotorSpeed(armTipSliderJointMaxMotorSpeed, 1, Mathf.Abs(armTipNormalizedDistance - joyStickLength));

        if (isLeftArm && test)
            print("slider: " + unsignedTargetArmTipSliderJointMotorSpeed);
    }

    /// <summary>
    /// Move the 2D armTip by update the 2D slider joint's limits
    /// </summary>
    public void MoveArmTip2D()
    {
        //// Update the limits based on the joystick length
        //targetArmTipSliderJointTranslationLimits.min = joyStickLength * armMaxLength;
        //targetArmTipSliderJointTranslationLimits.max = joyStickLength * armMaxLength;
        //armTipSliderJoint2D.limits = targetArmTipSliderJointTranslationLimits;

        // Update the slider joint's motor to move the 2D armTip
        armTipSliderJoint2D.motor = targetArmTipSliderJointMotor;
    }

    /// <summary>
    /// Display the clampped joystick input
    /// </summary>
    public void DisplayJoystickPosition()
    {
        realClamppedJoystickInput.position = transform.position + (joystickPosition * armMaxLength); // Get the joystick position based on the clampped joystick input
    }

    /// <summary>
    /// Adjust the arm's transform according to the position of the arm-body joint and the armTip
    /// </summary>
    public void UpdateArmTransform()
    {
        arm.localPosition = armTip.localPosition / 2f; // Put the center of the arm in the middle between the armTip and the body center
        //arm.localScale = new Vector3(1, 1, armTip.localPosition.z / 2f); // Extend the arm towards the armTip
        arm.localScale = new Vector3(0.1f, armTip.localPosition.z / 2f, 0.1f); // Extend the arm towards the armTip
        //armTip.localPosition = Vector3.zero;
        //armTip.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // Keep the armTip's angular velocity always (0, 0, 0)
        armTip.localEulerAngles = Vector3.zero; // Keep the armTip's local euler angles always (0, 0, 0)
    }


    ///// <summary>
    ///// Rotate the arm according to the joystick's rotation
    ///// </summary>
    ///// <param name="left"></param>
    //public override void RotateArm(bool left)
    //{
    //    transform.eulerAngles = new Vector3(0, joyStickRotationAngle, 0);
    //}

    // Calculate the direction and the amount of the force that should be applied
    public Vector3 CalculateArmForce(Vector3 currentPosition, float carryingWeight)
    {
        Vector3 targetPosition;


        // If the arm is not grabbing floor
        //if (!isGrabbingFloor)
        {
            targetPosition = body.position + (joystickPosition * armMaxLength);
        }


        float targetDistance = Vector3.Magnitude(targetPosition - currentPosition);

        if (targetDistance <= armStopThreshold) // If the current position is close to the target position, then only apply a small force
        {
            //if (isGrabbingFloor)
            //{
            //    print("body reach");
            //}
            //if (armTip.GetComponent<Rigidbody>().velocity.magnitude * Time.smoothDeltaTime > Vector3.Distance(targetPosition, currentPosition)) // Slow the armTip down if it will shoot over target position
            //{
            //    return -armTip.GetComponent<Rigidbody>().velocity * armTip.GetComponent<Rigidbody>().mass;
            //}
            //else
            {
                return Vector3.Normalize(targetPosition - currentPosition) * armDefaultStretchForce * targetDistance;
            }
        }
        else
        {

            {
                return Vector3.Normalize(targetPosition - currentPosition) * armDefaultStretchForce * targetDistance;
            }
        }
    }

    /// <summary>
    /// Moves the arm tip
    /// </summary>
    /// <param name="left"></param>
    public void MoveArm(bool left)
    {
        //Vector3 armTipAppliedForce = CalculateArmForce(armTip.transform.position, armTip.GetComponent<Rigidbody>().mass);

        //armTip.GetComponent<Rigidbody>().AddForce(armTipAppliedForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Using the custom motor speed function to get the target motor speed based on the maximum motor speed, 
    /// the current and maximum possible difference between the current and the target joint position
    /// </summary>
    /// <param name="maxSpeed"></param>
    /// <param name="maxDifference"></param>
    /// <param name="currentDifference"></param>
    /// <returns></returns>
    public float CalculateJointMotorSpeed(float maxSpeed, float maxDifference, float currentDifference)
    {
        // Get the normalized difference between the current joint position and the target joint position
        float normalizedDifference = currentDifference / maxDifference;

        // Get the normalized motor speed based on the motor function and the parameters b,c,d
        float normalizedSpeed = Mathf.Log((normalizedDifference / functionParamC * functionParamD) + 1, functionParamB) / functionParamD;

        // Return the target motor speed
        return normalizedSpeed * maxSpeed;
    }

    /// <summary>
    /// Get the 2D body's z rotation
    /// </summary>
    public void GetBodyRotation()
    {
        bodyRotation = body2D.transform.eulerAngles.z;
    }

    /// <summary>
    /// Change the control to land-based
    /// </summary>
    public void SwitchToLandControl()
    {
        foreach (ArmControlModeInfo a in armControlModeInfos)
        {
            if (a.controlModeName == "Land")
            {
                ChangeArmControlParameters(a);
            }
        }
    }

    /// <summary>
    /// Change the control to water-based
    /// </summary>
    public void SwitchToWaterControl()
    {
        foreach (ArmControlModeInfo a in armControlModeInfos)
        {
            if (a.controlModeName == "Water")
            {
                ChangeArmControlParameters(a);
            }
        }
    }

    /// <summary>
    /// Change the control to air-based
    /// </summary>
    public void SwitchToAirControl()
    {
        foreach (ArmControlModeInfo a in armControlModeInfos)
        {
            if (a.controlModeName == "Air")
            {
                ChangeArmControlParameters(a);
            }
        }
    }

    /// <summary>
    /// Change different physics parameters and component variebles for an arm control mode
    /// </summary>
    /// <param name="controlParameters"></param>
    public void ChangeArmControlParameters(ArmControlModeInfo controlParameters)
    {
        // Update armTip's rigidbody constraints
        armTip.GetComponent<Rigidbody>().constraints =
            controlParameters.armTipRigidBodyConstraint |
            RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        // Update the first arm segment
        jointConnectingBody.GetComponent<Rigidbody>().drag = controlParameters.firstArmSegmentRigidbodyDrag;
        jointConnectingBody.massScale = controlParameters.firstArmSegmentJointMassScale;
        jointConnectingBody.connectedMassScale = controlParameters.firstArmSegmentJointConnectedMassScale;
        // Update the last arm segment
        lastArmSegment.GetComponent<SpringJoint>().connectedMassScale = controlParameters.lastArmSegmentArmTipJointConnectedMassScale;
        //lastArmSegment.GetComponent<ArmUpdatePhysicsVariebles>().armTipJointMaximumConnectedMassScale = controlParameters.lastArmSegmentMaximumConnectedMassScale;
    }
}

///// <summary>
///// Stores informations for an echo that collided on something
///// </summary>
//[Serializable]
//public class EchoCollisionInfo
//{
//    public float timeThisEchoWasCreated; // When did the player created this echo
//    public float echoDuration; // How long this echo lasted
//    public Vector3 echoCollidingPosition; // Where does this echo collides
//    public bool activated; // Has this echo "travelled" to the colliding point thus being "activated" 
//                           // (An echo will only start to check the player's position if it's being activated)
//}

/// <summary>
/// Stores informations of an arm control mode, this include but not limit to:
/// Rigidbody parameters,
/// Arm segments joint parameters
/// </summary>
[Serializable]
public class ArmControlModeInfo
{
    public string controlModeName; // The name for the control mode
    public RigidbodyConstraints armTipRigidBodyConstraint; // The armTip's rigidbody's constriants
    // The ArmMinConnectedAnchor value on the ArmUpdatePhysicsVariebles component for the arm segments between the first and the last segment
    public float middleJointsMinConnectedAnchor;

    // First arm segment variebles
    public float firstArmSegmentRigidbodyDrag; // The drag on the rigidbody on the first arm segment
    public float firstArmSegmentJointMassScale;
    public float firstArmSegmentJointConnectedMassScale;
    public bool firstArmSegmentUpdateJointMassScale; // The UpdateJointMassScale boolean on the ArmUpdatePhysicsVariebles on the first segment
    public float firstArmSegmentDefaultMassScale;
    public float firstArmSegmentMaximumMassScale;
    public bool firstArmSegmentUpdateAnchor;
    public float firstArmSegmentMinAnchor;
    public float firstArmSegmentMaxJointChangingSpeed;

    // Last arm segment variebles
    public float lastArmSegmentArmTipJointConnectedMassScale;
    public float lastArmSegmentDefaultConnectedMassScale;
    //public float lastArmSegmentMaximumConnectedMassScale;


}
