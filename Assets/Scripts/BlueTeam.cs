using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Networking;
using FullSerializer;

public class BlueTeam : MonoBehaviour, IInteractable
{
    private const string LeaderboardTimeField = "finalTotalTime";

    private static readonly Regex StringLiteralRegex = new Regex("\"(?:\\\\.|[^\"\\\\])*\"", RegexOptions.Compiled);
    private static readonly Regex NumberRegex = new Regex("\\b\\d+\\b", RegexOptions.Compiled);
    private static readonly Regex ColorTagRegex = new Regex("</?color(?:=[^>]*)?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex NoparseTagRegex = new Regex("</?noparse>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex SqlTypeRegex = new Regex("\\b(PreparedStatement|ResultSet|Connection|Statement|SQLException|Integer|Boolean)\\b", RegexOptions.Compiled);
    private static readonly Regex SqlMethodRegex = new Regex("\\b(prepareStatement|createStatement|setString|setInt|setBoolean|executeQuery|executeUpdate|next|getString|getInt|close)\\b", RegexOptions.Compiled);
    private static readonly Regex SqlKeywordRegex = new Regex("\\b(String|SELECT|FROM|WHERE|AND|OR|INSERT|INTO|UPDATE|DELETE|VALUES|SET)\\b", RegexOptions.Compiled);

    private static readonly Regex B4FrameworkRegex = new Regex("\\b(app|get|post|use|authenticate|requireRole|render|json|send|status|next|admin_panel)\\b", RegexOptions.Compiled);
    private static readonly Regex B4RequestRegex = new Regex("\\b(req|res)\\b", RegexOptions.Compiled);
    private static readonly Regex B4JsKeywordRegex = new Regex("\\b(const|let|var|return|if|else|function|async|await|true|false|null)\\b", RegexOptions.Compiled);

    private static readonly Regex B3DomObjectRegex = new Regex("\\b(display|request|body|comment)\\b", RegexOptions.Compiled);
    private static readonly Regex B3DomApiRegex = new Regex("\\b(innerHTML|querySelector|textContent)\\b", RegexOptions.Compiled);
    private static readonly Regex B3JsKeywordRegex = new Regex("\\b(const|let|var|return|if|else|function|true|false|null)\\b", RegexOptions.Compiled);

    private static readonly Regex B2HtmlTagRegex = new Regex("\\b(form|input|button)\\b", RegexOptions.Compiled);
    private static readonly Regex B2HtmlAttrRegex = new Regex("\\b(action|method|type|name|value)\\b", RegexOptions.Compiled);
    private static readonly Regex B2CsrfTokenRegex = new Regex("\\b(POST|csrf_token|session|amount|to_account|Transfer|hidden|text|submit)\\b", RegexOptions.Compiled);

    private static readonly Regex B1AuthObjectRegex = new Regex("\\b(user|db|users|data|userId|role)\\b", RegexOptions.Compiled);
    private static readonly Regex B1AuthApiRegex = new Regex("\\b(findById|renderAdminPanel)\\b", RegexOptions.Compiled);
    private static readonly Regex B1JsKeywordRegex = new Regex("\\b(const|if|await|async|true|false|null)\\b", RegexOptions.Compiled);

    [Header("Mission")]
    public string questID;
    public TMP_Text checkHint;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private TMP_Text codeInputHighlightText;
    [SerializeField] private TMP_Text expectedCodeText;
    [SerializeField, TextArea(4, 12)] private string correctCode;

    [Header("GameObjects")]
    public GameObject IntroScrollView;
    public GameObject CodingArea;

    [Header("Timer")]
    public GameObject timer;
    [SerializeField] private Vector2 timerAnchoredPosition;

    [Header("Reveal Mask")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform revealMask;
    [SerializeField] private RectTransform revealMaskVisual;
    [SerializeField] private GameObject correctCodeLayer;
    [SerializeField] private Vector3 closedMaskScale = Vector3.zero;
    [SerializeField] private Vector3 openMaskScale = Vector3.one;
    [SerializeField] private float maskTweenDuration = 0.2f;
    [SerializeField] private float maskRotationSpeed = 180f;
    [SerializeField] private float maskPulseAmount = 0.08f;
    [SerializeField] private float maskPulseSpeed = 8f;

    public PlayerInput playerInput;
    public GameObject CodingCanvas;

    public GameObject areYouSure;

    [Header("EndGame Variables")]
    public GameObject EndgameCanvas;
    public ScrollRect leaderboardScrollRect;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private Transform leaderboardContent;


    [Header("Guider Bot")]
               [SerializeField] GameObject guider;
        public GuiderBot guiderBot;
        private RectTransform guiderRect;
        private bool hasGuiderInitialAnchoredPosition;
        private Vector2 guiderInitialAnchoredPosition;

        public GameObject playerHint;
        public GameObject chatbubble;


    public bool IsOpened {get; private set;}



    private bool challengeStarted;
    private bool isRevealHeld;
    private bool isCompleted;
    private Camera canvasCamera;
    private Vector3 maskVisualBaseScale = Vector3.one;
    private RectTransform correctCodeRect;
    private Vector2 correctCodeBaseAnchoredPos;
    private string _plainText = string.Empty;
    private string _expectedPlainText = string.Empty;
    private string _lastAllowedPlainText = string.Empty;
    private bool _isHighlighting;
    private bool _blockPasteThisFrame;
    private bool _endgameSequenceStarted;

    private bool firstTime = true;

     public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        CodingCanvas.SetActive(!CodingCanvas.activeSelf);
        IntroScrollView.SetActive(true);
        CodingArea.SetActive(false);
        challengeStarted = false;

        if (guiderBot != null)
        {
            if (CodingCanvas.activeSelf && firstTime)
            {
                FirstTimeGuider();
            }
        }

 


  
        
        if (CodingCanvas.activeSelf == true)
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

        public bool CanInteract()
    {
        return !IsOpened;
    }

    private void Awake()
    {
        if (revealMaskVisual == null)
        {
            revealMaskVisual = revealMask;
        }

        if (uiCanvas == null && revealMask != null)
        {
            uiCanvas = revealMask.GetComponentInParent<Canvas>();
        }

        if (uiCanvas != null && uiCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = uiCanvas.worldCamera;
        }

        if (revealMask != null)
        {
            Mask mask = revealMask.GetComponent<Mask>();
            if (mask != null)
            {
                mask.showMaskGraphic = false;
            }

            revealMask.localScale = closedMaskScale;
            revealMask.gameObject.SetActive(false);
        }

        if (revealMaskVisual != null)
        {
            maskVisualBaseScale = revealMaskVisual.localScale;
        }

        if (correctCodeLayer != null)
        {
            correctCodeLayer.SetActive(true);
            correctCodeRect = correctCodeLayer.GetComponent<RectTransform>();

            if (correctCodeRect != null)
            {
                correctCodeBaseAnchoredPos = correctCodeRect.anchoredPosition;
            }
        }

        if (codeInput != null)
        {
            // Keep the input plain text for stable caret behavior.
            codeInput.textComponent.richText = false;
            codeInput.onValueChanged.AddListener(OnCodeInputChanged);
            SyncInputHighlight();
        }

        if (codeInputHighlightText != null)
        {
            codeInputHighlightText.richText = true;
            codeInputHighlightText.text = HighlightCode(_plainText);
        }

        SyncExpectedCodeHighlight();
    }

    private void OnEnable()
    {
        SyncInputHighlight();
        SyncExpectedCodeHighlight();
    }

    private void Update()
    {
        HandleClipboardShortcuts();

        if (!challengeStarted || isCompleted || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            if (guiderBot != null && firstTime)
            {
                guiderBot.StartDialogueRange(138, 139, () =>
                {
                    closeGuiderBot();
                });
                firstTime = false;

            }


            BeginReveal();
        }

        if (Keyboard.current.leftCtrlKey.wasReleasedThisFrame)
        {
            EndReveal();
        }

        if (!isRevealHeld)
        {
            return;
        }

        UpdateMaskPosition();
        AnimateMaskVisual();
    }
//close button

public void CloseChallenge()
    {

        areYouSure.SetActive(false);
        CodingCanvas.SetActive(false);
        IntroScrollView.SetActive(true);
        CodingArea.SetActive(false);
        challengeStarted = false;
        LeanTween.scale(timer, new Vector3(0f, 0f, 0f), 0.3f)
         .setEase(LeanTweenType.easeInOutBack);
         Timer.Instance.StopTimer();
         Timer.Instance.ResetTimer();
         BackgroundMusicManager.SwitchToDefaultMusic();

if (questID == "B5"){
         codeInput.text = @"String sql = ""SELECT * FROM users WHERE user='"" + user + ""' AND pass='"" + pass + ""'"";
Statement st = conn.createStatement();
ResultSet rs = st.executeQuery(sql);";
} else if (questID == "B4"){
codeInput.text = @"app.get(""/admin/dashboard"", authenticate, (req, res) => {
  res.render(""admin_panel""); 
});";


}else if (questID == "B3"){
codeInput.text = @"let comment = request.body.comment; 
display.innerHTML = ""<div class='message'>"" + comment + ""</div>"";";


}else if (questID == "B2"){
codeInput.text = @"<form action=""/transfer"" method=""POST"">
    <input type=""text"" name=""amount"">
    <input type=""text"" name=""to_account"">
    <button type=""submit"">Transfer</button>
</form>";

}else if (questID == "B1"){

codeInput.text = @"const token = req.headers['auth'];
const data = jwt.decode(token);

if (data.membership === ""premium"") {
  grantAccess();
}";


}
    

        PauseController.SetPause(false);
        playerInput.SwitchCurrentActionMap("Player");
    }


public void Xbutton(){
    areYouSure.SetActive(true);
    closeGuiderBot();
}

public void CancelExit(){
    areYouSure.SetActive(false);
    closeGuiderBot();
}
    private void LateUpdate()
    {
        _blockPasteThisFrame = false;
    }

