
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleHighscore : MonoBehaviour
{


    // Me just reusing shit because I am rushing

    [SerializeField] Color CollectableColorGood = Color.yellow;
    [SerializeField] Color CollectableColorBad = Color.black;

    [SerializeField] Transform highscoreContainer;
    [SerializeField] SceneChanger sceneChanger;

    private void Start()
    {
        if(!GameManager.Instance) return;

        
        loadInHighscore(GameManager.Instance.newGame);
        StartCoroutine(flicker());
    }


    IEnumerator flicker()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(highscoreContainer.GetComponent<RectTransform>());

        for (int i = 0; i < 6; i++)
        {
            if (highscoreContainer.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(2);
            }
            else
            {
                yield return new WaitForSeconds(.5f);
            }
            highscoreContainer.gameObject.SetActive(!highscoreContainer.gameObject.activeSelf);
        }
        yield return new WaitForSeconds(2);

        sceneChanger.LoadNewScene();
    }



    void loadInHighscore(LevelData currentHighscore)
    {

        // Name
        highscoreContainer.GetChild(0).GetComponent<TMP_Text>().text = string.Empty;
        highscoreContainer.GetChild(0).GetComponent<TMP_Text>().text = currentHighscore.name;


        // Time

        highscoreContainer.GetChild(1).GetComponent<TMP_Text>().text = FormattedTime(currentHighscore.timePlayed);


        // Collectables

        FormattedCollectable(highscoreContainer.GetChild(2), currentHighscore.JacksCollected);


        // Total Deaths

        highscoreContainer.GetChild(3).GetComponentInChildren<TMP_Text>().text = "x " + currentHighscore.totalDeaths.ToString("N0");


    }


    void FormattedCollectable(Transform container, List<bool> data)
    {
        int count = 0;
        foreach (Image item in container.GetComponentsInChildren<Image>())
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
