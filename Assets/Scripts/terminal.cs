using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;


public class terminal : MonoBehaviour
{


        
        public GuiderBot guiderBot;

        public GameObject terminalCanvas;

        public GameObject filesPage;

        public bool FirstTime = false;

        public bool Training = false;

        public bool element34 = false;

        public GameObject playerHint;

        public bool brainTime = false;
        public GameObject longHint;

        public GameObject ScollView;
        [SerializeField] private float scollViewYOffset = -20f;









  

    public void closeTerminal()
    {

        terminalCanvas.SetActive(false);
        filesPage.SetActive(false);   

        if (brainTime || Training == false) {
            return;
        }
        else
        {
         LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

                         
       

        if (element34 == true)
        {
            guiderBot.StartDialogueRange(34, 34);
            element34 = false;
        }
           
        }
        
         
         
    }

    public void toggleTerminal()
    {
        filesPage.SetActive(false);   
        bool willOpen = !terminalCanvas.activeSelf;
        if (willOpen)
        {
            CenterTerminalCanvas();
            CenterScollRect();
        }

        terminalCanvas.SetActive(willOpen);
        if (brainTime || Training == false) {
            return;
        }
        else
        {
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
        LeanTween.scale(longHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

        if (FirstTime == false)
        {
            FirstTime = true;
         guiderBot.StartDialogueRange(21, 23, () =>
         {
             
ShowLongHint(-23f, 153f, 3.33f, 0.77f);
         });
        }    
        }
        
    }
        private void CenterScollRect()
    {
        if (ScollView == null)
        {
            return;
        }

        RectTransform rect = ScollView.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, scollViewYOffset);
    }

    private void CenterTerminalCanvas()
    {
        if (terminalCanvas == null)
        {
            return;
        }

        RectTransform rect = terminalCanvas.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
    }
    

    public void toggleFiles()
    {
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
        LeanTween.scale(longHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

        terminalCanvas.SetActive(false); 
        filesPage.SetActive(!filesPage.activeSelf);

        if (brainTime || Training == false) {
            return;
        }
        else
        {
            guiderBot.StartDialogueRange(28, 28);        
    
         LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
                    
        }
       
    }

    public void ShowLongHint(float positionX , float positionY, float Xscale = 0.98f, float Yscale = 0.32f){

     SoundManager.Play("playerHint");

    RectTransform hintRect = longHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

    LeanTween.scale(longHint, new Vector3(Xscale, Yscale, 0f), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}

    public void ShowHint(float positionX , float positionY){

    RectTransform hintRect = playerHint.GetComponent<RectTransform>();
     hintRect.anchoredPosition = new Vector2(positionX, positionY);

     SoundManager.Play("playerHint");
      LeanTween.scale(playerHint, new Vector3(1, 1, 0), 0.3f)
                        .setEase(LeanTweenType.easeInOutBack);
     
}
}
