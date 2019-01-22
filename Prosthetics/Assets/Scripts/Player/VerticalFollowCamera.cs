using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalFollowCamera : MonoBehaviour
{
    public float chaseSpeed; // How fast the camera moves
    public GameObject target; // The following target
    public Vector3 offset; // Offset of the camera
    public bool isMainGameCamera; // Is this the main game camera

    public float defaultCameraDistance; // The default distance of the camera from the player
    public static VerticalFollowCamera mainGameCamera; // The main camera in the game
    public float targetDistance; // The target camera height
    public float interpVelocity; // The camera move speed each fixedupdate
    Vector3 targetPos; // Camera's target position

    //Test
    public bool test;

    // Use this for initialization
    void Start()
    {
        targetPos = transform.position;
        // Get the camera's default height
        defaultCameraDistance = transform.position.z;
        targetDistance = defaultCameraDistance;

        // Set up as main game camera
        if (isMainGameCamera)
        {
            mainGameCamera = this;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (test)
        {


            return;
        }

        if (target)
        {
            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * chaseSpeed;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
            // Get the height offset
            offset.z = (targetDistance - transform.position.z) * interpVelocity * Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);

        }
    }

    /// <summary>
    /// Change the camera's following object
    /// </summary>
    /// <param name="newFollowingObject"></param>
    /// <param name="cameraHeight"></param>
    public static void CameraChangeFollowingTarget(GameObject newFollowingObject, float cameraHeight)
    {
        if (mainGameCamera.target != newFollowingObject)
        {
            // Change following target
            mainGameCamera.target = newFollowingObject;
            // Change the camera offset
            mainGameCamera.targetDistance = cameraHeight;
        }
    }
}

// Original post with image here  >  http://unity3diy.blogspot.com/2015/02/unity-2d-camera-follow-script.html
