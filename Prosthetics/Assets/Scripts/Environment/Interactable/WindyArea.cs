using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the wind strength of a windy area and give force to the player's arms if they entered the area
/// </summary>
public class WindyArea : MonoBehaviour
{
    public float windStrength; // The strength of the wind

    // Test
    public bool test;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.GetComponent<ArmUseItem>()) // If this is an armTip
        //{
        //    other.GetComponentInParent<ControlArm_UsingPhysics>().armCurrentTotalReceivedWindForce += windStrength * transform.forward;
        //}

        if (other.GetComponent<ArmSegmentReceiveAirResistance>()) // If this is an arm segment
        {
            other.GetComponent<ArmSegmentReceiveAirResistance>().armSegmentCurrentTotalReceivedWindForce += windStrength * transform.forward;
        }
    }
}
