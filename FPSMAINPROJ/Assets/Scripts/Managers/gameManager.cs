using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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
    [SerializeField] public GameObject gameAlterMenu;
    [SerializeField] public GameObject gameOptionsMenu;
    [SerializeField] public Slider sensSlider;
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
    [SerializeFeild] bool endless;
    public TMP_Text ammoCount;
    [SerializeField] public TMP_Text gunName;
    public TMP_Text maxAmmoCount;
    public Image ammoCircle;
    public GameObject flashDamage;
    public GameObject playerInteract;
    public Image playerHPBar;
    public Image playerManaBar;
    public Image playerSprintBar;
    public Image SprintBarBoarder;
    public Image AmmoHUD;
    public Image checkpoint;
    public Image gem;

    [Header("----PLAYER----")]
    public PlayerController playerScript;
    public WeaponController playerWeapon;
    public cameraController cameraController;
    public WeaponManager weaponManager;

    [Header("----AUDIO-----")]
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public Slider masterVolSlider;
    [SerializeField] public Slider SFXVolSlider;
    [SerializeField] public Slider musicVolSlider;

    [Header("----Misc---")]
    //Objects
    public GameObject playerSpawnPoint;
    public EnemySpawner enemySpawner;
    public bool isReqItemsUIDisplay = false;
    private GameObject enemy;
  
    private bool isNewRoundStarting = false;
    public bool isUserKareDead;
    public bool isSekhmetDead;
    public GameObject UserKare;
    public GameObject SekhMet;
    public SekhmetBoss Sekhmet;
    public Userkare Userkare;

  public  bool SekhmetisBerserk;
  public  bool UserkareIsUncaped;
    public Transform SekhmetRespawn;
    //int variables 
    int EnemyCount;
    public int PointCount;
    int GameRound;
    public bool canUnlock;
    public bool hasStartedRitual = false;
    // Private reference for the Player
    private GameObject _Player;
    private bool isNewEnemies;


    int cycle = 0;
    public bool BlinkingJab;
    public bool LightGautlening;
    int BossesKilled;

    // Public property to access the Player

    public GameObject player
    {
        get { return _Player; }
        private set { _Player = value; }
    }

    Camera gameCamera;
    public Camera MainCam
    {
        get { return gameCamera; }
        private set { gameCamera = value; }
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

        BossesKilled = 0;
        BlinkingJab = false;
        LightGautlening = false;


        if (Userkare == null)
        {
            isUserKareDead = true;
        }
        else
            isUserKareDead = false;

        if (SekhMet == null)
        {
            isSekhmetDead = true;
        }
        else
            isSekhmetDead = false;


        SekhmetisBerserk = false;
        UserkareIsUncaped = false;

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
        cameraController = FindAnyObjectByType<cameraController>();
        weaponManager = FindAnyObjectByType<WeaponManager>();
        //weaponScript = FindObjectOfType<Weapon>();
        playerSpawnPoint = GameObject.FindWithTag("Player Spawner");

        enemySpawner = FindObjectOfType<EnemySpawner>();
        enemySpawner.PopulateSpawnPoints();
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found.");
        }
        else
            Debug.Log("Enemy Spawner Valid");


        MainCam = Camera.main;

        //audio

        

        masterVolSlider.value = PlayerPrefs.GetFloat("MasterVol");
        SFXVolSlider.value = PlayerPrefs.GetFloat("SFXVol");
        musicVolSlider.value = PlayerPrefs.GetFloat("MusicVol");
        sensSlider.value = PlayerPrefs.GetInt("sens");

        cameraController.sens = PlayerPrefs.GetInt("sens");

        masterVolSlider.onValueChanged.AddListener(onMasterSliderChange);
        SFXVolSlider.onValueChanged.AddListener(onSFXSliderChange);
        musicVolSlider.onValueChanged.AddListener(onMusicSliderChange);

    }

    void Start()
    {
        StartCoroutine(fadeOut());

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolSlider.value) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(SFXVolSlider.value) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolSlider.value) * 20);
    }


    // Update is called once per frame
    void Update()
    {
        // Pause Menu Logic
        if (Input.GetButtonDown("Cancel"))
        {
            if (gameAlterMenu.activeSelf)
            {
                gameAlterMenu.SetActive(false);
                gameActiveMenu = gameAlterMenu;
                resumePlayerControls();
            }
            else if (gameActiveMenu == null && !gameAlterMenu.activeSelf && !gameOptionsMenu.activeSelf)
            {
                PauseGame();
                gameActiveMenu = gamePauseMenu;
                gameActiveMenu.SetActive(gameIsPaused);
            }
            else if (gameOptionsMenu.activeSelf)
            {
                saveSettings();
                gameOptionsMenu.SetActive(false);
                gameActiveMenu = gamePauseMenu;
                gamePauseMenu.SetActive(true);
            }
            else
            {
                UnpauseGame();
            }

        }
        if (BossesKilled == 2)
        {
            Debug.Log("entering boss killed");
            winScreen();
        }

        if (EnemyCount < 4 || cycle == 10000)
        {
            enemySpawner.ZombieSpawner(3);
            cycle = 0;
        }
        displayInventoryMenu();

        roundCount.text = GameRound.ToString("F0");
        pointCount.text = PointCount.ToString("F0");

        PlayerPrefs.SetInt("sens", (int)sensSlider.value);
        
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

    public void pausePlayerControls()
    {
        playerScript.enabled = false;
        cameraController.enabled = false;
        playerWeapon.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void resumePlayerControls()
    {
        playerScript.enabled = true;
        cameraController.enabled = true;
        playerWeapon.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        EnemyCount += amount;
        
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

    public void BossKilled()
    {
        BossesKilled ++;

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
        enemySpawner.ZombieSpawner(3);

        if (SpecialZombieIncrament > 0)
        {
            if (GameRound % SpecialZombieIncrament == 0)
            {
                Debug.Log("Special Round");
                enemySpawner.SpecialZombieSpawner(SpecialZombieIncrament);
            }
        }
        
    }

    public void SpawnSekhmet()
    {
        SekhMet = GameObject.FindGameObjectWithTag("Sekhmet");
        isSekhmetDead = false;
    }

    public void SpawnUserkare()
    {
        UserKare = GameObject.FindGameObjectWithTag("Userkare");
        isUserKareDead = false;
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
        gameWinMenu.SetActive(true);
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

    public void UserkareDead()
    {

        SekhmetisBerserk = true;
      
        isUserKareDead = true;
    }

    public void SekhmetDead()
    {
        UserkareIsUncaped = true;
        isSekhmetDead = true;

    }

    public void SekhmetDeathLocation(Transform location)
    {
        SekhmetRespawn = location;
    }

    public void onSliderChange(float value)
    {
        PlayerPrefs.SetInt("sens", (int)value);

        saveSettings();
    } 

    public void onMasterSliderChange(float value)
    {
        Debug.Log("Master Volume Slider Value: " + value);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20); //log10 for decibles 
        saveSettings();

    }

    public void onSFXSliderChange(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20); //log10 for decibles 
        saveSettings();
    }

    public void onMusicSliderChange(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20); //log10 for decibles 
        saveSettings();
    }

    public void saveSettings()
    {
        PlayerPrefs.SetInt("sens", (int)sensSlider.value);

        PlayerPrefs.SetFloat("MasterVol", (float)masterVolSlider.value);
        PlayerPrefs.SetFloat("SFXVol", (float)SFXVolSlider.value);
        PlayerPrefs.SetFloat("MusicVol", (float)musicVolSlider.value);

        PlayerPrefs.Save();
    }

}


