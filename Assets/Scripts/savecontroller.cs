using UnityEngine;
using System.IO;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Globalization;
using System.Text;
using FullSerializer;
using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class savecontroller : MonoBehaviour
{
        public static savecontroller Instance;

    public string ActiveUserId => activeUserId;
    public string FirebaseProjectId => firebaseProjectId;
    public string FirebaseApiKey => firebaseApiKey;

    public CanvasGroup introVideoCanvasGroup;

    private const string FirebaseSaveField = "unityGameSaveJson";
    private const string FirebaseSaveObjectField = "unityGameSaveObject";

    private string saveLocation;

    private const string WindowsSaveDirectory = @"C:\Users\Yew CY\Penetration-Education-Cyber-Game\top down game";

    private InventoryController InventoryController;
    private HotBarController hotbarController;
private Dynamite[] dynamites;

    [Header("Firebase")]
    [SerializeField] private bool useFirebaseInEditor = true;
    [SerializeField] private string manualUserIdForEditor = "";
    [SerializeField] private string firebaseApiKey = "REPLACE_WITH_FIREBASE_API_KEY";
    [SerializeField] private string firebaseProjectId = "unity-cyber-game";

    public VidPlayer vidPlayer;
    [SerializeField] private RawImage introVideoRawImage;
    [SerializeField] private float introVideoFadeInDuration = 0.25f;
    [SerializeField] private float introVideoFadeOutDuration = 0.35f;
    [SerializeField] private GameObject guider;
    [SerializeField] private GameObject chatbubble;
            public GuiderBot guiderBot;
    [SerializeField] private TMP_Text totalTimeText;


    private string activeUserId;
    private bool useRemoteSave;
    private bool introVideoFinished;
    private bool hasPlayFlowStarted;
    private SaveData lastLoadedSaveData;
    private const float DuplicateLevelDeductionWindowSeconds = 0.2f;
    private float lastLevelDeductionUnscaledTime = -999f;
    private int lastLevelDeductionIndex = -1;
    private float lastLevelDeductionSeconds = -1f;

    [Serializable]
    private class FirestoreStringValue
    {
        public string stringValue;
    }

    [Serializable]
    private class FirestoreFields
    {
        public FirestoreStringValue unityGameSaveJson;
    }

    [Serializable]
    private class FirestoreDocument
    {
        public FirestoreFields fields;
    }

    [Serializable]
    private class EmbeddedSaveJsonPayload
    {
        public string unitySaveJson;
        public string unityGameSaveJson;
        public SaveData unityGameSaveObject;
    }

    [Serializable]
    private class FirebaseSaveFieldPayload
    {
        public SaveData unityGameSaveObject;
    }



        void Start()
    {
        Instance = this;
        InitializeComponents();

        activeUserId = ResolveUserId();
        useRemoteSave = ShouldUseRemoteSave();

        // Loading/new-user intro is triggered from the Play button.
    }

    public void OnPlayButtonPressed()
    {
        if (hasPlayFlowStarted)
        {
            return;
        }

        hasPlayFlowStarted = true;
        StartFadeOutAndDisable(introVideoCanvasGroup);

        if (useRemoteSave)
        {
            if (string.IsNullOrWhiteSpace(activeUserId))
            {
                Debug.LogWarning("Firebase mode is enabled but userId is missing. Falling back to local saveData.json.");
                LoadGame();
                return;
            }

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                LoadGameFromFirebase(activeUserId);
            }
            else
            {
                StartCoroutine(LoadGameFromFirebaseRest(activeUserId));
            }

            return;
        }

        LoadGame();
    }
    

    private bool ShouldUseRemoteSave()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return true;
        }

#if UNITY_EDITOR
        return useFirebaseInEditor;
#else
        return false;
