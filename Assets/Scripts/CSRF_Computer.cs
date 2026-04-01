using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;
public class CSRF_Computer : MonoBehaviour, IInteractable, IChestable
{
    private const string RequiredQuestId = "4.4";

[Header("Essentials")]
    public GameObject CSRFPanel;
        public PlayerInput playerInput;

        [SerializeField] GameObject guider;


        public GameObject Xbutton;
       

        public GameObject timer;
    [Header("Terminal Variables")]

        public terminal terminalScript;
        public TMP_InputField commandInput;

        public GameObject commandInputObject;

        public TMP_InputField UpdateText;
        [SerializeField] private ScrollRect outputScrollRect;
        [SerializeField, Range(0.01f, 0.5f)] private float successScrollStep = 0.12f;


    private string StripRichText(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return Regex.Replace(input, "<.*?>", string.Empty);
    }

        [Header("Guider Bot Variables")]

        public GameObject playerHint;

        public GameObject LongHint;
        public GameObject LongHint2;
        public GameObject LongHint3;
        public GuiderBot guiderBot;
        public GameObject chatbubble;

     private bool MissionTime = false;
    public bool brainTime = false;

            private bool PassedFirstMision = false;
        private bool PassedSecondMision = false;
        private bool PassedThirdMision = false;
        private bool PassedFourthMision = false;

    [SerializeField] private float typewriterDelay = 0.002f;
    [SerializeField, Min(1)] private int charsPerStep = 4;
    private Coroutine typewriterRoutine;
    private RectTransform guiderRect;
    private bool hasGuiderInitialAnchoredPosition;
        private Vector2 guiderInitialAnchoredPosition;

    private const float HINT_SOUND_COOLDOWN = 0.5f;
    private float lastHintSoundTime = -1f;



        
[Header("IChestable Variables")]
        public bool IsOpened {get; private set;}
        [SerializeField] private int stableChestID;
        public int ChestID => stableChestID;

        public GameObject itemPrefab;
        public Sprite openedSprite;

 [Header("Brain Time Variables")]
        public GameObject lockImage;

        public GameObject brainText;

        public Button bombButton;

        public CanvasGroup brainCanvas;

        public bombButton bombButtonScript;


        public CanvasGroup brainTextContainer;
        private TMP_Text brainTextTMP;

[Header("Explosion Variables")]
        public GameObject Wall_to_Break;
        public Sprite brokenWall;

        [Header("TopBar Variables")]
public GameObject terminalCanvas;





[Header("Inspect f12 variables")]

public GameObject f12InspectView;

public GameObject ExpandedView;

public TMP_InputField accountNumberFrom;
public TMP_InputField accountNumberTo;
public TMP_InputField amount;


[Header("CSRF Terminal Guider")]


public TMP_InputField targetlinkInput;
public TMP_InputField accountNumberFromInput;
public TMP_InputField accountNumberToInput;
public TMP_InputField amountInput;

public TMP_InputField fileNameInput;

public TMP_InputField copyCommandInput;

public TMP_InputField fileLocationInput;

public GameObject createdFileGuider;
public GameObject copyFileGuider;


[Header("Whatsapp Variables")]

public GameObject whatsappPanel;
public Transform contentLocation; 

public TMP_InputField whatsappInputField;

public GameObject whiteChatPrefab;
public GameObject greenChatPrefab;
[SerializeField] private GameObject whatsappEvidenceImagePrefab;
[SerializeField] private ScrollRect whatsappScrollRect;
private bool whatsappSubmitting = false;






private CanvasGroup feedbackAlertCanvasGroup;
private TMP_Text feedbackAlertText;
private Coroutine feedbackAlertRoutine;
private bool hasShownTerminalDialogue = false;
private bool hasShownInputFieldDialogue = false;
private bool allowOpenWhatsapp = false;


    void Awake()
    {
        if (stableChestID == 0)
        {
            stableChestID = GenerateStableChestID();
        }
    }

