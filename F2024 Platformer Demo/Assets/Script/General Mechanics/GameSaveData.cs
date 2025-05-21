
using UnityEngine;

public class GameSaveData : MonoBehaviour
{
    public static GameSaveData instance;

    public GameData gameData;
    public int highscoreSaveLimit = 15;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameData = SaveManager.LoadGame();
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
