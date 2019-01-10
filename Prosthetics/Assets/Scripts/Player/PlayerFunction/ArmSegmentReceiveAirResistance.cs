using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculate how much air resistance (include wind force) this arm segment should receive
/// then apply the resistance force to this arm segment
/// </summary>
public class ArmSegmentReceiveAirResistance : MonoBehaviour
{
    public Rigidbody bodyRigidBody; // The rigidbody on the player's body
    public Transform armTip; // The armTip on this arm
    public float armSegmentAirDragMultiplier; // The strength multiplier of this arm segment's drag when the player is in the air
    public float minEffectiveArmLength; // How long the arm at least have to extend to receive wind resistance (it won't receive any if it's retract into the body)
    public float bodyAppliedForceMultiplier; // The strength multiplier of this arm segment's drag that's applied on the body

    public float maximumArmLength; // How far the arm can extend
    public Vector3 armSegmentCurrentTotalReceivedWindForce; // What's the total wind force currently added to this arm segment

    // Use this for initialization
    void Start()
    {
        maximumArmLength = armTip.GetComponentInParent<ControlArm_UsingPhysics>().armMaxLength;
    }

    // Update is called once per frame
    void Update()
    {
        // Apply air drag force to arm segment if the body is in air
        if (PlayerInfo.sPlayerInfo.inAir)
        {
            ApplyArmAirDrag();
        }
    }

    private void LateUpdate()
    {
        armSegmentCurrentTotalReceivedWindForce = Vector3.zero;
    }

    /// <summary>
    /// Calculates how much drag the arm segment should have on the player's body based on the player's current velocity and applies it to the player's body
    /// </summary>
    /// <returns></returns>
    public void ApplyArmAirDrag()
    {
        Vector3 finalDragForceAppliedToArmSegment = Vector3.one;
        // Calculate the armTip's relative position to the body
        Vector3 armTipRelativePosition = armTip.position - bodyRigidBody.transform.position;
        // Calculate the body's current velocity relative to the current wind velocity at its position
        Vector3 bodyVelocityRelativeToWindForce = bodyRigidBody.GetComponent<Rigidbody>().velocity - armSegmentCurrentTotalReceivedWindForce;
        // Calculate the angle between the arm's stretch direction (from body to the armTip) and the direction of the wind
        float angleBetweenArmAndBodyVelocity = Vector3.Angle(armTipRelativePosition, bodyVelocityRelativeToWindForce.normalized);
        // Calculate the actual wind force that is applying on a unit length of the arm
        float effectiveDragStrength = bodyVelocityRelativeToWindForce.magnitude * Mathf.Sin(angleBetweenArmAndBodyVelocity * Mathf.Deg2Rad) *
                                      Mathf.Sign(PlayerInfo.sGameCamera.InverseTransformVector(Vector3.Cross(armTipRelativePosition,
                                                                                                             bodyVelocityRelativeToWindForce.normalized)).z);
        // Calculate the direction of the force that is applying on the arm
        Vector3 appliedForceDirection = -Vector3.Cross(armTipRelativePosition, PlayerInfo.sGameCamera.position - bodyRigidBody.position).normalized;
        // Calculate the final force the entire arm provide to the body based on the actual arm length
        finalDragForceAppliedToArmSegment =
            effectiveDragStrength * appliedForceDirection * armSegmentAirDragMultiplier *
            Mathf.Clamp(Vector3.Magnitude(armTipRelativePosition) - minEffectiveArmLength, 0, maximumArmLength);
        // Simulate the physics of arm segment being pushed by air
        GetComponent<Rigidbody>().AddForce(finalDragForceAppliedToArmSegment, ForceMode.Force);

        // Calculate and apply this arm segment's provided drag force to the body
        Vector3 finalDragForceAppliedToBody = finalDragForceAppliedToArmSegment / armSegmentAirDragMultiplier * bodyAppliedForceMultiplier;
        bodyRigidBody.AddForce(finalDragForceAppliedToBody, ForceMode.Force);
    }
}