#endif
    }

    private string ResolveUserId()
    {
        string uidFromUrl = GetQueryValueFromAbsoluteUrl("uid");
        if (!string.IsNullOrWhiteSpace(uidFromUrl))
        {
            return uidFromUrl;
        }

#if UNITY_EDITOR
        if (!string.IsNullOrWhiteSpace(manualUserIdForEditor))
        {
            return manualUserIdForEditor.Trim();
        }
#endif

        return string.Empty;
    }

    private string GetQueryValueFromAbsoluteUrl(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(Application.absoluteURL))
        {
            return string.Empty;
        }

        int queryIndex = Application.absoluteURL.IndexOf('?');
        if (queryIndex < 0 || queryIndex >= Application.absoluteURL.Length - 1)
        {
            return string.Empty;
        }

        string query = Application.absoluteURL.Substring(queryIndex + 1);
        string[] parts = query.Split('&');
        for (int index = 0; index < parts.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(parts[index]))
            {
                continue;
            }

            string[] pair = parts[index].Split('=');
            if (pair.Length == 0)
            {
                continue;
            }

            if (!string.Equals(Uri.UnescapeDataString(pair[0]), key, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (pair.Length < 2)
            {
                return string.Empty;
            }

            return Uri.UnescapeDataString(pair[1]);
        }

        return string.Empty;
    }














  public void InitializeComponents()
    {
        string saveDirectory;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL does not use local file paths
            saveDirectory = Application.persistentDataPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor ||
                 Application.platform == RuntimePlatform.WindowsPlayer)
        {
            saveDirectory = WindowsSaveDirectory;
        }
        else
        {
            // Fallback for other platforms
            saveDirectory = Application.persistentDataPath;
        }

        Directory.CreateDirectory(saveDirectory);
        saveLocation = Path.Combine(saveDirectory, "saveData.json");

        Debug.Log($"Save path: {saveLocation}");

        InventoryController = FindFirstObjectByType<InventoryController>();
        hotbarController = FindFirstObjectByType<HotBarController>();
        dynamites = FindObjectsByType<Dynamite>(FindObjectsSortMode.None);
    }

    public void SaveGame()
    {
        SaveData existingData = useRemoteSave ? lastLoadedSaveData : LoadExistingSaveData();

        if (existingData == null)
        {
            existingData = lastLoadedSaveData;
        }

        SaveData saveData = BuildCurrentSaveData(existingData);
        UpdateTotalTimeText(saveData.totalTime);

        if (useRemoteSave && !string.IsNullOrWhiteSpace(activeUserId))
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SaveGameToFirebase(activeUserId, saveData);
            }
            else
            {
                StartCoroutine(SaveGameToFirebaseRest(activeUserId, saveData));
            }

            return;
        }

        // 3. Serialize and Write
        string json = JsonUtility.ToJson(saveData, true); // 'true' makes the JSON readable
        File.WriteAllText(saveLocation, json);
        lastLoadedSaveData = CloneSaveData(saveData);
    } 

    private SaveData BuildCurrentSaveData(SaveData existingData)
    {
        InventoryController ??= FindFirstObjectByType<InventoryController>();
        hotbarController ??= FindFirstObjectByType<HotBarController>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPosition = playerObject != null
            ? playerObject.transform.position
            : (existingData != null ? existingData.playerPosition : Vector3.zero);

        CinemachineConfiner confiner = FindFirstObjectByType<CinemachineConfiner>();
        string mapBoundaryName = existingData != null ? existingData.mapBoundary : string.Empty;
        if (confiner != null && confiner.m_BoundingShape2D != null)
        {
            mapBoundaryName = confiner.m_BoundingShape2D.gameObject.name;
        }


        List<InventorySaveData> currentHotbarData = hotbarController != null
            ? hotbarController.GetHotbarItems()
            : null;
        List<InventorySaveData> existingHotbarData = existingData != null
            ? existingData.HotbarSaveData
            : null;
        List<InventorySaveData> hotbarData = MergeHotbarData(currentHotbarData, existingHotbarData);

        List<ChestSaveData> chestData = MergeChestData(
            GetAllChestStates(),
            existingData != null ? existingData.chestSaveData : null);

        List<QuestProgress> questData = MergeQuestData(
            MissionController.Instance != null ? MissionController.Instance.activeMissions : null,
            existingData != null ? existingData.questProgressData : null);

        List<LevelSaveData> levelData = MergeLevelData(
            LevelController.Instance != null ? LevelController.Instance.GetAllLevelData() : null,
            existingData != null ? existingData.levelData : null);

        return new SaveData()
        {
            playerPosition = playerPosition,
            mapBoundary = mapBoundaryName,
            totalTime = CalculateTotalLevelTime(levelData),
            HotbarSaveData = hotbarData,
            chestSaveData = chestData,
            questProgressData = questData,
            levelData = levelData
        };
    }

    private SaveData LoadExistingSaveData()
    {
        if (string.IsNullOrEmpty(saveLocation) || !File.Exists(saveLocation))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(saveLocation);
            return string.IsNullOrWhiteSpace(json) ? null : JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            // If existing save cannot be parsed, proceed with current runtime data.
            return null;
        }
    }

    private List<T> PreferCurrentOrExisting<T>(List<T> current, List<T> existing)
    {
        if (current != null && current.Count > 0)
        {
            return current;
        }

        return existing != null ? new List<T>(existing) : new List<T>();
    }

    private List<ChestSaveData> MergeChestData(List<ChestSaveData> current, List<ChestSaveData> existing)
    {
        Dictionary<int, ChestSaveData> merged = new Dictionary<int, ChestSaveData>();

        if (existing != null)
        {
            for (int index = 0; index < existing.Count; index++)
            {
                ChestSaveData entry = existing[index];
                if (entry == null)
                {
                    continue;
                }

                merged[entry.chestID] = new ChestSaveData
                {
                    chestID = entry.chestID,
                    isOpened = entry.isOpened
                };
            }
        }

        if (current != null)
        {
            for (int index = 0; index < current.Count; index++)
            {
                ChestSaveData entry = current[index];
                if (entry == null)
                {
                    continue;
                }

                if (merged.TryGetValue(entry.chestID, out ChestSaveData existingEntry))
                {
                    // Chest open state should be monotonic: once opened, keep it opened.
                    existingEntry.isOpened = existingEntry.isOpened || entry.isOpened;
                }
                else
                {
                    merged[entry.chestID] = new ChestSaveData
                    {
                        chestID = entry.chestID,
                        isOpened = entry.isOpened
                    };
                }
            }
        }

        return merged.Values.ToList();
    }

    private List<QuestProgress> MergeQuestData(List<QuestProgress> current, List<QuestProgress> existing)
    {
        Dictionary<string, QuestProgress> merged = new Dictionary<string, QuestProgress>(StringComparer.OrdinalIgnoreCase);

        if (existing != null)
        {
            for (int index = 0; index < existing.Count; index++)
            {
                UpsertQuestProgress(merged, existing[index], "existing", index);
            }
        }

        if (current != null)
        {
            for (int index = 0; index < current.Count; index++)
            {
                UpsertQuestProgress(merged, current[index], "current", index);
            }
        }

        return merged.Values.ToList();
    }

    private List<InventorySaveData> MergeHotbarData(List<InventorySaveData> current, List<InventorySaveData> existing)
    {
        // Preserve previously saved slots; only replace a slot if runtime data has a valid item for it.
        Dictionary<int, InventorySaveData> merged = new Dictionary<int, InventorySaveData>();

        if (existing != null)
        {
            for (int index = 0; index < existing.Count; index++)
            {
                InventorySaveData data = existing[index];
                if (data == null || data.slotindex < 0 || data.itemID <= 0)
                {
                    continue;
                }

                merged[data.slotindex] = CloneInventorySaveData(data);
            }
        }

        if (current != null)
        {
            for (int index = 0; index < current.Count; index++)
            {
                InventorySaveData data = current[index];
                if (data == null || data.slotindex < 0 || data.itemID <= 0)
                {
                    continue;
                }

                merged[data.slotindex] = CloneInventorySaveData(data);
            }
        }

        return merged
            .OrderBy(pair => pair.Key)
            .Select(pair => pair.Value)
            .ToList();
    }

    private InventorySaveData CloneInventorySaveData(InventorySaveData source)
    {
        if (source == null)
        {
            return null;
        }

        return new InventorySaveData
        {
            itemID = source.itemID,
            slotindex = source.slotindex
        };
    }

    private void UpsertQuestProgress(Dictionary<string, QuestProgress> merged, QuestProgress source, string sourceTag, int index)
    {
        if (source == null)
        {
            return;
        }

        string questId = GetQuestId(source);
        string key = !string.IsNullOrWhiteSpace(questId)
            ? questId
            : "__unknown_" + sourceTag + "_" + index;

        if (merged.TryGetValue(key, out QuestProgress existing))
        {
            // Quest completion should be monotonic: once completed, keep it completed.
            existing.isCompleted = existing.isCompleted || source.isCompleted;

            if (existing.mission == null && source.mission != null)
            {
                existing.mission = source.mission;
            }

            return;
        }

        merged[key] = CloneQuestProgress(source);
    }

    private string GetQuestId(QuestProgress progress)
    {
        if (progress == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(progress.questID))
        {
            return progress.questID;
        }

        return progress.QuestID;
    }

    private QuestProgress CloneQuestProgress(QuestProgress source)
    {
        if (source == null)
        {
            return null;
        }

        string json = JsonUtility.ToJson(source);
        return JsonUtility.FromJson<QuestProgress>(json);
    }

    private SaveData CloneSaveData(SaveData source)
    {
        if (source == null)
        {
            return null;
        }

        string json = JsonUtility.ToJson(source);
        return JsonUtility.FromJson<SaveData>(json);
    }

    private List<LevelSaveData> MergeLevelData(List<LevelSaveData> current, List<LevelSaveData> existing)
    {
        int currentCount = current != null ? current.Count : 0;
        int existingCount = existing != null ? existing.Count : 0;
        int mergedCount = Mathf.Max(currentCount, existingCount);

        var merged = new List<LevelSaveData>(mergedCount);
        for (int index = 0; index < mergedCount; index++)
        {
            LevelSaveData currentEntry = index < currentCount ? current[index] : null;
            LevelSaveData existingEntry = index < existingCount ? existing[index] : null;

            LevelSaveData source = HasMeaningfulLevelData(currentEntry) ? currentEntry : existingEntry;
            if (source == null)
            {
                source = currentEntry;
            }

            merged.Add(CloneLevelData(source));
        }

        return merged;
    }

    private bool HasMeaningfulLevelData(LevelSaveData data)
    {
        if (data == null)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(data.levelName)
               || data.timeSpent > 0f
               || data.isCompleted;
    }

    private LevelSaveData CloneLevelData(LevelSaveData source)
    {
        if (source == null)
        {
            return new LevelSaveData();
        }

        return new LevelSaveData
        {
            levelName = source.levelName,
            timeSpent = source.timeSpent,
            isCompleted = source.isCompleted
        };
    }

    private float CalculateTotalLevelTime(List<LevelSaveData> levelData)
    {
        if (levelData == null)
        {
            return 0f;
        }

        float totalTime = 0f;
        for (int index = 0; index < levelData.Count; index++)
        {
            LevelSaveData level = levelData[index];
            if (level != null)
            {
                totalTime += Mathf.Max(0f, level.timeSpent);
            }
        }

        return totalTime;
    }

    private bool HasMeaningfulProgress(SaveData data)
    {
        if (data == null)
        {
            return false;
        }

        bool hasHotbar = data.HotbarSaveData != null
                         && data.HotbarSaveData.Any(item => item != null && item.itemID > 0);

        bool hasQuestProgress = data.questProgressData != null
                                && data.questProgressData.Any(q => q != null && q.isCompleted);

        bool hasOpenedChest = data.chestSaveData != null
                              && data.chestSaveData.Any(c => c != null && c.isOpened);

        bool hasLevelProgress = data.levelData != null
                                && data.levelData.Any(level => level != null && (level.isCompleted || level.timeSpent > 0f));

        bool hasTotalTime = data.totalTime > 0f;

        return hasHotbar || hasQuestProgress || hasOpenedChest || hasLevelProgress || hasTotalTime;
    }

    private void SyncRuntimeLevelData(List<LevelSaveData> levelData)
    {
        if (LevelController.Instance == null)
        {
            return;
        }

        List<LevelSaveData> runtimeLevels = LevelController.Instance.GetAllLevelData();
        runtimeLevels.Clear();

        if (levelData == null)
        {
            return;
        }

        for (int index = 0; index < levelData.Count; index++)
        {
            runtimeLevels.Add(CloneLevelData(levelData[index]));
        }
    }


    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            if (saveData == null)
            {
                Debug.LogWarning("Save file is invalid. Creating a new save.");
                CreateNewGame(true);
                return;
            }
            ApplyLoadedSaveData(saveData);
        
        
        }
        else
        {
            CreateNewGame(true);

        }
    }

    private void ApplyLoadedSaveData(SaveData saveData)
    {
        lastLoadedSaveData = CloneSaveData(saveData);
        UpdateTotalTimeText(saveData.totalTime);

        PolygonCollider2D saveMapBoundry = null;
        if (!string.IsNullOrWhiteSpace(saveData.mapBoundary))
        {
            GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
            if (boundaryObject != null)
            {
                saveMapBoundry = boundaryObject.GetComponent<PolygonCollider2D>();
            }
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerObject.transform.position = saveData.playerPosition;
        }

        CinemachineConfiner confiner = FindFirstObjectByType<CinemachineConfiner>();
        if (confiner != null && saveMapBoundry != null)
        {
            confiner.m_BoundingShape2D = saveMapBoundry;
        }

        if (!string.IsNullOrWhiteSpace(saveData.mapBoundary))
        {
            MapController_Manual.Instance?.HighlightArea(saveData.mapBoundary);
        }

        InventoryController ??= FindFirstObjectByType<InventoryController>();
        hotbarController ??= FindFirstObjectByType<HotBarController>();

        hotbarController?.SetHotbarItem(saveData.HotbarSaveData ?? new List<InventorySaveData>());

        if (saveData.chestSaveData != null)
        {
            LoadChestStates<Dynamite>(saveData.chestSaveData);
            LoadChestStates<computer>(saveData.chestSaveData);
            LoadChestStates<BrokenAccessComputer>(saveData.chestSaveData);
            LoadChestStates<CSRF_Computer>(saveData.chestSaveData);
            LoadChestStates<XSSComputer>(saveData.chestSaveData);
            LoadChestStates<JWT_Computer>(saveData.chestSaveData);
        }

        MissionController.Instance?.LoadQuestProgress(saveData.questProgressData);
        RemoveCashBagIfQuestExists(saveData.questProgressData, "B1");
        SyncRuntimeLevelData(saveData.levelData);
    }

    private void UpdateTotalTimeText(float totalTime)
    {
        if (totalTimeText == null)
        {
            return;
        }

        totalTimeText.text = totalTime.ToString("F1") + "s";
    }

    public void DeductSavedLevelTime(int index, float seconds)
    {
        if (index < 0)
        {
            Debug.LogWarning("Invalid level index for time deduction.");
            return;
        }

        float safeSeconds = Mathf.Abs(seconds);
        if (safeSeconds <= 0f)
        {
            return;
        }

        // Ignore accidental duplicate calls that happen in the same instant.
        if (lastLevelDeductionIndex == index
            && Mathf.Approximately(lastLevelDeductionSeconds, safeSeconds)
            && (Time.unscaledTime - lastLevelDeductionUnscaledTime) <= DuplicateLevelDeductionWindowSeconds)
        {
            return;
        }

        lastLevelDeductionIndex = index;
        lastLevelDeductionSeconds = safeSeconds;
        lastLevelDeductionUnscaledTime = Time.unscaledTime;

        List<LevelSaveData> runtimeLevels = LevelController.Instance != null
            ? LevelController.Instance.GetAllLevelData()
            : null;

        if (runtimeLevels == null)
        {
            Debug.LogWarning("Cannot deduct level time because LevelController is unavailable.");
            return;
        }

        while (runtimeLevels.Count <= index)
        {
            runtimeLevels.Add(new LevelSaveData());
        }

        SaveData saveData = BuildCurrentSaveData(lastLoadedSaveData);
        if (saveData.levelData == null)
        {
            saveData.levelData = new List<LevelSaveData>();
        }

        while (saveData.levelData.Count <= index)
        {
            saveData.levelData.Add(new LevelSaveData());
        }

        float runtimeLevelTime = runtimeLevels[index].timeSpent;
        float savedLevelTime = saveData.levelData[index].timeSpent;

        if (runtimeLevelTime <= 0f && savedLevelTime > 0f)
        {
            runtimeLevelTime = savedLevelTime;
        }

        float updatedLevelTime = Mathf.Max(0f, runtimeLevelTime - safeSeconds);
        runtimeLevels[index].timeSpent = updatedLevelTime;
        saveData.levelData[index].timeSpent = updatedLevelTime;
        saveData.totalTime = CalculateTotalLevelTime(saveData.levelData);

        PersistSaveData(saveData);

        Debug.Log($"Deducted {safeSeconds} seconds from level index {index}. Updated time: {saveData.levelData[index].timeSpent:F2}s");
    }

  

