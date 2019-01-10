using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the basic movement of the player arms
/// </summary>
public class ControlArm_UsingPhysics : ControlArm
{
    //public bool isLeftArm; // Is this the left arm
    //public Transform armTip; // The tip of the arm
    //public Transform arm; // The actual arm that extends from the center of the body to the arm tip
    //public float armMaxLength; // How long is the maximum arm length
    //public Transform bodyRotatingCenter; // What is the center that the body is rotating around
    //public Transform body; // The main body
    //public DetectCollision bodyWallDetector; // The collider on the body that detects the walls
    //public LayerMask armCollidingLayer; // The object layers that the arm can collide with
    //public LayerMask bodyCollidingLayer; // The object layers that the body can collide with
    public ControlArm_UsingPhysics otherArm_Physics; // The other arm
    //public float triggerThreshold; // How much the trigger has to be pressed down to register
    //public float armLiftingStrength; // How much weight the arm can lift? (The currently holding item's weight)
    //public float armHoldingJointBreakForce; // How much force the fixed joint between the armTip and the currently holding item can bearing before break
    //public float collisionRaycastOriginSetBackDistance; // How far should the raycast's origin move back from the pivot center when detecting collision of armTip/body
    public float armDefaultStretchForce; // How much force should be applied for the armTip to stretch and retract
    public float armStopThreshold; // How close the armTip has to be to the target position for it to stop being pushed
    public float armMaximumStamina; // How much total stamina each arm has
    public float armStaminaDefaultRecoverSpeed; // The default speed each arm will recharge its stamina
    public float armStaminaConsumptionRate; // How much stamina the arm will consume per sec while it is moving the body
    public float maxStaminaInitialBurstMulti; // How much times of the default force the arm can apply when it just start moving while on maximum stamina
    //public Transform armTipStretchLimiter; // The inverted sphere collider that limits how far the armTips can be away from the body
    public float startSwimMinVelocityThershold; // The minimum velocity of the body for it to be considered start "swimming" in the water
    public float stopSwimMinVelocityThershold; // The minimum velocity of the body for it to be considered stop "swimming" in the water
    public float joystickLengthThresholdForAngleBetweenTwoArms; // How long the joystick has to extend to be considered stretched out to calculate the angle
    //public float armDefaultDragInWater; // The default drag of the arm when it is in the water
    //public float armDefaultAngularDragInWater; // The default angular drag of the arm when it is in the water
    //public float armMinSwimmingSpreadingAngle;
    public float armAirDragMultiplier; // The strength multiplier of the arm's drag when the player is in the air
    public float joyStickMoveSpeedControlBurstForceThreshold; // How fast the joystick has to be moved for the arm to try to use burst force
    // How much time is given the player to use the burst force when they started grabbing on full stamina
    public float timeAllowedToUseBurstForceSinceMaxStamina;
    public GameObject echoProjectile; // The echo projectile created by the player's armTip
    public float maxEchoTravelDistance; // How far the viberation echo can travel
    public float echoTravelSpeed; // How fast the echo travels
    public float echoProjectileWidth; // How wide is the echo projectile
    public SpringJoint jointConnectingBody; // The SpringJoint in the first arm segment that is connected to the body
    public Transform lastArmSegment; // The arm segment that connects the armTip
    public ArmControlModeInfo[] armControlModeInfos; // Infos of different arm control modes
    // The controllers that update the physics variebles for arm segments between the first and the last
    public AudioClip armTipGrabbingGroundSFX; // The sfx for armTip grabbing onto ground
    public AudioClip armTipStopGrabbingGroundSFX; // The sfx for armTip grabbing move away from ground
    public float startAdjustingSoftBodyDistanceFromBody; // How far the soft body is away should the last arm segment start adjusting joint parameters
    public float softBodyLeavingDistanceFromBody; // How far the soft body is away is considered "leaving" the body
    // The multiplier for adjusting connected mass scale on the spring joint of the last arm segment that connects to the armTip
    public float lastArmSegmentAdjustConnectedMassScaleMultiplier;
    // The maximum changing rate of the connected mass scale on the spring joint of the last arm segment that connects to the armTip when the soft body is returning
    public float maxLastArmSegmentConnectedMassScaleChangeRate;

    /// <summary>
    /// Arm flags
    /// </summary>
    public bool canGrabObject; // Can the armTip grab object or not
    //public bool isTouchingGround; // Is the arm touching ground or not
    //public bool isSideScroller; // Is the camera in side-scroller mode or not
    public bool isUsingMechanicalArm; // Is this arm currently operating a mechanical extending arm or not
    public bool inWater; // Is this armTip currently in water
    public bool onLand; // Is this armTip currently touching ground
    public bool inAir; // Is this armTip currently in the air

