using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

/// <summary>
/// Stores the info that relates to the player's body
/// </summary>
public class PlayerInfo : MonoBehaviour
{
    public float maxHealth; // How much health the player can have
    public Transform gameCamera; // The transform of the camera that is looking at the player
    public float targetSideScrollOrbitRadius; // The radius of the player's orbit for the game's side-scrolling session
    public float armTipCollideWallHapticStrength; // How strong is the haptic feedback when the armTip collide with an object
    public float armTipCollideWallHapticDuration; // How long is the haptic feedback when the armTip collide with an object
    //public float reflectedEchoHapticStrength; // How strong is the haptic feedback when the player received a reflected echo
    //public float reflectedEchoHapticDuration; // How long is the haptic feedback when the player received a reflected echo
    public float bodyCollideWallHapticStrength; // How strong is the haptic feedback when the body collide with an object
    public float bodyCollideWallHapticDuration; // How long is the haptic feedback when the body collide with an object
    public float echoBounceBackHapticStrength; // How strong is the haptic feedback when the echo bounce back from an object
    public float echoBounceBackHapticDuration; // How long is the haptic feedback when the echo bounce back from an object
    public float reflectedEchoHitEdgeHapticStrength; // How strong is the haptic feedback when the reflected echo hits the edge of an object
    public float reflectedEchoHitEdgeHapticDuration; // How long is the haptic feedback when the reflected echo hits the edge of an object
    public float reflectedEchoTravelObjectHapticStrength; // How strong is the haptic feedback when the reflected echo travelling in a non-moving object
    public float reflectedEchoTravelObjectHapticDuration; // How long is the haptic feedback when the reflected echo travelling in a non-moving object
    public GameObject echoIndicator; // Can have different color status indicating different vibration echo status (such as can the player create echo)
    public Color canCreateEchoColor; // The color of the indicator when the player can create an echo
    public Color cannotCreateEchoColor; // The color of the indicator when the player cannot create an echo
    public float echoIndicatorEmissionIntensity; // How high is the emission intensity for the echo indicator
    public ControlArm_UsingPhysics leftArmController; // The controller of the left arm
    public ControlArm_UsingPhysics rightArmController; // The controller of the right arm
    public BodyMovementModeInfo[] bodyMovementModeInfos; // Infos of different body movement modes
    public GameObject bodySoftBodyCenter; // The center of the soft body of the player body

    public bool inWater; // Is the body in the water
    public bool onLand; // Is the body touching ground
    public bool inAir; // Is the body in the air
    public static Transform sGameCamera; // The static reference to gameCamera
    public static PlayerInfo sPlayerInfo; // The static reference to PlayerInfo
    public static bool isSideScroller; // If the game is in side-scroller mode
    public static bool isInEchoMode; // If the player is in listening to vibration echo mode
    public float totalHapticStrength; // What's the total controller haptic strength currently
    public static bool canCreateEcho; // Since the player can only create one echo at a time, need to check if the last echo returned

    // Test
    public bool test;

    /// XInput
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    private void Awake()
    {
        // Assigning static references for variebles that has to be assigned in the editor thus cannot be static.
        sGameCamera = gameCamera;
        sPlayerInfo = this;

        canCreateEcho = true;
    }

