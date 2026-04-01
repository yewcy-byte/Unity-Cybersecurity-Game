using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;

public class JWT_Computer : MonoBehaviour, IInteractable, IChestable
{
    private const string RequiredQuestId = "5.7";

    [Header("Essentials")]
    public GameObject CSRFPanel;
        public PlayerInput playerInput;

     


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

           [SerializeField] GameObject guider;
        public GuiderBot guiderBot;
        private RectTransform guiderRect;
        private bool hasGuiderInitialAnchoredPosition;
        private Vector2 guiderInitialAnchoredPosition;
        public GameObject chatbubble;

        public Item item;

     private bool MissionTime = false;
    public bool brainTime = false;

            private bool PassedFirstMision = false;
        private bool PassedSecondMision = false;
        private bool PassedThirdMision = false;
        private bool PassedFourthMision = false;
        private bool PassedFifthMision = false;
        private bool PassedSixthMision = false;
        private bool PassedSeventhMision = false;

        private bool doneRegistered = false;

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

        public Button bombButton;

        public CanvasGroup brainCanvas;


        public CanvasGroup brainTextContainer;
        private TMP_Text brainTextTMP;

[Header("Explosion Variables")]
        public GameObject Wall_to_Break;
        public Sprite brokenWall;

        [Header("TopBar Variables")]
public GameObject terminalCanvas;



[Header("Burp Suite Variables")]
public GameObject burpSuitePanel;
public GameObject burpSuiteButton;

public GameObject proxyPanel;

public GameObject InterceptPanel;

public GameObject RepeatPanel;

public Sprite burpSuiteInterceptedSprite;

public Sprite burpsuiteNotInterceptedSprite;

public TMP_InputField burpSuiterequestInput;

public TMP_InputField burpSuiteresponseInput;

public GameObject ActionListPanel;
public GameObject ActionButtonObject;

[Header("Inspect f12 variables")]

public GameObject f12InspectView;

public GameObject storageView;

public TMP_InputField storageTokenInput;

[Header("FoxyProxy Variables")]

public GameObject foxyproxyIconTopBar;

public GameObject foxyproxyEnabled;

public GameObject foxyproxyDisabled;

private bool isfoxyproxyEnabled = false;

[Header("JWTio Variables")]
public GameObject JWTioPanel;

public TMP_InputField JWTioTokenInput;
public TMP_InputField JWTioDecodedOutput;

[Header("Login Variables")]

public GameObject LoggedinPanel;
public TMP_InputField usernameInput;
public TMP_InputField passwordInput;

public TMP_InputField RegisterUsernameInput;
public TMP_InputField RegisterPasswordInput;
public TMP_InputField RegisterConfirmPasswordInput;

public TMP_Text RegisterHintText;


private string registeredName = "";

public GameObject RegisterPanel;


public TMP_Text LoginHintText;

private const string PremiumMembership = "premium";
private const string DefaultMembership = "standard";

private sealed class RegisteredAccount
{
    public string Password { get; }
    public string Membership { get; }

    public RegisteredAccount(string password, string membership)
    {
        Password = password;
        Membership = membership;
    }
}

private readonly Dictionary<string, RegisteredAccount> registeredAccounts =
    new Dictionary<string, RegisteredAccount>(System.StringComparer.OrdinalIgnoreCase);



private bool hasShownTerminalDialogue = false;

private bool TrainingFoxyProxy = false;
private bool hasTriggeredRegisterMatchDialogue = false;
private bool hasTriggeredPremiumRequestDialogue = false;

private bool justSentBurpSuiteRequest = false;


void Awake()
    {
        if (stableChestID == 0)
        {
            stableChestID = GenerateStableChestID();
        }

        // Default account so existing tutorial/login behavior still works.
        registeredAccounts["premium"] = new RegisteredAccount("guard", PremiumMembership);
        registeredAccounts["PREMIUM"] = new RegisteredAccount("GUARD", PremiumMembership);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JWTioTokenInput.onValueChanged.AddListener(OnJwtTokenInputChanged);
        decodeToken();

        RegisterPasswordInput.onSelect.AddListener(OnRegisterPasswordSelected);
        RegisterConfirmPasswordInput.onSelect.AddListener(OnRegisterConfirmPasswordSelected);
        RegisterPasswordInput.onValueChanged.AddListener(OnRegisterPasswordValueChanged);
        RegisterConfirmPasswordInput.onValueChanged.AddListener(OnRegisterPasswordValueChanged);
        burpSuiterequestInput.onValueChanged.AddListener(OnBurpSuiteRequestChanged);

        if (commandInput != null)
        {
            commandInput.onEndEdit.AddListener(OnTerminalEndEdit);
        }
    }

