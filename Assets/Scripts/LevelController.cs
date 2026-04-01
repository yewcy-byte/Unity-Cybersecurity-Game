using UnityEngine;
using System.Collections.Generic;


public class LevelController : MonoBehaviour
{

public static LevelController Instance;

void Awake()
    {
        // Initialize the instance so other scripts can find it
        if (Instance == null) Instance = this;
    }
    
    // This list stays active while the game is running
    public List<LevelSaveData> currentRunLevelData = new List<LevelSaveData>();

    public List<LevelSaveData> GetAllLevelData()
    {
        return currentRunLevelData;
    }

    public void UpdateLevelStatus(int index, string name, bool completed)
    {
        // Ensure the list is initialized
        while (currentRunLevelData.Count <= index) currentRunLevelData.Add(new LevelSaveData());

        currentRunLevelData[index].levelName = name;
        currentRunLevelData[index].timeSpent = Timer.Instance.GetTimeData();
        currentRunLevelData[index].isCompleted = completed;

        savecontroller.Instance.SaveGame();
    }

}
