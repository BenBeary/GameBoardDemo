using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Save Settings")]
    public RegionController regionController;
    public string requiredItemToPlay;
    public float returnPlayerControlsDelay = 1f;
    public bool turnOnCutsceneBars,repeatableCutscene;

    public float repeatCooldown = 3f;
    bool onCooldown;

    public static bool CutsceneRunning;


    [Header("Events")]
    public UnityEvent onStart;
    public UnityEvent onEnd;


    public bool hasBeenPlayed;
    bool playingCutscene, token;


    private void Start()
    {
        PlayerController.playerReset += resetCutscene;
    }


    private void OnDisable()
    {
        PlayerController.playerReset -= resetCutscene;
    }

    private void Update()
    {
        if(hasBeenPlayed  && !repeatableCutscene || !token || onCooldown) return;

        if (playingCutscene && PlayerController.instance.grounded)
        {
            PlayerController.instance.hasInputPaused = true;
            PlayerController.instance.motionInput = Vector2.zero;
            PlayCutscene();
            token = false;
        }
    }


    void PlayCutscene()
    {
        if (turnOnCutsceneBars) DialogueManager.instance.MoveBars(true);
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
        if (turnOnCutsceneBars) DialogueManager.instance.MoveBars(false);
        Debug.Log("Cutscene Finished");
        hasBeenPlayed = true;
        PlayerController.playerReset -= resetCutscene;
        CutsceneRunning = false;

        if (repeatableCutscene)
        {
            StartCoroutine(cutSceneCooldown());
        }

    }

    IEnumerator cutSceneCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(repeatCooldown);
        onCooldown = false;
    }

    void resetCutscene()
    {
        Debug.Log("Resetting Cutscene...");
        playingCutscene = false;

        if (hasBeenPlayed && !repeatableCutscene) return;
        StopAllCoroutines();
        CancelInvoke();

        CutsceneRunning = false;
        onCooldown = false;


        DialogueManager.instance.MoveBars(false);
        PlayerController.instance.hasInputPaused = false;

        CutsceneDialogue anyDialogue = GetComponent<CutsceneDialogue>();
        if (anyDialogue) anyDialogue.CancelDialogue();
        CutscenePlayerMovement anyMovement = GetComponent<CutscenePlayerMovement>();
        if (anyMovement) anyMovement.CancelMovement();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenPlayed && !repeatableCutscene && playingCutscene || CutsceneRunning || onCooldown) return;

        if (collision.CompareTag("Player"))
        {
            if (requiredItemToPlay != string.Empty && !regionController.savedItemIds.Contains(requiredItemToPlay)) return;
            playingCutscene = true;
            token = true;
            CutsceneRunning = true;
        }
    }

}
