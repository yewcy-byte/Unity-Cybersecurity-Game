using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class MissionUI : MonoBehaviour
{

    public static MissionUI Instance;

    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;
    public Image checkIcon;
    public GameObject missionAlert;
     public TMP_Text missionCount;

void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        UpdateQuestUI();
    }

public void UpdateQuestUI()
{   
    // Safety check for the container
    if (questListContent == null) return;

    foreach (Transform child in questListContent)
    {
        Destroy(child.gameObject);
    }

  foreach (var quest in MissionController.Instance.activeMissions)
    {
        GameObject entry = Instantiate(questEntryPrefab, questListContent);
        
        // Set Text
        entry.transform.Find("Mission Text").GetComponent<TMP_Text>().text = quest.mission.questName;

        // Visual Handling for Completion
        Image entryIcon = entry.transform.Find("Image").GetComponent<Image>();
        if (quest.isCompleted) {
            entryIcon.color = Color.green; // Highlight completed in green
        } else {
            entryIcon.color = new Color(1, 1, 1, 0.2f); // Dim for active
        }

        // Find the objective list safely
        Transform objectiveList = entry.transform.Find("ObjectiveList");
        if (objectiveList != null)
        {
            foreach (var objective in quest.objectives)
            {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                objTextGO.GetComponent<TMP_Text>().text = objective.description;
            }
        }

       
    }
 

}


public void CompleteMission(string questID)
    {
        
        foreach (var quest in MissionController.Instance.activeMissions)
    {
        GameObject entry = Instantiate(questEntryPrefab, questListContent);

         Transform iconTransform = entry.transform.Find("Image"); 
    if (iconTransform != null)
    {
        Image entryIcon = iconTransform.GetComponent<Image>();
        
        if (quest.isCompleted)
        {
            entryIcon.color = Color.green;
NotificationManager.Instance.Display($"Mission {questID} Completed!", 4f);        }
        else
        {
            entryIcon.color = new Color(1, 1, 1, 0.2f); 
        }
    }
    }
    MissionController.Instance.MissionAlert();
    UpdateQuestUI();
    }
}
