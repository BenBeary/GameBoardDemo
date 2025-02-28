using Cinemachine;
using System.Collections;
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
    
    [Header("UI Stuff")]
    [SerializeField] Image displayNumber;
    [SerializeField] Sprite[] numbers;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject PauseExitButton;
    [SerializeField] GameObject StartButton;

    [Header("Debug")]
    public bool DevMode = true;
    public bool isPaused;
    public bool gameStarted;
    public bool cantPause;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
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

    private void Update()
    {
        if(Input.GetButtonDown("Pause") && !cantPause)
        {
            if(!isPaused) PauseGame();
            else UnPauseGame();
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

        savedRegionData.Add(new regionData(newRegion.RegionName, newRegion.saveditemIDs));
        
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
        SceneManager.LoadScene("LoadFirst");
    }

    public void ResetToCheckpoint()
    {
        UnPauseGame();
        if (PlayerController.instance.checkPoint != null) PlayerController.instance.resetToCheckpoint();

    }
    #endregion


    


}