    void OnDestroy()
    {
        JWTioTokenInput.onValueChanged.RemoveListener(OnJwtTokenInputChanged);
        RegisterPasswordInput.onSelect.RemoveListener(OnRegisterPasswordSelected);
        RegisterConfirmPasswordInput.onSelect.RemoveListener(OnRegisterConfirmPasswordSelected);
        RegisterPasswordInput.onValueChanged.RemoveListener(OnRegisterPasswordValueChanged);
        RegisterConfirmPasswordInput.onValueChanged.RemoveListener(OnRegisterPasswordValueChanged);
        burpSuiterequestInput.onValueChanged.RemoveListener(OnBurpSuiteRequestChanged);

        if (commandInput != null)
        {
            commandInput.onEndEdit.RemoveListener(OnTerminalEndEdit);
        }
    }

    private void OnTerminalEndEdit(string _)
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // TMP onEndEdit also fires on focus loss, so only trigger on Enter.
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            checkCommand();
        }
    }

    private void OnRegisterPasswordSelected(string _)
    {
        if (MissionTime)
        {
            return;
        }

        guiderBot.StartDialogueRange(118, 118);
    }

    private void OnRegisterConfirmPasswordSelected(string _)
    {
        if (MissionTime)
        {
            return;
        }

        guiderBot.StartDialogueRange(119, 119);
    }

    private void OnRegisterPasswordValueChanged(string _)
    {
        if (MissionTime)
        {
            hasTriggeredRegisterMatchDialogue = false;
            return;
        }

        string password = RegisterPasswordInput.text;
        string confirmPassword = RegisterConfirmPasswordInput.text;
        bool passwordsMatch = !string.IsNullOrEmpty(password) && password == confirmPassword;

        if (passwordsMatch)
        {
            if (!hasTriggeredRegisterMatchDialogue)
            {
                guiderBot.StartDialogueRange(120, 120);
                hasTriggeredRegisterMatchDialogue = true;
            }
        }
        else
        {
            hasTriggeredRegisterMatchDialogue = false;
        }
    }

    private void OnBurpSuiteRequestChanged(string _)
    {
        if (MissionTime)
        {
            hasTriggeredPremiumRequestDialogue = false;
            return;
        }

        string expectedPayload = $@"{{
    ""sub"": ""1234567890"",
    ""name"": ""{registeredName}"",
    ""membership"": ""premium"",
    ""iat"": 1516239022
}}";

        bool isMatch = NormalizeForComparison(burpSuiterequestInput.text) == NormalizeForComparison(expectedPayload);

        if (isMatch)
        {
            if (!hasTriggeredPremiumRequestDialogue)
            {
                guiderBot.StartDialogueRange(127, 127, () =>
                {
                    ShowHint(-289.6f, 84.2f);
                });
                hasTriggeredPremiumRequestDialogue = true;
            }
        }
        else
        {
            hasTriggeredPremiumRequestDialogue = false;
        }
    }

    private string NormalizeForComparison(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return Regex.Replace(input, "\\s+", string.Empty);
    }

    // Update is called once per frame
