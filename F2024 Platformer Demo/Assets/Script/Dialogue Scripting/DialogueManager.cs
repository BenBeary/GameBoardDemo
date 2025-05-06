using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;


    [Header("UI Inputs")]
    [SerializeField] UIMovement textContainer;
    [SerializeField] Image characterIcon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;

    [Header("Settings")]
    [SerializeField] float writeSpeed = 0.05f;

    [Header("Auto Continue")]
    [SerializeField] Toggle autoContinueToggle;
    [SerializeField] float autoContinueDelay = 1f;


    [Header("Debug")]
    [SerializeField] DialogueData currentDialogue;
    [SerializeField] int currentDialoguePosition;
    [SerializeField] int currentEntry;
    [SerializeField] bool skipWriting;
    [SerializeField] bool finishedTyping;
    [SerializeField] public bool dialogueIsDone;
    [SerializeField] bool SpeedUpTyping;

    Coroutine typingCoroutine;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

    }


    /// <summary>
    /// Initializes the dialogue system with a new dialogue dataset.
    /// Resets entry and line positions, and shows the first line.
    /// </summary>
    /// <param name="data">The dialogue data to display.</param>
    public void TriggerDialogue(DialogueData data)
    {
        textContainer.MoveToTarget();
        characterIcon.GetComponent<UIMovement>().MoveToTarget();

        finishedTyping = false;
        currentDialogue = data;
        currentEntry = 0;
        currentDialoguePosition = 0;
        dialogueText.text = "";
        characterIcon.sprite = null;
        ShowCurrentLine();
    }

    /// <summary>
    /// Advances to the next line or entry in the dialogue.
    /// If the line is still typing, it finishes it instantly.
    /// </summary>
    public void Next()
    {
        if(currentDialogue == null) return;

        if (!finishedTyping)
        {
            skipWriting = true;
            return;
        }

        currentDialoguePosition++;

        if (currentDialoguePosition >= GetCurrentEntry().dialogueStrips.Length)
        {
            currentEntry++;
            currentDialoguePosition = 0;

            if (currentEntry >= currentDialogue.entries.Length)
            {
                EndDialogue();
                return;
            }
        }

        ShowCurrentLine();
    }

    /// <summary>
    /// Displays the current line of dialogue from the current entry.
    /// Starts the typewriter coroutine.
    /// </summary>
    void ShowCurrentLine()
    {
        DialogueData.data entry = GetCurrentEntry();
        characterIcon.sprite = entry.icon ? entry.icon : null;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        nameText.text = entry.name;
        typingCoroutine = StartCoroutine(TypeText(entry.dialogueStrips[currentDialoguePosition]));
    }

    /// <summary>
    /// Coroutine that types out a dialogue line one character at a time.
    /// Supports skipping or speeding up typing with debug flags.
    /// </summary>
    /// <param name="line">The full dialogue line to type.</param>
    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";

        finishedTyping = false;
        skipWriting = false;

        float currentSpeed = SpeedUpTyping ? writeSpeed * 0.25f : writeSpeed;

        foreach (char c in line)
        {
            if (skipWriting)
            {
                dialogueText.text = line;
                break;
            }

            dialogueText.text += c;
            yield return new WaitForSeconds(currentSpeed);
        }

        finishedTyping = true;
        skipWriting = false;
        typingCoroutine = null;

        if (autoContinueToggle != null && autoContinueToggle.isOn)
        {
            StartCoroutine(AutoContinueAfterDelay());
        }
    }

    public void toggleButton()
    {
        if (autoContinueToggle.isOn)
        {
            StopCoroutine(AutoContinueAfterDelay());
            StartCoroutine(AutoContinueAfterDelay());
        }
    }

    /// <summary>
    /// Waits for a delay and auto-advances the dialogue if the toggle is enabled.
    /// </summary>
    IEnumerator AutoContinueAfterDelay()
    {
        yield return new WaitForSeconds(autoContinueDelay);

        if (autoContinueToggle != null && autoContinueToggle.isOn && finishedTyping)
        {
            Next();
        }
    }


    /// <summary>
    /// Gets the current entry (character + lines) based on the current entry index.
    /// </summary>
    /// <returns>The current DialogueData.data entry.</returns>
    DialogueData.data GetCurrentEntry()
    {
        return currentDialogue.entries[currentEntry];
    }

    /// <summary>
    /// Called when all dialogue entries have been shown.
    /// Clears the dialogue and icon, and can trigger cleanup.
    /// </summary>
    void EndDialogue()
    {
        Debug.Log("dialogue Complete");
        finishedTyping = true;
        // Optional: disable UI or call event
        dialogueIsDone = true;
        currentDialogue = null;
        textContainer.MoveToStart();
        characterIcon.GetComponent<UIMovement>().MoveToStart();
    }



    [System.Serializable]
    public class DialogueData
    {
        [System.Serializable]
        public class data
        {
            public string name;
            public Sprite icon;
            public string[] dialogueStrips;
        }

        public data[] entries;
    }
}


