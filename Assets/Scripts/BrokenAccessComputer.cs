using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;



public class BrokenAccessComputer : MonoBehaviour, IInteractable, IChestable
{
    private const string RequiredQuestId = "2.1";

    private const string ExpectedWordlistPath = "/usr/share/wordlists/dirb/common.txt";
    private const string GobusterOutput = @"
============================================================
Gobuster v3.6
by OJ Reeves (@TheColonial) & Christian Mehlmauer (@firefart)
============================================================
[+] Url:                     http://bank.com
[+] Method:                  GET
[+] Threads:                 10
[+] Wordlist:                /usr/share/wordlists/dirb/common.txt
[+] Negative Status codes:   404
[+] User Agent:              gobuster/3.6
[+] Timeout:                 10s
===============================================================
Starting gobuster in directory enumeration mode
===============================================================
/admin.php              <color=#2ECC71>(Status: 200)</color> <color=#95A5A6>[Size: 278]</color>
/internal/transfer.php  <color=#2ECC71>(Status: 200)</color> <color=#95A5A6>[Size: 450]</color>
/db_test.php            <color=#2ECC71>(Status: 200)</color> <color=#95A5A6>[Size: 55]</color>
/images                 <color=#F1C40F>(Status: 301)</color> <color=#95A5A6>[Size: 312]</color>
/uploads                <color=#F1C40F>(Status: 301)</color> <color=#95A5A6>[Size: 315]</color>
===============================================================
Finished.";


    public GameObject BrokenPanel;
        public PlayerInput playerInput;

        [SerializeField] GameObject guider;

        public GuiderBot guiderBot;
        public GameObject chatbubble;
        public GameObject Xbutton;
        public GameObject LongHint;

  

      

        public TMP_InputField urlInput;

        public TMP_InputField worklistInput;
        public TMP_InputField commandInput;

            public GameObject timer;




// Search Bar in Firefox
        public Sprite Login;
        public Sprite Transfer;
        public Sprite db_test;
        public Sprite admin;
        public Sprite NotFound;
        public TMP_InputField firefoxSearchBar;
        public Image firefoxSearchResults;
        public Slider firefoxLoadingBar;
        [SerializeField] private float firefoxPageLoadDuration = 0.8f;
        private Coroutine firefoxLoadingRoutine;

        public terminal terminalScript;



[Header("Files Search")]

        public GameObject noFiles;
        public TMP_InputField searchBar;

        public GameObject FilePathInput;
        public TMP_InputField filePathInput;

        public GameObject searchPage;

        public Image searchPageImage;

        public GameObject trainingText;
        public Sprite SearchPageDefault;

        public Sprite SearchPageFound;

        public GameObject doneButton;

        public TMP_InputField UpdateText;
        [SerializeField] private ScrollRect outputScrollRect;
        [SerializeField, Range(0.01f, 0.5f)] private float successScrollStep = 0.12f;

        //player hint

        public GameObject playerHint;

     private bool MissionTime = false;
    public bool brainTime = false;
    [SerializeField] private float typewriterDelay = 0.002f;
    [SerializeField, Min(1)] private int charsPerStep = 4;
    private Coroutine typewriterRoutine;

        
//IChestable Variable
        public bool IsOpened {get; private set;}
        [SerializeField] private int stableChestID;
        public int ChestID => stableChestID;

        public GameObject itemPrefab;
        public Sprite openedSprite;

 [Header("Brain Time Variables")]
        public GameObject lockImage;

        public GameObject brainText;

        public Button bombButton;
        public bombButton bombButtonScript;

        public CanvasGroup brainCanvas;

        public GameObject terminalCanvas;

        public GameObject filesPage;
        private CanvasGroup brainTextCanvas;
        private TMP_Text brainTextTMP;

        public CanvasGroup brainTextContainer;

[Header("Explosion Variables")]
        public GameObject Wall_to_Break;
        public Sprite brokenWall;




        public static bool FirstTime = false;
        private bool hasTriggeredUrlMatchDialogue;
        private bool hasTriggeredFirefoxCopyDialogue;
        private bool hasTriggeredFilePathDialogue;
        private bool isFilePathListenerBound;
        private RectTransform guiderRect;
        private Vector2 guiderInitialAnchoredPosition;
        private bool hasGuiderInitialAnchoredPosition;
        private int lastCommandSubmitFrame = -1;
        private int lastSearchSubmitFrame = -1;
        private bool PassedFirstMision = false;
        private bool PassedSecondMision = false;
        private bool isApplyingCommandHighlight;
        




