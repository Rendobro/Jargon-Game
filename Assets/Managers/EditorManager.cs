using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EditorManager : MonoBehaviour, IDataPersistence
{
    public static EditorManager Instance { get; private set; }

    [SerializeField]
    public List<ObjectData> editorObjects = new();

    [SerializeField]
    public List<LevelData> playerLevels = new();

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
        LevelData level = new LevelData();
        level.levelID = playerLevels.Count;
    
        for (int i = 0; i < editorObjects.Count; i++)
        {
            ObjectInfo info = new ObjectInfo();
            info.objID = editorObjects[i].objID;
            info.position = editorObjects[i].transform.position;
            info.rotation = editorObjects[i].transform.rotation;
            info.scale = editorObjects[i].transform.localScale;
            level.objectInfos.Add(info);
            Debug.Log($"level 1 object info {i}: {level.objectInfos[i].objID}");
        }

        playerLevels.Add(level);
    }

    [ContextMenu("Initialize Prefab List")]
    private void InitializePrefabList()
    {
        ObjectData[] tempPrefabList = FindObjectsByType<ObjectData>(FindObjectsSortMode.None);
        if (editorObjects.Count < tempPrefabList.Length)
        {
            int index = 0;
            foreach (ObjectData obj in tempPrefabList)
            {
                obj.objID = index;
                editorObjects.Add(obj);
                Debug.Log($"editor object {index}: {editorObjects[index].objID}");
                index++;
            }
        }
    }

    public void LoadData(GameData data)
    {

    }

    public void SaveData(ref GameData data)
    {

    }
}