    public void StartChallenge()
    {
            BackgroundMusicManager.SwitchToBrainTimeMusic();

        if (guiderBot != null && firstTime)
        {
            guiderBot.StartDialogueRange(137, 137);
        }
        IntroScrollView.SetActive(false);
        CodingArea.SetActive(true);
        challengeStarted = true;
        isCompleted = false;

        if (timer != null)
        {
            RectTransform timerRect = timer.GetComponent<RectTransform>();
            if (timerRect != null)
            {
                timerRect.anchoredPosition = timerAnchoredPosition;
            }

            
            LeanTween.cancel(timer);
            LeanTween.scale(timer, new Vector3(1f, 1f, 0f), 0.3f)
                .setEase(LeanTweenType.easeInOutBack);
        }

        if (Timer.Instance != null)
        {
                    Timer.Instance.ResetTimer();

            Timer.Instance.StartTimer();
        }

        BackgroundMusicManager.SwitchToMissionMusic();

        if (checkHint != null)
        {
            checkHint.text = string.Empty;
        }

        if (codeInput != null)
        {
            SyncInputHighlight();
            codeInput.ActivateInputField();
        }

        SyncExpectedCodeHighlight();
    }

    public void CheckCode()
    {
        if (isCompleted)
        {
            return;
        }

        SyncInputHighlight();

        string typedCode = NormalizeCode(_plainText);
        string expectedCode = NormalizeCode(GetExpectedCode());

        if (typedCode != expectedCode)
        {
            if (checkHint != null)
            {
                checkHint.text = "Patch failed, please make sure its same";
            }

            SoundManager.Play("failure");
            return;
        }

        isCompleted = true;

        QuestProgress quest = MissionController.Instance != null
            ? MissionController.Instance.activeMissions.Find(q => q.questID == questID)
            : null;

        if (quest != null)
        {
            quest.isCompleted = true;
        }

        if (MissionUI.Instance != null)
        {
            MissionUI.Instance.CompleteMission(GetMissionNumber());
        }

        if (checkHint != null)
        {
            checkHint.text = string.Empty;
        }

        if (questID == "B5"){
            
             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(9, "SQL Injection Prevention", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);

        } else if (questID == "B4"){
         
             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(8, "Broken Access Control Prevention", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);    

        }else if (questID == "B3"){
         
             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(7, "XSS Prevention", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);    

        }else if (questID == "B2"){
         
             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(6, "CSRF Prevention", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);    

        }else if (questID == "B1"){
         
             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(5, "JWT Exploitation Prevention", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);    

        }
        savecontroller.Instance?.SaveGame();
        SoundManager.Play("Success");
        BackgroundMusicManager.SwitchToDefaultMusic();
        Interact();
        CheckAllBMissionsComplete();
    }

