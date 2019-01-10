using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the behavior and the functions of the security spot light in lv2
/// </summary>
public class SecuritySpotLightController : MonoBehaviour
{
    public Transform spotLightTransform; // The spot light's transform
    public Transform spotLightRoute; // The patrol route the spot light should be following while patroling (not chasing an object)
    public Transform objectDetectorWrap; // The transform for the wrap of the object detector trigger
    public DetectCollision objectDetector; // The trigger representing the spot light's detecting range
    public float spotLightAngle; // How wide is the spot light's angle
    public float spotLightMaxChaseSpeed; // How fast the spot light can chase an object
    public bool chasePlayer; // Does this spot light chase player

    public float spotLightDefaultIntensity; // The spot light's default intensity
    public float spotLightDefaultRange; // The spot light's default range
    public float distanceToWaterSurface; // How far is the spot light's origin to the water's surface
    public Transform spotLightFollowingTransform; // Which transform the spot light should follow
    public Coroutine spotLightReturnPatrolCoroutine; // The coroutine when the spot light is returning to patrol
    public GameObject objectChaser; // The empty gameObject that chase behind the spot light's chasing object

    // Use this for initialization
    void Start()
    {
        InitializeSpotLight();
    }

    // Update is called once per frame
    void Update()
    {
        MoveSpotLight();
        CheckSpotLightArea();
        AutoReplayPatrolRoute();
    }

    /// <summary>
    /// Move the spot light
    /// </summary>
    public void MoveSpotLight()
    {
        // If the spot light is not chasing anything then it should patrol
        if (spotLightFollowingTransform == null)
        {
            SpotLightPatrol();
        }
        else
        {
            SpotLightChase();
        }
    }

    /// <summary>
    /// Spot light patrols a route
    /// </summary>
    public void SpotLightPatrol()
    {
        // Make the spot light aim the patrol route
        spotLightTransform.LookAt(spotLightRoute);
    }

    /// <summary>
    /// Spot light chase an object
    /// </summary>
    public void SpotLightChase()
    {
        // Get the displacement between the following object and the object chaser
        Vector3 targetDis = spotLightFollowingTransform.position - objectChaser.transform.position;
        // Move the object chaser towards the following object
        objectChaser.transform.position +=
            (targetDis).normalized *
            Mathf.Clamp(targetDis.magnitude, 0, spotLightMaxChaseSpeed * Time.deltaTime);

        // Make the spot light aim the object chaser
        spotLightTransform.LookAt(objectChaser.transform);
    }

    /// <summary>
    /// Check if the spot light shines on something
    /// </summary>
    public void CheckSpotLightArea()
    {
        // If the spot light should chase player
        if (chasePlayer)
        {
            // If the spot light spot on player
            if (objectDetector.isEnteringCollider && objectDetector.enteringCollider.gameObject == GameManager.sPlayer)
            {
                if (spotLightFollowingTransform == null || spotLightFollowingTransform.gameObject != GameManager.sPlayer)
                {
                    SpotLightDetectPlayer();
                }
            }

            // If the spot light does not spot on player
            if (!objectDetector.isEnteringCollider || objectDetector.enteringCollider.gameObject != GameManager.sPlayer)
            {
                if (spotLightFollowingTransform != null && spotLightFollowingTransform.gameObject == GameManager.sPlayer)
                {
                    PlayerEscapedSpotLight();
                }
            }
        }
    }

    /// <summary>
    /// If the spot light detected player
    /// </summary>
    public void SpotLightDetectPlayer()
    {
        if (spotLightReturnPatrolCoroutine != null)
        {
            StopCoroutine(spotLightReturnPatrolCoroutine);
            spotLightReturnPatrolCoroutine = null;
        }

        spotLightFollowingTransform = objectDetector.enteringCollider.transform;

        // Increase the spot light's intensity
        GetComponentInChildren<Light>().intensity = spotLightDefaultIntensity * 2f;

        // Pause the patrol route
        spotLightRoute.GetComponent<LinearObjectMovement>().pause = true;

        // If there is already an object chaser
        if (objectChaser != null)
        {
            // Destroy the player chaser
            Destroy(objectChaser);
        }

        // Create an empty object that follows the player
        objectChaser = Instantiate(new GameObject(), CalculateSpotLightIntersectingPositionWithWater(), Quaternion.identity);
        objectChaser.transform.parent = transform;
    }