void Update()
    {
        if (storageTokenInput.isFocused && IsCtrlCPressed())
        {
            if (MissionTime == false){
                LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);

                guiderBot.StartDialogueRange(102, 102, () =>
            {
                ShowHint(-112.3f, 174f);
            });

            }
            
        }

        if (Keyboard.current != null && Keyboard.current[Key.I].wasPressedThisFrame && LoggedinPanel.activeSelf && !terminalCanvas.activeSelf && !burpSuitePanel.activeSelf)
        {
          
            
                f12InspectView.SetActive(!f12InspectView.activeSelf);
                LoggedinPanel.SetActive(LoggedinPanel.activeSelf);
                storageView.SetActive(storageView.activeSelf);


                if (f12InspectView.activeSelf == true && !MissionTime && !brainTime){

                    guiderBot.StartDialogueRange(99, 99, () =>
                    {
                        ShowHint(-2.2f, -71.2f);
                    });

                }else{
                    LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
 
                }


            
        }
    }

private bool IsCtrlCPressed()
{
    if (Keyboard.current == null)
    {
        return false;
    }

    bool ctrlPressed = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
    return ctrlPressed && Keyboard.current.cKey.wasPressedThisFrame;
}



//Premium login function

public void PremiumLogin()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (!registeredAccounts.TryGetValue(username, out RegisteredAccount savedAccount))
        {
            LoginHintText.text = "Username does not exist.";
            SoundManager.Play("failure");
            usernameInput.SetTextWithoutNotify(string.Empty);
            passwordInput.SetTextWithoutNotify(string.Empty);
            return;
        }

        if (!string.Equals(savedAccount.Password, password, System.StringComparison.OrdinalIgnoreCase))
        {
            LoginHintText.text = "Incorrect password. Please try again.";
            SoundManager.Play("failure");
            passwordInput.SetTextWithoutNotify(string.Empty);
            return;
        }

        if (!string.Equals(savedAccount.Membership, PremiumMembership, System.StringComparison.OrdinalIgnoreCase))
        {
            LoginHintText.text = "not a premium member.";
            SoundManager.Play("failure");
            passwordInput.SetTextWithoutNotify(string.Empty);
            return;
        }
        f12InspectView.SetActive(false);
        storageView.SetActive(false);

        LoggedinPanel.SetActive(true);
        usernameInput.SetTextWithoutNotify(string.Empty);
        passwordInput.SetTextWithoutNotify(string.Empty);
        SoundManager.Play("Success");

        if (username == registeredName){
             MissionTime = true;
                        guiderBot.StartDialogueRange(130, 131, () =>{

                 LeanTween.moveLocal ( timer,  new Vector3 (322.28f,48.91f,0f) ,2f )
                    .setEase(LeanTweenType.easeInOutBack);
                     LeanTween.scale(timer, new Vector3(1, 1, 0), 0.3f)
                    .setEase(LeanTweenType.easeInOutBack);

                    Timer.Instance.ResetTimer();
                    Timer.Instance.StartTimer();

                    BackgroundMusicManager.SwitchToMissionMusic();

                    closeGuiderBot();
                    commandInput.SetTextWithoutNotify(string.Empty);
                    StartTypewriter("");
                    isfoxyproxyEnabled = false;
                    burpSuiteButton.SetActive(false);
                    foxyproxyIconTopBar.SetActive(false);
                    foxyproxyEnabled.SetActive(false);
                    Image burpSuiteImage = InterceptPanel.GetComponent<Image>();
                burpSuiteImage.sprite = burpsuiteNotInterceptedSprite;
                ActionButtonObject.SetActive(false);
                InterceptPanel.SetActive(false);
                JWTioDecodedOutput.text = "<color=#FF6B6B>Wrong token. Try copying again.</color>";
                JWTioTokenInput.SetTextWithoutNotify(string.Empty);
                burpSuiteresponseInput.text = string.Empty;
                burpSuiterequestInput.text = string.Empty;
                f12InspectView.SetActive(false);
                storageView.SetActive(false);
                RepeatPanel.SetActive(false);



                   });



                        return;


        }
        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(97, 98);
             }

    }


