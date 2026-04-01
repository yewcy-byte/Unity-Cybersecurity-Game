using UnityEngine;
using System.Collections.Generic;


[System.Serializable]

public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public float totalTime;

    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> HotbarSaveData;

    public List<ChestSaveData> chestSaveData;

    public List<QuestProgress> questProgressData;

public List<LevelSaveData> levelData;
}

[System.Serializable]

public class ChestSaveData
{
    public int chestID;
    public bool isOpened;
}

[System.Serializable]
public class LevelSaveData
{
    public string levelName;
    public float timeSpent;
    public bool isCompleted;
}
