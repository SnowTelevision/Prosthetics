using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Starts unity events when something enter its trigger area
/// </summary>
public class TriggerDetectStartEvent : MonoBehaviour
{
    public EventInfo[] eventsToBeTriggered; // The events to be triggered
    public string detectLayer; // If this trigger varifies the collider's layer
    public string detectName; // If this trigger varifies the collider's name
    public GameObject detectGameObject; // If this trigger varifies the collider's gameobject

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
        // If this trigger check against layer and the collider's layer matches
        if (detectLayer != "" && other.gameObject.layer == LayerMask.NameToLayer(detectLayer))
        {
            TriggerEvents(false);
        }
        // If this trigger check against name and the collider's name matches
        else if (detectName != "" && other.name == detectName)
        {
            TriggerEvents(false);
        }
        // If this trigger check against gameObject and the collider's gameObject matches
        else if (detectGameObject != null && other.gameObject == detectGameObject)
        {
            TriggerEvents(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If this trigger check against layer and the collider's layer matches
        if (detectLayer != "" && other.gameObject.layer == LayerMask.NameToLayer(detectLayer))
        {
            TriggerEvents(true);
        }
        // If this trigger check against name and the collider's name matches
        else if (detectName != "" && other.name == detectName)
        {
            TriggerEvents(true);
        }
        // If this trigger check against gameObject and the collider's gameObject matches
        else if (detectGameObject != null && other.gameObject == detectGameObject)
        {
            TriggerEvents(true);
        }
    }

    /// <summary>
    /// Trigger events if they can still be triggered
    /// </summary>
    /// <param name="triggerOnExit"></param>
    public void TriggerEvents(bool triggerOnExit)
    {
        foreach (EventInfo e in eventsToBeTriggered)
        {
            if (e.triggerOnExit == triggerOnExit &&
                e.timeHappened < e.eventRepeatNumber)
            {
                e.timeHappened++;
                e.eventToHappen.Invoke();
            }
        }
    }
}

/// <summary>
/// Stores information about an event
/// </summary>
[Serializable]
public class EventInfo
{
    public UnityEvent eventToHappen; // The unity event that will be invoked
    public float eventRepeatNumber; // How many time can this event be triggered
    public bool triggerOnExit; // Does this event triggers on collider exit or enter

    public int timeHappened; // How many time has the event been invoked
}
