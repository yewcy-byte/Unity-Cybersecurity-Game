using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;






public class NPC : MonoBehaviour, IInteractable
{
    
public NPCDialogue dialogueData;
public GameObject ItemPrefab;

[SerializeField] private float itemDropMinDistance = 2f;
[SerializeField] private float itemDropMaxDistance = 3f;


public GameObject guider;

private DialogueController dialogueUI;

private AudioSource audioSource;


private int dialogueIndex;
private bool isTyping, isDialogueActive;

private enum QuestState {NotStarted, InProgress, Completed}
private QuestState questState = QuestState.NotStarted;



private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

private void Update()
{
    if (!isDialogueActive || dialogueUI == null || dialogueUI.dialoguePanel == null)
        return;

    if (!dialogueUI.dialoguePanel.activeInHierarchy)
    {
        EndDialogue();
    }
}


void Awake()
    {
            audioSource = GetComponent<AudioSource>();

    }
public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (guider != null){
            closeGuiderBot();
        }
        
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
        return;
        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {

            StartDialogue();
        }
    }

public void closeGuiderBot()
    {
        
        LeanTween.scale(guider, new Vector3(0, 0, 0), 0.6f)
    .setEase(LeanTweenType.easeOutQuart)
    .setOnComplete(() => guider.SetActive(false));
    }

    void StartDialogue()
    {
        //Sync quest data
        SyncQuestState();

        SoundManager.SetMuted(true);
        //Set dialgoue line based on queststate
        if(questState == QuestState.NotStarted)
        {
            dialogueIndex = 0;
            audioSource.PlayOneShot(dialogueData.voiceClip);

        } else if (questState == QuestState.InProgress)
        {
            dialogueIndex = dialogueData.questInProgressIndex;
            
            audioSource.PlayOneShot(dialogueData.Goodluck);



        }else if (questState == QuestState.Completed)
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }
        isDialogueActive = true;


        Debug.Log(dialogueData.npcPortrait);

        isDialogueActive = true;

       
        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        
        PauseController.SetPause(true);
        



DisplayCurrentLines();
    }

 private void SyncQuestState()
{
    if (dialogueData.missions == null || dialogueData.missions.Count == 0)
        return;

    foreach (Mission mission in dialogueData.missions)
    {
        if (MissionController.Instance.IsMissionActive(mission.questID))
        {
            questState = QuestState.InProgress;
            return;
        }
    }

    questState = QuestState.NotStarted;
}




    void NextLine()
    {
        if (isTyping)
        {
            //Skip animation and show whole line
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        
        dialogueUI.ClearChoices();
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex]){
            EndDialogue();
            return;
        }

        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }
        
        
        
         if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLines();
        }else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping  = true;
          dialogueUI.SetDialogueText("");

        foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
        {
              dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
            


        }
        isTyping = false;
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();

        }

    }

void DisplayChoices(DialogueChoice choice)
{
    dialogueUI.ShowChoices(true);

    for (int i = 0; i < choice.choices.Length; i++)
    {
        int optionIndex = i; // closure fix
        int nextIndex = choice.nextDialogueIndexes[i];
        bool givesQuest = choice.givesQuest[i];

        dialogueUI.CreateChoiceButton(
            choice.choices[i],
            () => ChooseOption(choice, optionIndex, nextIndex, givesQuest)
        );
    }
}



void ChooseOption(DialogueChoice choice, int optionIndex, int nextIndex, bool givesQuest)
{
    // 🔊 Play option audio
    if (choice.OptionsAudio != null &&
        optionIndex < choice.OptionsAudio.Length &&
        choice.OptionsAudio[optionIndex] != null)
    {
        audioSource.pitch = dialogueData.voicePitch;
        audioSource.PlayOneShot(choice.OptionsAudio[optionIndex]);
    }

    if (givesQuest)
    {
        DropItem();

        foreach (Mission m in dialogueData.missions)
{
    MissionController.Instance.AcceptMission(m);
    questState = QuestState.InProgress;
}

        savecontroller.Instance?.SaveGame();

        
        
    }

    dialogueIndex = nextIndex;
    dialogueUI.ClearChoices();
    DisplayCurrentLines();
}



    void DisplayCurrentLines()
    {
        dialogueUI.ShowChoices(false);
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }


public void EndDialogue()
{
    StopAllCoroutines();
            SoundManager.SetMuted(false);


    // 🔊 STOP VOICE
    if (audioSource != null && audioSource.isPlaying)
    {
        audioSource.Stop();
    }

    isDialogueActive = false;
    dialogueUI.SetDialogueText("");
    dialogueUI.ShowDialogueUI(false);
    PauseController.SetPause(false);
}


void DropItem()
{
    Transform ownerTransform = transform;

    float minDistance = Mathf.Min(itemDropMinDistance, itemDropMaxDistance);
    float maxDistance = Mathf.Max(itemDropMinDistance, itemDropMaxDistance);

    Vector2 dropOffset = Random.insideUnitCircle.normalized * 
                         Random.Range(minDistance, maxDistance);

    Vector2 dropPosition = (Vector2)ownerTransform.position + dropOffset;

    GameObject droppedItem = Instantiate(
        ItemPrefab,
        dropPosition,
        Quaternion.identity
    );

    droppedItem.GetComponent<Bouncing>()?.StartBounce();

    

}
}