// Firebase Setup
 public void SaveGameToFirebase(string userId)
{
    SaveData saveData = BuildCurrentSaveData(lastLoadedSaveData);
    SaveGameToFirebase(userId, saveData);
}

private void SaveGameToFirebase(string userId, SaveData saveData)
{
    if (saveData == null)
    {
        return;
    }

    UpdateTotalTimeText(saveData.totalTime);

    FirebaseSaveFieldPayload payload = new FirebaseSaveFieldPayload
    {
        unityGameSaveObject = saveData
    };

    string json = JsonUtility.ToJson(payload);

    FirebaseFirestore.UpdateDocument(
        "users",
        userId,
        json,
        gameObject.name,
        "OnSaveSuccess",
        "OnSaveError"
    );
}

public void OnSaveSuccess(string info)
{
    Debug.Log("Game saved to Firebase ✅");
}

public void OnSaveError(string error)
{
    Debug.LogError("Save failed ❌: " + error);
}


public void LoadGameFromFirebase(string userId)
{
    FirebaseFirestore.GetDocument(
        "users",
        userId,
        gameObject.name,
        "OnLoadSuccess",
        "OnLoadError"
    );
}


public void OnLoadSuccess(string json)
{
    if (string.IsNullOrEmpty(json) || json == "null")
    {
        Debug.Log("No save found, creating new save");
        CreateNewGame(true);
        return;
    }

    bool hasEmbeddedSaveField = json.Contains("\"unitySaveJson\"")
                                || json.Contains("\"unityGameSaveJson\"")
                                || json.Contains("\"unityGameSaveObject\"");

    if (!hasEmbeddedSaveField)
    {
        Debug.Log("No Unity save field found in Firebase document, creating new save");
        CreateNewGame(true);
        return;
    }

    EmbeddedSaveJsonPayload payload = JsonUtility.FromJson<EmbeddedSaveJsonPayload>(json);
    if (payload != null)
    {
        if (payload.unityGameSaveObject != null)
        {
            if (!HasMeaningfulProgress(payload.unityGameSaveObject))
            {
                Debug.Log("No meaningful Firebase save found, creating new save");
                CreateNewGame(true);
                return;
            }

            ApplyLoadedSaveData(payload.unityGameSaveObject);
            Debug.Log("Firebase load complete ✅");
            return;
        }

        if (!string.IsNullOrWhiteSpace(payload.unityGameSaveJson))
        {
            json = payload.unityGameSaveJson;
        }
        else if (!string.IsNullOrWhiteSpace(payload.unitySaveJson))
        {
            // Backward compatibility with the old field name.
            json = payload.unitySaveJson;
        }
        else
        {
            Debug.Log("Unity save fields exist but contain no data, creating new save");
            CreateNewGame(true);
            return;
        }
    }

    SaveData saveData = JsonUtility.FromJson<SaveData>(json);
    if (saveData == null || !HasMeaningfulProgress(saveData))
    {
        Debug.Log("No meaningful Firebase save found, creating new save");
        CreateNewGame(true);
        return;
    }

    ApplyLoadedSaveData(saveData);

    Debug.Log("Firebase load complete ✅");
}