    // Use this for initialization
    void Start()
    {
        // Set up body mode (Assume if the body start on land)
        SwitchToLandMode();

        // Test
        if (test)
        {
            isSideScroller = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /// XInput
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    //Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        prevState = state;
        state = GamePad.GetState(playerIndex);
    }

    private void FixedUpdate()
    {
        if (isSideScroller) // If the game is in side-scrolling session
        {
            CorrectPlayerSideScrollingOrbit();
        }

        // If the player received vibration
        if (isInEchoMode)
        {
            GamePad.SetVibration(playerIndex, totalHapticStrength, totalHapticStrength);
        }
    }

    /// <summary>
    /// Calculate the velocity that is tangent to the orbit that the player's body is currently at
    /// based on the player's current velocity. (assume the center of the orbit is (0, y, 0) in global)
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateTangentVelocity()
    {
        float currentVerticalSpeed = GetComponent<Rigidbody>().velocity.y; // The player's current speed along the y-axis
        // Calculate the player's current speed in the x-z plain
        Vector3 currentHorizontalVelocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
        float currentHorizontalSpeed = currentHorizontalVelocity.magnitude;
        // Determine if the player is going clockwise or counter-clockwise (1 is clockwise, -1 is counter-clockwise)
        float playerVelocityDirection = Mathf.Sign(Vector3.Cross(transform.position, GetComponent<Rigidbody>().velocity).y);
        // Calculate the x and z component of the tangent horizontal velocity
        float currentHorizontalDistance = new Vector3(transform.position.x, 0, transform.position.z).magnitude; // The horizontal distance from the player to the center
        float tangentHorizontalSpeedX = currentHorizontalSpeed / currentHorizontalDistance * Mathf.Abs(transform.position.z);
        float tangentHorizontalSpeedZ = currentHorizontalSpeed / currentHorizontalDistance * Mathf.Abs(transform.position.x);
        // Get the final converted tangent velocity
        Vector3 tangentVelocity =
            new Vector3(Mathf.Sign(transform.position.x) * Mathf.Sign(transform.position.x * transform.position.z) * playerVelocityDirection * tangentHorizontalSpeedX,
                        currentVerticalSpeed,
                        -Mathf.Sign(transform.position.z) * Mathf.Sign(transform.position.x * transform.position.z) * playerVelocityDirection * tangentHorizontalSpeedZ);

        if (test)
        {
            //print("x: " + Mathf.Sign(transform.position.x) * Mathf.Sign(transform.position.x * transform.position.z) * playerVelocityDirection);
            //print("z: " + -Mathf.Sign(transform.position.z) * Mathf.Sign(transform.position.x * transform.position.z) * playerVelocityDirection);
        }

        return tangentVelocity;
    }

    /// <summary>
    /// Calculate the force needed to help the player's body stays on the orbital track in side-scrolling session
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateOrbitalAdjustmentForce()
    {
        // Calculate the player's current distance from the center
        float playerCurrentDistance = new Vector3(transform.position.x, 0, transform.position.z).magnitude;
        // Calculate the coordinate of the "correct" player's position if it is on the orbit
        Vector3 targetPosition = new Vector3(targetSideScrollOrbitRadius / playerCurrentDistance * transform.position.x,
                                             0,
                                             targetSideScrollOrbitRadius / playerCurrentDistance * transform.position.z);

        if (test)
        {
            //Debug.DrawRay(transform.position, targetPosition - new Vector3(transform.position.x, 0, transform.position.z), Color.red);
            //print(targetPosition - new Vector3(transform.position.x, 0, transform.position.z));
        }

        return targetPosition - new Vector3(transform.position.x, 0, transform.position.z);
    }

    /// <summary>
    /// Make the player moves along the side-scrolling orbit
    /// </summary>
    public void CorrectPlayerSideScrollingOrbit()
    {
        if (test)
        {
            //Debug.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().velocity, Color.white);
            //Debug.DrawLine(transform.position, transform.position + CalculateTangentVelocity(), Color.blue);
            //Debug.DrawLine(transform.position, transform.position + CalculateTangentVelocity() - GetComponent<Rigidbody>().velocity, Color.red);
        }

        GetComponent<Rigidbody>().AddForce(CalculateTangentVelocity() - GetComponent<Rigidbody>().velocity, ForceMode.Acceleration);
        GetComponent<Rigidbody>().AddForce(CalculateOrbitalAdjustmentForce(), ForceMode.VelocityChange);
    }

    ///// <summary>
    ///// Vibrates the controller when the player receives the reflected echo
    ///// </summary>
    //public void ReceivedReflectedEcho()
    //{
    //    //print("receive reflected echo");
    //    //GamePad.SetVibration(playerIndex, reflectedEchoHapticStrength, reflectedEchoHapticStrength);
    //    StartCoroutine(AddEchoVibration(reflectedEchoHapticStrength, reflectedEchoHapticDuration));
    //}

    /// <summary>
    /// Vibrates the controller when the player receives the reflected echo
    /// </summary>
    public void ReceivedReflectedEcho()
    {
        //print("receive reflected echo");
        //GamePad.SetVibration(playerIndex, reflectedEchoHapticStrength, reflectedEchoHapticStrength);
        canCreateEcho = true; // Allow the player to create new echo when the previous one returns

        // Change the echo indicator's colors
        Vector4 emissionColor = canCreateEchoColor;
        echoIndicator.GetComponent<MeshRenderer>().material.color = emissionColor;
        echoIndicator.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", GetHDRcolor.GetColorInHDR(emissionColor, echoIndicatorEmissionIntensity));
    }

    /// <summary>
    /// The haptic feedback when one of the player's armTip collides an object while in echo mode
    /// </summary>
    public void ArmTipCollideObjectHaptic()
    {
        //print("arm collide");
        //GamePad.SetVibration(playerIndex, armTipCollideWallHapticStrength, armTipCollideWallHapticStrength);
        StartCoroutine(AddEchoVibration(armTipCollideWallHapticStrength, armTipCollideWallHapticDuration));
    }

    /// <summary>
    /// The haptic feedback when player's body collides an object while in echo mode
    /// </summary>
    public void BodyCollideObjectHaptic()
    {
        //print("body collide");
        //GamePad.SetVibration(playerIndex, bodyCollideWallHapticStrength, bodyCollideWallHapticStrength);
        StartCoroutine(AddEchoVibration(bodyCollideWallHapticStrength, bodyCollideWallHapticDuration));
    }

    /// <summary>
    /// The haptic feedback when the echo bounced back from an object
    /// </summary>
    public void EchoBounceBackHaptic()
    {
        StartCoroutine(AddEchoVibration(echoBounceBackHapticStrength, echoBounceBackHapticDuration));
    }

    /// <summary>
    /// The haptic feedback when the reflected echo hits the edge of an object
    /// </summary>
    public void ReflectedEchoHitsEdgeHaptic()
    {
        StartCoroutine(AddEchoVibration(reflectedEchoHitEdgeHapticStrength, reflectedEchoHitEdgeHapticDuration));
    }

    /// <summary>
    /// The haptic feedback when the reflected echo is travelling inside a non-moving object
    /// </summary>
    public void ReflectedEchoTravelInObjectHaptic()
    {
        StartCoroutine(AddEchoVibration(reflectedEchoTravelObjectHapticStrength, reflectedEchoTravelObjectHapticDuration));
    }

    /// <summary>
    /// Add a certain amount of haptic strength to the total haptic strength for a certain period of time
    /// </summary>
    /// <param name="strength"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator AddEchoVibration(float strength, float duration)
    {
        totalHapticStrength += strength; // Add haptic strength

        if (duration == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            yield return new WaitForSeconds(duration); // Wait for this haptic duration to finish
        }

        totalHapticStrength -= strength; // Remove haptic strength
    }

    /// <summary>
    /// Change the mode to land-based
    /// </summary>
    public void SwitchToLandMode()
    {
        foreach (BodyMovementModeInfo b in bodyMovementModeInfos)
        {
            if (b.movementModeName == "Land")
            {
                ChangeBodyMovementParameters(b);
            }
        }
    }

    /// <summary>
    /// Change the mode to water-based
    /// </summary>
    public void SwitchToWaterMode()
    {
        foreach (BodyMovementModeInfo b in bodyMovementModeInfos)
        {
            if (b.movementModeName == "Water")
            {
                ChangeBodyMovementParameters(b);
            }
        }
    }

    /// <summary>
    /// Change the mode to air-based
    /// </summary>
    public void SwitchToAirMode()
    {
        foreach (BodyMovementModeInfo b in bodyMovementModeInfos)
        {
            if (b.movementModeName == "Air")
            {
                ChangeBodyMovementParameters(b);
            }
        }
    }

    /// <summary>
    /// Change different physics parameters and component variebles for a body movement mode
    /// </summary>
    /// <param name="movementModeParameters"></param>
    public void ChangeBodyMovementParameters(BodyMovementModeInfo movementModeParameters)
    {
        GetComponent<Rigidbody>().drag = movementModeParameters.bodyRigidbodyDrag;
        GetComponent<Rigidbody>().angularDrag = movementModeParameters.bodyRigidbodyAngularDrag;
        //bodySoftBodyFixedJoint.connectedMassScale = movementParameters.softBodySurfaceJointConnectedMassScale;
    }
}

/// <summary>
/// Stores informations of a body movement mode, this include but not limit to:
/// Rigidbody parameters
/// </summary>
[Serializable]
public class BodyMovementModeInfo
{
    public string movementModeName; // The name for the movement mode
    public float bodyRigidbodyDrag; // The drag of the rigidbody on the body
    public float bodyRigidbodyAngularDrag; // The angular drag on the rigidbody on the body
    public float softBodyCenterRigidbodyMass; // The mass of the rigidbody on the center
    public float softBodyCenterRigidbodyDrag; // The drag of the rigidbody on the center
    public float softBodyCenterRigidbodyAngularDrag; // The angular drag of the rigidbody on the center
    public float softBodyCenterFixedJointMassScale; // The mass scale on the fixed joint on the center
    public float softBodyCenterFixedJointConnectedMassScale; // The connected mass scale on the fixed joint on the center
    public float softBodyCenterSpringJointSpring; // The spring on the spring joints on the center
    public float softBodyCenterSpringJointDamper; // The damper on the spring joints on the center
    public float softBodyCenterSpringJointTolerance; // The tolerance on the spring joints on the center
    public float softBodyCenterSpringJointMassScale; // The mass scale on the spring joints on the center
    public float softBodyCenterSpringJointConnectedMassScale; // The connected mass scale on the spring joints on the center
    public float softBodySurfaceRigidbodyMass; // The mass of the rigidbody on the surface objects
    public float softBodySurfaceRigidbodyDrag; // The drag of the rigidbody on the surface objects
    public float softBodySurfaceRigidbodyAngularDrag; // The angular drag of the rigidbody on the surface objects
    public float softBodySurfaceJointSpring; // The spring on the spring joint on the surface objects
    public float softBodySurfaceJointDamper; // The damper on the spring joint on the surface objects
    public float softBodySurfaceJointTolerance; // The tolerance on the spring joints on the surface objects
    public float softBodySurfaceJointMassScale; // The mass scale on the spring joint on the surface objects
    public float softBodySurfaceJointConnectedMassScale; // The connected mass scale on the spring joint on the surface objects
}
