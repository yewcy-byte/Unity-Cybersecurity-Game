using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{

private ItemDictionary itemDictionary;
public GameObject inventoryPanel;
public GameObject Hotbar;
public GameObject slotPrefab;
public int slotCount;


public GameObject[] ItemPrefabs;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

/*
        for (int i  = 0; i<slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform ).GetComponent<Slot>(); 
        
        if (i < ItemPrefabs.Length)
            {
                GameObject item = Instantiate(ItemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem= item;
            }
        }
        */
        }

        public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in Hotbar.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
        }   

        Debug.Log("Inventory is full");
        return false;
    }



}
