using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save player's progress when he pass a check point
/// </summary>
public class SaveWhenPlayerPassCheckPoint : MonoBehaviour
{

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
        // If the player enters the check point
        if (other.gameObject == GameManager.sPlayer)
        {
            // Save the player's position
            ES3.Save<Vector3>("PlayerPosition", transform.position);
            // Prevent duplicate save
            this.enabled = false;
        }
    }
}