private string BuildFirestoreDocumentUrl(string userId)
{
    string escapedUserId = UnityWebRequest.EscapeURL(userId);
    return $"https://firestore.googleapis.com/v1/projects/{firebaseProjectId}/databases/(default)/documents/users/{escapedUserId}?key={firebaseApiKey}";
}

private string BuildFirestoreDocumentUpdateUrl(string userId)
{
    string escapedUserId = UnityWebRequest.EscapeURL(userId);
    string escapedStringField = UnityWebRequest.EscapeURL(FirebaseSaveField);
    string escapedObjectField = UnityWebRequest.EscapeURL(FirebaseSaveObjectField);
    return $"https://firestore.googleapis.com/v1/projects/{firebaseProjectId}/databases/(default)/documents/users/{escapedUserId}?key={firebaseApiKey}&updateMask.fieldPaths={escapedStringField}&updateMask.fieldPaths={escapedObjectField}";
}

private IEnumerator SaveGameToFirebaseRest(string userId, SaveData saveData)
{
    if (string.IsNullOrWhiteSpace(userId))
    {
        Debug.LogWarning("Cannot save to Firebase in Editor because userId is empty.");
        yield break;
    }

    string saveJson = JsonUtility.ToJson(saveData, true);
    string firestoreObjectFields = BuildFirestoreMapFieldsFromJson(saveJson);
    string requestBody = "{\"fields\":{\"" + FirebaseSaveObjectField + "\":{\"mapValue\":{\"fields\":" + firestoreObjectFields + "}}}}";

    using (UnityWebRequest request = new UnityWebRequest(BuildFirestoreDocumentUpdateUrl(userId), "PATCH"))
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Editor Firebase REST save complete ✅");
        }
        else
        {
            Debug.LogError("Editor Firebase REST save failed: " + request.error + " | " + request.downloadHandler.text);
        }
    }
}