    /// <summary>
    /// If the player escaped the spot light area
    /// </summary>
    public void PlayerEscapedSpotLight()
    {
        spotLightFollowingTransform = spotLightRoute;

        // Decrease the spot light's intensity
        GetComponentInChildren<Light>().intensity = spotLightDefaultIntensity * 0.5f;

        // Return to the patrol route
        spotLightReturnPatrolCoroutine = StartCoroutine(SpotLightReturnPatrol());
    }

    /// <summary>
    /// Spot light return to the patrol route when it is no longer chasing an object
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpotLightReturnPatrol()
    {
        // While the spot light is not back to the patrol route
        int whileStopper = int.MaxValue;

        while ((Mathf.Abs(objectChaser.transform.position.x - spotLightRoute.position.x) > 0.1f ||
                Mathf.Abs(objectChaser.transform.position.z - spotLightRoute.position.z) > 0.1f) &&
               whileStopper > 0)
        {
            yield return null;
        }

        // Resume patrol
        spotLightRoute.GetComponent<LinearObjectMovement>().pause = false;
        spotLightFollowingTransform = null;
        spotLightReturnPatrolCoroutine = null;

        // Destroy the player chaser
        Destroy(objectChaser);
        objectChaser = null;
    }

    public void AutoReplayPatrolRoute()
    {
        // Restart the patrol route when it finishes one loop
        if (spotLightRoute.GetComponent<LinearObjectMovement>().animationFinished)
        {
            spotLightRoute.GetComponent<LinearObjectMovement>().StartAnimation();
        }
    }

    /// <summary>
    /// Initialize the spot light parameters and the detector's transform
    /// </summary>
    public void InitializeSpotLight()
    {
        spotLightDefaultIntensity = GetComponentInChildren<Light>().intensity;
        spotLightDefaultRange = GetComponentInChildren<Light>().range;

        // Set the spot light's angle
        GetComponentInChildren<Light>().spotAngle = spotLightAngle;
        // Set the object detector's local scale
        objectDetectorWrap.localScale =
            new Vector3(GetComponentInChildren<Light>().range * Mathf.Tan(Mathf.Deg2Rad * GetComponentInChildren<Light>().spotAngle * 0.5f) * 0.9f,
                        GetComponentInChildren<Light>().range * Mathf.Tan(Mathf.Deg2Rad * GetComponentInChildren<Light>().spotAngle * 0.5f) * 0.9f,
                        GetComponentInChildren<Light>().range);

        // Start the patrol route
        spotLightRoute.GetComponent<LinearObjectMovement>().StartAnimation();
    }

    /// <summary>
    /// Calculate the position where the spot light's forward vector intersect with the water
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateSpotLightIntersectingPositionWithWater()
    {
        // Get the spot light's altitude
        float spotLightHeight = spotLightTransform.position.y;

        // Get the base position to calculate the final position
        Vector3 basePosition = new Vector3(spotLightTransform.position.x, 0, spotLightTransform.position.z);

        // Calculate the x and z distance from the base position based on the forward unit vector and spot light height
        float xDistance = spotLightHeight / (-spotLightTransform.forward.y) * spotLightTransform.forward.x;
        float zDistance = spotLightHeight / (-spotLightTransform.forward.y) * spotLightTransform.forward.z;

        // Get the final position
        basePosition.x += xDistance;
        basePosition.z += zDistance;

        return basePosition;
    }
}
