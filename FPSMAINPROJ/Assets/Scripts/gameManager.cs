using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] TMP_Text roundCount;
  
    //Objects
    private EnemySpawner enemySpawner;
    private GameObject enemy;


    //int variables 
    public int EnemyCount;

    public int GameRound;

    // Private reference for the Player
    private GameObject _Player;
    private bool isNewEnemies;
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

        enemySpawner = FindObjectOfType<EnemySpawner>();

        //CheckForEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        // Pause Menu Logic
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePause();
        }
        /* if(isNewEnemies)
         {
             CheckForEnemies();
         }*/

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
    public void UpdateGameGoal(int amount)
    {

        if(GameRound == 0)
        {
            GameRound = 1;
        }

        if(EnemyCount < 0)
        {
            EnemyCount = 0;
        }

        EnemyCount += amount;

        Debug.Log("enemies " + EnemyCount.ToString());
        if (EnemyCount <= 0)
        {
            GameRound++;
            roundCount.text = GameRound.ToString("F0");
            enemySpawner.SpawnZombies(GameRound);
            // isNewEnemies = true;
        }
    }
}


/*    public void CheckForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject enemy in enemies)
        {
            UpdateGameGoal(1);
        }
        isNewEnemies = false;
    }    
}*/
