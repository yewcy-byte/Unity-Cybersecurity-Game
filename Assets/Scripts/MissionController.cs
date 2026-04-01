using UnityEngine;
using System.Collections.Generic;
using System.Collections;         
using System.Linq;

public class MissionController : MonoBehaviour
{
    public static MissionController Instance { get; private set; }

    public List<QuestProgress> activeMissions = new();


    

    private MissionUI missionUI;

    private int missionCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        missionUI = FindFirstObjectByType<MissionUI>();
    }

    public void AcceptMission(Mission mission)
    {

        if (IsMissionActive(mission.questID)) return;

        activeMissions.Insert(0, new QuestProgress(mission));
        missionUI.UpdateQuestUI();
        MissionAlert();

    }

    public bool IsMissionActive(string missionID)
    {
        return activeMissions.Exists(m => m.QuestID == missionID);
    }


    public void MissionAlert()
    {
                missionCount = activeMissions.Count(m => !m.isCompleted);
        

          LeanTween.scale(missionUI.missionAlert, new Vector3(0.6f, 0.6f, 1f), 0.5f)
            .setEase(LeanTweenType.easeOutCirc);

        LeanTween.scale(missionUI.missionAlert, new Vector3(0.2970431f, 0.2970431f,0.5f ), 1f)
            .setDelay(1f)
            .setEase(LeanTweenType.easeInCubic);

                missionUI.missionCount.text =  $"{missionCount}";
    }

public void LoadQuestProgress(List<QuestProgress> savedQuests)
{
    activeMissions = savedQuests ?? new List<QuestProgress>();

    foreach (var progress in activeMissions)
    {
        if (progress.mission == null && !string.IsNullOrEmpty(progress.questID))
        {
            progress.mission = Resources.Load<Mission>("Missions/" + progress.questID);
        }
    }

    missionCount = activeMissions.Count(m => !m.isCompleted);
    MissionAlert();

    // Use a Coroutine to wait until the end of the frame
    StartCoroutine(RefreshUIReady());
}

private IEnumerator RefreshUIReady()
{
    yield return new WaitForEndOfFrame(); // Wait for UI to initialize
    
    if (missionUI != null) 
    {
        missionUI.missionCount.text = missionCount.ToString();
        missionUI.UpdateQuestUI();
        Debug.Log("UI Refreshed with loaded missions.");
    }
}


}

