using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemInfo : MonoBehaviour
{
    public bool canUse; // Is this an usable item? If it is not then the player has to keep holding down the grab item button to hold it
    public float itemWeight; // The item's weight. If it is smaller or equal than how much the arm can carry, it should be able to be lifted by the arm and not touching the ground
    public UnityEvent setupEvent; // An event to be triggered when the item is picked up
    public UnityEvent singleUseEvent; // An event to be triggered for a single use of the item
    public UnityEvent stopUsingEvent; // An event to be triggered when the item is stop being used
    public UnityEvent resetEvent; // An event to be triggered when the item is dropped
    public float eventCoolDown; // When the player is holding down the use button and constantly using the item, how long it should wait before each use
    public int useLimit; // How many times can this item being used (0 means infinite)
    public bool fixedPosition; // Can this item be picked up by the player or is fixed on the ground
    public MeshRenderer itemStatusIndicator; // The emissive mesh that indicate the item's status
    public int itemStatusIndicatorMaterialIndex; // The index of the material for the indicator
    public MeshRenderer itemEmissiveIndicator; // The emissive mesh that indicate the item's on/off emission
    public float statusIndicatorOnEmissionIntensity; // The emission intensity of the status indicator when it's in the player's camera view
    public float statusIndicatorOffEmissionIntensity; // The emission intensity of the status indicator when it's out of the player's camera view
    public float statusIndicatorLerpIntensityDuration; // The duration of the lerping intensity animation
    public Light statusIndicatorLight; // The light on the status indicator
    public float statusIndicatorLightOnIntensity; // The intensity when the light is on
    public Color defaultStatusColor; // The color of the indicator for default status;
    //public Color inRangeStatusColor; // The color of the indicator for the player's armTip is within using range status;
    public Color isUsingStatusColor; // The color of the indicator for the player is using (attached armTip to the item, but not controlling) status;
    public Color isControllingStatusColor; // The color of the indicator for the player is controlling (is pressing down the trigger) status;
    public Color unusableStatusColor; // The color of the indicator for not usable status;
    public float playerCameraDetectorDistance; // How far is each player camera detector away from the item
    public GameObject playerCameraDetectorPrefab; // The player camera detector prefab
    public float playerDetectingRange; // How close the player has to be with the fixed item to turn on the emission
    public bool isFixedToggle; // Is this a fixed usable item that is only used to toggle on/off some functions
    public bool noPuttingArmOnState; // Is this a fixed usable item that the player only need to press down the trigger to start using

    public float normalDrag; // The item's normal drag
    public float normalAngularDrag; // The item's normal angular drag
    public float normalMass; // The item's normal mass
    public bool isUsingGravity; // Does the item has gravity
    public bool isBeingHeld; // If this item is currently being held by an arm
    public Transform holdingArmTip; // The arm that is holding this item
    public Coroutine usingItem; // The coroutine that continuously triggers the using event
    public int timeUsed; // How many time has this item been used
    public int beingSeenCameraDetectorCount; // The number of player camera detectors that's being seen by the player's camera
    public Coroutine lerpStatusIndicatorEmissionCoroutine; // The current coroutine that is lerping the status indicator's emission
    public bool isIndicatorEmissionOn; // Is the indicator's emission turned on

    // Test
    public Vector4 emissionColorV4; // The emission color in vector4 form
    public Color emissionColor; // The emission color

    // Use this for initialization
    void Start()
    {
        if (GetComponent<Rigidbody>())
        {
            normalDrag = GetComponent<Rigidbody>().drag;
            normalAngularDrag = GetComponent<Rigidbody>().angularDrag;
            normalMass = GetComponent<Rigidbody>().mass;
            itemWeight = GetComponent<Rigidbody>().mass;
            isUsingGravity = GetComponent<Rigidbody>().useGravity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculatingItemTransform();
        ControlFixedItemStatusIndicatorEmission();

        // Test
        if (itemStatusIndicator != null)
        {
            emissionColorV4 = itemStatusIndicator.material.GetColor("_EmissionColor");
            emissionColor = itemStatusIndicator.material.GetColor("_EmissionColor");
            //print(Mathf.Log(emissionColorV4.x * (1f / 0.7490196f), 2));
        }
    }

    /// <summary>
    /// Make an usable item to be unusable
    /// </summary>
    public void MakeItemUnusable()
    {
        canUse = false;
    }

    /// <summary>
    /// Turn on the status indicator if the fixed item moves into player's camera view,
    /// turn off when it leaves the player's camera view
    /// </summary>
    public void ControlFixedItemStatusIndicatorEmission()
    {
        // If there is no emissive indicator on this item
        if (itemEmissiveIndicator == null)
        {
            return;
        }

        // Detect if lerp status indicator emission
        // If the player get close to the fixed item
        if (!isIndicatorEmissionOn && Vector3.Distance(transform.position, PlayerInfo.sPlayerInfo.transform.position) < playerDetectingRange)
        {
            // If there is a lerping animation currently running
            if (lerpStatusIndicatorEmissionCoroutine != null)
            {
                StopCoroutine(lerpStatusIndicatorEmissionCoroutine);
            }
            // "Turn on" the emission
            isIndicatorEmissionOn = true;
            lerpStatusIndicatorEmissionCoroutine =
                StartCoroutine(LerpStatusIndicatorEmission(statusIndicatorOnEmissionIntensity, statusIndicatorLightOnIntensity, statusIndicatorLerpIntensityDuration));
        }
        // If the player move away from the fixed item
        else if (isIndicatorEmissionOn && Vector3.Distance(transform.position, PlayerInfo.sPlayerInfo.transform.position) >= playerDetectingRange)
        {
            // If there is a lerping animation currently running
            if (lerpStatusIndicatorEmissionCoroutine != null)
            {
                StopCoroutine(lerpStatusIndicatorEmissionCoroutine);
            }
            // "Turn off" the emission
            isIndicatorEmissionOn = false;
            lerpStatusIndicatorEmissionCoroutine =
                StartCoroutine(LerpStatusIndicatorEmission(statusIndicatorOffEmissionIntensity, 0, statusIndicatorLerpIntensityDuration));
        }

        /*
        // Stored the number of seen camera detectors in the last frame
        int previousSeenCameraDetectorCount = beingSeenCameraDetectorCount;

        // Count the number of currently seen detectors
        beingSeenCameraDetectorCount = 0;
        foreach (DetectPlayerCameraViewRange d in playerCameraDetectors)
        {
            if (d.isInCamera)
            {
                beingSeenCameraDetectorCount++;
            }
        }

        // Detect if lerp status indicator emission
        // If the item enters the player's camera view
        if (previousSeenCameraDetectorCount < 4 && beingSeenCameraDetectorCount == 4)
        {
            // If there is a lerping animation currently running
            if (lerpStatusIndicatorEmissionCoroutine != null)
            {
                StopCoroutine(lerpStatusIndicatorEmissionCoroutine);
            }
            // "Turn on" the emission
            lerpStatusIndicatorEmissionCoroutine =
                StartCoroutine(LerpStatusIndicatorEmission(statusIndicatorOnEmissionIntensity, statusIndicatorLightOnIntensity, statusIndicatorLerpIntensityDuration));
        }
        // If the item leaves the player's camera view
        if (previousSeenCameraDetectorCount == 4 && beingSeenCameraDetectorCount < 4)
        {
            // If there is a lerping animation currently running
            if (lerpStatusIndicatorEmissionCoroutine != null)
            {
                StopCoroutine(lerpStatusIndicatorEmissionCoroutine);
            }
            // "Turn off" the emission
            lerpStatusIndicatorEmissionCoroutine =
                StartCoroutine(LerpStatusIndicatorEmission(statusIndicatorOffEmissionIntensity, 0, statusIndicatorLerpIntensityDuration));
        }
        */
    }

    /// <summary>
    /// Lerp the emission and light intensity on the status indicator
    /// </summary>
    /// <param name="endEmissionIntensity"></param>
    /// <param name="endLightIntensity"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator LerpStatusIndicatorEmission(float endEmissionIntensity, float endLightIntensity, float duration)
    {
        // Get the current emission and light intensity
        Vector4 startColor = itemEmissiveIndicator.material.GetColor("_EmissionColor");
        float currentEmissionIntensity = Mathf.Log(startColor.x * (1f / 0.7490196f), 2);
        float currentLightIntensity = statusIndicatorLight.intensity;

        // Lerp the emission intensity
        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            itemEmissiveIndicator.material.SetColor("_EmissionColor", GetHDRcolor.GetColorInHDR(defaultStatusColor, Mathf.Lerp(currentEmissionIntensity, endEmissionIntensity, t)));
            statusIndicatorLight.intensity = Mathf.Lerp(currentLightIntensity, endLightIntensity, t);
            yield return null;
        }

        itemEmissiveIndicator.material.SetColor("_EmissionColor", GetHDRcolor.GetColorInHDR(defaultStatusColor, endEmissionIntensity));
        lerpStatusIndicatorEmissionCoroutine = null;
    }

    /// <summary>
    /// Change the emissive color on the indicator
    /// </summary>
    /// <param name="newColor"></param>
    /// <param name="emissionIntensity"></param>
    public void ChangeIndicatorColor(Color newColor, float emissionIntensity)
    {
        // Change Albedo color
        itemStatusIndicator.materials[itemStatusIndicatorMaterialIndex].color = newColor;
        // Change emissive color
        itemStatusIndicator.materials[itemStatusIndicatorMaterialIndex].SetColor("_EmissionColor", GetHDRcolor.GetColorInHDR(newColor, statusIndicatorOnEmissionIntensity));

        // If the player dropped the item, then set emission color to default
        if (newColor == Color.black)
        {
            itemStatusIndicator.materials[itemStatusIndicatorMaterialIndex].SetColor("_EmissionColor", GetHDRcolor.GetColorInHDR(defaultStatusColor, statusIndicatorOnEmissionIntensity));
        }
    }

    public void CalculatingItemTransform()
    {
        if (isBeingHeld)
        {
            // If the item is not actually being hold by player arm
            if (holdingArmTip == null)
            {
                return;
            }
            // If the item can be lifted by the arm then match its transform with the armTip
            if (!fixedPosition && itemWeight <= holdingArmTip.GetComponentInParent<ControlArm>().armLiftingStrength)
            {
                //GetComponent<Rigidbody>().velocity = Vector3.zero;
                //GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                transform.eulerAngles = new Vector3(0, holdingArmTip.GetComponentInParent<ControlArm>().joyStickRotationAngle, 0);
                //transform.position = holdingArm.position;
                //print(holdingArm.position);
            }
        }
    }

    public void ForceDropItem()
    {
        // If the item is being holding by an armTip
        if (holdingArmTip != null)
        {
            StopUsing(); // Stop using the item
            holdingArmTip.GetComponentInParent<ControlArm>().DropDownItem(gameObject);
        }
    }

    /// <summary>
    /// Setup this item
    /// </summary>
    public void SetupItem()
    {
        setupEvent.Invoke();

        // If this is a fixed item that is used for toggle on/off
        if (isFixedToggle)
        {
            // Invoke the using event
            StartUsing();
            // Let the item be dropped from the armTip
            ForceDropItem();
        }

        // If this is a fixed item that the player does not need to "start put the arm on the item",
        // means as soon as the player's arm is within the item range and press down the trigger, 
        // the item should begin to be used by the player
        if (noPuttingArmOnState)
        {
            // Invoke the using event
            StartUsing();
        }
    }

    /// <summary>
    /// The toggle event for a toggle item
    /// </summary>
    /// <returns></returns>
    public IEnumerator ToggleEvent()
    {
        yield return null;
    }

    /// <summary>
    /// Start using this item
    /// </summary>
    public void StartUsing()
    {
        if (itemStatusIndicator != null)
        {
            // Change the status indicator color
            ChangeIndicatorColor(isControllingStatusColor, 1);
        }

        usingItem = StartCoroutine(UsingItem());
    }

    /// <summary>
    /// Stop using this item
    /// </summary>
    public void StopUsing()
    {
        // If the user is using the item
        if (usingItem != null)
        {
            StopCoroutine(usingItem);

            // If there is an event for when the item is stopped being used, then triggers it
            if (stopUsingEvent != null)
            {
                stopUsingEvent.Invoke();
            }

            usingItem = null;

            if (itemStatusIndicator != null)
            {
                // Change the status indicator color
                ChangeIndicatorColor(isUsingStatusColor, 1);
            }

            // If this is a fixed item that the player does not need to "start put the arm on the item",
            // means as soon as the player's arm is within the item range and press down the trigger, 
            // the item should begin to be used by the player
            if (noPuttingArmOnState)
            {
                // Let the item be dropped from the armTip
                ForceDropItem();
            }
        }
    }

    public void ResetItem()
    {
        StopUsing(); // Stop using the item
        resetEvent.Invoke(); // Reset the item
    }

    /// <summary>
    /// The coroutine that keep using the item
    /// </summary>
    /// <returns></returns>
    public IEnumerator UsingItem()
    {
        // Keep using the item (maybe need to change "while (true)" to "while (still can be used)")
        while (true)
        {
            singleUseEvent.Invoke();

            timeUsed++; // Increase the counter of how many times it has been used
            // Stop the item from been used if it has a limited use and the limit is reached
            if (useLimit != 0 && useLimit == timeUsed)
            {
                canUse = false;
                StopCoroutine(usingItem);
            }

            if (eventCoolDown == 0)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(eventCoolDown);
            }
        }
    }
}
