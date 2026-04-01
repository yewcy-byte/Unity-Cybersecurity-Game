using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;


public class computer : MonoBehaviour, IInteractable, IChestable
{
    private const string RequiredQuestId = "4";


    public GameObject loginPanel;
        public PlayerInput playerInput;

        public SQLInjection sQLInjection;
        [SerializeField] GameObject guider;

        public GuiderBot guiderBot;
        public GameObject chatbubble;
        public GameObject Xbutton;

        public GameObject Wall_to_Break;
        public Sprite brokenWall;
        public Button bombButton;
        public bool brainTime;




        

        public bool IsOpened {get;set;}
    
public GameObject itemPrefab;
public Sprite openedSprite;
        [SerializeField] private int stableChestID;
        public int ChestID => stableChestID;



        public static bool FirstTime = false;



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


     public void OpenChest()
{

   SoundManager.Play("timer");
   StartCoroutine(OpenChestRoutine());

}


private IEnumerator OpenChestRoutine()
    {
        yield return new WaitForSeconds(3f);
         SetOpened(true);
        savecontroller.Instance?.SaveGame();
    SoundManager.Play("Dynamite");

    if (itemPrefab)
    {
        GameObject droppedItem =
            Instantiate(itemPrefab, transform.position , Quaternion.identity);
            //i want to change some tiles with collision to non collision and change their sprite

    }

  
    
    }


    public void SetOpened(bool opened)
    {
       
        if (opened)
        {
              Wall_to_Break.GetComponent<Collider2D>().enabled = false;
    Wall_to_Break.GetComponent<SpriteRenderer>().sprite = brokenWall;
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }

 public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        loginPanel.SetActive(!loginPanel.activeSelf);
        guider.SetActive(!guider.activeSelf);


        if (FirstTime == true)
        {
            sQLInjection.GuiderDialogue();
        }
        else
        {

            guider.transform.localScale = Vector3.zero;
            FirstTimeGuider();
        }

      
        
        if (loginPanel.activeSelf == true)
        {
        PauseController.SetPause(true);
                playerInput.SwitchCurrentActionMap("UI");
               

        }
        else
        {
            PauseController.SetPause(false);
                    playerInput.SwitchCurrentActionMap("Player");

        }
        


        

    }


    public bool CanInteract()
    {
        return HasRequiredMission();
    }

    private bool HasRequiredMission()
    {
        if (MissionController.Instance == null || MissionController.Instance.activeMissions == null)
        {
            return false;
        }

        return MissionController.Instance.activeMissions.Exists(q =>
            q != null && string.Equals(q.questID, RequiredQuestId, System.StringComparison.OrdinalIgnoreCase));
    }

public void FirstTimeGuider()
{
RectTransform guiderRect =  guider.GetComponent<RectTransform>();

    guiderRect.anchoredPosition = new Vector2(291.62f, -134.2f);


LeanTween.scale(guider, new Vector3(6, 6, 0), 0.6f)
    .setDelay(0.5f)
    .setEase(LeanTweenType.easeOutQuart)
    .setOnComplete(() =>
    {
        LeanTween.scale(chatbubble, new Vector3(1, 1, 1), 0.5f)
            .setEase(LeanTweenType.easeOutQuart)
            .setOnComplete(() => guiderBot.StartDialogueRange(0, 1));
    });

LeanTween.scale(Xbutton, new Vector3(1, 1, 0), 0.3f)
    .setDelay(2.5f)
    .setEase(LeanTweenType.easeOutQuart);

}


}