private string BuildFirestoreMapFieldsFromJson(string json)
{
    if (string.IsNullOrWhiteSpace(json))
    {
        return "{}";
    }

    try
    {
        fsData root = fsJsonParser.Parse(json);
        if (!root.IsDictionary)
        {
            return "{}";
        }

        return BuildFirestoreFieldsJson(root.AsDictionary);
    }
    catch (Exception exception)
    {
        Debug.LogWarning("Failed to build Firestore mapValue JSON. Falling back to string-only shape in this request. " + exception.Message);
        return "{}";
    }
}

private string BuildFirestoreFieldsJson(Dictionary<string, fsData> dictionary)
{
    var builder = new StringBuilder();
    builder.Append("{");

    bool isFirst = true;
    foreach (KeyValuePair<string, fsData> pair in dictionary)
    {
        if (!isFirst)
        {
            builder.Append(',');
        }

        isFirst = false;
        builder.Append('"').Append(EscapeJsonString(pair.Key)).Append("\":");
        AppendFirestoreValueJson(pair.Value, builder);
    }

    builder.Append("}");
    return builder.ToString();
}

private void AppendFirestoreValueJson(fsData value, StringBuilder builder)
{
    if (value == null || value.IsNull)
    {
        builder.Append("{\"nullValue\":null}");
        return;
    }

    if (value.IsBool)
    {
        builder.Append("{\"booleanValue\":").Append(value.AsBool ? "true" : "false").Append("}");
        return;
    }

    if (value.IsString)
    {
        builder.Append("{\"stringValue\":\"").Append(EscapeJsonString(value.AsString)).Append("\"}");
        return;
    }

    if (value.IsInt64)
    {
        builder.Append("{\"integerValue\":\"").Append(value.AsInt64.ToString(CultureInfo.InvariantCulture)).Append("\"}");
        return;
    }

    if (value.IsDouble)
    {
        builder.Append("{\"doubleValue\":").Append(value.AsDouble.ToString("R", CultureInfo.InvariantCulture)).Append("}");
        return;
    }

    if (value.IsDictionary)
    {
        builder.Append("{\"mapValue\":{\"fields\":");
        builder.Append(BuildFirestoreFieldsJson(value.AsDictionary));
        builder.Append("}}");
        return;
    }

    if (value.IsList)
    {
        builder.Append("{\"arrayValue\":{\"values\":[");

        List<fsData> list = value.AsList;
        for (int index = 0; index < list.Count; index++)
        {
            if (index > 0)
            {
                builder.Append(',');
            }

            AppendFirestoreValueJson(list[index], builder);
        }

        builder.Append("]}}");
        return;
    }

    builder.Append("{\"nullValue\":null}");
}

