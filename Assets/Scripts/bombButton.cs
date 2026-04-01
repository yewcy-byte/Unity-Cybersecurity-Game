using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;


public class bombButton : MonoBehaviour
{

    [Header("Other to Disable")]
    public GameObject chainImage;
    public GameObject guider;
    public bool BrainTime;

    public PlayerInput playerInput;

            public GameObject playerHint;

        public GameObject LongHint;
        public GameObject LongHint2;
        public GameObject LongHint3;
        public GameObject pixelIpad;

    

    [Header("SQL Injection")]
    public computer computer;

    public SQLInjection sQLInjection;
    public GameObject loginPanel;

    public GameObject brainText;
 

    [Header("Broken Access Control")]


    public BrokenAccessComputer brokenAccessComputer;
    public GameObject BrokenPanel;

        [Header("Broken Access Control")]
    public XSSComputer XSSComputer;
    public GameObject XSSCanvas;

    [Header("CSRF")]
    public CSRF_Computer cSRF_Computer;
    public GameObject CSRFCanvas;

        [Header("JWT")]
    public JWT_Computer jWT_Computer;
    public GameObject JWTCanvas;

     
    public void OnPress()
    {
        bool wasBrokenPanelOpen = BrokenPanel != null && BrokenPanel.activeInHierarchy;
        bool wasLoginPanelOpen = loginPanel != null && loginPanel.activeInHierarchy;
        bool wasXssCanvasOpen = XSSCanvas != null && XSSCanvas.activeInHierarchy;
        bool wasCsrfCanvasOpen = CSRFCanvas != null && CSRFCanvas.activeInHierarchy;
        bool wasJwtCanvasOpen = JWTCanvas != null && JWTCanvas.activeInHierarchy;

        sQLInjection.moveBombButtonDown();
        brainText.SetActive(false);
        chainImage.SetActive(false);
        guider.SetActive(false);

        loginPanel.SetActive(false);
        BrokenPanel.SetActive(false);         
        XSSCanvas.SetActive(false);
        CSRFCanvas.SetActive(false);
        JWTCanvas.SetActive(false);

            StartCoroutine(MoveIpadAndWait(new Vector3(554.16f, 97.8f, 0f), 1f));


                    LeanTween.scale(LongHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
                         LeanTween.scale(LongHint2, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
                         LeanTween.scale(LongHint3, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);
            LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
                         .setEase(LeanTweenType.easeInOutBack);

                    PauseController.SetPause(false);
                    playerInput.SwitchCurrentActionMap("Player");

if (BrainTime == true)
        {
            BrainTime = false;
                    // Prioritize the currently open mission panel for deterministic behavior.
        if (wasBrokenPanelOpen)
        {
            BrokenAccessControl();
        }
        else if (wasLoginPanelOpen)
        {
            SQLInjection();
        } else if (wasXssCanvasOpen)
         {
           XSS();
         }
        else if (wasCsrfCanvasOpen)
        {
            CSRF();
        }
        else if (wasJwtCanvasOpen)
        {
            JWT();
        }
        }

    }

    public void SQLInjection()
    {
         computer.OpenChest();
         
  
         
    }

    public void BrokenAccessControl()
    {
         brokenAccessComputer.OpenChest();
    }

      public void XSS()
    {
         XSSComputer.OpenChest();
    }

    public void CSRF()
    {
        cSRF_Computer.OpenChest();
    }

    public void JWT()
    {
        jWT_Computer.OpenChest();
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
}
