using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    public static event Action OnNewGameStarted;
    [SerializeField] private string fileName = "";
    private FileDataHandler dataHandler;
    public static DataPersistenceManager Instance {get; private set;}
    private List<IDataPersistence> dataPersistenceObjects;
    private GameData gameData;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is already a DataPersistenceManager in this script.\n"+
            "Destroying current instance.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath,fileName);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    [ContextMenu("New Game Start")]
    public void NewGame()
    {
        this.gameData = new GameData();
        OnNewGameStarted?.Invoke();
    }

    private void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (!HasGameData())
        {
            Debug.Log("No data found. Need to start a new game first.");
            return;
        }

        foreach (IDataPersistence dpo in dataPersistenceObjects)
        {
            Debug.Log($"Loaded Script: {dpo.GetType().Name} editorSensitivity: {gameData.editorSensitivity}");
            dpo.LoadData(gameData);

        }

        Debug.Log($"gameData loaded: {gameData}");
    }

    private void SaveGame()
    {
        foreach (IDataPersistence dpo in dataPersistenceObjects)
        {
            Debug.Log($"Saved Script: {dpo.GetType().Name} editorSensitivity: {gameData.editorSensitivity}");
            dpo.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
        Debug.Log($"gameData saved: {gameData}");
    }

    private void OnApplicationQuit() 
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return new List<IDataPersistence>(FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>());
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
