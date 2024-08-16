using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    private float lastRoundStartTime = 0f;
    [SerializeField] private float minRoundDuration = 10f;
    // Serialized Variables
    [SerializeField] GameObject gameActiveMenu;
    [SerializeField] GameObject gameMainMenu;
    [SerializeField] GameObject gamePauseMenu;
    [SerializeField] GameObject gameWinMenu;
    [SerializeField] GameObject gameLoseMenu;
    [SerializeField] TMP_Text roundCount;
    [SerializeField] TMP_Text enemyCount;
    [SerializeField] float RoundDelay;
    public GameObject flashDamage;
    public GameObject playerInteract;
    public Image playerHPBar;
    

    //Objects
    private EnemySpawner enemySpawner;
    private BufferSpawner bufferSpawner;
    private GameObject enemy;
    private bool isCheckingEnemyCount = false;
    private bool isNewRoundStarting = false;


    //int variables 
    int EnemyCount;

    int GameRound;

    // Private reference for the Player
    private GameObject _Player;
    private bool isNewEnemies;
    // Public property to access the Player
    public GameObject player
    {
        get { return _Player; }
        private set { _Player = value; }
    }

    private Camera camera;
    public Camera MainCam
    {
        get { return camera; }
        private set { camera = value; }
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

    public bool hasWinCondition;
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
        enemySpawner.PopulateSpawnPoints();
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found.");
        }
        else
            Debug.Log("Enemy Spawner Valid");

        bufferSpawner = FindObjectOfType<BufferSpawner>(); 
        if (bufferSpawner == null)
        {
           Debug.LogError("EnemySpawner not found.");
        }
        else
            Debug.Log("Enemy Spawner Valid");
       
        MainCam = Camera.main;

        if(EnemyCount == 0)
        {
            StartNewRound();
        }
       
    }


    // Update is called once per frame
    void Update()
    {
        // Pause Menu Logic
        if (Input.GetButtonDown("Cancel"))
        {
            if(gameActiveMenu == null)
            {
                PauseGame();
                gameActiveMenu = gamePauseMenu;
                gameActiveMenu.SetActive(gameIsPaused);
            }
            else
            {
                UnpauseGame();
            }
        }

     

        roundCount.text = GameRound.ToString("F0");
        enemyCount.text = EnemyCount.ToString("F0");
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
    public void UnpauseGame()
    {
        gameIsPaused = !gameIsPaused;
        // Adjust the timescale
        Time.timeScale = 1;
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


        if (EnemyCount < 0)
        {
            EnemyCount = 0;
        }

        SetEnemyCount(amount);

        Debug.Log("enemies " + EnemyCount.ToString());



        if (EnemyCount == 0) {

            StartNewRound();
        }

    }


    void SetEnemyCount(int amount)
    {
        EnemyCount += amount;
    }


    void UpdateEnemyCount()
    {
        // Reset EnemyCount based on the number of enemies tagged as "Enemy"
        EnemyCount = GameObject.FindGameObjectsWithTag("Zombie").Length;
        enemyCount.text = EnemyCount.ToString("F0");
    }


    public int GetEnemyCount()
    {

        return EnemyCount;

    }

  

    void StartNewRound()
    {
        if (hasWinCondition)
        {
            if (GameRound == 2 && EnemyCount == 0)
            {
                winScreen();
            }
        }

        SetGameRound(1);
        Debug.Log("SpanwFunctionCalled");
        enemySpawner.ZombieSpawner();
        if (GameRound % 5 == 0)
        {
            SpawnBufferZombie();
        }
        
    }
    void SpawnBufferZombie()
    {
        Debug.Log("Special Round");
        bufferSpawner.SetWaveMax(GameRound);
        bufferSpawner.BufferSpawnZombies(GetGameRound());
    }


    public int GetGameRound()
    {
        return GameRound;
       
    }

    public void SetGameRound(int amount)
    {
        GameRound += amount;

      
    }

    public void loseScreen()
    {
        PauseGame();
        gameActiveMenu = gameLoseMenu;
        gameActiveMenu.SetActive(gameIsPaused);
    }

    public void winScreen()
    { 
        PauseGame();
        gameActiveMenu = gameWinMenu;
        gameActiveMenu.SetActive(gameIsPaused);
    }

    public void interactScreen()
    {
        gameManager.gameInstance.playerInteract.SetActive(true);
        
    }
}


