
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChunkLoading : MonoBehaviour
{

    [SerializeField] SceneField loadScene;
    [SerializeField] SceneField unloadScene;

    bool loadingScene;



    void SceneLoading()
    {

        if(loadingScene) { return; }

        if(SceneManager.GetSceneByName(unloadScene.SceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(unloadScene);
            Debug.Log("Unloading Scene " +  unloadScene.SceneName);
        }

        Debug.Log(loadScene + " | " + loadScene.SceneName);
        if(loadScene.SceneName == string.Empty) return;

        if(!SceneManager.GetSceneByName(loadScene.SceneName).isLoaded)
        {
            loadingScene = true;
            StartCoroutine(loadSceneTime());
        }



    }

    IEnumerator loadSceneTime()
    {
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
        Debug.Log("loading Scene " + loadScene.SceneName);

        float count = 0;

        while (!asyncLoading.isDone)
        {
            count += Time.deltaTime;
            yield return null;
        }

        count *= 100f;
        count = Mathf.Round(count) / 100f;

        Debug.Log("Scene Loaded in " + count + " Seconds");
        loadingScene = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneLoading();
        }
    }


}
