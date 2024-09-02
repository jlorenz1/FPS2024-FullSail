using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [Header("----ROUND SETTINGS----")]
    [SerializeField] private float minRoundDuration = 10f;
    [SerializeField] int EndRound;
    [SerializeField] float RoundDelay;
    [SerializeField] int SpecialZombieIncrament;
    // Serialized Variables
    [Header("----UI----")]
    [SerializeField] public Image fadeOverlay;
    public float fadeDuration;
    [SerializeField] GameObject gameActiveMenu;
    [SerializeField] GameObject gameMainMenu;
    [SerializeField] GameObject gamePauseMenu;
    [SerializeField] GameObject gameWinMenu;
    [SerializeField] GameObject gameLoseMenu;
    [SerializeField] TMP_Text roundCount;
    [SerializeField] TMP_Text enemyCount;
    [SerializeField] TMP_Text pointCount;
    [SerializeField] public GameObject inventoryMenu;
    [SerializeField] public TMP_Text runesCount;
    [SerializeField] public TMP_Text lighterCount;
    [SerializeField] public TMP_Text keyCount;
    [SerializeField] public TMP_Text itemsCompleteText;
    [SerializeField] public GameObject ritualInProgress;
    [SerializeField] public GameObject quickTime;
    [SerializeField] public GameObject requiredItemsContainer;
    [SerializeField] TMP_Text requiredItemsDis;
    public TMP_Text ammoCount;
    public TMP_Text maxAmmoCount;
    public Image ammoCircle;
    public GameObject flashDamage;
    public GameObject playerInteract;
    public Image playerHPBar;
    public Image playerSprintBar;
    public Image SprintBarBoarder;

    [Header("----PLAYER----")]
    public PlayerController playerScript;
    public WeaponController playerWeapon;

    //Objects
    public EnemySpawner enemySpawner;
    public bool isReqItemsUIDisplay = false;
    private GameObject enemy;
    private bool isCheckingEnemyCount = false;
    private bool isNewRoundStarting = false;
    

    //int variables 
    int EnemyCount;
    public int PointCount;
    int GameRound;
    public bool canUnlock;
    public bool hasStartedRitual = false;
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
        fadeOverlay.gameObject.SetActive(true);
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
        playerScript = FindObjectOfType<PlayerController>();
        playerWeapon = FindObjectOfType<WeaponController>();
        //weaponScript = FindObjectOfType<Weapon>();

        enemySpawner = FindObjectOfType<EnemySpawner>();
        enemySpawner.PopulateSpawnPoints();
        if (enemySpawner == null)
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

    void Start()
    {
        StartCoroutine(fadeOut());
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

        displayInventoryMenu();

        roundCount.text = GameRound.ToString("F0");
        enemyCount.text = EnemyCount.ToString("F0");
        pointCount.text = PointCount.ToString("F0");

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

  IEnumerator delayRoundStart()
    {
        yield return new WaitForSeconds(RoundDelay);
        StartNewRound();

    }

    void StartNewRound()
    {
        if (hasWinCondition)
        {
            if (GameRound == EndRound && EnemyCount == 0)
            {
                winScreen();
            }
        }

        SetGameRound(1);
        Debug.Log("SpanwFunctionCalled");
        enemySpawner.ZombieSpawner();

        if (SpecialZombieIncrament > 0)
        {
            if (GameRound % SpecialZombieIncrament == 0)
            {
                Debug.Log("Special Round");
                enemySpawner.SpecialZombieSpawner(SpecialZombieIncrament);
            }
        }
        
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

    public void displayRequiredIemsUI(string message, float duration)
    {
        if(isReqItemsUIDisplay)
        {
            StopCoroutine("requiredItemsUI");
        }
        StartCoroutine(requiredItemsUI(message, duration));
    }

    public IEnumerator requiredItemsUI(string textToDisplay, float duration)
    {
        isReqItemsUIDisplay = true;
        float givenDuration = duration;
        requiredItemsContainer.SetActive(true);
        requiredItemsDis.text = textToDisplay;
        yield return new WaitForSeconds(givenDuration);
        givenDuration = 0;
        requiredItemsDis.text = string.Empty;
        requiredItemsContainer.SetActive(false);
        isReqItemsUIDisplay = false;
    }

    public void displayInventoryMenu()
    {
        if (!hasStartedRitual)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                inventoryMenu.SetActive(true);

            }
            else
            {
                inventoryMenu.SetActive(false);
            }
        }
        else if (hasStartedRitual)
        {
            if(Input.GetKey(KeyCode.Tab)) 
            {
                ritualInProgress.SetActive(true);
            }
            else
            {
                ritualInProgress.SetActive(false);
            }
        }
    }

    public IEnumerator fadeOut()
    {
        Color fadeOutColor = fadeOverlay.color;
        float startAlpha = fadeOutColor.a;
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeOverlay.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, alpha);
            elapsed += Time.deltaTime; 
            yield return null;
        }

        fadeOverlay.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, endAlpha);
    }
}


