using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;




public class SQLInjection : MonoBehaviour
{
    public computer Computer;
    public InputField usernameInput;
    public InputField passwordInput;
    public CanvasGroup Success_Image;
    public CanvasGroup Failed_Image;

    public GameObject playerHint;

    public GameObject guiderBotParent;

    public GameObject lockImage;

    public GameObject timer;

    public TextMeshProUGUI sqlText;

    public Image loginPanel;

    public CanvasGroup trainingText;



    public CanvasGroup logo;

    public Button bombButton;

    public bombButton bombButtonScript;

    public CanvasGroup brainCanvas;
      public CanvasGroup brainTextContainer;

        public GameObject brainText;

        private TMP_Text brainTextTMP;


    public bool passedFirstLevel;
    public bool passedSecondLevel;
    public bool passedThirdLevel;

      public bool PassedFirstMision;
    public bool PassedSecondMision;
    public bool PassedThirdMision;

     private bool MissionTime;
    public bool brainTime;


    
    public GuiderBot guiderBot; 

public void DebugMissionStatus(string callSource)
{
    Debug.Log($"--- MISSION DEBUG (Source: {callSource}) ---");
    
    // Level Flags (The individual logic checks)
    Debug.Log($"[LEVELS] 1: {passedFirstLevel} | 2: {passedSecondLevel} | 3: {passedThirdLevel}");
    
    // Mission Flags (The actual quest completion)
    Debug.Log($"[MISSIONS] 1: {PassedFirstMision} | 2: {PassedSecondMision} | 3: {PassedThirdMision}");
    
    // Global State
    Debug.Log($"[STATE] MissionTime Active: {MissionTime}");
    Debug.Log("------------------------------------------");
}

    void Start()
    {

    
        trainingText.alpha = 1f;
        brainTextContainer.alpha = 0f;
            logo.alpha = 0f;           

        usernameInput.onValueChanged.AddListener(_ =>UpdateQuery());
        passwordInput.onValueChanged.AddListener(_ =>UpdateQuery());
        usernameInput.onValueChanged.AddListener(_ => AutoValidate());
        Success_Image.gameObject.SetActive(false);
      
    }
  

 public void AutoValidate()
    {
        if (brainTime == false) return;
        
        string username = usernameInput.text;
        if (MatchesAdminBypass(username) || MatchesOrBypass(username) || MatchesDeletePayload(username)) 
        {
            Debug.Log($"brainTime status: {brainTime}");
            bombButton.onClick.Invoke();
            NotificationManager.Instance.Display("Successfully deducted 10 seconds!");
            Timer.Instance.DeductTime(10f);
            savecontroller.Instance.DeductSavedLevelTime(0, 10f);
            BackgroundMusicManager.SwitchToDefaultMusic();
   
            lockImage.SetActive(false);
            brainTime = false;
            brainTextContainer.alpha = 0f;
            PositionBrainTextContainer( -50.772f);


            
        }
    }


 public void UpdateQuery()
{
    string rawQuery =
        "SELECT * FROM users \n" +
        "WHERE username = '" + usernameInput.text + "' \n" +
        "AND password = '" + passwordInput.text + "'";

    sqlText.text = ColorizeSQL(rawQuery);
if (MissionTime == false)
        {
    string username = usernameInput.text;
    if (MatchesAdminBypass(username) && passedFirstLevel == false)
    {
        guiderBot.StartDialogueRange(3,3);
       guiderBot.StartDialogueRangeDelayed(4,4,4f);
        
       
    }
     else if (MatchesOrBypass(username) && passedSecondLevel == false)
    {
        guiderBot.StartDialogueRange(9,9);
               guiderBot.StartDialogueRangeDelayed(4,4,4f);
    }else if (MatchesDeletePayload(username) && passedThirdLevel == false)
        {
           guiderBot.StartDialogueRange(13,13); 
        guiderBot.StartDialogueRangeDelayed(4,4,4.5f);

        }
        
        }
        else
        {
            return;
        }
    
           

}


string ColorizeSQL(string sql)
{
    int commentIndex = sql.IndexOf("--");
    if (commentIndex >= 0)
    {
        string before = sql.Substring(0, commentIndex);
        string after = sql.Substring(commentIndex);

        return ColorizeSQL(before) +
               "<color=#007D0C>" + after + "</color>";
    }

    // Color strings in single quotes
    sql = Regex.Replace(sql, @"'[^']*'", m => $"<color=#FFA500>{m.Value}</color>");

    if (sql.Contains("1=1"))
    {
        sql = sql.Replace(
            "1=1",
            "<color=#F227F5>1=1</color>" 
        );
    }

    return ColorKeywords(sql);
}


string ColorKeywords(string sql)
{
    return sql
        .Replace("SELECT", "<color=blue>SELECT</color>")
    .Replace("DELETE", "<color=blue>DELETE</color>")
        .Replace("WHERE", "<color=blue>WHERE</color>")
        .Replace("AND", "<color=blue>AND</color>");
}






