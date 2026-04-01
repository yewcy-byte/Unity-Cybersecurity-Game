using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class HotBarController : MonoBehaviour
{

public GameObject hotbarPanel;
public GameObject slotPrefab;

public int slotCount = 5;

private ItemDictionary itemDictionary;

private Key[] hotbarKeys;

private void Awake()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for (int i = 0 ; i < slotCount ; i++)
        {
        hotbarKeys[i] = Key.Digit1 + i;
        }
    }








public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot == null)
            {
                continue;
            }

            // Prefer the real hierarchy child to avoid stale slot.currentItem references.
            GameObject itemObject = slot.currentItem;
            if (itemObject == null || itemObject.transform.parent != slotTransform)
            {
                Item childItem = slotTransform.GetComponentInChildren<Item>(true);
                itemObject = childItem != null ? childItem.gameObject : null;
                slot.currentItem = itemObject;
            }

            if (itemObject == null)
            {
                continue;
            }

            Item item = itemObject.GetComponent<Item>();
            if (item == null || item.Id <= 0)
            {
                continue;
            }

            hotbarData.Add(new InventorySaveData { itemID = item.Id, slotindex = slotTransform.GetSiblingIndex() });
        }

        return hotbarData;
    }
    public void  SetHotbarItem(List<InventorySaveData> inventorySaveData)
    {
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }
    foreach(InventorySaveData data in inventorySaveData)
        {
            if (data.slotindex < slotCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotindex).GetComponent<Slot>();
                GameObject ItemPrefab = itemDictionary.GetItemPrefab(data.itemID);

                if (ItemPrefab != null)
                {
                GameObject item = Instantiate(ItemPrefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
                

                }
                }
        }
    
    

    }






}
