using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EditorManager : MonoBehaviour, IDataPersistence
{
    public static EditorManager Instance { get; private set; }

    [SerializeField] private int levelNum = 0;

    [SerializeField] private EditorPrefabsContainer editorPrefabsContainer;
    private List<ObjectData> editorPrefabs;

    [SerializeField] private List<LevelData> playerLevels = new();

    private ObjectData[] allCurrentSceneObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
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
        InitializeLists();
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
    private void InitializeLists()
    {
        editorPrefabs = editorPrefabsContainer.editorPrefabs;
        Debug.Log("Editor Prefabs: \n"+editorPrefabs.ToLineSeparatedString());
        allCurrentSceneObjects = FindObjectsByType<ObjectData>(FindObjectsSortMode.None);
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
            ObjectData newObj = Instantiate(editorPrefabs[oi.objID], oi.position, oi.rotation, GameObject.FindGameObjectWithTag("ObjectContainer").transform);
            newObj.transform.localScale = oi.scale;
            newObj.gameObject.SetActive(true);

            foreach (var component in newObj.GetComponents<MonoBehaviour>()) component.enabled = true;

            foreach (Collider collider in newObj.GetComponents<Collider>()) collider.enabled = true;

            foreach (Renderer renderer in newObj.GetComponents<Renderer>()) renderer.enabled = true;

            foreach (Outline outline in newObj.GetComponents<Outline>())
            {
                outline.OutlineColor = SelectionScript.GetHoverColor();
                outline.OutlineMode = Outline.Mode.OutlineHidden;
            }
        }
    }

    private void ConstructObjectDataWith(ref ObjectData newObjData, ObjectData oldObjData)
    {
        newObjData.position = oldObjData.position;
        newObjData.scale = oldObjData.scale;
        newObjData.rotation = oldObjData.rotation;
        newObjData.objID = oldObjData.objID;
        newObjData.IsSelected = oldObjData.IsSelected;
    }

    public void SaveData(ref GameData data)
    {
        data.playerLevels = new List<LevelData>(playerLevels);
        data.editorPrefabs = editorPrefabs;
    }

    public void LoadData(GameData data)
    {
        playerLevels = new List<LevelData>(data.playerLevels);
        editorPrefabs = data.editorPrefabs;
    }
}
