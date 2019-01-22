using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Send the player back to check point if this collide with the player
/// </summary>
public class PlayerGetShotSendBack : MonoBehaviour
{
    public Vector3 sendBackPosition; // Where the player should be sent back
    public float scriptedEventPauseTime; // How long the game should pause after the player is sent back
    public string tutorialTextToDisplay; // What to display as the tutorial text after the player is sent back

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "PlayerBody")
        {
            CreateSendBackEvent();
        }
    }

    /// <summary>
    /// Create an empty gameobject that will start the send back event coroutine
    /// </summary>
    public void CreateSendBackEvent()
    {
        GameObject eventController = Instantiate(new GameObject());
        eventController.AddComponent<PlayerGetShotSendBack>();
        eventController.GetComponent<PlayerGetShotSendBack>().StartSendBackEvent(scriptedEventPauseTime, tutorialTextToDisplay, sendBackPosition);
    }

    /// <summary>
    /// Start the send back coroutine
    /// </summary>
    /// <param name="pauseTime"></param>
    /// <param name="tutorialText"></param>
    /// <param name="sendBackPosition"></param>
    public void StartSendBackEvent(float pauseTime, string tutorialText, Vector3 sendBackPosition)
    {
        GameManager.sPlayer.transform.position = sendBackPosition;
        GameManager.sTutorialText.text = tutorialText;
        GameManager.sTutorialText.transform.parent.gameObject.SetActive(true);
        GameManager.inScriptedEvent = true;

        StartCoroutine(SendBackEvent(pauseTime));
    }

    /// <summary>
    /// The Send back event coroutine
    /// </summary>
    /// <param name="pauseTime"></param>
    /// <returns></returns>
    public IEnumerator SendBackEvent(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
        GameManager.sTutorialText.transform.parent.gameObject.SetActive(false);
        GameManager.sTutorialText.text = "";
        GameManager.inScriptedEvent = false;
        Destroy(gameObject);
    }
}