 void Awake()
    {
        if (stableChestID == 0)
        {
            stableChestID = GenerateStableChestID();
        }
    }

 void Start()
    {
        firefoxSearchBar.text = "http://bank.com";
        

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
        if (!isFilePathListenerBound)
        {
            ConfigureFilePathInputChangeListener();
        }

        HandleSearchShortcut();
        HandleSearchCloseShortcut();
        HandleFirefoxCopyShortcut();

        if (!IsEnterPressedThisFrame())
        {
            return;
        }


        if (!MissionTime && urlInput != null && urlInput.isFocused)
        {

            checkCommand();
            return;
        }

        if (!MissionTime && worklistInput != null && worklistInput.isFocused)
        {
            checkCommand();
            return;
        }

        if ((MissionTime || brainTime) && commandInput != null && commandInput.isFocused)
        {
            checkCommand();
            return;
        }

        if (searchPage != null && searchPage.activeSelf && searchBar != null && searchBar.isFocused)
        {
            HandleSearchPageSubmit();
            return;
        }
        
        if (firefoxSearchBar != null && firefoxSearchBar.isFocused)
        {
            searchfirefoxPage();
            Debug.Log("Search Enter Pressed in Firefox Search Bar");
        }


    }


    public void searchfirefoxPage()
    {
        if (firefoxSearchBar == null || firefoxSearchResults == null)
        {
            return;
        }

        string searchText = firefoxSearchBar.text.Trim();
        Sprite nextPage = GetFirefoxPageSprite(searchText);

        if (firefoxLoadingRoutine != null)
        {
            StopCoroutine(firefoxLoadingRoutine);

        }


        firefoxLoadingRoutine = StartCoroutine(LoadFirefoxPage(nextPage));
    }

