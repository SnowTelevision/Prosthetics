using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the UI elements that shows game informations
/// </summary>
public class UpdatePlayerUI : MonoBehaviour
{
    public Image leftArmStaminaFill; // The fill bar for left arm's stamina
    public Image rightArmStaminaFill; // The fill bar for right arm's stamina
    public Color armNormalStaminaColor; // The color of the arm's stamina bar when the stamina is not low
    public Color armLowStaminaColor; // The color of the arm's stamina bar when the stamina is low
    public float lowStaminaPercent; // How much percent stamina left is "Low Stamina"

    public float playerCurrentMaxArmStamina; // The current maximum stamina for player's arms

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Updates the stamina bar fill image for the left and right arm
        playerCurrentMaxArmStamina = PlayerInfo.sPlayerInfo.leftArmController.armMaximumStamina;
        UpdateArmStaminaFill(leftArmStaminaFill, PlayerInfo.sPlayerInfo.leftArmController);
        UpdateArmStaminaFill(rightArmStaminaFill, PlayerInfo.sPlayerInfo.rightArmController);
    }

    /// <summary>
    /// Updates the fill and color for player's arm's stamina bar
    /// </summary>
    /// <param name="fill"></param>
    /// <param name="arm"></param>
    public void UpdateArmStaminaFill(Image fill, ControlArm_UsingPhysics arm)
    {
        // Update the fill amount
        fill.fillAmount = arm.armCurrentStamina / arm.armMaximumStamina;
        // Update the fill color
        if (arm.armCurrentStamina / arm.armMaximumStamina <= lowStaminaPercent)
        {
            fill.color = armLowStaminaColor;
        }
        else
        {
            fill.color = armNormalStaminaColor;
        }
    }
}