    void Start()
    {
        //Ichestable Start

        void OnInputFieldSelected(string _)
        {
            if (!MissionTime && !hasShownInputFieldDialogue)
            {
                hasShownInputFieldDialogue = true;
                guiderBot.StartDialogueRange(70, 70);
                ShowHint(-190.9f, 199.2f);
            }
        }

        void OnTerminalSubmitted(string _)
        {
            checkCommand();
        }

        void OnGuidedCommandSubmitted(string _)
        {
            checkCommandGuided();
        }

        void OnCopyCommandSubmitted(string _)
        {
            checkCopyCommand();
        }

        accountNumberFrom.onSelect.AddListener(OnInputFieldSelected);
        accountNumberTo.onSelect.AddListener(OnInputFieldSelected);
        amount.onSelect.AddListener(OnInputFieldSelected);

        if (commandInput != null)
        {
            commandInput.onSubmit.AddListener(OnTerminalSubmitted);
        }

        if (targetlinkInput != null)
        {
            targetlinkInput.onSubmit.AddListener(OnGuidedCommandSubmitted);
        }

        if (accountNumberFromInput != null)
        {
            accountNumberFromInput.onSubmit.AddListener(OnGuidedCommandSubmitted);
        }

        if (accountNumberToInput != null)
        {
            accountNumberToInput.onSubmit.AddListener(OnGuidedCommandSubmitted);
        }

        if (amountInput != null)
        {
            amountInput.onSubmit.AddListener(OnGuidedCommandSubmitted);
        }

        if (fileNameInput != null)
        {
            fileNameInput.onSubmit.AddListener(OnGuidedCommandSubmitted);
        }

        if (copyCommandInput != null)
        {
            copyCommandInput.onSubmit.AddListener(OnCopyCommandSubmitted);
        }

        if (fileLocationInput != null)
        {
            fileLocationInput.onSubmit.AddListener(OnCopyCommandSubmitted);
        }

        void OnWhatsappSubmitted(string _)
        {
            SubmitWhatsappMessage();
        }

        if (whatsappInputField != null)
        {
            whatsappInputField.onSubmit.AddListener(OnWhatsappSubmitted);
        }

        if (targetlinkInput != null)
        {
            targetlinkInput.onValueChanged.AddListener(val =>
            {
                if (!brainTime && !MissionTime && val.Trim() == "http://bank.com/transfer")
                    {guiderBot.StartDialogueRange(73, 73);
                         ShowLongHint(152.7f, 39.1f);
                    }
            });
        }

        if (accountNumberFromInput != null)
        {
            accountNumberFromInput.onValueChanged.AddListener(val =>
            {
                if (!brainTime && !MissionTime && val.Trim() == "account_number_from")
                    {guiderBot.StartDialogueRange(74, 74);
                         ShowLongHint2(149.9f, -1f);
                    }
            });
        }

        if (accountNumberToInput != null)
        {
            accountNumberToInput.onValueChanged.AddListener(val =>
            {
                if (!brainTime && !MissionTime && val.Trim() == "account_number_to")
                    {guiderBot.StartDialogueRange(75, 75);
                         ShowLongHint3(135.7f, -51.5f);
                    }
            });
        }

        if (amountInput != null)
        {
            amountInput.onValueChanged.AddListener(val =>
            {
                if (!brainTime && !MissionTime && Regex.IsMatch(val.Trim(), @"^amount\s*=\s*\d+$"))
                    {guiderBot.StartDialogueRange(76, 76);
                    }
            });
        }

        if (fileNameInput != null)
        {
            fileNameInput.onValueChanged.AddListener(val =>
            {
                if (!brainTime && !MissionTime && val.Trim() == "index.html")
                    guiderBot.StartDialogueRange(77, 77);
            });
        }

        if (copyCommandInput != null)
        {
            copyCommandInput.onValueChanged.AddListener(val =>
            {
                if (!MissionTime && val.Trim() == "cp index.html")
                    guiderBot.StartDialogueRange(80, 80);
            });
        }

        if (fileLocationInput != null)
        {
            fileLocationInput.onValueChanged.AddListener(val =>
            {
                if (!MissionTime && val.Trim() == "/var/www/html/index.html")
                    guiderBot.StartDialogueRange(81, 81);
            });
        }

        

                {
            whatsappInputField.onValueChanged.AddListener(val =>
            {
                if (!MissionTime && val.Trim() == "http://192.168.21.129/index.html")
                    guiderBot.StartDialogueRange(81, 81);
            });
        }

        if (commandInput != null)
        {
            commandInput.onValueChanged.AddListener(val =>
            {
                if (!MissionTime && val.Trim() == "sudo systemctl start apache2")
                    guiderBot.StartDialogueRange(83, 83);

                if (!MissionTime && val.Trim() == "hostname -I")
                    guiderBot.StartDialogueRange(86, 86);
            });
        

        }

        
    }