public void RegisterTrigger()
    {


        string username = RegisterUsernameInput.text.Trim();
        string password = RegisterPasswordInput.text.Trim();
        string confirmPassword = RegisterConfirmPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            SoundManager.Play("failure");
            RegisterHintText.text = "Please fill in all fields.";
            return;
        }

        if (password != confirmPassword)
        {
            SoundManager.Play("failure");
            RegisterHintText.text = "Passwords do not match. Please try again.";
            RegisterPasswordInput.SetTextWithoutNotify(string.Empty);
            RegisterConfirmPasswordInput.SetTextWithoutNotify(string.Empty);
            return;
        }

        if (registeredAccounts.ContainsKey(username))
        {
            RegisterHintText.text = "Username already exists. Please choose a different one.";
            SoundManager.Play("failure");
            return;
        }


        registeredAccounts.Add(username, new RegisteredAccount(password, DefaultMembership));
        SoundManager.Play("Success");

        RegisterUsernameInput.SetTextWithoutNotify(string.Empty);
        RegisterPasswordInput.SetTextWithoutNotify(string.Empty);
        RegisterConfirmPasswordInput.SetTextWithoutNotify(string.Empty);

        if (isfoxyproxyEnabled)
        {
            Image burpSuiteImage = InterceptPanel.GetComponent<Image>();
                burpSuiteImage.sprite = burpSuiteInterceptedSprite;
                ActionButtonObject.SetActive(true);
                registeredName = username;
                doneRegistered = true;

                if (MissionTime == false)
                {
                    guiderBot.StartDialogueRange(121, 122, () =>
                    {
                        ShowHint(10.1f, 174.6f);
                    });
                } else if (MissionTime == true)
                    {
        QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "5.5");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("5");
            SoundManager.Play("Success");
        
        PassedFifthMision = true;
        AllMissionCompleted();

        }
        }
    }
public void gotoRegisterfromLogin()
    {
        if (MissionTime == false)
        {
         return;
        }
        OpenRegisterPanel();
    }

public void OpenRegisterPanel()
    {
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

        foxyproxyDisabled.SetActive(false);
        RegisterPanel.SetActive(true);
        LoggedinPanel.SetActive(false);

        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(117, 117);
        }

    }

public void OpenBankPage(){
    LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

        JWTioPanel.SetActive(false);
        burpSuitePanel.SetActive(false);
        closeTerminal();

        if (justSentBurpSuiteRequest)
            {
                GoToLogin();
                return;
            }

        if (TrainingFoxyProxy == true)
        {
            ShowHint(259.6f, 151.7f);
            guiderBot.StartDialogueRange(113, 114);
        }

}

public void GoToLogin()
    {
 

        RegisterPanel.SetActive(false);

    }

public void goToNormalInspector(){
    f12InspectView.SetActive(true);

storageView.SetActive(false);
}

//Inspect Storage function

public void ToggleStorageView()
    {
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

        f12InspectView.SetActive(false);
        storageView.SetActive(true);

        if (MissionTime == false)
        {guiderBot.StartDialogueRange(100, 101, () =>
        {
            ShowLongHint(-110.6f, -130.4f);
        });
        }
    }
//JWT Exploitation Methods

public void decodeToken(){

 

    string correctTokenForDecoding = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlBsYXllciBPbmUiLCJtZW1iZXJzaGlwIjoiZnJlZSIsImlhdCI6MTUxNjIzOTAyMn0.AkrZ1_1SqqOC05XQGGXib1IysTvvrFQAWlYYzoLaPlY";
    if (JWTioTokenInput.text.Trim() == correctTokenForDecoding)
    {
        if (MissionTime == false)
        {
            TweenGuiderAnchoredX(362f, 0.3f);
            ShowLongHint2(83.7f, 15.3f, 1.65f, 0.48f);
            guiderBot.StartDialogueRange(104, 107, () =>
            {
                ShowHint(-191.3f, 201.3f);
            });
        }else if (MissionTime == true)
                    {
            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "5.1");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("1");
            SoundManager.Play("Success");
        
        PassedFirstMision = true;
        AllMissionCompleted();
                    }
        JWTioDecodedOutput.text = BuildColoredDecodedJwtJson();
    } else{
    
        JWTioDecodedOutput.text = "<color=#FF6B6B>Wrong token. Try copying again.</color>";
        
    }

    
}

