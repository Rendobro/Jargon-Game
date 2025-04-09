using UnityEngine;
using System.Collections.Generic;

public class EditorManager : MonoBehaviour, IDataPersistence
{
    public static EditorManager Instance { get; private set; }

    [SerializeField] private List<ObjectData> editorPrefabs = new();

    [SerializeField] private List<LevelData> playerLevels = new();

    private ObjectData[] allCurrentSceneObjects;

    private void Awake()
    {
        if (Instance != null)
        {
        Debug.LogError("Already an EditorManager Instance in this scene.\nDestroying current instance.");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.Instance.OnCreateNewLevel.AddListener(CreateNewLevel);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnCreateNewLevel.RemoveListener(CreateNewLevel);
    }

    private void Start()
    {
        InitializePrefabList();
    }

    [ContextMenu("Create New Level")]
    private void CreateNewLevel()
    {
        LevelData level = new() { levelID = playerLevels.Count };

        for (int i = 0; i < allCurrentSceneObjects.Length; i++)
        {
            ObjectInfo info = new()
            {
                objID = allCurrentSceneObjects[i].objID,
                position = allCurrentSceneObjects[i].transform.position,
                rotation = allCurrentSceneObjects[i].transform.rotation,
                scale = allCurrentSceneObjects[i].transform.localScale
            };
            level.objectInfos.Add(info);
            Debug.Log($"level {level.levelID} object info {i}: {level.objectInfos[i].objID}");
        }

        playerLevels.Add(level);
    }

    [ContextMenu("Initialize Prefab List")]
    private void InitializePrefabList()
    {
        allCurrentSceneObjects = FindObjectsByType<ObjectData>(FindObjectsSortMode.None);
        if (editorPrefabs.Count < allCurrentSceneObjects.Length)
        {
            int index = 0;
            foreach (ObjectData obj in allCurrentSceneObjects)
            {
                if (!editorPrefabs.Exists(thing => thing.objID == obj.objID))
                {
                    obj.objID = index;
                    editorPrefabs.Add(obj);
                    Debug.Log($"editor object {index}: {editorPrefabs[index].objID}");
                }
                index++;
            }
        }
    }

    public void LoadData(GameData data)
    {
        playerLevels = new List<LevelData>(data.playerLevels);
    }

    public void SaveData(ref GameData data)
    {
        data.playerLevels = new List<LevelData>(playerLevels);
    }
}