private string EscapeJsonString(string value)
{
    if (string.IsNullOrEmpty(value))
    {
        return string.Empty;
    }

    return value
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\n", "\\n")
        .Replace("\r", "\\r")
        .Replace("\t", "\\t");
}

private IEnumerator LoadGameFromFirebaseRest(string userId)
{
    using (UnityWebRequest request = UnityWebRequest.Get(BuildFirestoreDocumentUrl(userId)))
    {
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Editor Firebase REST load failed, falling back to local save. " + request.error);
            LoadGame();
            yield break;
        }

        SaveData saveData = ParseSaveDataFromFirestoreRestDocument(request.downloadHandler.text);
        if (saveData == null || !HasMeaningfulProgress(saveData))
        {
            Debug.Log("No Editor Firebase save found for this user. Creating new game data.");
            CreateNewGame(true);
            yield break;
        }

        ApplyLoadedSaveData(saveData);
        Debug.Log("Editor Firebase REST load complete ✅");
    }
}

private SaveData ParseSaveDataFromFirestoreRestDocument(string firestoreDocumentJson)
{
    if (string.IsNullOrWhiteSpace(firestoreDocumentJson))
    {
        return null;
    }

    try
    {
        fsData root = fsJsonParser.Parse(firestoreDocumentJson);
        if (!root.IsDictionary)
        {
            return null;
        }

        Dictionary<string, fsData> rootDictionary = root.AsDictionary;
        if (!rootDictionary.TryGetValue("fields", out fsData fieldsData) || !fieldsData.IsDictionary)
        {
            return null;
        }

        Dictionary<string, fsData> fields = fieldsData.AsDictionary;

        if (fields.TryGetValue(FirebaseSaveObjectField, out fsData objectField))
        {
            fsData plainObjectData = ConvertFirestoreValueToPlainJsonData(objectField);
            if (plainObjectData != null && plainObjectData.IsDictionary)
            {
                string plainObjectJson = fsJsonPrinter.CompressedJson(plainObjectData);
                SaveData objectSaveData = JsonUtility.FromJson<SaveData>(plainObjectJson);
                if (objectSaveData != null)
                {
                    return objectSaveData;
                }
            }
        }

        // Backward compatibility for legacy string saves.
        if (fields.TryGetValue(FirebaseSaveField, out fsData stringField))
        {
            fsData plainStringData = ConvertFirestoreValueToPlainJsonData(stringField);
            if (plainStringData != null && plainStringData.IsString)
            {
                string legacyJson = plainStringData.AsString;
                if (!string.IsNullOrWhiteSpace(legacyJson))
                {
                    SaveData stringSaveData = JsonUtility.FromJson<SaveData>(legacyJson);
                    if (stringSaveData != null)
                    {
                        return stringSaveData;
                    }
                }
            }
        }

        return null;
    }
    catch (Exception exception)
    {
        Debug.LogWarning("Failed to parse Firestore REST document. " + exception.Message);
        return null;
    }
}

