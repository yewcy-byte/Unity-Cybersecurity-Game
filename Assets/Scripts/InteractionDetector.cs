using UnityEngine;
using UnityEngine.InputSystem;


public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    public GameObject interactionButton;







    void Start()
    {
    interactionIcon.SetActive(false);    
    interactionButton.SetActive(false);    
    }


public void InteractButton()
{
    if (interactableInRange != null && interactableInRange.CanInteract())
    {
        interactableInRange.Interact();
    }
}




    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
            interactionButton.SetActive(true);

   LeanTween.scale(interactionButton, new Vector3(1f,1f,1f), 0.3f)
            .setEase(LeanTweenType.easeOutBack);        }


    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
               LeanTween.scale(interactionButton, new Vector3(0f,0f,0f), 0.3f)
            .setEase(LeanTweenType.easeOutQuart).setOnComplete(()=>interactionButton.SetActive(false));

        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IInteractable interactable))
            return;

        if (interactable.CanInteract())
        {
            if (interactableInRange == null)
            {
                interactableInRange = interactable;
                interactionIcon.SetActive(true);
                interactionButton.SetActive(true);
                LeanTween.scale(interactionButton, new Vector3(1f,1f,1f), 0.3f)
                    .setEase(LeanTweenType.easeOutBack);
            }
        }
        else if (interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
            LeanTween.scale(interactionButton, new Vector3(0f,0f,0f), 0.3f)
                .setEase(LeanTweenType.easeOutQuart).setOnComplete(() => interactionButton.SetActive(false));
        }
    }

}