    void Update()
    {
        if (CSRFPanel.activeSelf && IsCtrlCPressedThisFrame())
        {
            HideHintPopups();
        }

        if (Keyboard.current != null && Keyboard.current[Key.I].wasPressedThisFrame && CSRFPanel.activeSelf && !terminalCanvas.activeSelf && !whatsappPanel.activeSelf)
        {
          
            
                f12InspectView.SetActive(!f12InspectView.activeSelf);

                if (f12InspectView.activeSelf == true && !MissionTime){
                                    ShowLongHint(39.7f, 60.8f);

                }else{
                    LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
 
                }
                if (MissionTime == false)
                {
                guiderBot.StartDialogueRange(67, 67);
                }
                

            
        }
    }

    private bool IsCtrlCPressedThisFrame()
    {
        if (Keyboard.current == null)
        {
            bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            return ctrlPressed && Input.GetKeyDown(KeyCode.C);
        }

        bool isCtrlPressed = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
        bool isCPressedThisFrame = Keyboard.current.cKey.wasPressedThisFrame;
        return isCtrlPressed && isCPressedThisFrame;
    }

    private void HideHintPopups()
    {
        if (playerHint != null)
        {
            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                .setEase(LeanTweenType.easeInOutBack);
        }

        if (LongHint != null)
        {
            LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                .setEase(LeanTweenType.easeInOutBack);
        }

        if (LongHint2 != null)
        {
            LeanTween.scale(LongHint2, new Vector3(0, 0, 0), 0.3f)
                .setEase(LeanTweenType.easeInOutBack);
        }

        if (LongHint3 != null)
        {
            LeanTween.scale(LongHint3, new Vector3(0, 0, 0), 0.3f)
                .setEase(LeanTweenType.easeInOutBack);
        }
    }

//CSRF Computer Methods

public void toggleExpandedView()
    {

            ExpandedView.SetActive(!ExpandedView.activeSelf);

            if (!ExpandedView.activeSelf)
             {
            LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
                         LeanTween.scale(LongHint2, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
                         LeanTween.scale(LongHint3, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
                         

             } else{
                if (MissionTime == false && !brainTime)
            {
                TweenGuiderAnchoredX(362f, 0.3f);
                guiderBot.StartDialogueRange(68, 69);
                ShowLongHint(152.7f, 39.1f);
                ShowLongHint2(149.9f, -1f);
                ShowLongHint3(135.7f, -51.5f);
            }
             }

            
        
    }


//IChestable & IInteractable Methods
private int GenerateStableChestID()
    {
        string scenePath = gameObject.scene.path;
        string hierarchyPath = GetHierarchyPath(transform);
        string key = scenePath + "|" + hierarchyPath;
        return DeterministicHash(key);
    }

    private string GetHierarchyPath(Transform target)
    {
        string path = target.name;
        Transform parent = target.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }

    private int DeterministicHash(string input)
    {
        unchecked
        {
            const int offsetBasis = (int)2166136261;
            const int prime = 16777619;
            int hash = offsetBasis;

            for (int index = 0; index < input.Length; index++)
            {
                hash ^= input[index];
                hash *= prime;
            }

            return hash == 0 ? 1 : hash;
        }
    }


     public void OpenChest()
{

   SoundManager.Play("timer");
   StartCoroutine(OpenChestRoutine());

}

private IEnumerator OpenChestRoutine()
    {
        yield return new WaitForSeconds(3f);
         SetOpened(true);
        savecontroller.Instance?.SaveGame();
    SoundManager.Play("Dynamite");

    if (itemPrefab)
    {
        GameObject droppedItem =
            Instantiate(itemPrefab, transform.position , Quaternion.identity);
            //i want to change some tiles with collision to non collision and change their sprite

    }

  
    
    }


    public void SetOpened(bool opened)
    {
        if (opened)
        {
              Wall_to_Break.GetComponent<Collider2D>().enabled = false;
    Wall_to_Break.GetComponent<SpriteRenderer>().sprite = brokenWall;
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }

 public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        CSRFPanel.SetActive(!CSRFPanel.activeSelf);
        guider.SetActive(!guider.activeSelf);

if (MissionTime == false)
{
terminalScript.Training = true;
FirstTimeGuider();
}
        


  
        
        if (CSRFPanel.activeSelf == true)
        {
        PauseController.SetPause(true);
                playerInput.SwitchCurrentActionMap("UI");
               

        }
        else
        {
            PauseController.SetPause(false);
                    playerInput.SwitchCurrentActionMap("Player");

        }

    }


    public void closeCSRFPanel()
    {
        CSRFPanel.SetActive(false);
        guider.SetActive(false);
        PauseController.SetPause(false);
                playerInput.SwitchCurrentActionMap("Player");
    }


 


    public bool CanInteract()
    {
        return !IsOpened && HasRequiredMission();
    }

    private bool HasRequiredMission()
    {
        if (MissionController.Instance == null || MissionController.Instance.activeMissions == null)
        {
            return false;
        }

        return MissionController.Instance.activeMissions.Exists(q =>
            q != null && string.Equals(q.questID, RequiredQuestId, System.StringComparison.OrdinalIgnoreCase));
    }


    //TopBar Methods
public void closeTerminal()
    {

        terminalCanvas.SetActive(false);
         
    }

public void OpenBankPage(){
        whatsappPanel.SetActive(false);
        closeTerminal();

}

public void openWhatsapp(){
    LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

    if (!allowOpenWhatsapp)
    {
        return;
    }
    whatsappPanel.SetActive(true);

    closeTerminal();

    if (MissionTime == false && !brainTime)
    {
        guiderBot.StartDialogueRange(89, 89);
    }
}

private void SubmitWhatsappMessage()
{
    if (whatsappSubmitting) return;
    if (whatsappInputField == null) return;

    string userMessage = whatsappInputField.text.Trim();
    if (string.IsNullOrEmpty(userMessage))
    {
        return;
    }

    whatsappInputField.SetTextWithoutNotify(string.Empty);
    StartCoroutine(SubmitWhatsappMessageRoutine(userMessage));
}

private IEnumerator SubmitWhatsappMessageRoutine(string userMessage)
{
    whatsappSubmitting = true;

    InstantiateWhatsappMessage(greenChatPrefab, userMessage, true);

    bool isCorrectPhishingLink = userMessage.Equals("http://192.168.21.129/index.html", System.StringComparison.OrdinalIgnoreCase);
    string replyMessage = isCorrectPhishingLink
        ? "WHAT? I DIDNT MAKE THIS TRANSFER!"
        : "Huh? what do you mean?";

    yield return new WaitForSecondsRealtime(1f);

    if (isCorrectPhishingLink)
    {
        SoundManager.Play("Success");

        InstantiateWhatsappImageMessage(false);
        yield return new WaitForSecondsRealtime(0.2f);
        InstantiateWhatsappMessage(whiteChatPrefab, replyMessage, false);
    
        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(90, 92, () =>{

                LeanTween.moveLocal ( timer,  new Vector3 (228.9f,170.5f,0f) ,2f )
                    .setEase(LeanTweenType.easeInOutBack);
                     LeanTween.scale(timer, new Vector3(1, 1, 0), 0.3f)
                    .setEase(LeanTweenType.easeInOutBack);

                    Timer.Instance.ResetTimer();
                    Timer.Instance.StartTimer();
                    BackgroundMusicManager.SwitchToMissionMusic();

                    closeGuiderBot();
                    commandInput.SetTextWithoutNotify(string.Empty);
                    StartTypewriter("");
                    commandInputObject.SetActive(false);
                    createdFileGuider.SetActive(true);

                    MissionTime = true;

            } 
            
            );

                
        } else if (MissionTime == true)
                    {
                                          SoundManager.Play("Success");
            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "4.4");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("4");

        
        PassedFourthMision = true;
        AllMissionCompleted();
                    
        } 


    
    }
    else
    {
        InstantiateWhatsappMessage(whiteChatPrefab, replyMessage, false);
    }

    yield return new WaitForEndOfFrame();
    ScrollWhatsappToBottom();
    whatsappSubmitting = false;
}

private GameObject InstantiateWhatsappMessage(GameObject chatPrefab, string message, bool alignRight)
{
    if (chatPrefab == null || contentLocation == null)
    {
        return null;
    }

    GameObject bubble = Instantiate(chatPrefab, contentLocation);

    TMP_Text bubbleText = bubble.GetComponentInChildren<TMP_Text>(true);
    if (bubbleText != null)
    {
        bubbleText.text = message;
    }

    ConfigureWhatsappBubbleAlignment(bubble, alignRight);

    return bubble;
}

private GameObject InstantiateWhatsappImageMessage(bool alignRight)
{
    if (whatsappEvidenceImagePrefab == null || contentLocation == null)
    {
        return null;
    }

    GameObject imageMessage = Instantiate(whatsappEvidenceImagePrefab, contentLocation);
    ConfigureWhatsappBubbleAlignment(imageMessage, alignRight);
    return imageMessage;
}

private void ConfigureWhatsappBubbleAlignment(GameObject bubble, bool alignRight)
{
    if (bubble == null)
    {
        return;
    }

    RectTransform bubbleRect = bubble.GetComponent<RectTransform>();
    if (bubbleRect == null)
    {
        return;
    }

    bubbleRect.anchorMin = alignRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
    bubbleRect.anchorMax = alignRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
    bubbleRect.pivot = alignRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);

