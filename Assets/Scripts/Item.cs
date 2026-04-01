using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerClickHandler
{

    private const float OpenPanelOpenAnchoredY = -10.15f;
    private const float OpenPanelClosedAnchoredY = -467.89f;

    public int Id;
    public string Name;
    public GameObject OpenPanel;

    public GameObject playerHintImage;

    public GuiderBot guiderBot;


    public bool isOpened = false;
    private bool pickupInProgress = false;

    public bool isDuringTraining = false;











    public virtual void UseItem()
    {
if (Id == 7 && isDuringTraining == true)
        {
            guiderBot.StartDialogueRange(95, 96);
     LeanTween.scale(playerHintImage, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
                        isDuringTraining = false;

        }   
     


     isOpened = !isOpened;

    if (isOpened == true)
        Open();
    else
        Close();
    }

    public void Open()
    {
        if (OpenPanel == null)
        {
            return;
        }

        AnimateOpenPanelY(OpenPanelOpenAnchoredY, LeanTweenType.easeOutQuint);

    }
     public void Close()
    {
        if (OpenPanel == null)
        {
            return;
        }

        AnimateOpenPanelY(OpenPanelClosedAnchoredY, LeanTweenType.easeInOutBack);
    }

    private void AnimateOpenPanelY(float targetY, LeanTweenType easeType)
    {
        RectTransform panelRect = OpenPanel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogWarning($"Item '{Name}' OpenPanel needs a RectTransform on {OpenPanel.name}.", this);
            return;
        }

        LeanTween.cancel(OpenPanel);

        float startY = panelRect.anchoredPosition.y;
        LeanTween.value(OpenPanel, startY, targetY, 0.6f)
            .setEase(easeType)
            .setOnUpdate((float currentY) =>
            {
                Vector2 anchoredPosition = panelRect.anchoredPosition;
                anchoredPosition.y = currentY;
                panelRect.anchoredPosition = anchoredPosition;
            });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UseItem();
    }

    public bool TryBeginPickup()
    {
        if (pickupInProgress)
        {
            return false;
        }

        pickupInProgress = true;
        return true;
    }

    public void CancelPickup()
    {
        pickupInProgress = false;
    }



    public virtual void Pickup()
    {
                savecontroller.Instance?.SaveGame();

        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.showItemPickup(Name,itemIcon);
        }    }






}
