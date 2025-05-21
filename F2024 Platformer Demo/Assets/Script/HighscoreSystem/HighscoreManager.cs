using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreManager : MonoBehaviour
{

    public static HighscoreManager instance;

    [SerializeField] Color CollectableColorGood = Color.yellow;
    [SerializeField] Color CollectableColorBad = Color.black;




    [SerializeField] Transform ScoreContainer;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    private void Start()
    {
        Invoke(nameof(loadHighscoreList), .5f);
    }


    void loadHighscoreList()
    {
        int count = 0;
        foreach (Transform UIItem in ScoreContainer.transform)
        {
            
            // Outside Highscore Array
            if(GameSaveData.instance.gameData.highScoreData.Count <= count)
            {
                UIItem.gameObject.SetActive(false);
                count++;
                continue;
            }

            UIItem.gameObject.SetActive(true);
            LevelData currentHighscore = GameSaveData.instance.gameData.highScoreData[count];


            // Name
            UIItem.GetChild(0).GetComponent<TMP_Text>().text = currentHighscore.name;


            // Time

            UIItem.GetChild(1).GetComponent<TMP_Text>().text = FormattedTime(currentHighscore.timePlayed);


            // Collectables

            FormattedCollectable(UIItem.GetChild(2), currentHighscore.JacksCollected);


            // Total Deaths

            UIItem.GetChild(3).GetComponentInChildren<TMP_Text>().text = "x " + currentHighscore.totalDeaths.ToString("N0");


            // done

            count++;
        }


    }


    void FormattedCollectable(Transform container, List<bool> data)
    {
        int count = 0;
        foreach(Image item in container.GetComponentsInChildren<Image>())
        {
            if (data[count])
            {
                item.color = CollectableColorGood;
            }
            else
            {
                item.color = CollectableColorBad;
            }
            count++;
        }
    }



    string FormattedTime(float time)
    {
        float timeInSeconds = time;

        int hours = (int)(timeInSeconds / 3600);
        int minutes = (int)((timeInSeconds % 3600) / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds - Mathf.Floor(timeInSeconds)) * 1000);

        string formattedTime = "";

        if (hours > 0)
        {
            formattedTime += hours + ":";
        }

        formattedTime += string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds / 10);

        return formattedTime; 
    }


}