    Vector2 anchored = bubbleRect.anchoredPosition;
    anchored.x = alignRight ? -20f : 20f;
    bubbleRect.anchoredPosition = anchored;
}

private void ScrollWhatsappToBottom()
{
    if (whatsappScrollRect == null && contentLocation != null)
    {
        whatsappScrollRect = contentLocation.GetComponentInParent<ScrollRect>();
    }

    StartCoroutine(ScrollWhatsappToBottomRoutine());
}

private IEnumerator ScrollWhatsappToBottomRoutine()
{
    yield return null;

    RectTransform contentRect = contentLocation as RectTransform;
    if (contentRect != null)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    Canvas.ForceUpdateCanvases();
    whatsappScrollRect.verticalNormalizedPosition = 0f;
}
    public void toggleTerminal()
    {
    
    LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
            LeanTween.scale(LongHint2, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
            LeanTween.scale(LongHint3, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
        
        bool willOpen = !terminalCanvas.activeSelf;
        if (willOpen)
        {
            CenterTerminalCanvas();
        }

        terminalCanvas.SetActive(willOpen);
        if (MissionTime || brainTime ) {
            return;
        }
        else if (!hasShownTerminalDialogue)
        {
        hasShownTerminalDialogue = true;

        guiderBot.StartDialogueRange(71, 72, ()=>{
ShowLongHint2(-64.3f, 153f, 3.48f, 0.68f);     
  });
        
  
        }
        
    }

    private void CenterTerminalCanvas()
    {

        RectTransform rect = terminalCanvas.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
    }

    //Terminal Methods

public void checkCommandGuided(){
    if (targetlinkInput.text.Trim() != "http://bank.com/transfer")
    {
        StartTypewriter("invalid Target Link");
        return;
    }

    if (accountNumberFromInput.text.Trim() != "account_number_from")
    {
        StartTypewriter("invalid variable 1");
        return;
    }

    if (accountNumberToInput.text.Trim() != "account_number_to")
    {
        StartTypewriter("invalid variable 2");
        return;
    }

    if (!Regex.IsMatch(amountInput.text.Trim(), @"^amount\s*=\s*\d+$"))
    {
        StartTypewriter("invalid amount");
        return;
    }

    if (fileNameInput.text.Trim() != "index.html")
    {
        StartTypewriter("invalid filename");
        return;
    }
    SoundManager.Play("Success");
    StartTypewriter("index.html created successfully!");
    targetlinkInput.SetTextWithoutNotify(string.Empty);
    accountNumberFromInput.SetTextWithoutNotify(string.Empty);
    accountNumberToInput.SetTextWithoutNotify(string.Empty);
    amountInput.SetTextWithoutNotify(string.Empty);
    fileNameInput.SetTextWithoutNotify(string.Empty);

    createdFileGuider.SetActive(false);
    copyFileGuider.SetActive(true);
    if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(78, 79);
            }
    else if (MissionTime == true)
                {
                            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "4.1");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("1");

        PassedFirstMision = true;
        AllMissionCompleted();
                }       
                 if (brainTime == true)
                {
                    OpenChest();
            bombButton.onClick.Invoke();
            NotificationManager.Instance.Display("Successfully deducted 10 seconds!");
            Timer.Instance.DeductTime(10f);
           savecontroller.Instance.DeductSavedLevelTime(3, 10f);               
                    lockImage.SetActive(false);
                    brainTime = false;
                    PositionBrainTextContainer( -50.772f);
                    brainTextContainer.alpha = 0f;
                    BackgroundMusicManager.SwitchToDefaultMusic();
                }
}

