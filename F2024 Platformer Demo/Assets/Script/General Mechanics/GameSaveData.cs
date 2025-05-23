using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameSaveData : MonoBehaviour
{
    public static GameSaveData instance;

    public GameData gameData;
    public int highscoreSaveLimit = 15;
    EventSystem backUpEventSystem;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        backUpEventSystem = transform.GetComponentInChildren<EventSystem>();
    }

    private void Start()
    {

        EventSystemChecker();


        gameData = SaveManager.LoadGame();
    }



    private void LateUpdate()
    {
        EventSystemChecker();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {

            if(EventSystem.current && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(GameObject.FindGameObjectWithTag("Finish"));
            }
        }
    }


    void EventSystemChecker()
    {
        if (backUpEventSystem == null)
        {
            GameObject temp = new GameObject("EventSystem");
            temp.transform.parent = transform;
            temp.AddComponent<EventSystem>();
            temp.AddComponent<StandaloneInputModule>();
            if (EventSystem.current != null)
            {
                temp.SetActive(false);
            }
            backUpEventSystem = temp.GetComponent<EventSystem>();
        }
        else if (EventSystem.current == null)
        {
            backUpEventSystem.gameObject.SetActive(true);
            EventSystem.current = backUpEventSystem;
        }
        else if (EventSystem.current != null && EventSystem.current != backUpEventSystem)
        {
            backUpEventSystem.gameObject.SetActive(false);


        }
    }


    public void AddNewHighscore(LevelData item)
    {

        // Only Save to the limit For UI
        if(gameData.highScoreData.Count == highscoreSaveLimit && item.timePlayed < gameData.highScoreData[14].timePlayed)
        {
            gameData.highScoreData.RemoveAt(14);
        }


        gameData.highScoreData.Add(item);

        gameData.highScoreData.Sort((a, b) => a.timePlayed.CompareTo(b.timePlayed));

        SaveManager.SaveGame(gameData);

    }


    public bool checkIfNewHighscore(LevelData item)
    {
        if(gameData.highScoreData.Count < highscoreSaveLimit)
        {
            return true;
        }

        if (gameData.highScoreData.Count == highscoreSaveLimit && item.timePlayed < gameData.highScoreData[14].timePlayed)
        {
            return true;
        }

        return false;
    }



    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.Alpha2))
        {
            Debug.Log("Application Closing");
            Application.Quit();
        }
    }
}