  public void ValidateInput()
    {
         string username = usernameInput.text;
         string password = passwordInput.text;

                if (MatchesAdminBypass(username))
        {
        SoundManager.Play("Success");
        passedFirstLevel = true;
        
        Failed_Image.gameObject.SetActive(false);
        Success_Image.gameObject.SetActive(true);
        StartCoroutine(FadeIn(Success_Image));
DebugMissionStatus("first level");
        if (MissionTime == true)
            {

        QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "111");
        quest.isCompleted = true;
        MissionUI.Instance.CompleteMission("1");

        
        PassedFirstMision = true;
        AllMissionCompleted();
        DebugMissionStatus("first mission");

        }

        } else if (MatchesOrBypass(username))
{              
            SoundManager.Play("Success");
            passedSecondLevel = true;
            Success_Image.gameObject.SetActive(true);
            Failed_Image.gameObject.SetActive(false);
            StartCoroutine(FadeIn(Success_Image));
            DebugMissionStatus("second level");

            if (MissionTime == true)
            {
            QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "2");
            quest.isCompleted = true;
            MissionUI.Instance.CompleteMission("2");


            PassedSecondMision = true;
            AllMissionCompleted();
            DebugMissionStatus("second mission");

        }

                } else if (MatchesDeletePayload(username)) {
          SoundManager.Play("Success");
          passedThirdLevel = true;
          Success_Image.gameObject.SetActive(true);
          Failed_Image.gameObject.SetActive(false);
          StartCoroutine(FadeIn(Success_Image));
          DebugMissionStatus("third level");

                
              
                if (MissionTime == true)
                {
                
                QuestProgress quest = MissionController.Instance.activeMissions.Find(q => q.questID == "4");
                quest.isCompleted = true;
                MissionUI.Instance.CompleteMission("3");

                PassedThirdMision= true;
                AllMissionCompleted();
                DebugMissionStatus("third mission");

       
                }
                else
                {
                    return;
                }
                    
        
        
        }
        else
        {
                        Failed_Image.gameObject.SetActive(true);
                                    SoundManager.Play("failure");


                        

            StartCoroutine(FadeIn(Failed_Image));
        }
    
    }


    public void AllMissionCompleted()
    {
        if (PassedFirstMision == true && PassedSecondMision == true && PassedThirdMision == true)
        {
             Computer.IsOpened = true;
            BackgroundMusicManager.SwitchToBrainTimeMusic();
            brainTime = true;
            bombButtonScript.BrainTime = true;
             Timer.Instance.StopTimer();
        LevelController.Instance.UpdateLevelStatus(0, "SQL Injection", true);

        LeanTween.scale(timer, new Vector3(2, 2, 0), 2f)
        .setEase(LeanTweenType.easeInOutBack);
       LeanTween.moveLocal ( timer,  new Vector3 (0,0,0) ,2f )
        .setEase(LeanTweenType.easeInOutBack);
         LeanTween.scale(timer, new Vector3(0, 0, 0), 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(5f);

        lockImage.SetActive(true);

        
        StartCoroutine(FadeOut(logo));
            brainTextContainer.gameObject.SetActive(true);
StartCoroutine(FadeInWaitAndFadeOut(brainCanvas, 3f));
        brainText.SetActive(true);
    brainTextTMP = brainText.GetComponent<TMP_Text>();

        brainTextTMP.text = "Input a learned protocol to deduct 10s off the clock";
    


        PositionBrainTextContainer(158.57f);
        brainTextContainer.alpha = 0f;

        StartCoroutine(FadeInWithDelay(brainTextContainer, 1f));

      LeanTween.moveY(bombButton.gameObject, 105.5f, 1f)
        .setEase(LeanTweenType.easeOutCubic).setDelay(8f);
      
        

    
        }
        else
        {
            return;
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


    private bool MatchesAdminBypass(string input)
    {
        return Regex.IsMatch(input ?? string.Empty, @"^\s*admin'\s*--", RegexOptions.IgnoreCase);
    }

    private bool MatchesOrBypass(string input)
    {
        return Regex.IsMatch(input ?? string.Empty, @"^\s*'\s*OR\s*1\s*=\s*1\s*--\s*$", RegexOptions.IgnoreCase);
    }

    private bool MatchesDeletePayload(string input)
    {
        return Regex.IsMatch(input ?? string.Empty, @"^\s*name'\s*\)\s*;\s*DELETE\s+FROM\s+users\s*;\s*$", RegexOptions.IgnoreCase);
    }


    public void moveBombButtonDown()
    {
        LeanTween.moveY(bombButton.gameObject,-445.14f, 1f)
        .setEase(LeanTweenType.easeOutCubic);
    }
    

    public void GuiderDialogue()
    {
      
             if (passedFirstLevel == false && passedSecondLevel==false && passedThirdLevel == false)
        {
            guiderBot.StartDialogueRange(2, 2);
        }
        else if (passedFirstLevel == true && passedSecondLevel==false && passedThirdLevel == false) 
        {
             guiderBot.StartDialogueRange(5,6);
             guiderBot.StartDialogueRangeDelayed(7,8,6f);
        }else if (passedFirstLevel == true && passedSecondLevel==true && passedThirdLevel == false)  {

       guiderBot.StartDialogueRange(10,12);

        }else if (passedFirstLevel == true && passedSecondLevel==true && passedThirdLevel == true)  {
            if (MissionTime == false)
                {
        MissionTime = true;
        
        BackgroundMusicManager.SwitchToMissionMusic();

                        DebugMissionStatus("dialgoue");


        closeGuiderBot();
       
       guiderBot.StartDialogueRange(14,15, () =>
       {
            ShowHint(-376f, 140f);
             LeanTween.scale(timer, new Vector3(1, 1, 0), 0.3f)
    .setEase(LeanTweenType.easeInOutBack);

              LeanTween.moveLocal ( timer,  new Vector3 (317f,103.41f,0f) ,2f )
        .setEase(LeanTweenType.easeInOutBack);

        Timer.Instance.ResetTimer();
    Timer.Instance.StartTimer();
            
            
            });

       loginPanel.color = new Color(0f, 0.807f, 1f, 1f);

        StartCoroutine(FadeIn(logo));
        StartCoroutine(FadeOut(trainingText));


             
                } else
            {
                return;
            }
       
       
        }
       
    }
public void ShowHint(float positionX , float positionY){

    RectTransform hintRect = playerHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     SoundManager.Play("playerHint");
      LeanTween.scale(playerHint, new Vector3(1.5f, 1.5f, 0), 0.3f)
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

IEnumerator FadeInWithDelay(CanvasGroup canvasGroup, float delaySeconds)
{
    yield return new WaitForSeconds(delaySeconds);
    yield return StartCoroutine(FadeIn(canvasGroup));
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
    CenterBrainCanvas();
    yield return StartCoroutine(FadeIn(canvasGroup));
    yield return new WaitForSeconds(waitTime);
            FadeOutSuccess();

    yield return StartCoroutine(FadeOut(canvasGroup));
}

public void FadeOutSuccess()
{
    StartFadeOutAndDisable(Success_Image);
    usernameInput.text = "";
passwordInput.text = "";
GuiderDialogue();

}

public void FadeOutFailure()
{
    StartFadeOutAndDisable(Failed_Image);

    if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
    {
        Debug.LogWarning("SQLInjection.FadeOutFailure was invoked while its GameObject is inactive. Check UI Button OnClick bindings for cross-linked events.", this);
    }

    usernameInput.text = "";
passwordInput.text = "";
computer.FirstTime = true;

    
        GuiderDialogue();
    
}

IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup)
{
    yield return StartCoroutine(FadeOut(canvasGroup));
    canvasGroup.gameObject.SetActive(false);
}

private void StartFadeOutAndDisable(CanvasGroup canvasGroup)
{
    if (canvasGroup == null)
    {
        return;
    }

    if (isActiveAndEnabled && gameObject.activeInHierarchy)
    {
        StartCoroutine(FadeOutAndDisable(canvasGroup));
        return;
    }

    canvasGroup.alpha = 0f;
    canvasGroup.gameObject.SetActive(false);
}

public void closeGuiderBot()
    {
        
        LeanTween.scale(guiderBotParent, new Vector3(0, 0, 0), 0.6f)
        .setDelay(6f)
    .setEase(LeanTweenType.easeOutQuart);
    }


        




}