public void checkCopyCommand(){
  
    if (copyCommandInput.text.Trim() != "cp index.html")
    {
        StartTypewriter("invalid copy command");
        return;
    }

      if (fileLocationInput.text.Trim() != "/var/www/html/index.html")
    {
        StartTypewriter("invalid file location");
        return;
    }
            SoundManager.Play("Success");
            StartTypewriter("File copied successfully!");
            copyCommandInput.SetTextWithoutNotify(string.Empty);
            fileLocationInput.SetTextWithoutNotify(string.Empty);

            copyFileGuider.SetActive(false);
            commandInputObject.SetActive(true);

            if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(82, 82);
            }
                else if (MissionTime == true)
                    {
                                            SoundManager.Play("Success");
            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "4.2");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("2");

        
        PassedSecondMision = true;
        AllMissionCompleted();
                    }


}

    public void checkCommand()
    {
        
        string command = commandInput != null ? commandInput.text.Trim() : string.Empty;

        if (command.Equals("sudo systemctl start apache2", System.StringComparison.OrdinalIgnoreCase))
        {
            commandInput.SetTextWithoutNotify(string.Empty);

            SoundManager.Play("Success");
            StartTypewriter("Apache server started successfully!");

            if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(84, 85);          
                 }else if (MissionTime == true)
                    {
                                          SoundManager.Play("Success");
            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "4.3");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("3");

        
        PassedThirdMision = true;
        AllMissionCompleted();
                    }
        }
        else if (command.Equals("hostname -I", System.StringComparison.OrdinalIgnoreCase))
        {
            commandInput.SetTextWithoutNotify(string.Empty);

            SoundManager.Play("Success");
            StartTypewriter("<color=green>192.168.21.129 172.17.0.1</color>");

            if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(87, 88, () =>{
                allowOpenWhatsapp = true;
                ShowHint (-117.6f, 176.8f);
                    
                });
                }

        }
        else
        {
            commandInput.SetTextWithoutNotify(string.Empty);
            StartTypewriter("invalid command");
        }    
        
        
    }

    private void StartTypewriter(string fullText)
    {
        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
        }

        typewriterRoutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        UpdateText.text = string.Empty;

        int step = Mathf.Max(1, charsPerStep);
        for (int index = 0; index < fullText.Length; index += step)
        {
            int chunkLength = Mathf.Min(step, fullText.Length - index);
            UpdateText.text += fullText.Substring(index, chunkLength);
            yield return new WaitForSecondsRealtime(typewriterDelay);
        }

        typewriterRoutine = null;
    }