private void OnJwtTokenInputChanged(string _)
{
    decodeToken();
}

private string BuildColoredDecodedJwtJson()
{
    const string keyColor = "#8BE9FD";
    const string stringValueColor = "#F1FA8C";
    const string numberValueColor = "#BD93F9";
    const string punctuationColor = "#F8F8F2";

    return
        "<color=" + punctuationColor + ">{</color>\n" +
        "  <color=" + keyColor + ">\"sub\"</color><color=" + punctuationColor + ">: </color><color=" + stringValueColor + ">\"1234567890\"</color><color=" + punctuationColor + ">,</color>\n" +
        "  <color=" + keyColor + ">\"name\"</color><color=" + punctuationColor + ">: </color><color=" + stringValueColor + ">\"User\"</color><color=" + punctuationColor + ">,</color>\n" +
        "  <color=" + keyColor + ">\"membership\"</color><color=" + punctuationColor + ">: </color><color=" + stringValueColor + ">\"premium\"</color><color=" + punctuationColor + ">,</color>\n" +
        "  <color=" + keyColor + ">\"iat\"</color><color=" + punctuationColor + ">: </color><color=" + numberValueColor + ">1516239022</color>\n" +
        "<color=" + punctuationColor + ">}</color>";
}

public void ToggleFoxyProxy()
    {

        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

        foxyproxyDisabled.SetActive(!foxyproxyDisabled.activeSelf);

        if (MissionTime == false && foxyproxyDisabled.activeSelf == true)
        {
            guiderBot.StartDialogueRange(115, 115, () =>
            {
                ShowLongHint2(174.7f, 82f, 2.13f, 0.72f);
                });
        }
        
    }

public void EnableFoxyProxy()
    {
            LeanTween.scale(LongHint2, new Vector3(0, 0, 0), 0.3f)
                .setEase(LeanTweenType.easeInOutBack);
        foxyproxyEnabled.SetActive(true);

        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(116, 116, () =>
            {
                ShowHint(-250.3f, 93.5f);
            });
        } else if (MissionTime == true)
                    {
                              QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "5.4");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("4");
            SoundManager.Play("Success");
        
        PassedFourthMision = true;
        AllMissionCompleted();
                    }

        isfoxyproxyEnabled = true;
    }

public void DisableFoxyProxy()
    {

        foxyproxyEnabled.SetActive(false);
        foxyproxyDisabled.SetActive(true);

        isfoxyproxyEnabled = false;
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
//BurpSuite Methods

public void ActionButton()
    {
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

        ActionListPanel.SetActive(!ActionListPanel.activeSelf);
        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(124, 124, () =>
            {
                ShowLongHint2(-129f, 45.9f, 1.14f, 0.31f);
            });
        }
    }

public void SendToRepeater(){
    ActionListPanel.SetActive(!ActionListPanel.activeSelf);

    LeanTween.scale(LongHint2, new Vector3(0, 0, 0), 0.3f)
                .setEase(LeanTweenType.easeInOutBack);

        burpSuiterequestInput.text = $@"{{
    ""sub"": ""1234567890"",
    ""name"": ""{registeredName}"",
    ""membership"": ""free"",
    ""iat"": 1516239022
}}";

if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(125, 125, () =>
            {
                ShowHint(-170.1f, 113.1f);
            });
        } else if (MissionTime == true)
        {
               QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "5.6");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("6");
            SoundManager.Play("Success");

        
        PassedSixthMision = true;
        AllMissionCompleted();
        }
        }

