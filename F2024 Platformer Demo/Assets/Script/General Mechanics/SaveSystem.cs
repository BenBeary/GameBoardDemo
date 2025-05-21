using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string name;
    public float timePlayed;
    public List<bool> JacksCollected;
    public int totalDeaths;

    public LevelData()
    {
        name = "High Score Data";
        timePlayed = 0f;
        JacksCollected = new List<bool>();
        totalDeaths = 0;
    }
}

[System.Serializable]
public class GameData
{
    public List<LevelData> highScoreData = new List<LevelData>();
}

public static class SaveManager
{
    private const string SaveKey = "GameSave";

    [System.Serializable]
    private class Wrapper<T>
    {
        public T Value;
    }

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(new Wrapper<GameData> { Value = data });
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static GameData LoadGame()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return new GameData();

        string json = PlayerPrefs.GetString(SaveKey);
        return JsonUtility.FromJson<Wrapper<GameData>>(json)?.Value ?? new GameData();
    }
}
