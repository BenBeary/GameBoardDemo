using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class regionData
    {
        public string RegionID;
        public List<string> savedData = new List<string>();
        public regionData(string regionID, List<string> savedItems)
        {
            
            RegionID = regionID;
            savedData.AddRange(savedItems);
        }
    }


    public static GameManager Instance;

    [Header("Player Stats")]
    [SerializeField] List<RuntimeAnimatorController> playerSkins = new List<RuntimeAnimatorController>();
    [SerializeField] List<RuntimeAnimatorController> playerHealthSkins = new List<RuntimeAnimatorController>();
    [SerializeField] int currentSkin = 0;
    [SerializeField] int currentHealthSkin = 1;
    [SerializeField] Animator playerHealthAnim;


    [Header("Game Stuff")]
    public ChunkData activeChunk;
    [Tooltip("Don't touch this")]
    public List<regionData> savedRegionData = new List<regionData>();
    public int playerDeathCount = 0;
    public float timePlayedInGame = 0;
    public bool startTimer;

    // OBSOLETE ################### Move to seperate Script ///
    [Header("UI Stuff")]
    [SerializeField] Image displayNumber;
    [SerializeField] Sprite[] numbers;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject PauseExitButton;
    [SerializeField] GameObject StartButton;
    // ##########################

    [Header("Collectable Stuff")]
    [SerializeField] GameObject UIPrefab;
    [SerializeField] Canvas collectableCanvas;
    [SerializeField] Transform collectableContainer;
    [Space(20)]
    public float fadeDuration = 0.2f;
    public float arcDuration = 1f;
    public float delayBefore = 0.1f;
    public float delayAfter = 0.1f;
    [Space(20)]
    public bool[] Collectables = new bool[10];


    [Header("Game Done Where To Load")]
    [SerializeField] SceneField newScore;
    [SerializeField] SceneField backToMenu;
    public LevelData newGame;

    public static event Action destroyOnMainMenuLoad;
    int currentSceneIndex;

    [Header("Debug")]
    public bool DevMode = true;
    public bool isPaused;
    public bool gameStarted;
    public bool cantPause;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        destroyOnMainMenuLoad += KillYourself;
        
    }
    private void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (!DevMode)
        {
            PlayerController.instance.hasInputPaused = true;
            PlayerController.instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
        else
        {
            // PlayerController.instance.GetComponent<JumpCountUpgrade>().enabled = false;
            //transform.GetChild(0).gameObject.SetActive(false);
        }
        //PauseMenu?.SetActive(false);
    }


    private void OnDestroy()
    {
        destroyOnMainMenuLoad -= KillYourself;
    }

    void KillYourself()
    {
        Destroy(gameObject);
    }


    public void SaveHighscore(ref SceneChanger whereToGo)
    {
        
        newGame.name = CharRandomizer().ToString() + CharRandomizer().ToString() + CharRandomizer().ToString();
        newGame.timePlayed = timePlayedInGame;
        newGame.JacksCollected = Collectables.ToList();
        newGame.totalDeaths = playerDeathCount;
        
        if(GameSaveData.instance.checkIfNewHighscore(newGame) ) 
        {
            whereToGo.loadThisScene = newScore;
        }
        else
        {
            whereToGo.loadThisScene = backToMenu;
        }
        
        GameSaveData.instance.AddNewHighscore(newGame);
        whereToGo.LoadNewScene();
        CutsceneTrigger.CutsceneRunning = false;
    }


    char CharRandomizer()
    {
        return (char)('a' + UnityEngine.Random.Range(0, 26));
    }


    private void Update()
    {
        if(Input.GetButtonDown("Pause") && !cantPause || Input.GetKeyDown(KeyCode.Alpha1) && !cantPause)
        {
            if(!isPaused) PauseGame();
            else UnPauseGame();
        }
        
        if(startTimer)
        {
            timePlayedInGame += Time.deltaTime;

        }

    }

    private void LateUpdate()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex != activeSceneIndex)
        {
            currentSceneIndex = activeSceneIndex;

            if(currentSceneIndex == 0)
            {
                destroyOnMainMenuLoad?.Invoke();
            }

        }
    }

    #region Chunk and Save Data
    public void SetActiveChunk(ChunkData newChunk)
    {
        activeChunk = newChunk;
    }

    public void addRegion(RegionController newRegion)
    {
        if (savedRegionData.Any(x => x.RegionID == newRegion.RegionName)) return;

        savedRegionData.Add(new regionData(newRegion.RegionName, newRegion.savedItemIds));
        GrabCollectableData(newRegion.savedItemIds);
        
    }

    public bool CheckForData(string regionID)
    {
        return savedRegionData.Any(x => x.RegionID == regionID);
    }

    // switch to strings because Region is lost on unload

    public List<string> GetRegionData(string regionID)
    {
        return savedRegionData.First(x => x.RegionID == regionID).savedData;
    }

    public void UpdateRegionData(string regionID, List<string> newData)
    {
        savedRegionData.First(x => x.RegionID == regionID).savedData = new List<string>(newData);
        GrabCollectableData(newData);
    }

    void GrabCollectableData(List<string> Data)
    {
        foreach(var item in Data)
        {
            if (item.Contains("Collectable"))
            {
                string numberPart = item.Substring("Collectable".Length);
                if(int.TryParse(numberPart,out int number))
                {
                    if (!Collectables[number - 1])
                    {
                        Debug.Log("New Collectable Added");
                        Collectables[number - 1] = true;
                        SpawnAndAnimate(number - 1);
                    }

                }
            }
        }
    }

    public void SpawnAndAnimate(int spriteLocation)
    {
       
        CanvasGroup canvasGroup = collectableContainer.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration)

        // Step 2: Arc animation (starts after fade in)
        .setOnComplete(() =>
        {
           
            collectableContainer.GetChild(spriteLocation).GetComponent<Image>().color = Color.yellow;

            LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration)
                .setDelay(delayAfter)
                .setOnComplete(() =>
                {
                    Debug.Log("Collectable Added?");
                });
        });
    }


    #endregion

    #region UI Buttons


    #region Skinning
    public void ChangePlayerSkin(bool goBack)
    {
        currentSkin += goBack ? -1 : 1;
        if(currentSkin >= playerSkins.Count) currentSkin = 0; 
        else if(currentSkin < 0) currentSkin = playerSkins.Count - 1;

        if (currentHealthSkin == currentSkin) ChangeHealthSkin(false);


        Animator playerAnim = PlayerController.instance.GetComponent<Animator>();
        string currentAnimName = playerAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        float currentFrame = playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        playerAnim.runtimeAnimatorController = playerSkins[currentSkin];
        playerAnim.Play(currentAnimName,0,currentFrame); // Continue where Last animator was at

        // ### UI
        displayNumber.sprite = numbers[currentSkin];
    }

    public void ChangeHealthSkin(bool goBack)
    {
        currentHealthSkin += goBack ? -1 : 1;
        if (currentHealthSkin == currentSkin) currentHealthSkin += goBack ? -1 : 1;  // add / subtract another if current is equal to player skin

        if (currentHealthSkin >= playerSkins.Count) currentHealthSkin = (currentSkin == 0) ? 1 : 0; // go to next one if player is at 0
        else if (currentHealthSkin < 0) currentHealthSkin = playerSkins.Count - 1; 

        playerHealthAnim.runtimeAnimatorController = playerHealthSkins[currentHealthSkin]; // Health Gets Skin That is not Player
        playerHealthAnim.gameObject.GetComponent<LifeDisplay>().InitializeAnimation();
    }
    #endregion


    public void StartGame()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        gameStarted = true;
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(PauseExitButton);

        isPaused = true;
    }

    public void UnPauseGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        EventSystem.current.SetSelectedGameObject(null);
        if (!gameStarted) EventSystem.current.SetSelectedGameObject(StartButton);
        isPaused = false;
    }

    public void RestartGame()
    {
        UnPauseGame();
        SceneManager.LoadScene(0);
    }

    public void ResetToCheckpoint()
    {
        UnPauseGame();
        if (PlayerController.instance.checkPoint != null) PlayerController.instance.resetToCheckpoint();

    }
    #endregion


    


}
