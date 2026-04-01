using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{

    private InventoryController inventoryController;
    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

 private void OnTriggerEnter2D (Collider2D collision)
    {
    if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                if (!item.TryBeginPickup())
                {
                    return;
                }

                bool itemAdded = inventoryController.AddItem(collision.gameObject);
                if (itemAdded)
                {
                    item.Pickup();
                    Destroy(collision.gameObject);
                }
                else
                {
                    item.CancelPickup();
                }

            }
        }       
    }
}