    //public bool isGrabbingFloor; // If the armTip is grabbing floor
    //public float joyStickRotationAngle; // The rotation of the arm
    //public float joyStickLength; // How much the joystick is pushed away
    public Vector3 joystickPosition; // Where is the joystick
    public Vector3 joystickLastPosition; // The joystick's position in the last frame
    public float armCurrentStamina; // This arm's current stamina amount
    public Vector3 armTipGrabbingPosition; // The armTip's position when it starts grabbing
    public float armPhysicsMagnitude; // The magnitude to be applied for different arm physics parameters
    public bool startedSwimming; // Does the player started swimming
    public bool isArmsSpreading; // Is the two arms spreading away from each other? (Is the angle between two arms increasing?)
    public bool isArmsClosing; // Is the two arms closing together? (Is the angle between two arms decreasing?)
    public float angleBetweenTwoArms; // The angle between the two arms
    public float currentLargestSpreadingAngle; // The largest spreading angle before the arm begin to closing
    public bool isArmSwimming; // Is the arms doing "swimming" movement
    public float lastArmSwimmingTime; // The last time the arm is doing the "swimming" movement
    public float horizontalCameraAngle; // The camera's horizontal when it is in "side-scrolling" mode
    public float currentArmTipTouchingWindStrength; // How much wind strength is the windy area the armTip currently in
    public Vector3 armCurrentTotalReceivedWindForce; // What's the total wind force currently added to the arm
    public float armStartGrabbingTime; // The time the last time this arm started to grab the floor or an object
    public bool isArmBurstForceUsed; // Did the arm already used burst force in this interaction cycle?
    //public EchoCollisionInfo currentCreatingEcho; // The vibration echo that's currently being created (if the player continuously generate it)
    //public List<VibrationEchoBehavior> currentExistingEchoProjectiles; // The list contains all the "living" echo projectiles
    public float firstSegmentDistanceFromBody; // How far is the first arm segments away from the body
    public float firstSegmentDistanceFromSoftBody; // How far is the first arm segments away from the body
    // The default connected mass scale on the spring joint of the last arm segment that connects to the armTip
    public float lastArmSegmentDefaultConnectedMassScale;
    public bool isSoftBodyReturning; // Is the soft body return to the core after it is stretched too far
    // The connected mass scale on the spring joint of the last arm segment that connects to the armTip in the last frame
    public float lastArmSegmentLastConnectedMassScale;

    //Test//
    public bool test; // Do we print test outputs
    public float bodySpeed; // The speed of the body
    public float bodyTopSpeed; // The top speed the body can reach when swimming
    public Vector3 testWindDirection; //
    public float testWindStrength; //
    public Vector3 testArmGlidingForce;

    // Use this for initialization
    void Start()
    {
        armCurrentStamina = armMaximumStamina;
        lastArmSegmentDefaultConnectedMassScale = lastArmSegment.GetComponents<SpringJoint>()[1].connectedMassScale;

        // Set up the flags
        canGrabObject = true;

        // Set up the arm stretch limiter
        if (isLeftArm)
        {
            //armTipStretchLimiter.localScale = armTipStretchLimiter.localScale * armMaxLength * 2;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        firstSegmentDistanceFromBody = Vector3.Distance(jointConnectingBody.transform.position, body.position);
        DetectingArmMovement();

        // Don't let the player control the character if the game is in a scripted event or is paused
        if (GameManager.inScriptedEvent || GameManager.gamePause)
        {
            // Stop the player from grabbing the floor if the armTip is grabbing floor when the event start
            if (isGrabbingFloor)
            {
                StopGrabbingFloor();
            }

            // Make sure the armTip doesn't "roll away"
            if (!armTip.GetComponent<Rigidbody>().isKinematic)
            {
                armTip.GetComponent<Rigidbody>().isKinematic = true;
            }
            return;
        }
        else
        {
            if (armTip.GetComponent<Rigidbody>().isKinematic)
            {
                armTip.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        // Collect player joystick input
        CalculateJoyStickRotation(isLeftArm);
        CalculateJoyStickLength(isLeftArm);
        GetClampedJoystickPosition();

        // Control mode changing detection
        DetectIfSwitchEchoMode();

        if (!isGrabbingFloor)
        {
            if (PlayerInfo.isSideScroller) // If the game is in side-scroller mode then calculate the current game camera forward direction
            {
                CalculateHorizontalCameraAngle();
            }

            if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null) // If the arm is not holding item
            {
                //RotateArm(isLeftArm);
                //StretchArm(isLeftArm);
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
                        //RotateArm(isLeftArm);
                        //StretchArm(isLeftArm);
                        MoveArm(isLeftArm);
                    }
                    else // If the item is heavy
                    {
                        MoveHeavyItem(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem);
                    }

                    DetectIfDropHeavyItem();
                }
                else
                {

                }
            }
        }
        //else
        //{
        //    armTip.position = bodyRotatingCenter.position;
        //}

        if (onLand) // If the arm is touching ground
        {
            DetectGrabbingFloorInput();
        }

        if (isGrabbingFloor)
        {
            // Stop the player from "grabbing the floor" if this armTip is no longer on land
            if (!onLand)
            {
                StopGrabbingFloor();
            }
            //armTip.position = bodyRotatingCenter.position;
            //RotateBody();
            MoveBody();
            if (armCurrentStamina > 0)
            {
                //armTip.position = armTipGrabbingPosition;
                armTip.GetComponent<Rigidbody>().AddForce(CalculateArmForce(false, armTip.position, armTip.GetComponent<Rigidbody>().mass), ForceMode.Impulse);
            }
        }

        //UpdateArmTransform();
        UpdateArmStamina();

        if (inWater)
        {
            UpdateSwimmingStatus();
            CalculateArmPhysicsMagnitudeInWater();
        }

        //// Apply arm drag when the body is flying in the air
        //if (PlayerInfo.sPlayerInfo.inAir)
        //{
        //    //ApplyGlidingForceToBody();
        //    ApplyArmAirDrag();
        //}

        // Test
        if (test)
        {
            //TestControllerInput();
            //bodySpeed = body.GetComponent<Rigidbody>().velocity.magnitude;
            //testArmGlidingForce = CalculateArmGlidingForce(testWindStrength, testWindDirection);
        }
    }

