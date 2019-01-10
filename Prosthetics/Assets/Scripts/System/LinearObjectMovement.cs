using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to animate the movement of any gameobject from point A to point B in a straight line at a constant speed
/// </summary>
public class LinearObjectMovement : MonoBehaviour
{
    public MovementInfo[] animationSequence; // The array that stores the information of each segment of the entire animation sequence
    public GameObject objectToBeAnimated; // The object that is going to be animated by this script. If this is null, then it will default to the GameObject the script is attached to
    public bool loopable; // Is this animation loopable
    public bool destroyGameObject; // Should the animated GameObject be destroyed after the animation finish

    public bool animationFinished; // Is the animation finished
    public bool pause; // Is the animation paused
    public float endDistanceTolerance; // How close the object has to be with the target position for the segment to finish
    public bool isAnimationRunning; // Is a segment of the animation sequence currently running

    // Testing
    public bool testing; // Is this been tested

    // Use this for initialization
    void Start()
    {
        //animationFinished = false;
        //pause = false;
        // Testing purpose
        //StartCoroutine(Animate());
        //isAnimationRunning = false;
    }

    // Testing
    private void Update()
    {
        if (testing)
        {
            StartCoroutine(Animate());
            testing = false;
        }
    }

    /// <summary>
    /// Resume the animation
    /// </summary>
    public void ResumeAnimation()
    {
        pause = false;
    }

    /// <summary>
    /// Start the animation sequence
    /// </summary>
    public void StartAnimation()
    {
        // Reset the animation finished flag if the animation is loopable
        if (loopable)
        {
            animationFinished = false;
        }

        StartCoroutine(Animate());
    }

    /// <summary>
    /// Destroy this component instance
    /// </summary>
    public void DestroyThis()
    {
        Destroy(this);
    }

    /// <summary>
    /// The coroutine that controls the entire animation sequence
    /// </summary>
    /// <returns></returns>
    public IEnumerator Animate()
    {
        int whileStopper = 0;
        Vector3 newPosition;
        Vector3 movementVector; // The velocity the animated object should be in a Vector3 form
        if (objectToBeAnimated == null)
        {
            objectToBeAnimated = gameObject;
        }

        for (int i = 0; i < animationSequence.Length; i++)
        {
            whileStopper = 0;

            // Store the initial start and target locations
            Vector3 initialStartPosition = animationSequence[i].startLocalPosition;
            Vector3 initialTargetPosition = animationSequence[i].targetLocalPosition;

            // If starting position is null or zero, it will be the object's current position
            if (animationSequence[i].startLocalPosition == null || animationSequence[i].startLocalPosition == Vector3.zero)
            {
                animationSequence[i].startLocalPosition = objectToBeAnimated.transform.localPosition;
            }

            if (!animationSequence[i].targetShouldBeZero)
            {
                // If one (or more) of the axis of target position is 0, then it will be the same as the object's current local axis coord
                if (animationSequence[i].targetLocalPosition.x == 0)
                {
                    animationSequence[i].targetLocalPosition.x = objectToBeAnimated.transform.localPosition.x;
                }
                if (animationSequence[i].targetLocalPosition.y == 0)
                {
                    animationSequence[i].targetLocalPosition.y = objectToBeAnimated.transform.localPosition.y;
                }
                if (animationSequence[i].targetLocalPosition.z == 0)
                {
                    animationSequence[i].targetLocalPosition.z = objectToBeAnimated.transform.localPosition.z;
                }
            }

            // Calculating the velocity in Vector3 form
            movementVector = (animationSequence[i].targetLocalPosition - animationSequence[i].startLocalPosition) / (Vector3.Distance(animationSequence[i].startLocalPosition, animationSequence[i].targetLocalPosition) / animationSequence[i].moveSpeed);
            movementVector.x *= objectToBeAnimated.transform.localScale.x / objectToBeAnimated.transform.lossyScale.x;
            movementVector.y *= objectToBeAnimated.transform.localScale.y / objectToBeAnimated.transform.lossyScale.y;
            movementVector.z *= objectToBeAnimated.transform.localScale.z / objectToBeAnimated.transform.lossyScale.z;
            //print(movementVector);

            // While the object is not at the target position, moving the object
            while (whileStopper < 100000000 && (Mathf.Abs(objectToBeAnimated.transform.localPosition.x - animationSequence[i].targetLocalPosition.x) > endDistanceTolerance
                                             || Mathf.Abs(objectToBeAnimated.transform.localPosition.y - animationSequence[i].targetLocalPosition.y) > endDistanceTolerance
                                             || Mathf.Abs(objectToBeAnimated.transform.localPosition.z - animationSequence[i].targetLocalPosition.z) > endDistanceTolerance))
            {
                while (pause) // Pause the animation
                {
                    isAnimationRunning = false;
                    yield return null;
                }

                isAnimationRunning = true;
                newPosition = objectToBeAnimated.transform.localPosition + movementVector * Time.deltaTime;
                objectToBeAnimated.transform.localPosition = newPosition;
                whileStopper++;
                yield return null;
            }

            objectToBeAnimated.transform.localPosition = animationSequence[i].targetLocalPosition;
            isAnimationRunning = false;

            // If the animation is loopable then restore its start and target position to its initial values set in the inspector
            if (loopable)
            {
                animationSequence[i].startLocalPosition = initialStartPosition;
                initialTargetPosition = animationSequence[i].targetLocalPosition = initialTargetPosition;
            }

            if (animationSequence[i].pauseBeforeNext)
            {
                pause = true;
            }

            while (pause) // Pause the animation
            {
                yield return null;
            }

            yield return new WaitForSeconds(animationSequence[i].waitTimeBeforeNext);
        }

        animationFinished = true;

        // If the animation is not loopable then just destroy this component
        if (!loopable)
        {
            yield return new WaitForSeconds(animationSequence[animationSequence.Length - 1].waitTimeBeforeNext);

            // If the animated object should be destroy
            if (destroyGameObject)
            {
                Destroy(objectToBeAnimated);
            }
        }
    }
}

/// <summary>
/// Stores information about the start position and the target position of the current segment of animation, 
/// the constant speed that the object will move at, and how much time to wait before starting the next segment
/// </summary>
[Serializable]
public class MovementInfo
{
    public Vector3 startLocalPosition; // The start position of the object. If it is null or zero, it will be the object's current position
    //public bool start
    public Vector3 targetLocalPosition; // The target position of the object. If an axis is 0, then it will not move along that axis
    public bool targetShouldBeZero; // Should the target location be zero
    public float moveSpeed; // The move speed of the object for this segment
    public float waitTimeBeforeNext; // The waiting time before starting of the next segment
    public bool pauseBeforeNext; // Should the animation pause after the completion of this segment and before the next segment
}
