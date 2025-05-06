using UnityEngine;
using UnityEngine.Events;

public class CutsceneDialogue : MonoBehaviour
{


    public DialogueObject dialogueData;

    public UnityEvent onFinished;

    bool running;


    private void Update()
    {
        if (running && DialogueManager.instance.dialogueIsDone)
        {
            running = false;
            DialogueManager.instance.dialogueIsDone = false;
            onFinished.Invoke();
        }
    }


    public void runDialogue()
    {
        DialogueManager.instance.TriggerDialogue(dialogueData.EntryData);
        running = true;
    }


}
