using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    // Private static instance to the gameManager
    private static gameManager _gameInstance;

    // Public static property to access the instance
    public static gameManager gameInstance
    { 
        get
        {
            if (_gameInstance == null)
            {
                Debug.LogError("GameManager is null");
            }
            return _gameInstance;
        }
    }

    // Serialized Variables
    [SerializeField] GameObject gameActiveMenu;
    [SerializeField] GameObject gameMainMenu;
    [SerializeField] GameObject gamePauseMenu;
    [SerializeField] GameObject gameWinMenu;
    [SerializeField] GameObject gameLoseMenu;

    // Private reference for the Player
    private GameObject _Player;

    // Public property to access the Player
    public GameObject player
    {
        get { return _Player; }
        private set { _Player = value; }
    }

    // Private access to player script
    // private playerController _PlayerScript;
    // Public property to access the player script
    // public playerController playerScript
    // {
    //      get { return _PlayerScript; }
    //      private set { playerScript = value; }
    // }

    // Private pause state control variable
    private bool _GameIsPaused;

    // Public property to access the pause state
    public bool gameIsPaused
    {
        get { return _GameIsPaused; }
        private set { _GameIsPaused = value; }
    }


    // Using Awake, for Manager
    void Awake()
    {
        // If instance already exists and it is not the game Manager, destroy this instance
        if (_gameInstance != null && _gameInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _gameInstance = this;
        }

        // Set the references of the player and it's script
        player = GameObject.FindWithTag("Player");
        // playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Pause Menu Logic
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePause();
        }
    }

    // Pause the Game
    public void PauseGame()
    {
        // Toggle the pause state of the game
        gameIsPaused = !gameIsPaused;
        // Adjust the game's time scale
        Time.timeScale = 0;
        // Adjust the cursor visibility and restrictions
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Unpause the Game
    private void UnpauseGame()
    {
        gameIsPaused = !gameIsPaused;
        // Adjust the timescale
        Time.timeScale = 0;
        // Readjust the cursor properties and it's visibility
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // Switch off of the Pause menu
        gameActiveMenu.SetActive(gameIsPaused);
        gameActiveMenu = null;

    }
    // Toggle Between Pause State
    private void TogglePause()
    {
        // Check active menu to toggle between
        if (gameActiveMenu == null || gameActiveMenu == gamePauseMenu)
        {
            if (gameIsPaused)
            {
                UnpauseGame();
            }
            else
            {
                // Bring up the pause menu
                gameActiveMenu = gamePauseMenu;
                gameActiveMenu.SetActive(gameIsPaused);
                PauseGame();
            }
        }
    }
}
