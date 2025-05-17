using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [System.Serializable]
    public enum TransitionType
    {
        random,
        Crossfade,
        Wipe,
        CircleWipe,
        FancyWipe
    }




    [SerializeField] Animator animManager;

    public static TransitionManager instance;
    public static bool transitioning;

    private void Awake()
    {
        if(!instance) instance = this;
        else Destroy(gameObject);


        DontDestroyOnLoad(gameObject);
        animManager?.gameObject.SetActive(false);
    }



    public void loadSceneIndex(int index, TransitionType transitionMode)
    {
        StartCoroutine(IndexSceneLoading(index, transitionMode));
    }

    IEnumerator IndexSceneLoading(int index, TransitionType transitionMode)
    {
        string animName = OutputAnimationTag(transitionMode);

        animManager.gameObject.SetActive(true);
        animManager.Play(animName);

        yield return null;
        yield return new WaitUntil(() => animManager.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        AsyncOperation loading = SceneManager.LoadSceneAsync(index);
        
        yield return new WaitUntil(() => loading.isDone);

        yield return new WaitForSeconds(.5f);

        animManager.SetTrigger("SceneLoaded");
        yield return new WaitForSeconds(.2f);
        yield return new WaitUntil(() => animManager.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        animManager.gameObject.SetActive(false);
    }
    


    public void loadScene(SceneField newScene, TransitionType transitionMode, bool killOtherScenes = default)
    {
        StartCoroutine(loadSceneTime(newScene, transitionMode, !killOtherScenes));
    }

    IEnumerator loadSceneTime(SceneField loadThisScene, TransitionType transitionMode, bool killOtherScenes)
    {
        transitioning = true;
        // Start Transition

        string animName = OutputAnimationTag(transitionMode);

        animManager.gameObject.SetActive(true);
        animManager.Play(animName);
        yield return null;

        yield return new WaitUntil(() => animManager.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);



        // When Finished with Transition start new Scene
        // kill Other scenes if necessary

        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(loadThisScene, LoadSceneMode.Additive);
        Debug.Log($"loading {loadThisScene.SceneName}...");

        float count = 0;

        while (!asyncLoading.isDone)
        {
            //Debug.Log($"{loadThisScene.SceneName} at: {(asyncLoading.progress).ToString("#.00")}%");
            count += Time.deltaTime;
            yield return null;
        }
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadThisScene));
        if (killOtherScenes) CloseOtherScenes(loadThisScene); // Should only remove if specified
        
        // Give some time to kill other scenes / let scene load
        yield return new WaitForSecondsRealtime(0.5f);
        
        count *= 100f;
        count = Mathf.Round(count) / 100f;
        Debug.Log($"{loadThisScene.SceneName} Loaded in {count.ToString("#.00")} Seconds");



        // Once scene is loaded, start End Transition
        // Animator should then Outro Version which shoud be the name of the first + _Out


        animManager.SetTrigger("SceneLoaded");
        yield return new WaitForSecondsRealtime(0.1f);

        yield return new WaitUntil(() => 
                            animManager.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && 
                            animManager.GetCurrentAnimatorStateInfo(0).IsName(animName + "_Out"));


        animManager.gameObject.SetActive(false);
        transitioning = false;
    }


    // Keeps the designated Scene and Transition open while unloading the rest
    void CloseOtherScenes(SceneField sceneToKeep)
    {
        int countLoaded = SceneManager.sceneCount;

        for(int i = 0; i < countLoaded; i++)
        {
            if(SceneManager.GetSceneAt(i).name == sceneToKeep.SceneName) 
            {
                continue; 
            }

            Scene temp = SceneManager.GetSceneAt(i);
            Debug.Log("Unloading " + temp.name);
            SceneManager.UnloadSceneAsync(temp);
        }
    }

    // Gives a string name for animation clip to run
    string OutputAnimationTag(TransitionType transitionMode)
    {
        if(transitionMode == TransitionType.random)
        {
            transitionMode = (TransitionType)Random.Range(1, System.Enum.GetValues(typeof(TransitionType)).Length - 1);
        }


        return transitionMode.ToString();

    }
}