private fsData ConvertFirestoreValueToPlainJsonData(fsData firestoreValue)
{
    if (firestoreValue == null || !firestoreValue.IsDictionary)
    {
        return fsData.Null;
    }

    Dictionary<string, fsData> valueDictionary = firestoreValue.AsDictionary;

    if (valueDictionary.TryGetValue("nullValue", out _))
    {
        return fsData.Null;
    }

    if (valueDictionary.TryGetValue("booleanValue", out fsData booleanValue))
    {
        if (booleanValue.IsBool)
        {
            return new fsData(booleanValue.AsBool);
        }
    }

    if (valueDictionary.TryGetValue("stringValue", out fsData stringValue))
    {
        if (stringValue.IsString)
        {
            return new fsData(stringValue.AsString);
        }
    }

    if (valueDictionary.TryGetValue("integerValue", out fsData integerValue))
    {
        if (integerValue.IsInt64)
        {
            return new fsData(integerValue.AsInt64);
        }

        if (integerValue.IsString && long.TryParse(integerValue.AsString, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsedInt))
        {
            return new fsData(parsedInt);
        }
    }

    if (valueDictionary.TryGetValue("doubleValue", out fsData doubleValue))
    {
        if (doubleValue.IsDouble)
        {
            return new fsData(doubleValue.AsDouble);
        }

        if (doubleValue.IsInt64)
        {
            return new fsData((double)doubleValue.AsInt64);
        }

        if (doubleValue.IsString && double.TryParse(doubleValue.AsString, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedDouble))
        {
            return new fsData(parsedDouble);
        }
    }

    if (valueDictionary.TryGetValue("mapValue", out fsData mapValue) && mapValue.IsDictionary)
    {
        Dictionary<string, fsData> plainMap = new Dictionary<string, fsData>();
        Dictionary<string, fsData> mapDictionary = mapValue.AsDictionary;

        if (mapDictionary.TryGetValue("fields", out fsData mapFields) && mapFields.IsDictionary)
        {
            Dictionary<string, fsData> nestedFields = mapFields.AsDictionary;
            foreach (KeyValuePair<string, fsData> pair in nestedFields)
            {
                plainMap[pair.Key] = ConvertFirestoreValueToPlainJsonData(pair.Value);
            }
        }

        return new fsData(plainMap);
    }

    if (valueDictionary.TryGetValue("arrayValue", out fsData arrayValue) && arrayValue.IsDictionary)
    {
        List<fsData> plainList = new List<fsData>();
        Dictionary<string, fsData> arrayDictionary = arrayValue.AsDictionary;

        if (arrayDictionary.TryGetValue("values", out fsData arrayItems) && arrayItems.IsList)
        {
            List<fsData> values = arrayItems.AsList;
            for (int index = 0; index < values.Count; index++)
            {
                plainList.Add(ConvertFirestoreValueToPlainJsonData(values[index]));
            }
        }

        return new fsData(plainList);
    }

    return fsData.Null;
}

private void RemoveCashBagIfQuestExists(List<QuestProgress> questData, string questId)
{
    if (questData == null || string.IsNullOrWhiteSpace(questId))
    {
        return;
    }

    bool questExists = questData.Any(q => q != null && q.QuestID == questId);
    if (!questExists)
    {
        return;
    }

    cashBag[] cashBags = FindObjectsByType<cashBag>(FindObjectsSortMode.None);
    for (int index = 0; index < cashBags.Length; index++)
    {
        if (cashBags[index] != null)
        {
            cashBags[index].ApplyPostTrapStationState();
            Destroy(cashBags[index].gameObject);
        }
    }
}


public void OnLoadError(string error)
{
    Debug.LogWarning("No Firebase save found, creating new game");
    CreateNewGame(true);
}




private void CreateNewGame(bool playIntro)
{
    SaveData freshSaveData = BuildFreshNewUserSaveData();
    ApplyLoadedSaveData(freshSaveData);
    PersistSaveData(freshSaveData);

    if (playIntro)
    {
        StartCoroutine(PlayNewUserIntroSequence());
    }

}

private IEnumerator PlayNewUserIntroSequence()
{
            BackgroundMusicManager.SetMuted(true);

    if (introVideoRawImage != null)
    {
        Color startColor = introVideoRawImage.color;
        startColor.a = 0f;
        introVideoRawImage.color = startColor;
    }


    introVideoFinished = false;
    VideoPlayer vp = vidPlayer.GetComponent<VideoPlayer>();
    if (vp != null)
    {
        vp.loopPointReached += OnIntroVideoFinished;
        vp.errorReceived += OnIntroVideoError;
    }

    vidPlayer.PlayVideo("intro.mp4");
    yield return StartCoroutine(FadeInIntroVideoRawImage());

    while (!introVideoFinished)
    {
        yield return null;
    }

    if (vp != null)
    {
        vp.loopPointReached -= OnIntroVideoFinished;
        vp.errorReceived -= OnIntroVideoError;
    }

    yield return StartCoroutine(FadeOutIntroVideoRawImage());
    PlayPostIntroTween();
}

private void StartFadeOutAndDisable(CanvasGroup canvasGroup)
{
    if (canvasGroup == null)
    {
        return;
    }

    if (isActiveAndEnabled && gameObject.activeInHierarchy)
    {
        StartCoroutine(FadeOutAndDisable(canvasGroup));
        return;
    }

    canvasGroup.alpha = 0f;
    canvasGroup.gameObject.SetActive(false);
}

IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup)
{
    yield return StartCoroutine(FadeOut(canvasGroup));
    canvasGroup.gameObject.SetActive(false);
}

IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        for (float a = 1; a>= 0 ; a -= Time.unscaledDeltaTime * 3)
        {
            canvasGroup.alpha = a;
          yield return null;

        }
           canvasGroup.alpha = 0f;
    }


private void PlayPostIntroTween()
{
    BackgroundMusicManager.SetMuted(false);
    guider.GetComponent<RectTransform>().anchoredPosition = new Vector2(291.62f, 27f);
    guider.SetActive(true);


    LeanTween.scale(guider, new Vector3(6, 6, 1), 0.6f)
        .setDelay(0.5f)
        .setEase(LeanTweenType.easeOutQuart)
        .setOnComplete(() =>
        {
            LeanTween.scale(chatbubble, new Vector3(1, 1, 1), 0.5f)
                .setEase(LeanTweenType.easeOutQuart)
                .setOnComplete(() =>
                {
                     guiderBot.StartDialogueRange(132, 132);
                });
        });
}