public void SendBurpSuiteRequest(){

    LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
        
    LeanTween.scale(LongHint2, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

    string expectedRequest = $@"{{
    ""sub"": ""1234567890"",
    ""name"": ""{registeredName}"",
    ""membership"": ""premium"",
    ""iat"": 1516239022
}}";

    if (NormalizeForComparison(burpSuiterequestInput.text) != NormalizeForComparison(expectedRequest))
    {
        burpSuiteresponseInput.text = "Request invalid, make sure \"membership\": \"premium\"";
        return;
    }

        if (!string.IsNullOrWhiteSpace(registeredName) && registeredAccounts.TryGetValue(registeredName, out RegisteredAccount account))
        {
                registeredAccounts[registeredName] = new RegisteredAccount(account.Password, PremiumMembership);
        }
    SoundManager.Play("Success");
        burpSuiteresponseInput.text = $@"{{
    <color=green>""status"": ""success"",</color>
    ""data"": {{
        ""sub"": ""1234567890"",
        ""name"": ""{registeredName}"",
        ""membership"": ""premium"",
        ""iat"": 1516239022
    }}
}}";

justSentBurpSuiteRequest = true;


if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(128, 129, () =>{
                ShowHint(-245.3f, 175.3f);
            });
        }else if (MissionTime == true)
        {
               QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "5.7");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("7");
            SoundManager.Play("Success");
        
        PassedSeventhMision = true;
        AllMissionCompleted();

            
        }
        }

public void ProxyButton()
    {
         LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

        RepeatPanel.SetActive(false);
        proxyPanel.SetActive(true);
        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(111, 111, ()=>
            {
                ShowHint(-190.6f, 77.6f);
            });
        }
    }

public void toggleIntercept()
    {            
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);


        InterceptPanel.SetActive(!InterceptPanel.activeSelf);

        if (InterceptPanel.activeSelf)
        {
        foxyproxyIconTopBar.SetActive(true);

        if (MissionTime == true)
            {
                  QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "5.3");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("3");
            SoundManager.Play("Success");
        
        PassedThirdMision = true;
        AllMissionCompleted();
            }
        }
        if (InterceptPanel.activeSelf == true && MissionTime == false)
        {
            guiderBot.StartDialogueRange(112, 112, () =>
            {
                ShowHint(-242f, 173.3f);
                TrainingFoxyProxy = true;
            });
        }
    }

public void RepeatPageButton(){
    RepeatPanel.SetActive(true);

            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

    if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(126, 126, () =>
            {
                ShowLongHint2(-224.8f, -35.3f, 1.62f, 0.51f);
            });
        }
     
}

public void GoToDashboard(){

   proxyPanel.SetActive(false);

InterceptPanel.SetActive(false) ;

RepeatPanel.SetActive(false);

}


    //TopBar Methods

public void openBurpSuitePanel(){

        
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);

        burpSuitePanel.SetActive(true);
        closeTerminal();
        JWTioPanel.SetActive(false);
        
    if (doneRegistered && MissionTime == false){

        guiderBot.StartDialogueRange(123, 123, () =>{
            ShowHint(-147.1f, 79.1f);
        });
        return;
    }
    if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(110, 110, () =>{
                ShowHint(-230.5f, 110.8f);
            });          
                 }



    }

    public void openJWTio(){
            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
        
        if (MissionTime == false)
        {
            guiderBot.StartDialogueRange(103, 103);
        }

        JWTioPanel.SetActive(true);
        closeTerminal();
        burpSuitePanel.SetActive(false);
    }
