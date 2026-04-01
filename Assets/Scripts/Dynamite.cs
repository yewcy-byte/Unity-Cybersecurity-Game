using UnityEngine;

public class Dynamite : MonoBehaviour, IInteractable, IChestable
{

public bool IsOpened {get; private set;}
    [SerializeField] private int stableChestID;
public int ChestID => stableChestID;
public GameObject itemPrefab;
public Sprite openedSprite;

    void Awake()
    {
        if (stableChestID == 0)
        {
            stableChestID = GenerateStableChestID();
        }
    }

    void Start()
    {
        if (stableChestID == 0)
        {
            stableChestID = GenerateStableChestID();
        }
    }

    private int GenerateStableChestID()
    {
        string scenePath = gameObject.scene.path;
        string hierarchyPath = GetHierarchyPath(transform);
        string key = scenePath + "|" + hierarchyPath;
        return DeterministicHash(key);
    }

    private string GetHierarchyPath(Transform target)
    {
        string path = target.name;
        Transform parent = target.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }

    private int DeterministicHash(string input)
    {
        unchecked
        {
            const int offsetBasis = (int)2166136261;
            const int prime = 16777619;
            int hash = offsetBasis;

            for (int index = 0; index < input.Length; index++)
            {
                hash ^= input[index];
                hash *= prime;
            }

            return hash == 0 ? 1 : hash;
        }
    }


    public bool CanInteract()
    {
        return !IsOpened;
    }


public void Interact()
    {
        if (!CanInteract()) return;
        OpenChest();
    }


  private void OpenChest()
{
    SetOpened(true);
    SoundManager.Play("Dynamite");

    if (itemPrefab)
    {
        GameObject droppedItem =
            Instantiate(itemPrefab, transform.position , Quaternion.identity);

    }
}


    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (opened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
