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

    }
}
