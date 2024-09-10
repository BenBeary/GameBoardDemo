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


    public static GameManager Instance;

    [Header("Player Stats")]
    [SerializeField] List<RuntimeAnimatorController> playerSkins = new List<RuntimeAnimatorController>();
    [SerializeField] List<RuntimeAnimatorController> playerHealthSkins = new List<RuntimeAnimatorController>();
    [SerializeField] int currentSkin = 0;
    [SerializeField] int currentHealthSkin = 1;
    [SerializeField] Animator playerHealthAnim;

    [Header("Camera Stuff")]
    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] GameObject groupTargeting;
    [SerializeField] Transform secondTarget;
    [SerializeField] Transform targetStorage;
    float shakeTimer;
    float shakeTimeTotal;
    float shakeIntensity;


    [Header("Game Stuff")]
    [SerializeField] string[] levelsToLoad;
    
    
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
            cam.GetCinemachineComponent<CinemachineFramingTransposer>().m_DeadZoneHeight = .5f;
        }
        else
        {
            PlayerController.instance.GetComponent<JumpCountUpgrade>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        PauseMenu.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if(!isPaused) PauseGame();
            else UnPauseGame();
        }
        
        secondTarget.position = (targetStorage == null) ? PlayerController.instance.transform.position : targetStorage.transform.position;
        DecreaseShaking();
    }

    #region Camera Management

    public void SetNewTarget(Transform target, float cameraSize)
    {
        targetStorage = target;
        if(cameraSize != 0)
        {
            groupTargeting.GetComponent<CinemachineTargetGroup>().m_Targets[1].radius = cameraSize;
        }
    }

    public void CameraShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cameraShakeComp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cameraShakeComp.m_AmplitudeGain = intensity;
        shakeIntensity = intensity;
        shakeTimer = time;
        shakeTimeTotal = time; ;
    }

    private void DecreaseShaking()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cameraShakeComp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cameraShakeComp.m_AmplitudeGain = Mathf.Lerp(shakeIntensity, 0f, 1 - (shakeTimer / shakeTimeTotal));

                cam.transform.parent.transform.Rotate(Vector3.zero); // resets camera because Cinemachine doesnt, Thanks for that
                cam.transform.parent.transform.position = new Vector3(cam.transform.parent.transform.position.x, cam.transform.parent.transform.position.y, -14);
            }
        }
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
        StartCoroutine(LoadScenesAsync(levelsToLoad,true));
        gameStarted = true;
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
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


    #region Scene Management
    private List<Scene> getOpenScenes() // Created to Avoide Deprecation fuck you unity it was a good function
    {
        List<Scene> temp = new List<Scene>();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            temp.Add(SceneManager.GetSceneAt(i));
        }
        return temp;
    }

    IEnumerator LoadScenesAsync(string[] sceneNames, bool FirstTimeLoad)
    {
        if(!sceneNames.All(string.IsNullOrEmpty))
        {
            foreach (string sceneName in sceneNames)
            {
                if (getOpenScenes().Any(x => x.name == sceneName)) continue;



                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncLoad.isDone) // Waits to load next scene until previous one is finished
                {
                    yield return null;
                }
            }
            Debug.Log("All Scenes Loaded!");
        }

        if(FirstTimeLoad)
        {
            PlayerController.instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            PlayerController.instance.hasInputPaused = false;
            Destroy(PlayerController.instance.GetComponent<JumpCountUpgrade>());
            Destroy(GetComponent<ParticleSystem>(), 3f);
            cam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_DeadZoneHeight = 0.054f; // Hard coding baby! WOOO
            yield return null;
        }
    }

    #endregion


}
