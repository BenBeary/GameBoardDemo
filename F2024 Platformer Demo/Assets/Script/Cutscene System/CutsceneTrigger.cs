using UnityEngine;
using UnityEngine.Events;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Save Settings")]
    public RegionController regionController;
    public string requiredItemToPlay;
    public float returnPlayerControlsDelay = 1f;


    [Header("Events")]
    public UnityEvent onStart;
    public UnityEvent onEnd;


    public bool hasBeenPlayed;
    bool playingCutscene;


    private void Update()
    {
        if(hasBeenPlayed) return;

        if (playingCutscene && PlayerController.instance.grounded)
        {
            PlayerController.instance.hasInputPaused = true;
            PlayerController.instance.motionInput = Vector2.zero;
            PlayCutscene();
            hasBeenPlayed = true;
        }
    }


    void PlayCutscene()
    {

        onStart.Invoke();
    }

    public void EndCutscene()
    {
        Invoke(nameof(delayedResponse), returnPlayerControlsDelay);
    }

    void delayedResponse()
    {
        onEnd.Invoke();
        PlayerController.instance.hasInputPaused = false;
        Debug.Log("Cutscene Finished");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenPlayed)
        {
            if (requiredItemToPlay != string.Empty && !regionController.savedItemIds.Contains(requiredItemToPlay)) return;
            playingCutscene = true;
        }
    }

}