//Guider Methods

private void CacheGuiderUiPosition()
{
    if (guider == null)
    {
        return;
    }

    guiderRect = guider.GetComponent<RectTransform>();
    if (guiderRect == null)
    {
        return;
    }

    guiderInitialAnchoredPosition = guiderRect.anchoredPosition;
    hasGuiderInitialAnchoredPosition = true;
}

private void RestoreGuiderUiPosition()
{
    if (!hasGuiderInitialAnchoredPosition || guiderRect == null)
    {
        return;
    }

    guiderRect.anchoredPosition = guiderInitialAnchoredPosition;
}

private void TweenGuiderAnchoredX(float targetX, float duration, float delay = 0f)
{
    if (guiderRect == null && guider != null)
    {
        guiderRect = guider.GetComponent<RectTransform>();
    }

    if (guiderRect == null)
    {
        LTDescr fallbackTween = LeanTween.moveX(guider, targetX, duration)
            .setEase(LeanTweenType.easeInOutBack);

        if (delay > 0f)
        {
            fallbackTween.setDelay(delay);
        }

        return;
    }

    float startX = guiderRect.anchoredPosition.x;
    LTDescr tween = LeanTween.value(guider, startX, targetX, duration)
        .setEase(LeanTweenType.easeInOutBack)
        .setOnUpdate((float x) =>
        {
            Vector2 pos = guiderRect.anchoredPosition;
            pos.x = x;
            guiderRect.anchoredPosition = pos;
        });

    if (delay > 0f)
    {
        tween.setDelay(delay);
    }
}

