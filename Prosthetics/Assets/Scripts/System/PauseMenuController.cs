using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Let the player control the pause menu
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu; // The UI for pause menu
    public GameObject[] subLevelMenus; // The second/third... level menus
    public EventSystem sceneEventSystem; // The event system in the scene
    public GameObject firstSelectedButtonInPauseMenu; // Which button should be selected first when the player opens the pause menu

    public bool menuOpened; // Is the pause menu opened

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the player press the "start" button on the controller
        if (Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            // If the pause menu is not opened
            if (!menuOpened)
            {
                OpenMenu();
            }
            // If the pause menu is opened
            else
            {
                CloseMenu();
            }
        }
    }

    /// <summary>
    /// Open the pause menu
    /// </summary>
    public void OpenMenu()
    {
        // Pause the game
        GameManager.PauseGame();
        // Turn on the pauseMenu
        pauseMenu.SetActive(true);
        // Set the first selected button
        sceneEventSystem.firstSelectedGameObject = firstSelectedButtonInPauseMenu;
        // Highlight the selected button
        firstSelectedButtonInPauseMenu.GetComponent<Button>().Select();
    }

    /// <summary>
    /// Close the pause menu
    /// </summary>
    public void CloseMenu()
    {
        // Reset the pause menu to first level
        ResetMenu();
        // Turn off the pauseMenu
        pauseMenu.SetActive(false);
        // Unpause the game
        GameManager.StaticUnpauseGame();
    }

    /// <summary>
    /// Reset the pause menu to first level for next use
    /// </summary>
    public void ResetMenu()
    {
        // Deactivate non-top level menus
        foreach (GameObject g in subLevelMenus)
        {
            g.SetActive(false);
        }
    }

    /// <summary>
    /// When the player press the "Restart Chapter" button in the pause menu
    /// </summary>
    public void RestartChapterButton()
    {

    }

    /// <summary>
    /// When the player press the "Load Last Checkpoint" button
    /// </summary>
    public void LoadCheckPointButton()
    {
        // Unpause the game before load scene
        GameManager.StaticUnpauseGame();
        // Reload the current scene, this will put the player to the last checkpoint
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Delete the save on current chapter and restart from the beginning
    /// </summary>
    public void DeletaSaveAndRestart()
    {
        ES3.DeleteKey("PlayerPosition");
        LoadCheckPointButton();
    }

    /// <summary>
    /// Test load scene
    /// </summary>
    public void TestLoadScene()
    {
        // Unpause the game before load scene
        //GameManager.StaticUnpauseGame();
        //GameManager.gamePause = false;
        //Time.timeScale = 1;
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
