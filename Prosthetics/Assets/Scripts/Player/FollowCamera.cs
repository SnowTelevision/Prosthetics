using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public float chaseSpeed;
    public GameObject target;
    public Vector3 offset;

    Vector3 targetPos;

    // Custom use
    public bool isMainGameCamera; // Is this the main game camera
    public float sideScrollerCamRadius; // The radius of the camera's orbit in side-scrolling session

    public float defaultCameraHeight; // The default height of the camera in top-down mode
    public static FollowCamera mainGameCamera; // The main camera in the game
    public float targetHeight; // The target camera height

    //Test
    public bool test;

    // Use this for initialization
    void Start()
    {
        targetPos = transform.position;
        // Get the camera's default height
        defaultCameraHeight = transform.position.y;
        targetHeight = defaultCameraHeight;

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
            if (!PlayerInfo.isSideScroller) // Controls the camera in top-down mode
            {
                Vector3 posNoY = transform.position;
                posNoY.y = target.transform.position.y;

                Vector3 targetDirection = (target.transform.position - posNoY);

                interpVelocity = targetDirection.magnitude * chaseSpeed;

                targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
                // Get the height offset
                offset.y = (targetHeight - transform.position.y) * interpVelocity * Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
            }
            else // Controls the camera in side-scroller mode
            {
                Vector3 targetDirection = new Vector3(sideScrollerCamRadius / PlayerInfo.sPlayerInfo.targetSideScrollOrbitRadius * target.transform.position.x,
                                                      target.transform.position.y,
                                                      sideScrollerCamRadius / PlayerInfo.sPlayerInfo.targetSideScrollOrbitRadius * target.transform.position.z) -
                                          transform.position;

                //if (test)
                //{
                //    print(new Vector3(targetDirection.x, 0, targetDirection.z).magnitude);
                //}

                interpVelocity = targetDirection.magnitude * chaseSpeed;

                targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

                transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);

                // Rotate Camera
                transform.LookAt(new Vector3(0, transform.position.y, 0), Vector3.up);
                transform.eulerAngles += new Vector3(0, 180, 0);
            }
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
            mainGameCamera.targetHeight = cameraHeight;
        }
    }
}

// Original post with image here  >  http://unity3diy.blogspot.com/2015/02/unity-2d-camera-follow-script.html