public void closeTerminal()
    {

        terminalCanvas.SetActive(false);
         
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
        guiderBot.StartDialogueRange(108, 108);

  
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


    public void checkCommand()
    {
        
        string command = commandInput != null ? commandInput.text.Trim() : string.Empty;

        if (command.Equals("burpsuite &", System.StringComparison.OrdinalIgnoreCase))
        {
            burpSuiteButton.SetActive(true);
            commandInput.SetTextWithoutNotify(string.Empty);

            SoundManager.Play("Success");
            StartTypewriter("Burp Suite started successfully!");

            if (MissionTime == false)
            {
                guiderBot.StartDialogueRange(109, 109, () =>{
                    ShowHint(20.4f, 175.2f);
                });          
                 } else if (MissionTime == true)
                    {
            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "5.2");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("2");
            SoundManager.Play("Success");
        PassedSecondMision = true;
        AllMissionCompleted();
                    } 
                    
                if (brainTime == true)
                {
                    OpenChest();
            bombButton.onClick.Invoke();
            NotificationManager.Instance.Display("Successfully deducted 10 seconds!");
            Timer.Instance.DeductTime(10f);
           savecontroller.Instance.DeductSavedLevelTime(4, 10f);               
                    lockImage.SetActive(false);
                    brainTime = false;
                    PositionBrainTextContainer( -50.772f);
                    brainTextContainer.alpha = 0f;
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
        foreach (Item i in FindObjectsByType<Item>(FindObjectsSortMode.None))
            if (i.Id == 7) i.isDuringTraining = true;
        TweenGuiderAnchoredX(291.62f, 0.3f);

LeanTween.scale(guider, new Vector3(6, 6, 1), 0.6f)
    .setDelay(0.5f)
    .setEase(LeanTweenType.easeOutQuart)
    .setOnComplete(() =>
    {
        LeanTween.scale(chatbubble, new Vector3(1, 1, 1), 0.5f)
            .setEase(LeanTweenType.easeOutQuart)
            .setOnComplete(() =>
            {

                guiderBot.StartDialogueRange(93, 94, () =>
                {
                    ShowHint(189.3f, -199.6f, 1.5f, 1.5f);
                });
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
public void ShowHint(float positionX , float positionY, float scaleX = 1f, float scaleY = 1f){

    RectTransform hintRect = playerHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     SoundManager.Play("playerHint");
      LeanTween.scale(playerHint, new Vector3(scaleX, scaleY, 0), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

public void ShowLongHint(float positionX , float positionY){

    RectTransform hintRect = LongHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     SoundManager.Play("playerHint");
    LeanTween.scale(LongHint, new Vector3(1.81f, 0.25f, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

public void ShowLongHint2(float positionX , float positionY, float Xscale = 0.98f, float Yscale = 0.32f){

     SoundManager.Play("playerHint");

    RectTransform hintRect = LongHint2.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

    LeanTween.scale(LongHint2, new Vector3(Xscale, Yscale, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

public void ShowLongHint3(float positionX , float positionY){

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
        if (PassedFirstMision == true && PassedSecondMision == true && PassedThirdMision == true && PassedFourthMision == true && PassedFifthMision == true && PassedSixthMision == true && PassedSeventhMision == true)
        {
            IsOpened = true;
            brainTime = true;
         
                bombButton bombButtonHandler = bombButton.GetComponent<bombButton>();
                if (bombButtonHandler != null)
                {
                    bombButtonHandler.BrainTime = true;
                }
            
            MissionTime = false;
            terminalScript.brainTime = true;
            BackgroundMusicManager.SwitchToBrainTimeMusic();

StartTypewriter("Hit Enter to excecute command") ;
            if (commandInput != null)
            {
                commandInput.SetTextWithoutNotify(string.Empty);
            }

             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(4, "JWT Exploitation", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);

        StartCoroutine(FadeInWaitAndFadeOut(brainCanvas, 3f));
        ApplyBrainTimeUiTransition();




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

 


    
    brainText.SetActive(true);
    brainTextTMP = brainText.GetComponent<TMP_Text>();

        brainTextTMP.text = "Activate burpsuite in Terminal to deduct 10s off the clock";
    


        PositionBrainTextContainer(-111.56f);
        brainTextContainer.alpha = 0f;

        StartCoroutine(FadeIn(brainTextContainer));
    

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
