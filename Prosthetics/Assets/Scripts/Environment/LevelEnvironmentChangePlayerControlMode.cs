using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This should be attached to trigger objects that help set the player's control mode
/// The triggers should be set to general movement areas includes where the player can walk, swim or fly
/// </summary>
public class LevelEnvironmentChangePlayerControlMode : MonoBehaviour
{
    public AreaType areaType; // The type of the area this trigger covers
    public bool forArmTipOnly; // Is this trigger only for the armTip and not for the body

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // If an armTip enters the trigger
        if (other.GetComponent<ArmUseItem>())
        {
            ArmTipEntersArea(other.GetComponentInParent<ControlArm_UsingPhysics>());
        }

        // If the body enters the trigger and this trigger is not just for the armTip
        if (!forArmTipOnly && other.GetComponent<PlayerInfo>())
        {
            BodyEntersArea(other.GetComponent<PlayerInfo>());
        }
    }

    /// <summary>
    /// When player's armTip enters this area
    /// </summary>
    /// <param name="armController"></param>
    public void ArmTipEntersArea(ControlArm_UsingPhysics armController)
    {
        if (areaType == AreaType.Walkable && !armController.onLand)
        {
            ChangeArmTipToLand(armController);
        }
        if (areaType == AreaType.Swimmable && !armController.inWater)
        {
            ChangeArmTipToWater(armController);
        }
        if (areaType == AreaType.Flyable && !armController.inAir)
        {
            ChangeArmTipToAir(armController);
        }
    }

    /// <summary>
    /// When player's body enters this area
    /// </summary>
    /// <param name="bodyInfo"></param>
    public void BodyEntersArea(PlayerInfo bodyInfo)
    {
        if (areaType == AreaType.Walkable && !bodyInfo.onLand)
        {
            ChangeBodyToLand(bodyInfo);
        }
        if (areaType == AreaType.Swimmable && !bodyInfo.inWater)
        {
            ChangeBodyToWater(bodyInfo);
        }
        if (areaType == AreaType.Flyable && !bodyInfo.inAir)
        {
            ChangeBodyToAir(bodyInfo);
        }
    }

    /// <summary>
    /// Change the armTip's control mode to land-based
    /// </summary>
    /// <param name="armController"></param>
    public void ChangeArmTipToLand(ControlArm_UsingPhysics armController)
    {
        armController.onLand = true;
        armController.inWater = false;
        armController.inAir = false;
        armController.SwitchToLandControl();
    }

    /// <summary>
    /// Change the armTip's control mode to water-based
    /// </summary>
    /// <param name="armController"></param>
    public void ChangeArmTipToWater(ControlArm_UsingPhysics armController)
    {
        armController.onLand = false;
        armController.inWater = true;
        armController.inAir = false;
        armController.SwitchToWaterControl();
    }

    /// <summary>
    /// Change the armTip's control mode to air-based
    /// </summary>
    /// <param name="armController"></param>
    public void ChangeArmTipToAir(ControlArm_UsingPhysics armController)
    {
        armController.onLand = false;
        armController.inWater = false;
        armController.inAir = true;
        armController.SwitchToAirControl();
    }

    /// <summary>
    /// Change the body's behavior to land-based
    /// </summary>
    /// <param name="bodyInfo"></param>
    public void ChangeBodyToLand(PlayerInfo bodyInfo)
    {
        bodyInfo.onLand = true;
        bodyInfo.inWater = false;
        bodyInfo.inAir = false;
        bodyInfo.SwitchToLandMode();
    }

    /// <summary>
    /// Change the body's behavior to water-based
    /// </summary>
    /// <param name="bodyInfo"></param>
    public void ChangeBodyToWater(PlayerInfo bodyInfo)
    {
        bodyInfo.onLand = false;
        bodyInfo.inWater = true;
        bodyInfo.inAir = false;
        bodyInfo.SwitchToWaterMode();
    }

    /// <summary>
    /// Change the body's behavior to air-based
    /// </summary>
    /// <param name="bodyInfo"></param>
    public void ChangeBodyToAir(PlayerInfo bodyInfo)
    {
        bodyInfo.onLand = false;
        bodyInfo.inWater = false;
        bodyInfo.inAir = true;
        bodyInfo.SwitchToAirMode();
    }
}

/// <summary>
/// Different movement areas that can be selected from a dropdown list
/// </summary>
[Serializable]
public enum AreaType
{
    Walkable,
    Swimmable,
    Flyable
}