    private void BeginReveal()
    {

        SoundManager.Play("scan");
        if (revealMask == null)
        {
            return;
        }

        isRevealHeld = true;
        Cursor.visible = false;

        revealMask.gameObject.SetActive(true);
        LeanTween.cancel(revealMask.gameObject);
        revealMask.localScale = closedMaskScale;
        LeanTween.scale(revealMask, openMaskScale, maskTweenDuration)
            .setEase(LeanTweenType.easeInOutBack)
            .setIgnoreTimeScale(true);

        UpdateMaskPosition();

        if (codeInput != null)
        {
            codeInput.DeactivateInputField();
        }
    }

    public void FirstTimeGuider()
{
       guider.SetActive(true);
TweenGuiderAnchored(291.62f, -180.2f, 0.3f);

LeanTween.scale(guider, new Vector3(6, 6, 1), 0.6f)
    .setDelay(0.5f)
    .setEase(LeanTweenType.easeOutQuart)
    .setOnComplete(() =>
    {
        LeanTween.scale(chatbubble, new Vector3(1, 1, 1), 0.5f)
            .setEase(LeanTweenType.easeOutQuart)
            .setOnComplete(() =>
            {

                guiderBot.StartDialogueRange(134, 136);
            });
    });



}

public void ShowHint(float positionX , float positionY){

    RectTransform hintRect = playerHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     SoundManager.Play("playerHint");
      LeanTween.scale(playerHint, new Vector3(1, 1, 0), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
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

private void TweenGuiderAnchored(float targetX, float targetY, float duration, float delay = 0f)
{
    if (guiderRect == null && guider != null)
    {
        guiderRect = guider.GetComponent<RectTransform>();
    }

    if (guiderRect == null)
    {
        LTDescr fallbackTween = LeanTween.move(guider, new Vector3(targetX, targetY, 0f), duration)
            .setEase(LeanTweenType.easeInOutBack);

        if (delay > 0f)
        {
            fallbackTween.setDelay(delay);
        }

        return;
    }

    Vector2 startPos = guiderRect.anchoredPosition;
    LTDescr tween = LeanTween.value(guider, 0f, 1f, duration)
        .setEase(LeanTweenType.easeInOutBack)
        .setOnUpdate((float t) =>
        {
            Vector2 pos = new Vector2(
                Mathf.Lerp(startPos.x, targetX, t),
                Mathf.Lerp(startPos.y, targetY, t)
            );
            guiderRect.anchoredPosition = pos;
        });

    if (delay > 0f)
    {
        tween.setDelay(delay);
    }
}

public void closeGuiderBot()
    {
               

TweenGuiderAnchored(291.62f, -134.2f, 0.3f);
        LeanTween.scale(guider, new Vector3(0, 0, 1), 0.6f)
        .setDelay(2f)
    .setEase(LeanTweenType.easeOutQuart).setOnComplete(() =>
    {
        guider.SetActive(false);
    });
    }


    private void EndReveal()
    {
        if (!isRevealHeld)
        {
            return;
        }

        isRevealHeld = false;
        Cursor.visible = true;

        if (revealMask != null)
        {
            LeanTween.cancel(revealMask.gameObject);
            LeanTween.scale(revealMask, closedMaskScale, maskTweenDuration)
                .setEase(LeanTweenType.easeInOutBack)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    if (revealMask != null)
                    {
                        revealMask.gameObject.SetActive(false);
                    }
                });
        }

        if (revealMaskVisual != null)
        {
            revealMaskVisual.localRotation = Quaternion.identity;
            revealMaskVisual.localScale = maskVisualBaseScale;
        }

        if (codeInput != null)
        {
            codeInput.ActivateInputField();
        }
    }

