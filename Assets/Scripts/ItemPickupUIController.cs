using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class ItemPickupUIController : MonoBehaviour
{

    public static ItemPickupUIController Instance{ get; private set;}

    public GameObject popupPrefab;
    public int maxPopups = 5;

    public float popupDuration = 3f;

    private Queue<GameObject> activePopups = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple ItemPickUpUIManager Instances detected! Destroying the extra one");
            Destroy(gameObject);
        
        }



    }

public void showItemPickup(string ItemName, Sprite itemIcon)
    {
        GameObject newPopup = Instantiate(popupPrefab, transform);
        newPopup.GetComponentInChildren<TMP_Text>().text = ItemName;

        Image itemImage = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = itemIcon;
        }

        activePopups.Enqueue(newPopup);
        if (activePopups.Count > maxPopups)
        {
            Destroy(activePopups.Dequeue());
        }

        StartCoroutine(FadeOutandDestroy(newPopup));

    }

private IEnumerator FadeOutandDestroy(GameObject popup)
    {
        yield return new WaitForSeconds (popupDuration);
        if (popup == null) yield break;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        for (float timePassed = 0f ; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (popup == null) yield break;
           canvasGroup.alpha = 1f - timePassed;
           yield return null;
        }

        Destroy(popup);
    }





}
