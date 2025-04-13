using UnityEngine;
using System.Collections.Generic;

public class EditorManager : MonoBehaviour, IDataPersistence
{
    public static EditorManager Instance { get; private set; }

    [SerializeField] private int levelNum = 0;

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
        if (EventManager.Instance != null)
            EventManager.Instance.OnCreateNewLevel.AddListener(CreateNewLevel);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.OnCreateNewLevel.RemoveListener(CreateNewLevel);
    }

    private void Start()
    {
        InitializePrefabList();
    }

    [ContextMenu("Create New Level")]
    public void CreateNewLevel()
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
                }
                index++;
            }
        }
    }

    [ContextMenu("Load Level")]
    public void LoadLevel()
    {
        LoadLevel(levelNum);
    }

    public void LoadLevel(int levelID)
    {
        if (levelID < 0 || levelID >= playerLevels.Count)
        {
            Debug.LogError("levelID out of range");
            return;
        }

        foreach (ObjectData obj in FindObjectsByType<ObjectData>(FindObjectsSortMode.None))
        {
            obj.gameObject.SetActive(false);
            Destroy(obj.gameObject);
        }

        foreach (ObjectInfo oi in playerLevels[levelID].objectInfos)
        {
            ObjectData newObj = Instantiate(editorPrefabs[oi.objID], oi.position, oi.rotation);
            newObj.transform.localScale = oi.scale;
            newObj.gameObject.SetActive(true);

            // Activate all components
            foreach (var component in newObj.GetComponents<MonoBehaviour>())
            {
                component.enabled = true; // Enable all MonoBehaviour components
            }

            // Optionally enable other components like Colliders or Renderers
            foreach (var collider in newObj.GetComponents<Collider>())
            {
                collider.enabled = true;
            }

            foreach (var renderer in newObj.GetComponents<Renderer>())
            {
                renderer.enabled = true;
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