public void FirstTimeGuider()
{
     
LeanTween.scale(guider, new Vector3(6, 6, 1), 0.6f)
    .setDelay(0.5f)
    .setEase(LeanTweenType.easeOutQuart)
    .setOnComplete(() =>
    {
        LeanTween.scale(chatbubble, new Vector3(1, 1, 1), 0.5f)
            .setEase(LeanTweenType.easeOutQuart)
            .setOnComplete(() =>
            {
                guiderBot.StartDialogueRange(60, 66);
            });
    });

LeanTween.scale(Xbutton, new Vector3(1, 1, 1), 0.3f)
    .setDelay(2.5f)
    .setEase(LeanTweenType.easeOutQuart);

}

public void closeGuiderBot()
    {
        float targetX = hasGuiderInitialAnchoredPosition ? guiderInitialAnchoredPosition.x : 290f;
        TweenGuiderAnchoredX(targetX, 0.3f, 2f);
        LeanTween.scale(guider, new Vector3(0, 0, 1), 0.6f)
        .setDelay(2f)
    .setEase(LeanTweenType.easeOutQuart);
    }



//show hint and play audio function
public void ShowHint(float positionX , float positionY){
    PlayHintSound();


    RectTransform hintRect = playerHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     
      LeanTween.scale(playerHint, new Vector3(1, 1, 0), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

public void ShowLongHint(float positionX , float positionY){

    PlayHintSound();

    RectTransform hintRect = LongHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     
    LeanTween.scale(LongHint, new Vector3(0.97f, 0.3f, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

public void ShowLongHint2(float positionX , float positionY, float Xscale = 0.98f, float Yscale = 0.32f){

    PlayHintSound();


    RectTransform hintRect = LongHint2.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

    LeanTween.scale(LongHint2, new Vector3(Xscale, Yscale, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

public void ShowLongHint3(float positionX , float positionY){
    PlayHintSound();


    RectTransform hintRect = LongHint3.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

    LeanTween.scale(LongHint3, new Vector3(0.5411814f, 0.2174604f, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

private IEnumerator ShowHintAfterDelay(float delay, float positionX, float positionY)
{
    yield return new WaitForSeconds(delay);
    ShowHint(positionX, positionY);
}

private void PlayHintSound()
{
    if (Time.time - lastHintSoundTime >= HINT_SOUND_COOLDOWN)
    {
        SoundManager.Play("playerHint");
        lastHintSoundTime = Time.time;
    }
}

private IEnumerator StartDialogueRangeDelayedWithCallback(int start, int end, float delay, System.Action onComplete)
{
    yield return new WaitForSeconds(delay);
    guiderBot.StartDialogueRange(start, end, onComplete);
}

private IEnumerator PlayDialogueRangeAndWait(int start, int end)
{
    bool finished = false;
    guiderBot.StartDialogueRange(start, end, () => finished = true);
    while (!finished)
    {
        yield return null;
    }
}




//BrainTime & Mission Methods

 public void AllMissionCompleted()
    {
        if (PassedFirstMision == true && PassedSecondMision == true && PassedThirdMision == true && PassedFourthMision == true)
        {
            IsOpened = true;
            brainTime = true;
            MissionTime = false;
            terminalScript.brainTime = true;
            bombButtonScript.BrainTime = true;
            BackgroundMusicManager.SwitchToBrainTimeMusic();

StartTypewriter("Hit Enter to excecute command") ;
            if (commandInput != null)
            {
                commandInput.SetTextWithoutNotify(string.Empty);
            }

             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(3, "CSRF Cross Site Request Forgery", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);

        StartCoroutine(FadeInWaitAndFadeOut(brainCanvas, 3f));



      LeanTween.moveY(bombButton.gameObject, 105.5f, 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(8f);
      
        

    
        }
    }

        

    private void PositionBrainTextContainer(float positionY)
    {
 

        RectTransform containerRect = brainTextContainer.GetComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(containerRect.anchoredPosition.x, positionY);
    }

IEnumerator FadeIn(CanvasGroup canvasGroup)
{
    for (float a = 0; a <= 1; a += Time.unscaledDeltaTime * 3)
    {
        canvasGroup.alpha = a;
        yield return null;
    }

    canvasGroup.alpha = 1f;
}

IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        for (float a = 1; a>= 0 ; a -= Time.unscaledDeltaTime * 3)
        {
            canvasGroup.alpha = a;
          yield return null;

        }

                canvasGroup.alpha = 0f;
    }

IEnumerator FadeInWaitAndFadeOut(CanvasGroup canvasGroup, float waitTime = 3f)
{
    yield return new WaitForSeconds(5f);  
    SoundManager.Play("brainTime");
    brainTextContainer.gameObject.SetActive(true);

    CenterBrainCanvas();
    yield return StartCoroutine(FadeIn(canvasGroup));
    ApplyBrainTimeUiTransition();
    yield return new WaitForSeconds(waitTime);

    yield return StartCoroutine(FadeOut(canvasGroup));
}

 private void CenterBrainCanvas()
{
    if (brainCanvas == null)
    {
        return;
    }

    RectTransform rect = brainCanvas.GetComponent<RectTransform>();
    if (rect == null)
    {
        return;
    }

    rect.anchorMin = new Vector2(0.5f, 0.5f);
    rect.anchorMax = new Vector2(0.5f, 0.5f);
    rect.pivot = new Vector2(0.5f, 0.5f);
    rect.anchoredPosition = Vector2.zero;
}

private void ApplyBrainTimeUiTransition()
{
    if (lockImage != null)
    {
        lockImage.SetActive(true);
    }

    if (terminalCanvas != null)
    {
        terminalCanvas.SetActive(true);
        whatsappPanel.SetActive(false);
        createdFileGuider.SetActive(true);
        commandInputObject.SetActive(false);
    }

 


    
    brainText.SetActive(true);
    brainTextTMP = brainText.GetComponent<TMP_Text>();

    if (brainTextTMP != null)
    {
        brainTextTMP.text = "Create a fake page to deduct 10s off the clock";
    }

    if (brainTextContainer != null)
    {
        PositionBrainTextContainer(-125.65f);
        brainTextContainer.alpha = 0f;

        StartCoroutine(FadeIn(brainTextContainer));
    }

    ScrollOutputToTop();
}




IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup)
{
    yield return StartCoroutine(FadeOut(canvasGroup));
    canvasGroup.gameObject.SetActive(false);
}

private void ScrollOutputToTop()
{
    if (outputScrollRect == null && UpdateText != null)
    {
        outputScrollRect = UpdateText.GetComponentInParent<ScrollRect>();
    }

    if (outputScrollRect == null)
    {
        return;
    }

    Canvas.ForceUpdateCanvases();
    outputScrollRect.verticalNormalizedPosition = 1f;
}
    





}
