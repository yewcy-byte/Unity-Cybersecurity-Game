using UnityEngine;
using UnityEngine.EventSystems;




public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

Transform originalParent;
CanvasGroup canvasGroup;

public float minDropdistance = 2f;
public float maxDropdistance = 3f;



public void OnBeginDrag(PointerEventData eventData)
    {
      originalParent = transform.parent;
      transform.SetParent(transform.root);
      canvasGroup.blocksRaycasts = false;
      canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
      
      
    }
    
  public void OnEndDrag(PointerEventData eventData)
{

    if (canvasGroup == null)
        return;

    canvasGroup.blocksRaycasts = true;
    canvasGroup.alpha = 1f;

    Slot dropSlot = null;

    if (eventData.pointerEnter != null)
    {
        dropSlot = eventData.pointerEnter.GetComponent<Slot>()
                   ?? eventData.pointerEnter.GetComponentInParent<Slot>();
    }

    if (originalParent == null)
        return;

    Slot originalSlot = originalParent.GetComponent<Slot>();
    bool shouldSave = false;

    if (dropSlot != null && originalSlot != null)
    {
        if (dropSlot.currentItem != null)
        {
            dropSlot.currentItem.transform.SetParent(originalSlot.transform);
            originalSlot.currentItem = dropSlot.currentItem;
            dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            originalSlot.currentItem = null;
        }

        transform.SetParent(dropSlot.transform);
        dropSlot.currentItem = gameObject;
        shouldSave = true;
    }
    else
    {
        if (!IsWithinInventory(eventData.position))
            {
            DropItem(originalSlot);
            }
            else
            {
                        transform.SetParent(originalParent);

            }
    }

    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

    if (shouldSave)
    {
        QueueSave();
    }
}

bool IsWithinInventory(Vector2 MousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, MousePosition);
    }

void DropItem(Slot originalSlot)
    {
        originalSlot.currentItem = null;



//Find Player
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("Missing Player Tag");
            return;
        }


        //Rabdom drop position
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropdistance, maxDropdistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;


    //Instantiate drop item
    GameObject dropItem = Instantiate(gameObject, dropPosition, Quaternion.identity);
dropItem.GetComponent<Bouncing>().StartBounce();
    //Destory the UI object
    Destroy(gameObject);

    QueueSave();

    }

void QueueSave()
    {
        if (savecontroller.Instance != null)
        {
            savecontroller.Instance.Invoke(nameof(savecontroller.SaveGame), 0f);
        }
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }


}