private IEnumerator FadeInIntroVideoRawImage()
{
    if (introVideoRawImage == null)
    {
        yield break;
    }

    float elapsed = 0f;
    Color currentColor = introVideoRawImage.color;
    float startingAlpha = currentColor.a;

    while (elapsed < introVideoFadeInDuration)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(elapsed / introVideoFadeInDuration);
        currentColor.a = Mathf.Lerp(startingAlpha, 1f, t);
        introVideoRawImage.color = currentColor;
        yield return null;
    }

    currentColor.a = 1f;
    introVideoRawImage.color = currentColor;
}

private IEnumerator FadeOutIntroVideoRawImage()
{
    guider.GetComponent<RectTransform>().anchoredPosition = new Vector2(291.62f, -134.2f);
    guider.SetActive(false);

    if (introVideoRawImage == null)
    {
        yield break;
    }

    float elapsed = 0f;
    Color currentColor = introVideoRawImage.color;
    float startingAlpha = currentColor.a;

    while (elapsed < introVideoFadeOutDuration)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(elapsed / introVideoFadeOutDuration);
        currentColor.a = Mathf.Lerp(startingAlpha, 0f, t);
        introVideoRawImage.color = currentColor;
        yield return null;
    }

    currentColor.a = 0f;
    introVideoRawImage.color = currentColor;
}

private void OnIntroVideoFinished(VideoPlayer source)
{
    introVideoFinished = true;
}

private void OnIntroVideoError(VideoPlayer source, string message)
{
    Debug.LogWarning("Intro video playback failed: " + message, this);
    introVideoFinished = true;
}

private SaveData BuildFreshNewUserSaveData()
{
    InventoryController ??= FindFirstObjectByType<InventoryController>();
    hotbarController ??= FindFirstObjectByType<HotBarController>();

    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
    Vector3 playerPosition = playerObject != null ? playerObject.transform.position : Vector3.zero;

    CinemachineConfiner confiner = FindFirstObjectByType<CinemachineConfiner>();
    string mapBoundaryName = string.Empty;
    if (confiner != null && confiner.m_BoundingShape2D != null)
    {
        mapBoundaryName = confiner.m_BoundingShape2D.gameObject.name;
    }

    List<ChestSaveData> chestData = GetAllChestStates();
    for (int index = 0; index < chestData.Count; index++)
    {
        chestData[index].isOpened = false;
    }

    List<LevelSaveData> levelData = new List<LevelSaveData>();
    if (LevelController.Instance != null)
    {
        List<LevelSaveData> runtimeLevelData = LevelController.Instance.GetAllLevelData();
        for (int index = 0; index < runtimeLevelData.Count; index++)
        {
            LevelSaveData source = runtimeLevelData[index];
            levelData.Add(new LevelSaveData
            {
                levelName = source != null ? source.levelName : string.Empty,
                timeSpent = 0f,
                isCompleted = false
            });
        }
    }

    return new SaveData
    {
        playerPosition = playerPosition,
        mapBoundary = mapBoundaryName,
        totalTime = 0f,
        inventorySaveData = new List<InventorySaveData>(),
        HotbarSaveData = new List<InventorySaveData>(),
        chestSaveData = chestData,
        questProgressData = new List<QuestProgress>(),
        levelData = levelData
    };
}

private void PersistSaveData(SaveData saveData)
{
    if (saveData == null)
    {
        return;
    }

    UpdateTotalTimeText(saveData.totalTime);

    if (useRemoteSave && !string.IsNullOrWhiteSpace(activeUserId))
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            SaveGameToFirebase(activeUserId, saveData);
        }
        else
        {
            StartCoroutine(SaveGameToFirebaseRest(activeUserId, saveData));
        }

        return;
    }

    string json = JsonUtility.ToJson(saveData, true);
    File.WriteAllText(saveLocation, json);
}



// Generic method for any component with ChestID and IsOpened
private List<ChestSaveData> GetChestState<T>() where T : MonoBehaviour, IChestable
{
    List<ChestSaveData> chestStates = new List<ChestSaveData>();
    T[] objects = FindObjectsByType<T>(FindObjectsSortMode.None);
    
    foreach (T obj in objects)
    {
        if (obj.ChestID == 0)
        {
            Debug.LogWarning($"Skipping {typeof(T).Name} with ChestID 0. Ensure chest IDs are initialized before saving.");
            continue;
        }

        ChestSaveData data = new ChestSaveData
        {
            chestID = obj.ChestID,
            isOpened = obj.IsOpened
        };
        chestStates.Add(data);
    }
    
    return chestStates;
}

private void LoadChestStates<T>(List<ChestSaveData> chestStates) where T : MonoBehaviour, IChestable
{
    T[] objects = FindObjectsByType<T>(FindObjectsSortMode.None);
    
    foreach (T obj in objects)
    {
        ChestSaveData data = chestStates.FirstOrDefault(c => c.chestID == obj.ChestID);
        if (data != null)
        {
            obj.SetOpened(data.isOpened);
        }
    }
}

private List<ChestSaveData> GetAllChestStates()
{
    var allChests = new List<ChestSaveData>();
    allChests.AddRange(GetChestState<Dynamite>());
    allChests.AddRange(GetChestState<computer>());
    allChests.AddRange(GetChestState<BrokenAccessComputer>());
    allChests.AddRange(GetChestState<CSRF_Computer>());
    allChests.AddRange(GetChestState<XSSComputer>());
    allChests.AddRange(GetChestState<JWT_Computer>());
    return allChests.GroupBy(c => c.chestID).Select(g => g.First()).ToList();
}
}

