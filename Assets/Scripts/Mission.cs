using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Mission")]
public class Mission : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public List<QuestObjective> objectives;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + "_" + Guid.NewGuid().ToString();
        }
    }

}
    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveID;
        public string description;
        public ObjectiveType type;

   

    }

    public enum ObjectiveType
    {
        CollectItem,
        DefeatEnemy,
        ReachLocation,
        TalkNPC,
        Custom
    }

[System.Serializable]
public class QuestProgress
{
    public string questID; 
    public Mission mission; 
    // You can keep the list for structure, or just reference the first one
    public List<QuestObjective> objectives;

    public bool isCompleted;      

    public QuestProgress(Mission mission)
    {
        this.mission = mission;
        this.questID = mission.questID; 
        this.isCompleted = false; 
        
        objectives = new List<QuestObjective>();

        // Just add the one objective from the ScriptableObject
        foreach (var obj in mission.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
            });
        }
    }

    public string QuestID => mission != null ? mission.questID : questID;
}