    private Sprite GetFirefoxPageSprite(string searchText)
    {
       /* if (searchText.Equals("http://bank.com/login.php", System.StringComparison.OrdinalIgnoreCase))
        {   
            guiderBot.StartDialogueRange(35, 36);
            closeGuiderBot();
            LeanTween.scale(timer, new Vector3(1, 1, 0), 0.3f)
    .setEase(LeanTweenType.easeInOutBack);
    Timer.Instance.StartTimer();
            return Login;
        } */

        if (searchText.Equals("http://bank.com/admin.php", System.StringComparison.OrdinalIgnoreCase))
        {
                    SoundManager.Play("Success");

            StartTypewriter("Hit Enter to excecute command") ;
            guiderBot.StartDialogueRange(35, 36, ()=>{

           LeanTween.moveLocal ( timer,  new Vector3 (317,93,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
                  LeanTween.scale(timer, new Vector3(1, 1, 0), 0.3f)
    .setEase(LeanTweenType.easeInOutBack);
    
        Timer.Instance.ResetTimer();
    Timer.Instance.StartTimer();
            });

                Refreshbutton();
                  commandInput.gameObject.SetActive(true);
                  commandInput.Select();
                  commandInput.ActivateInputField();
                  trainingText.SetActive(false);
          
    BackgroundMusicManager.SwitchToMissionMusic();
        MissionTime = true;
        terminalScript.Training = false;
        


            closeGuiderBot();
            return admin;
        }

        if (searchText.Equals("http://bank.com/internal/transfer.php", System.StringComparison.OrdinalIgnoreCase))
        {
          
                    SoundManager.Play("Success");

            return Transfer;

        }

        if (searchText.Equals("http://bank.com/db_test.php", System.StringComparison.OrdinalIgnoreCase))
        {
                    SoundManager.Play("Success");

              if (MissionTime == true)
            {
               

        QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "2.2");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("2");

        
        PassedSecondMision = true;
        AllMissionCompleted();

      

        

            }

          
            return db_test;
        }

        SoundManager.Play("failure");
        return NotFound;
    }

    public void AllMissionCompleted()
    {
        if (PassedFirstMision == true && PassedSecondMision == true)
        {
            Refreshbutton();
            IsOpened = true;
            brainTime = true;
            bombButtonScript.BrainTime = true;

            MissionTime = false;
            terminalScript.brainTime = true;
            BackgroundMusicManager.SwitchToBrainTimeMusic();

            
            filesPage.SetActive(false);

StartTypewriter("Hit Enter to excecute command") ;
            if (commandInput != null)
            {
                commandInput.SetTextWithoutNotify(string.Empty);
            }

             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(1, "Broken Access Control", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);

        StartCoroutine(FadeInWaitAndFadeOut(brainCanvas, 3f));
        brainText.SetActive(true);
    brainTextTMP = brainText.GetComponent<TMP_Text>();

        brainTextTMP.text = "Input a learned terminal command to deduct 10s off the clock";
    


        PositionBrainTextContainer(-114.27f);
        brainTextContainer.alpha = 0f;
        StartCoroutine(FadeInWithDelay(brainTextContainer, 1f));
      LeanTween.moveY(bombButton.gameObject, 105.5f, 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(8f);
      
        

    
        }
    }

            private void PositionBrainTextContainer(float positionY)
    {
 

        RectTransform containerRect = brainTextContainer.GetComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(containerRect.anchoredPosition.x, positionY);
    }
IEnumerator FadeInWithDelay(CanvasGroup canvasGroup, float delaySeconds)
{
    yield return new WaitForSeconds(delaySeconds);
    yield return StartCoroutine(FadeIn(canvasGroup));
}

    public void Refreshbutton(){
        firefoxSearchBar.text = "http://bank.com";
    LoadFirefoxPage(Login);
        }

    private IEnumerator LoadFirefoxPage(Sprite targetPage)
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

    public void closeSearchPage()
    {
        if (searchPage != null)
        {
            searchPage.SetActive(false);
        }
    }

    private void HandleSearchShortcut()
    {
        bool isCtrlPressed;
        bool isShiftPressed;
        bool isFPressedThisFrame;

        if (Keyboard.current == null)
        {
            isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            isFPressedThisFrame = Input.GetKeyDown(KeyCode.F);
        }
        else
        {
            isCtrlPressed = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
            isShiftPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            isFPressedThisFrame = Keyboard.current.fKey.wasPressedThisFrame;
        }

        if (!isCtrlPressed || !isShiftPressed || !isFPressedThisFrame)
        {
            return;
        }

        RestoreGuiderUiPosition();
        CenterSearchPage();
        searchPage.SetActive(true);
        searchBar.Select();
        searchBar.ActivateInputField();

        if (!MissionTime && !brainTime)
        {
            guiderBot.StartDialogueRange(29, 29);
        }
    }

    private void CenterSearchPage()
    {
        if (searchPage == null)
        {
            return;
        }

        RectTransform rect = searchPage.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
    }

    private void HandleSearchCloseShortcut()
    {
        bool isEscapePressedThisFrame;

        if (Keyboard.current == null)
        {
            isEscapePressedThisFrame = Input.GetKeyDown(KeyCode.Escape);
        }
        else
        {
            isEscapePressedThisFrame = Keyboard.current.escapeKey.wasPressedThisFrame;
        }

        if (!isEscapePressedThisFrame || searchPage == null || !searchPage.activeSelf)
        {
            return;
        }

        if (searchBar != null)
        {
            searchBar.text = string.Empty;
            searchBar.DeactivateInputField();
        }

        searchPage.SetActive(false);
    }

    private void HandleFirefoxCopyShortcut()
    {
        if (hasTriggeredFirefoxCopyDialogue || guiderBot == null || firefoxSearchBar == null || !firefoxSearchBar.isFocused)
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

        hasTriggeredFirefoxCopyDialogue = true;
        guiderBot.StartDialogueRange(24, 24);
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
        ConfigureInput(urlInput);
        ConfigureInput(worklistInput);
        ConfigureInput(commandInput);
        ConfigureInput(filePathInput);
        ConfigureFirefoxSearchInput();
        ConfigureSearchBarInput();
        ConfigureUrlInputChangeListener();
        ConfigureFilePathInputChangeListener();
        ConfigureCommandInputHighlighting();
    }

    private void ConfigureSearchBarInput()
    {
        if (searchBar == null)
        {
            return;
        }

        searchBar.lineType = TMP_InputField.LineType.SingleLine;
        searchBar.onFocusSelectAll = false;
        searchBar.onEndEdit.RemoveListener(OnSearchBarEndEdit);
        searchBar.onEndEdit.AddListener(OnSearchBarEndEdit);
    }

    private void OnSearchBarEndEdit(string _)
    {
        if (!BrokenPanel.activeSelf || searchPage == null || !searchPage.activeSelf)
        {
            return;
        }

        HandleSearchPageSubmit();
    }

    private void HandleSearchPageSubmit()
    {
        if (lastSearchSubmitFrame == Time.frameCount)
        {
            return;
        }

        lastSearchSubmitFrame = Time.frameCount;

        string searchText = searchBar.text.Trim();
        if (searchText.Equals("common.txt", System.StringComparison.OrdinalIgnoreCase))
        {
            searchPageImage.sprite = SearchPageFound;
            FilePathInput.SetActive(true);
            noFiles.SetActive(false);

            if (MissionTime == false)
            {
                ShowHint(89.26f, 77.9f);
                guiderBot.StartDialogueRange(30, 30);
            }
        }
        else
        {
            searchPageImage.sprite = SearchPageDefault;
            noFiles.SetActive(true);
            FilePathInput.SetActive(false);
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

    private void ConfigureFilePathInputChangeListener()
    {
        if (isFilePathListenerBound)
        {
            return;
        }

        if (filePathInput == null && FilePathInput != null)
        {
            filePathInput = FilePathInput.GetComponent<TMP_InputField>();
        }

        if (filePathInput == null && FilePathInput != null)
        {
            filePathInput = FilePathInput.GetComponentInChildren<TMP_InputField>(true);
        }

        if (filePathInput == null)
        {
            return;
        }

        filePathInput.onValueChanged.RemoveListener(OnFilePathInputChanged);
        filePathInput.onValueChanged.AddListener(OnFilePathInputChanged);
        isFilePathListenerBound = true;
    }

    private void OnFilePathInputChanged(string currentValue)
    {
        EvaluateFilePathMatch(currentValue);
    }

    private void EvaluateFilePathMatch(string currentValue)
    {
        if (guiderBot == null)
        {
            return;
        }

        string normalizedValue = currentValue == null ? string.Empty : currentValue.Trim();
        bool isPathMatch = normalizedValue.Equals(ExpectedWordlistPath, System.StringComparison.OrdinalIgnoreCase);

        if (isPathMatch && !hasTriggeredFilePathDialogue)
        {
            hasTriggeredFilePathDialogue = true;
            guiderBot.StartDialogueRange(31, 31);
            return;
        }

        if (!isPathMatch)
        {
            hasTriggeredFilePathDialogue = false;
        }
    }

    private void ConfigureUrlInputChangeListener()
    {
        if (urlInput == null)
        {
            return;
        }

        urlInput.onValueChanged.AddListener(OnUrlInputChanged);
    }

    private void OnUrlInputChanged(string currentValue)
    {
        if (guiderBot == null)
        {
            return;
        }

        bool isUrlMatch = currentValue.Trim().Equals("http://bank.com", System.StringComparison.OrdinalIgnoreCase);

        if (isUrlMatch && !hasTriggeredUrlMatchDialogue)
        {
            hasTriggeredUrlMatchDialogue = true;
            guiderBot.StartDialogueRange(25, 25);

            guiderBot.StartDialogueRangeDelayed(27, 27, 2.0f);
            StartCoroutine(ShowHintAfterDelay(2.0f, -272.9f, 200.1f));
           
            
            return;
        }

        if (!isUrlMatch)
        {
            hasTriggeredUrlMatchDialogue = false;
        }
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
        if (!BrokenPanel.activeSelf || !IsEnterPressedNow())
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

    private void ConfigureFirefoxSearchInput()
    {
        if (firefoxSearchBar == null)
        {
            return;
        }

        firefoxSearchBar.lineType = TMP_InputField.LineType.SingleLine;
        firefoxSearchBar.onFocusSelectAll = false;
        firefoxSearchBar.onEndEdit.AddListener(OnFirefoxSearchEndEdit);
    }

    private void OnFirefoxSearchEndEdit(string _)
    {
        if (!BrokenPanel.activeSelf || !IsEnterPressedNow())
        {
            return;
        }

        searchfirefoxPage();
    }



      public void checkCommand()
    {
                string normalizedUrl = urlInput != null ? urlInput.text.Trim() : string.Empty;
                string normalizedWordlist = worklistInput != null ? worklistInput.text.Trim() : string.Empty;

        if (!MissionTime && !brainTime)
        {
                if (normalizedUrl.Equals("http://bank.com", System.StringComparison.OrdinalIgnoreCase)
                        && normalizedWordlist.Equals(ExpectedWordlistPath, System.StringComparison.OrdinalIgnoreCase))
        {
                    SoundManager.Play("Success");

            StartTypewriter(GobusterOutput);

            if (MissionTime == false){
                guiderBot.StartDialogueRange(32, 32, ()=>{
                    guiderBot.StartDialogueRangeDelayed(133, 133, 1.0f);
                    LeanTween.scale(doneButton, new Vector3(1, 1, 0), 0.3f)
    .setEase(LeanTweenType.easeInOutBack).setDelay(0.5f);
                });
                terminalScript.element34 = true;
 
                  

            }
                
        }
        else
        {
            Debug.Log("Incorrect Command");
             StartTypewriter("invalid command") ;
        }    
        } else {
        string command = commandInput != null ? StripRichText(commandInput.text).Trim() : string.Empty;
        if (command.Equals("gobuster dir -u http://bank.com -w /usr/share/wordlists/dirb/common.txt", System.StringComparison.OrdinalIgnoreCase))
        {
                    SoundManager.Play("Success");

            StartTypewriter(GobusterOutput);

            Debug.Log("Correct Command");

            if (MissionTime == true)
                {
                            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "2.1");
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
            savecontroller.Instance.DeductSavedLevelTime(1, 10f);
               
                    lockImage.SetActive(false);
                    brainTime = false;
                    brainText.SetActive(false);
                    BackgroundMusicManager.SwitchToDefaultMusic();
                }
                        
        }
        else
        {
            Debug.Log("Incorrect Command");
            StartTypewriter("invalid command");
        }    
        }
        
    }

    public void doneButtonClicked()
    {
            LeanTween.scale(doneButton, new Vector3(0, 0, 0), 0.3f)
    .setEase(LeanTweenType.easeInOutBack);

            guiderBot.StartDialogueRange(33, 33, ()=>{
                ShowHint(-211.7f, 197.5f);
            });

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

        BrokenPanel.SetActive(!BrokenPanel.activeSelf);
        guider.SetActive(!guider.activeSelf);

if (MissionTime == false)
{
terminalScript.Training = true;
FirstTimeGuider();
}
    


  
        
        if (BrokenPanel.activeSelf == true)
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


    public void closeBrokenPanel()
    {
        BrokenPanel.SetActive(false);
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
            {           StartCoroutine(ShowLongHintAfterDelay(2.5f,-23f, 153f, 3.04f, 0.72f));

                guiderBot.StartDialogueRange(16, 20, () =>
            {
                        ShowHint(-190.6f, 200.1f);
              
            });
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

private IEnumerator ShowHintAfterDelay(float delay, float positionX, float positionY)
{
    yield return new WaitForSeconds(delay);
    ShowHint(positionX, positionY);
}

private IEnumerator ShowLongHintAfterDelay(float delay, float positionX, float positionY, float scaleX, float scaleY)
{
    yield return new WaitForSeconds(delay);
    ShowLongHint(positionX, positionY, scaleX, scaleY);
}

public void ShowLongHint(float positionX , float positionY, float Xscale = 0.98f, float Yscale = 0.32f){

     SoundManager.Play("playerHint");

    RectTransform hintRect = LongHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

    LeanTween.scale(LongHint, new Vector3(Xscale, Yscale, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
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
    }

    if (filesPage != null)
    {
        filesPage.SetActive(false);
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
        brainTextTMP.text = "Run gobuster in Terminal to deduct 10s off the clock";
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
        
        LeanTween.scale(guider, new Vector3(0, 0, 1), 0.6f)
        .setDelay(6f)
    .setEase(LeanTweenType.easeOutQuart);
    }

}