    private void UpdateMaskPosition()
    {
        if (revealMask == null || Mouse.current == null)
        {
            return;
        }

        RectTransform parentRect = revealMask.parent as RectTransform;
        if (parentRect == null)
        {
            return;
        }

        Vector2 pointerPosition = Mouse.current.position.ReadValue();
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pointerPosition, canvasCamera, out localPoint))
        {
            revealMask.anchoredPosition = localPoint;

            if (correctCodeRect != null)
            {
                // Keep the code content visually fixed while the mask window moves.
                correctCodeRect.anchoredPosition = correctCodeBaseAnchoredPos - localPoint;
            }
        }
    }

    private void AnimateMaskVisual()
    {
        if (revealMaskVisual == null)
        {
            return;
        }

        revealMaskVisual.localRotation = Quaternion.Euler(0f, 0f, Time.unscaledTime * maskRotationSpeed);

        if (revealMaskVisual != revealMask)
        {
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * maskPulseSpeed) * maskPulseAmount;
            revealMaskVisual.localScale = maskVisualBaseScale * pulse;
        }
    }

    private string GetMissionNumber()
    {
        if (string.IsNullOrWhiteSpace(questID))
        {
            return string.Empty;
        }

        string[] parts = questID.Split('.');
        return parts[parts.Length - 1];
    }

    private string GetExpectedCode()
    {
        if (!string.IsNullOrWhiteSpace(_expectedPlainText))
        {
            return _expectedPlainText;
        }

        return correctCode;
    }

    private void OnCodeInputChanged(string value)
    {
        if (_isHighlighting)
        {
            return;
        }

        if (_blockPasteThisFrame && IsChallengeInputActive())
        {
            int caret = codeInput.caretPosition;

            _isHighlighting = true;
            _plainText = _lastAllowedPlainText;
            codeInput.SetTextWithoutNotify(_plainText);
            codeInput.caretPosition = Mathf.Clamp(caret, 0, _plainText.Length);
            _isHighlighting = false;
            UpdateCodeInputOverlayHighlight();

            if (checkHint != null)
            {
                checkHint.text = "Copy and paste are disabled in this mode";
            }

            return;
        }

        _plainText = value;
        _lastAllowedPlainText = _plainText;

        UpdateCodeInputOverlayHighlight();
        _lastAllowedPlainText = _plainText;
    }

    private void HandleClipboardShortcuts()
    {
        if (!IsChallengeInputActive() || Keyboard.current == null)
        {
            return;
        }

        bool ctrlHeld = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
        if (!ctrlHeld)
        {
            return;
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            GUIUtility.systemCopyBuffer = string.Empty;

            if (checkHint != null)
            {
                checkHint.text = "Copy and paste are disabled in this mode";
            }
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            _blockPasteThisFrame = true;
            GUIUtility.systemCopyBuffer = string.Empty;
        }
    }

    private bool IsChallengeInputActive()
    {
        return challengeStarted
            && CodingCanvas != null
            && CodingCanvas.activeInHierarchy
            && codeInput != null;
    }

    private void SyncInputHighlight()
    {
        if (codeInput == null)
        {
            return;
        }

        _plainText = codeInput.text;
        UpdateCodeInputOverlayHighlight();
    }

    private void UpdateCodeInputOverlayHighlight()
    {
        if (codeInputHighlightText == null)
        {
            return;
        }

        codeInputHighlightText.text = HighlightCode(_plainText);
    }

    private string StripRichTextTags(string value)
    {
        string noTags = ColorTagRegex.Replace(value, string.Empty);
        noTags = NoparseTagRegex.Replace(noTags, string.Empty);
        return DecodeHtmlEntities(noTags);
    }

    private string DecodeHtmlEntities(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&amp;", "&")
            .Replace("&quot;", "\"")
            .Replace("&#39;", "'");
    }

    private void SyncExpectedCodeHighlight()
    {
        if (expectedCodeText == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_expectedPlainText))
        {
            string expectedTextPlain = StripRichTextTags(expectedCodeText.text);
            _expectedPlainText = !string.IsNullOrWhiteSpace(expectedTextPlain)
                ? expectedTextPlain
                : correctCode;
        }

        expectedCodeText.richText = true;
        expectedCodeText.text = HighlightCode(_expectedPlainText);
    }


    private string HighlightCode(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        // B5 should also color SQL tokens inside quoted query strings.
        if (IsB5Quest())
        {
            string b5Text = EscapeRichText(value);
            b5Text = SqlTypeRegex.Replace(b5Text, "<color=#4EC9B0>$1</color>");
            b5Text = SqlMethodRegex.Replace(b5Text, "<color=#DCDCAA>$1</color>");
            b5Text = SqlKeywordRegex.Replace(b5Text, "<color=#569CD6>$1</color>");
            b5Text = NumberRegex.Replace(b5Text, "<color=#B5CEA8>$0</color>");
            return b5Text;
        }

        List<string> stringLiterals = new List<string>();
        string workingText = StringLiteralRegex.Replace(value, match =>
        {
            stringLiterals.Add(match.Value);
            return $"__STRING_{stringLiterals.Count - 1}__";
        });

        workingText = EscapeRichText(workingText);

        if (IsB4Quest())
        {
            workingText = B4FrameworkRegex.Replace(workingText, "<color=#4EC9B0>$1</color>");
            workingText = B4RequestRegex.Replace(workingText, "<color=#DCDCAA>$1</color>");
            workingText = B4JsKeywordRegex.Replace(workingText, "<color=#569CD6>$1</color>");
        }
        else if (IsB2Quest())
        {
            workingText = B2HtmlTagRegex.Replace(workingText, "<color=#4EC9B0>$1</color>");
            workingText = B2HtmlAttrRegex.Replace(workingText, "<color=#DCDCAA>$1</color>");
            workingText = B2CsrfTokenRegex.Replace(workingText, "<color=#569CD6>$1</color>");
        }
        else if (IsB3Quest())
        {
            workingText = B3DomObjectRegex.Replace(workingText, "<color=#4EC9B0>$1</color>");
            workingText = B3DomApiRegex.Replace(workingText, "<color=#DCDCAA>$1</color>");
            workingText = B3JsKeywordRegex.Replace(workingText, "<color=#569CD6>$1</color>");
        }
        else if (IsB1Quest())
        {
            workingText = B1AuthObjectRegex.Replace(workingText, "<color=#4EC9B0>$1</color>");
            workingText = B1AuthApiRegex.Replace(workingText, "<color=#DCDCAA>$1</color>");
            workingText = B1JsKeywordRegex.Replace(workingText, "<color=#569CD6>$1</color>");
        }
        else
        {
            workingText = SqlTypeRegex.Replace(workingText, "<color=#4EC9B0>$1</color>");
            workingText = SqlMethodRegex.Replace(workingText, "<color=#DCDCAA>$1</color>");
            workingText = SqlKeywordRegex.Replace(workingText, "<color=#569CD6>$1</color>");
        }

        workingText = NumberRegex.Replace(workingText, "<color=#B5CEA8>$0</color>");

        for (int i = 0; i < stringLiterals.Count; i++)
        {
            string escapedLiteral = EscapeRichText(stringLiterals[i]);
            workingText = workingText.Replace($"__STRING_{i}__", $"<color=#CE9178>{escapedLiteral}</color>");
        }

        return workingText;
    }

    private bool IsB4Quest()
    {
        return string.Equals(questID, "B4", System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsB5Quest()
    {
        return string.Equals(questID, "B5", System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsB3Quest()
    {
        return string.Equals(questID, "B3", System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsB2Quest()
    {
        return string.Equals(questID, "B2", System.StringComparison.OrdinalIgnoreCase);
    }

    private bool IsB1Quest()
    {
        return string.Equals(questID, "B1", System.StringComparison.OrdinalIgnoreCase);
    }

    private string EscapeRichText(string value)
    {
        // Use TMP noparse to render literal '<' without showing '&lt;'.
        return value
            .Replace("<", "<noparse><</noparse>");
    }

    private string NormalizeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        string normalized = value.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
        normalized = Regex.Replace(normalized, @"\s+", string.Empty);
        return normalized;
    }

    private void OnDisable()
    {
        Cursor.visible = true;

        if (revealMask != null)
        {
            LeanTween.cancel(revealMask.gameObject);
            revealMask.localScale = closedMaskScale;
            revealMask.gameObject.SetActive(false);
        }

        if (revealMaskVisual != null)
        {
            revealMaskVisual.localRotation = Quaternion.identity;
            revealMaskVisual.localScale = maskVisualBaseScale;
        }

        if (correctCodeRect != null)
        {
            correctCodeRect.anchoredPosition = correctCodeBaseAnchoredPos;
        }

        isRevealHeld = false;
    }

    private void OnDestroy()
    {
        if (codeInput != null)
        {
            codeInput.onValueChanged.RemoveListener(OnCodeInputChanged);
        }
    }

    private void CheckAllBMissionsComplete()
    {
        var levelData = LevelController.Instance?.currentRunLevelData;
        if (levelData == null || levelData.Count < 10) return;

        bool allDone = levelData[5].isCompleted && levelData[6].isCompleted &&
                       levelData[7].isCompleted && levelData[8].isCompleted &&
                       levelData[9].isCompleted;

        if (!allDone) return;
        if (_endgameSequenceStarted) return;

        _endgameSequenceStarted = true;

        float totalTime = 0f;
        for (int i = 5; i <= 9; i++)
            totalTime += Mathf.Max(0f, levelData[i].timeSpent);

        StartCoroutine(ShowEndgameAfterDelay(totalTime));
    }

    private IEnumerator ShowEndgameAfterDelay(float totalTime)
    {
        yield return new WaitForSeconds(5f);

        EndgameCanvas.SetActive(true);
        CanvasGroup cg = EndgameCanvas.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            StartCoroutine(FadeIn(cg));
        }

        StartCoroutine(SaveTotalTimeAndLoadLeaderboard(totalTime));
    }

    private IEnumerator SaveTotalTimeAndLoadLeaderboard(float totalTime)
    {
        yield return StartCoroutine(SaveTotalTimeToFirebase(totalTime));
        yield return StartCoroutine(FetchAndPopulateLeaderboard(totalTime));
    }

    private IEnumerator SaveTotalTimeToFirebase(float totalTime)
    {
        string userId = savecontroller.Instance?.ActiveUserId;
        string projectId = savecontroller.Instance?.FirebaseProjectId;
        string apiKey = savecontroller.Instance?.FirebaseApiKey;
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(projectId)) yield break;

        // Recalculate totalTime from current levelData to ensure accuracy
        var levelData = LevelController.Instance?.currentRunLevelData;
        float calculatedTotalTime = totalTime;
        if (levelData != null && levelData.Count > 0)
        {
            calculatedTotalTime = 0f;
            for (int i = 0; i < levelData.Count; i++)
            {
                calculatedTotalTime += Mathf.Max(0f, levelData[i].timeSpent);
            }
        }

        string escapedId = UnityWebRequest.EscapeURL(userId);
        string url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{escapedId}?key={apiKey}&updateMask.fieldPaths={LeaderboardTimeField}";
        string body = "{\"fields\":{\"" + LeaderboardTimeField + "\":{\"doubleValue\":" + calculatedTotalTime.ToString("F2", CultureInfo.InvariantCulture) + "}}}";
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

        using (UnityWebRequest req = new UnityWebRequest(url, "PATCH"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyBytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogWarning($"LeaderboardSave failed: {req.error}");
        }
    }

    private IEnumerator FetchAndPopulateLeaderboard(float myTotalTime)
    {
        string projectId = savecontroller.Instance?.FirebaseProjectId;
        string apiKey = savecontroller.Instance?.FirebaseApiKey;
        string myUid = savecontroller.Instance?.ActiveUserId;
        if (string.IsNullOrWhiteSpace(projectId)) yield break;

        string url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery?key={apiKey}";
        string body = "{\"structuredQuery\":{\"from\":[{\"collectionId\":\"users\"}]," +
            "\"select\":{\"fields\":[{\"fieldPath\":\"username\"},{\"fieldPath\":\"" + LeaderboardTimeField + "\"}]}," +
            "\"where\":{\"fieldFilter\":{\"field\":{\"fieldPath\":\"" + LeaderboardTimeField + "\"},\"op\":\"GREATER_THAN\",\"value\":{\"doubleValue\":0}}}," +
            "\"orderBy\":[{\"field\":{\"fieldPath\":\"" + LeaderboardTimeField + "\"},\"direction\":\"ASCENDING\"}],\"limit\":50}}";
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyBytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"LeaderboardFetch failed: {req.error}");
                yield break;
            }

            var entries = ParseLeaderboardResponse(req.downloadHandler.text);
            PopulateLeaderboard(entries, myUid, myTotalTime);
        }
    }

    private List<LeaderboardData> ParseLeaderboardResponse(string json)
    {
        var results = new List<LeaderboardData>();
        fsData data;
        if (fsJsonParser.Parse(json, out data).Failed || !data.IsList) return results;

        foreach (var item in data.AsList)
        {
            if (!item.IsDictionary) continue;
            var itemDict = item.AsDictionary;
            if (!itemDict.TryGetValue("document", out fsData docData) || !docData.IsDictionary) continue;

            var docDict = docData.AsDictionary;
            string docName = docDict.TryGetValue("name", out fsData nameData) && nameData.IsString ? nameData.AsString : "";
            string uid = docName.Length > 0 ? docName.Substring(docName.LastIndexOf('/') + 1) : "";

            if (!docDict.TryGetValue("fields", out fsData fieldsData) || !fieldsData.IsDictionary) continue;
            var fields = fieldsData.AsDictionary;

            string username = "";
            if (fields.TryGetValue("username", out fsData unameField) && unameField.IsDictionary &&
                unameField.AsDictionary.TryGetValue("stringValue", out fsData sv) && sv.IsString)
                username = sv.AsString;

            float totalTime = 0f;
            if (fields.TryGetValue(LeaderboardTimeField, out fsData timeField) && timeField.IsDictionary)
            {
                var td = timeField.AsDictionary;
                if (td.TryGetValue("doubleValue", out fsData dv))
                    totalTime = dv.IsDouble ? (float)dv.AsDouble : dv.IsInt64 ? (float)dv.AsInt64 : 0f;
            }

            if (!string.IsNullOrEmpty(uid))
                results.Add(new LeaderboardData { uid = uid, username = username, totalTime = totalTime });
        }

        return results;
    }

    private void PopulateLeaderboard(List<LeaderboardData> entries, string myUid, float myTotalTime)
    {
        if (leaderboardContent == null || leaderboardEntryPrefab == null) return;

        foreach (Transform child in leaderboardContent)
            Destroy(child.gameObject);

        bool inTopTen = false;
        int topCount = Mathf.Min(10, entries.Count);
        for (int i = 0; i < topCount; i++)
        {
            bool isMe = entries[i].uid == myUid;
            var go = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            go.GetComponent<LeaderboardEntryUI>().SetData(i + 1, entries[i].username, entries[i].totalTime, isMe);
            if (isMe) inTopTen = true;
        }

        if (!inTopTen)
        {
            int myRank = -1;
            string myUsername = "You";
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].uid == myUid)
                {
                    myRank = i + 1;
                    myUsername = entries[i].username;
                    break;
                }
            }

            if (myRank < 0)
            {
                myRank = 1;
                foreach (var e in entries)
                    if (e.totalTime < myTotalTime) myRank++;
            }

            var go = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            go.GetComponent<LeaderboardEntryUI>().SetData(myRank, myUsername, myTotalTime, true);
        }

        if (leaderboardScrollRect != null)
            leaderboardScrollRect.verticalNormalizedPosition = 1f;
    }

    private class LeaderboardData
    {
        public string uid;
        public string username;
        public float totalTime;
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
}