    private void LateUpdate()
    {
        armCurrentTotalReceivedWindForce = Vector3.zero;
    }

    private void FixedUpdate()
    {

    }

    /// <summary>
    /// Get the joystick's clamped (from 0 to 1) position
    /// </summary>
    public void GetClampedJoystickPosition()
    {
        joystickLastPosition = joystickPosition; // Assign the position in the last frame
        // Calculate the new position in this frame
        joystickPosition = new Vector3(Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad), 0, Mathf.Cos(joyStickRotationAngle * Mathf.Deg2Rad)) *
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

    //private void FixedUpdate()
    //{
    //    UpdateArmTransform();
    //}

    /// <summary>
    /// Calculate and returns this arm's current strength output
    /// </summary>
    /// <returns></returns>
    public float CalculateCurrentArmStrength()
    {
        float armStrength = armDefaultStretchForce / armMaximumStamina * armCurrentStamina;

        return armStrength;
    }

    /// <summary>
    /// Updates this arm's current stamina based on its current behavior
    /// </summary>
    public void UpdateArmStamina()
    {
        if (isGrabbingFloor) // If the arm is grabbing onto floor
        {
            armCurrentStamina -= armStaminaConsumptionRate * Time.deltaTime;
            if (armCurrentStamina < 0) // Prevent the stamina goes below 0
            {
                armCurrentStamina = 0;
            }
        }
        else if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem != null) // If the arm is holding an item
        {
            if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().itemWeight > armLiftingStrength)
            {
                armCurrentStamina -= armStaminaConsumptionRate * Time.deltaTime;
                if (armCurrentStamina < 0) // Prevent the stamina goes below 0
                {
                    armCurrentStamina = 0;
                }
            }
        }
        else
        {
            if (armCurrentStamina < armMaximumStamina) // If the arm's current stamina is not full then start recover
            {
                armCurrentStamina += armStaminaDefaultRecoverSpeed * Time.deltaTime;
            }
            else
            {
                armCurrentStamina = armMaximumStamina;
            }
        }
    }

    /// <summary>
    /// Detect if the player want to start grabbing the floor with the armTip, and see if can grab or not
    /// If the armTip is currently holding an usable item, then drop the item first. The player need to press
    /// the grab button again to start grabbing floor
    /// </summary>
    public override void DetectGrabbingFloorInput()
    {
        if (isLeftArm)
        {
            // If the left armTip start grabbing the floor
            if (Input.GetKeyDown(KeyCode.JoystickButton4))
            {
                // If the armTip is currently empty and not holding any item, then start grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StartGrabbingFloor();
                }
                else // Drop the current holding item (no matter it can be used or not
                {
                    DropDownItem(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem);
                }
            }
            if (Input.GetKeyUp(KeyCode.JoystickButton4))
            {
                // If the armTip is currently empty and not holding any item, then stop grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StopGrabbingFloor();
                }
            }
        }

        if (!isLeftArm)
        {
            // If the right armTip start grabbing the floor
            if (Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                // If the armTip is currently empty and not holding any item, then start grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StartGrabbingFloor();
                }
                else // Drop the current holding item (no matter it can be used or not
                {
                    DropDownItem(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem);
                }
            }
            if (Input.GetKeyUp(KeyCode.JoystickButton5))
            {
                // If the armTip is currently empty and not holding any item, then stop grabbing the ground
                if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null)
                {
                    StopGrabbingFloor();
                }
            }
        }
    }

    /// <summary>
    /// Detect if the armTip is colliding with an item, and if the player want to pick up an item
    /// </summary>
    public override void DetectIfPickingUpItem()
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
    /// Detect if the player release the trigger to drop a heavy item
    /// </summary>
    public override void DetectIfDropHeavyItem()
    {
        if (isLeftArm)
        {
            // If the left armTip is holding a heavy item and the trigger is lifting up or arm's stamina is empty
            if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().itemWeight > armLiftingStrength &&
                (Input.GetAxis("LeftTrigger") < triggerThreshold || armCurrentStamina <= 0))
            {
                DropDownItem(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem);
            }

        }

        if (!isLeftArm)
        {
            // If the right armTip is holding an unusable item and the trigger is lifting up
            if (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().itemWeight > armLiftingStrength &&
                (Input.GetAxis("RightTrigger") < triggerThreshold || armCurrentStamina <= 0))
            {
                DropDownItem(armTip.GetComponent<ArmUseItem>().currentlyHoldingItem);
            }
        }
    }

    /// <summary>
    /// Start picking up the item
    /// </summary>
    /// <param name="pickingItem"></param>
    /// <returns></returns>
    public override IEnumerator PickUpItem(GameObject pickingItem)
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
    public override void DropDownItem(GameObject droppingItem)
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

        isArmBurstForceUsed = false;
    }

    /*
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

    public void UpdateArmTransform()
    {
        arm.localPosition = armTip.localPosition / 2f; // Put the center of the arm in the middle between the armTip and the body center
        //arm.localScale = new Vector3(1, 1, armTip.localPosition.z / 2f); // Extend the arm towards the armTip
        arm.localScale = new Vector3(0.1f, armTip.localPosition.z / 2f, 0.1f); // Extend the arm towards the armTip
        //armTip.localPosition = Vector3.zero;
        armTip.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // Keep the armTip's angular velocity always (0, 0, 0)
        armTip.localEulerAngles = Vector3.zero; // Keep the armTip's local euler angles always (0, 0, 0)
    }
    */

    ///// <summary>
    ///// Rotate the arm according to the joystick's rotation
    ///// </summary>
    ///// <param name="left"></param>
    //public override void RotateArm(bool left)
    //{
    //    transform.eulerAngles = new Vector3(0, joyStickRotationAngle, 0);
    //}

    // Calculate the direction and the amount of the force that should be applied
    public Vector3 CalculateArmForce(bool moveBody, Vector3 currentPosition, float carryingWeight)
    {
        Vector3 targetPosition;

        if (!PlayerInfo.isSideScroller) // If the game is in top-down mode (not in the flying level)
        {
            if (moveBody)
            {
                // Move in the same direction of the joystick
                //targetPosition = armTip.position + (new Vector3(Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad), 0, Mathf.Cos(joyStickRotationAngle * Mathf.Deg2Rad)) *
                //                                    joyStickLength * armMaxLength);

                // Move in the opposite direction of the joystick
                targetPosition = armTip.position - (joystickPosition * armMaxLength);

                //print(Vector3.Magnitude(targetPosition - currentPosition) + ", " + armStopThreshold);
                //print(currentPosition + ", " + targetPosition);
                //print("armTip: " + armTip.position + ", target: " + targetPosition);
                //Debug.DrawLine(armTip.position, armTip.position + new Vector3(Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad), 0, Mathf.Cos(joyStickRotationAngle * Mathf.Deg2Rad)) * joyStickLength * armMaxLength);
                //print("joystick angle: " + joyStickRotationAngle);
            }
            else
            {
                // If the arm is not grabbing floor
                if (!isGrabbingFloor)
                {
                    targetPosition = body.position + (joystickPosition * armMaxLength);
                }
                else
                {
                    targetPosition = armTipGrabbingPosition;

                    // If the armTip moves away from its current grabbing point then the grabbing point should change
                    if (Vector3.Distance(armTip.position, armTipGrabbingPosition) > 0.1f)
                    {
                        armTipGrabbingPosition = armTip.position;
                    }
                }
            }
        }
        else
        {
            if (moveBody)
            {
                targetPosition = armTip.position + (new Vector3(Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad) * Mathf.Cos(horizontalCameraAngle * Mathf.Deg2Rad),
                                                                Mathf.Cos(joyStickRotationAngle * Mathf.Deg2Rad),
                                                                Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad) * Mathf.Sin(horizontalCameraAngle * Mathf.Deg2Rad)) *
                                                    joyStickLength * armMaxLength);
            }
            else
            {
                targetPosition = body.position + (new Vector3(Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad) * Mathf.Cos(horizontalCameraAngle * Mathf.Deg2Rad),
                                                              Mathf.Cos(joyStickRotationAngle * Mathf.Deg2Rad),
                                                              Mathf.Sin(joyStickRotationAngle * Mathf.Deg2Rad) * Mathf.Sin(horizontalCameraAngle * Mathf.Deg2Rad)) *
                                                  joyStickLength * armMaxLength);
            }
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
                return Vector3.Normalize(targetPosition - currentPosition) * CalculateCurrentArmStrength() * targetDistance;
            }
        }
        else
        {
            // If the joystick is moving fast and its stamina is almost full, then add an extra "bonus" force (if the bonus force is not used in this cycle)
            if (!isArmBurstForceUsed &&
                armCurrentStamina >= (armMaximumStamina - timeAllowedToUseBurstForceSinceMaxStamina * armStaminaConsumptionRate) &&
                Vector3.Magnitude(joystickLastPosition - joystickPosition) / Time.deltaTime >= joyStickMoveSpeedControlBurstForceThreshold)
            {
                // Only give bonus when the arm is moving body or other large objects
                if (isGrabbingFloor ||
                    (armTip.GetComponent<ArmUseItem>().currentlyHoldingItem != null &&
                     armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.GetComponent<ItemInfo>().itemWeight > armLiftingStrength))
                {
                    isArmBurstForceUsed = true;

                    return Vector3.Normalize(targetPosition - currentPosition) * CalculateCurrentArmStrength() * maxStaminaInitialBurstMulti;
                }
                else
                {
                    return Vector3.Normalize(targetPosition - currentPosition) * CalculateCurrentArmStrength() * targetDistance;
                }
            }

            return Vector3.Normalize(targetPosition - currentPosition) * CalculateCurrentArmStrength() * targetDistance;
        }
    }

    /// <summary>
    /// Moves the arm tip
    /// </summary>
    /// <param name="left"></param>
    public void MoveArm(bool left)
    {
        Vector3 armTipAppliedForce = CalculateArmForce(false, armTip.transform.position, armTip.GetComponent<Rigidbody>().mass);

        //// If the arm is stretched too far away from the body then apply less force to move the armTip
        //if (firstSegmentDistanceFromBody >= 0.1f)
        //{
        //    //if (test)
        //    //{
        //    //    print(Mathf.Clamp01(-4.0f * Mathf.Pow((firstSegmentDistanceFromBody - 0.25f), 2) + 1));
        //    //}

        //    armTipAppliedForce *= Mathf.Clamp01(-4.0f * Mathf.Pow((firstSegmentDistanceFromBody - 0.1f), 2) + 1);
        //    //MoveArmTipTowardsLastArmSegment();
        //}

        armTip.GetComponent<Rigidbody>().AddForce(armTipAppliedForce, ForceMode.Impulse);
    }

    /// <summary>
    /// If the arm is stretched too long, move the armTip towards the last segment to help the arm retract
    /// </summary>
    public void MoveArmTipTowardsLastArmSegment()
    {
        // Calculate the retract force
        Vector3 retractForce = (lastArmSegment.position - armTip.position) * Mathf.Pow((firstSegmentDistanceFromBody + 0.75f), 2);
        // Apply the force to the last arm segment's direction
        armTip.GetComponent<Rigidbody>().AddForce(retractForce, ForceMode.Impulse);
    }

    ///// <summary>
    ///// Stretch the arm according to the joystick's tilt
    ///// </summary>
    ///// <param name="left"></param>
    //public override void StretchArm(bool left)
    //{
    //    Vector3 armTipAppliedForce = CalculateArmForce(false, armTip.transform.position, armTip.GetComponent<Rigidbody>().mass);
    //    armTip.GetComponent<Rigidbody>().AddForce(armTipAppliedForce, ForceMode.Impulse);
    //}

    /// <summary>
    /// When this armTip start grabbing floor
    /// </summary>
    public override void StartGrabbingFloor()
    {
        isGrabbingFloor = true;
        armTipGrabbingPosition = armTip.position;
        //if (otherArm_Physics.isGrabbingFloor)
        //{
        //    otherArm_Physics.isGrabbingFloor = false;
        //    body.SetParent(null, true);
        //}

        //bodyRotatingCenter.position = armTip.position;
        //bodyRotatingCenter.eulerAngles = new Vector3(0, joyStickRotationAngle - (Mathf.Sign(joyStickRotationAngle) * 180), 0);
        //body.SetParent(bodyRotatingCenter, true);
        UpdateArmFlags();

        // Play start grabbing sfx
        if (GetComponentInChildren<AudioSource>())
        {
            GetComponentInChildren<AudioSource>().PlayOneShot(armTipGrabbingGroundSFX);
        }
    }

    /// <summary>
    /// When this armTip stop grabbing floor
    /// </summary>
    public override void StopGrabbingFloor()
    {
        if (isGrabbingFloor)
        {
            isGrabbingFloor = false;
            //body.parent = null;
            //body.SetParent(null, true);
            isArmBurstForceUsed = false;
        }

        UpdateArmFlags();

        // Play stop grabbing sfx
        if (GetComponentInChildren<AudioSource>())
        {
            GetComponentInChildren<AudioSource>().PlayOneShot(armTipStopGrabbingGroundSFX);
        }
    }

    /// <summary>
    /// Rotate the body around the armTip
    /// </summary>
    public override void RotateBody()
    {
        //bodyRotatingCenter.eulerAngles = new Vector3(0, joyStickRotationAngle - (Mathf.Sign(joyStickRotationAngle) * 180), 0);
        body.eulerAngles = new Vector3(0, joyStickRotationAngle, 0);
    }

    /// <summary>
    /// Move the body around the armTip
    /// </summary>
    public override void MoveBody()
    {
        //body.localPosition = new Vector3(0, 0, joyStickLength * armMaxLength);
        body.GetComponent<Rigidbody>().AddForce(CalculateArmForce(true, body.position, body.GetComponent<Rigidbody>().mass), ForceMode.Impulse);

        //if (bodyWallDetector.isColliding)
        //{
        //    Debug.DrawLine(bodyRotatingCenter.position, bodyWallDetector.collidingPoint, Color.red);
        //    body.localPosition =
        //        new Vector3(0, 0, Vector3.Distance(bodyRotatingCenter.position, bodyWallDetector.collidingPoint));
        //    //body.position = bodyWallDetector.collidingPoint;
        //}

        /*
        Debug.DrawLine(bodyRotatingCenter.position, bodyRotatingCenter.position + bodyRotatingCenter.forward * (joyStickLength * armMaxLength + body.localScale.x), Color.green);
        // Don't extend if the armTip will go into collider
        RaycastHit hit;
        if (Physics.Raycast(bodyRotatingCenter.position - bodyRotatingCenter.forward * collisionRaycastOriginSetBackDistance, bodyRotatingCenter.forward,
            out hit, joyStickLength * armMaxLength + collisionRaycastOriginSetBackDistance + 0 * body.localScale.x, bodyCollidingLayer))
        {
            // If the ray hits the object that is currently holding by the other armTip, then ignore it, don't retract the arm
            if (otherArm_Physics.armTip.GetComponent<ArmUseItem>().currentlyHoldingItem == null ||
                hit.transform != otherArm_Physics.armTip.GetComponent<ArmUseItem>().currentlyHoldingItem.transform)
            {
                //print("Angle: " + Vector3.Angle(hit.normal, transform.forward) + ", Cos: " + Mathf.Cos(Vector3.Angle(hit.normal, transform.forward) * Mathf.Deg2Rad));
                Debug.DrawLine(bodyRotatingCenter.position, hit.point, Color.red);
                body.localPosition =
                    new Vector3(0, 0, hit.distance - collisionRaycastOriginSetBackDistance);// - body.localScale.x / 2f / Mathf.Cos(Vector3.Angle(hit.normal, transform.forward) * Mathf.Deg2Rad));
            }
        }
        */
    }

    /// <summary>
    /// Moving the heavy item that is currently holding in the armTip
    /// </summary>
    /// <param name="movingItem"></param>
    public void MoveHeavyItem(GameObject movingItem)
    {
        //movingItem.GetComponent<Rigidbody>().AddForce(CalculateArmForce(false, movingItem.transform.position, movingItem.GetComponent<Rigidbody>().mass), ForceMode.Impulse);
        armTip.GetComponent<Rigidbody>().AddForce(CalculateArmForce(false, armTip.transform.position, movingItem.GetComponent<Rigidbody>().mass), ForceMode.Impulse);
    }

    /// <summary>
    /// Calculates the magnitude to apply on different physics parameters for the arm when the player is in the water
    /// </summary>
    public void CalculateArmPhysicsMagnitudeInWater()
    {
        Vector3 bodyVelocity = body.GetComponent<Rigidbody>().velocity;
        if (bodyVelocity.magnitude > bodyTopSpeed)
        {
            bodyTopSpeed = bodyVelocity.magnitude;
        }

        if (!startedSwimming) // If the body is not moving very fast
        {
            // Set magnitude to 1
            armPhysicsMagnitude = 1;
        }
        else
        {
            // Adjust magnitude depending on the body's velocity and arm's orientation and arm's length and
            // the sine of(angle between two arms / 2)
            armPhysicsMagnitude = Mathf.Clamp(Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad *
                                                                  Vector3.Angle(bodyVelocity, armTip.position - body.position))) *
                                                                  joyStickLength,
                                              0.01f, 1.0f);

            armPhysicsMagnitude *= Mathf.Clamp(Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angleBetweenTwoArms)) / 2.0f, 0.01f, 1.0f);
        }

        // Determine if the arms are "swimming"
        if (isArmsSpreading)
        {
            currentLargestSpreadingAngle = angleBetweenTwoArms;
        }
        if (isArmsClosing)
        {
            if (currentLargestSpreadingAngle != 0 && currentLargestSpreadingAngle < 170) // If the arms are not properly "swimming"
            {
                armPhysicsMagnitude *= Mathf.Clamp(Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angleBetweenTwoArms)) / 2.0f, 0.01f, 1.0f);
                isArmSwimming = false;
            }

            if (currentLargestSpreadingAngle > 170) // If the arms are "swimming" in a circle
            {
                isArmSwimming = true;
                lastArmSwimmingTime = Time.time;
                currentLargestSpreadingAngle = 0;
            }
        }

        if (Time.time - lastArmSwimmingTime > 2) // If the arms are not "swimming"
        {
            armPhysicsMagnitude *= 0.01f;// Mathf.Clamp(Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * angleBetweenTwoArms)) / 2.0f, 0.01f, 1.0f);
        }

        // Further panelize the force arm can provide to swim when the arms are not stretched out
        Mathf.Pow(armPhysicsMagnitude, 2);
    }

    /// <summary>
    /// Check and calculate whether the player is swimming (having some speed in the water) or not
    /// </summary>
    public void UpdateSwimmingStatus()
    {
        Vector3 bodyVelocity = body.GetComponent<Rigidbody>().velocity;

        if (!startedSwimming) // If the player is not swimming
        {
            if (bodyVelocity.magnitude > startSwimMinVelocityThershold)
            {
                startedSwimming = true;
            }
        }
        else
        {
            if (bodyVelocity.magnitude < stopSwimMinVelocityThershold)
            {
                startedSwimming = false;
            }
        }
    }

    /// <summary>
    /// Detects different arm movement patterns
    /// </summary>
    public void DetectingArmMovement()
    {
        float previousAngleBetweenTwoArms = angleBetweenTwoArms;
        if (Vector3.Magnitude(armTip.position - body.position) >= joystickLengthThresholdForAngleBetweenTwoArms * armMaxLength &&
            Vector3.Magnitude(otherArm_Physics.armTip.position - body.position) >= joystickLengthThresholdForAngleBetweenTwoArms * armMaxLength)
        {
            angleBetweenTwoArms = Vector3.Angle(otherArm_Physics.armTip.position - body.position, armTip.position - body.position);
        }

        if (angleBetweenTwoArms < previousAngleBetweenTwoArms) // If the angle between two arms is decreasing
        {
            isArmsClosing = true;
            isArmsSpreading = false;
        }
        else if (angleBetweenTwoArms > previousAngleBetweenTwoArms) // If the angle between two arms is increading
        {
            isArmsClosing = false;
            isArmsSpreading = true;
        }
        else
        {
            isArmsClosing = false;
            isArmsSpreading = false;
        }
    }

    /// <summary>
    /// Calculates the angle between the camera's facing direction and the global x-axis on the x-z plain 
    /// (when the direction vector passes x=1,z=1 coordinate, the angle will be 45 degrees, when passing x=0,z=1, angle will be 90
    ///  when passing x=-1,z=0, angle will be 180)
    /// </summary>
    public void CalculateHorizontalCameraAngle()
    {
        horizontalCameraAngle = Mathf.Atan(PlayerInfo.sGameCamera.forward.z / PlayerInfo.sGameCamera.forward.x) * Mathf.Rad2Deg;
        if (PlayerInfo.sGameCamera.forward.x < 0)
        {
            horizontalCameraAngle += 180;
        }
        else if (PlayerInfo.sGameCamera.forward.x >= 0 && PlayerInfo.sGameCamera.forward.z < 0)
        {
            horizontalCameraAngle += 360;
        }

        horizontalCameraAngle -= 90;
    }

    ///// <summary>
    ///// Calculates how much force the arm is giving to the body when gliding through lifting (or other directions) air blow
    ///// </summary>
    ///// <param name="windStrength"></param>
    ///// <param name="windDirecion"></param>
    ///// <returns></returns>
    //public Vector3 CalculateArmGlidingForce(float windStrength, Vector3 windDirecion)
    //{
    //    Vector3 finalForceAppliedToBody = Vector3.one;
    //    // Calculate the angle between the arm's stretch direction (from body to the armTip) and the direction of the wind
    //    float angleBetweenArmAndWind = Vector3.Angle(armTip.position - transform.position, windDirecion);
    //    // Calculate the actual wind force that is applying on a unit length of the arm
    //    float effectiveWindStrength = windStrength * Mathf.Sin(angleBetweenArmAndWind * Mathf.Deg2Rad) *
    //                                  Mathf.Sign(PlayerInfo.sGameCamera.InverseTransformVector(Vector3.Cross(armTip.position - transform.position, windDirecion)).z);
    //    // Calculate the direction of the force that is applying on the arm
    //    Vector3 appliedForceDirection = Vector3.Cross(armTip.position - transform.position, PlayerInfo.sGameCamera.position - body.position).normalized;
    //    // Calculate the final force the entire arm provide to the body based on the actual arm length
    //    finalForceAppliedToBody = effectiveWindStrength * appliedForceDirection * Vector3.Magnitude(armTip.position - transform.position);
    //    //finalForceAppliedToBody = effectiveWindStrength * appliedForceDirection * joyStickLength * armMaxLength;
    //    //finalForceAppliedToBody = effectiveWindStrength * appliedForceDirection * armMaxLength; // Uniform version

    //    return finalForceAppliedToBody;
    //}

    ///// <summary>
    ///// Check if the armTip is in a windy area and will provide the body lifting force
    ///// </summary>
    //public void CheckIfApplyingGlidingForce()
    //{

    //}

    ///// <summary>
    ///// Give body gliding force
    ///// </summary>
    //public void ApplyGlidingForceToBody()
    //{
    //    body.GetComponent<Rigidbody>().AddForce(CalculateArmGlidingForce(armCurrentTotalReceivedWindForce.magnitude, armCurrentTotalReceivedWindForce.normalized),
    //                                            ForceMode.Force);
    //}

    /// <summary>
    /// Calculates how much drag the arm should have on the player's body based on the player's current velocity and applies it to the player's body
    /// </summary>
    /// <returns></returns>
    public void ApplyArmAirDrag()
    {
        Vector3 finalDragForceAppliedToBody = Vector3.one;
        // Calculate the body's current velocity relative to the current wind velocity at its position
        Vector3 bodyVelocityRelativeToWindForce = body.GetComponent<Rigidbody>().velocity - armCurrentTotalReceivedWindForce;
        // Calculate the angle between the arm's stretch direction (from body to the armTip) and the direction of the wind
        float angleBetweenArmAndBodyVelocity = Vector3.Angle(armTip.position - transform.position, bodyVelocityRelativeToWindForce.normalized);
        // Calculate the actual wind force that is applying on a unit length of the arm
        float effectiveDragStrength = bodyVelocityRelativeToWindForce.magnitude * Mathf.Sin(angleBetweenArmAndBodyVelocity * Mathf.Deg2Rad) *
                                      Mathf.Sign(PlayerInfo.sGameCamera.InverseTransformVector(Vector3.Cross(armTip.position - transform.position,
                                                                                                             bodyVelocityRelativeToWindForce.normalized)).z);
        // Calculate the direction of the force that is applying on the arm
        Vector3 appliedForceDirection = -Vector3.Cross(armTip.position - transform.position, PlayerInfo.sGameCamera.position - body.position).normalized;
        // Calculate the final force the entire arm provide to the body based on the actual arm length
        finalDragForceAppliedToBody = effectiveDragStrength * appliedForceDirection * Vector3.Magnitude(armTip.position - transform.position) * armAirDragMultiplier;
        //finalForceAppliedToBody = effectiveWindStrength * appliedForceDirection * joyStickLength * armMaxLength;
        //finalForceAppliedToBody = effectiveWindStrength * appliedForceDirection * armMaxLength; // Uniform version

        body.GetComponent<Rigidbody>().AddForce(finalDragForceAppliedToBody, ForceMode.Force);
    }

    /// <summary>
    /// Detects if the player is switch on or off the echo mode in level 5
    /// </summary>
    public void DetectIfSwitchEchoMode()
    {
        if (isLeftArm)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton8))
            {
                SwitchEchoMode();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton9))
            {
                SwitchEchoMode();
            }
        }
    }

    /// <summary>
    /// Switch on or off echo mode
    /// </summary>
    public void SwitchEchoMode()
    {
        PlayerInfo.isInEchoMode = !PlayerInfo.isInEchoMode;
        PlayerInfo.sPlayerInfo.echoIndicator.SetActive(PlayerInfo.isInEchoMode); // Turn on or off the echo indicator
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
