using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    [Tooltip("keeps other Scenes loaded in the background")]
    [SerializeField] bool loadSceneAdditive;
    [SerializeField] TransitionManager.TransitionType transitionMode;
    [Header("Scene Input")]
    public SceneField loadThisScene;
    [Header("Indexing Settings")]
    [SerializeField] bool useIndexInstead;
    public int SceneIndex;
    [Space(10)]
    [SerializeField] bool reloadCurrentScene;

    public void LoadNewScene()
    {
        if (useIndexInstead)
        {
            TransitionManager.instance.loadSceneIndex(SceneIndex,transitionMode);
        }
        else if (reloadCurrentScene)
        {
            TransitionManager.instance.loadSceneIndex(SceneManager.GetActiveScene().buildIndex, transitionMode);
        }
        else
        {
            TransitionManager.instance.loadScene(loadThisScene,transitionMode,loadSceneAdditive);
        }
    }

}
