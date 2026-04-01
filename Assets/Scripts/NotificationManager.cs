using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    public TMP_Text notificationText;
    public GameObject notificationPanel;

    void Awake()
    {
        Instance = this;
        notificationPanel.SetActive(false); // Hide at start
    }

public void Display(string message, float duration = 3f)
{
    notificationPanel.SetActive(true); 

    StopAllCoroutines(); 
    StartCoroutine(ShowMessage(message, duration));
}

private IEnumerator ShowMessage(string message, float duration)
{
    notificationText.text = message;

    yield return new WaitForSeconds(duration);

    notificationPanel.SetActive(false);

    }
}