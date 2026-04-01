using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TMPLinkOpener : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button buttonToInvoke;

    private TMP_Text clickableText;

    private void Awake()
    {
        TMP_InputField inputField = GetComponent<TMP_InputField>();
        TMP_Text text = GetComponent<TMP_Text>();

        clickableText = inputField != null ? inputField.textComponent : text;

        if (inputField != null && inputField.textComponent != null)
        {
            inputField.textComponent.richText = true;
            inputField.textComponent.raycastTarget = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickableText == null)
        {
            return;
        }

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(clickableText, eventData.position, eventData.pressEventCamera);
        if (linkIndex < 0)
        {
            return;
        }

        if (buttonToInvoke != null)
        {
            buttonToInvoke.onClick.Invoke();
        }
    }
}
