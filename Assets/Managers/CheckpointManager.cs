using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using lfs = LevelFinishScript;
using cb = CheckpointBroadcast;
using vs = VoidScript;
using prm = PlayerResetManager;
using psm = PlayerStatsManager;
using bls = ButtonLoaderScript;
public class CheckpointManager : MonoBehaviour, IDataPersistence
{
    public static CheckpointManager Instance { get; private set; }

    // Each value in the array stores the stored checkpoint number of the player in a certain buildIndex - 1.
    // e.g. if checkpointNums[4] == 3 then the player is on checkpoint 3 for level 5;
    // p.s. checkpointNum of 0 means initialCheckpoint

    private readonly int[] checkpointNums = new int[bls.numLevels];

    private int currentLevelIndex;    
    
    private readonly Transform[] currentCheckpoints = new Transform[bls.numLevels];

    private readonly Transform[] initialCheckpoints = new Transform[bls.numLevels];
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There is already a CheckpointManager in this script.\n"+
            "Destroying current instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnLevelFinish.AddListener(ResetCheckpoint);
        EventManager.Instance.OnCheckpointHit.AddListener(HandleCheckpointHit);
        EventManager.Instance.OnPlayerHitVoid.AddListener(ResetPlayer);
        EventManager.Instance.OnPlayerHardReset.AddListener(ResetCheckpoint);
        SceneManager.sceneLoaded += InitializeCheckpoints;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnLevelFinish.RemoveListener(ResetCheckpoint);
        EventManager.Instance.OnCheckpointHit.RemoveListener(HandleCheckpointHit);
        EventManager.Instance.OnPlayerHitVoid.RemoveListener(ResetPlayer);
        EventManager.Instance.OnPlayerHardReset.RemoveListener(ResetCheckpoint);
        SceneManager.sceneLoaded -= InitializeCheckpoints;
    }

    private void ResetPlayer(Transform playerTransform) => prm.Instance.ResetChar(currentCheckpoints[currentLevelIndex - 1] != null ? currentCheckpoints[currentLevelIndex - 1] : initialCheckpoints[currentLevelIndex - 1]);
    
    public int GetCheckpointNum(int levelIndex) => checkpointNums[levelIndex - 1];

    public void SetCheckpointNum(int levelIndex, int newNum) => checkpointNums[levelIndex - 1] = newNum;

    public Transform GetCurrentCheckpointTransform() => currentCheckpoints[currentLevelIndex - 1];

    public Transform GetInitialCheckpointTransform(int levelIndex) => initialCheckpoints[levelIndex - 1];

    public void ChangeCurrentCheckpointTransform(Transform newCheckpoint)
    {
        if (currentCheckpoints[currentLevelIndex - 1] != null)
        {
            currentCheckpoints[currentLevelIndex - 1].position = newCheckpoint.position;
        }
        else
        {
            currentCheckpoints[currentLevelIndex - 1] = initialCheckpoints[currentLevelIndex - 1];
        }
    }

    public void ChangeCurrentCheckpointPosition(Vector3 newCheckpointPos)
    {
        if (currentCheckpoints[currentLevelIndex - 1] != null)
        {
            currentCheckpoints[currentLevelIndex - 1].position = newCheckpointPos;
        }
        else
        {
            currentCheckpoints[currentLevelIndex - 1] = initialCheckpoints[currentLevelIndex - 1];
        }
    }
    public void ResetCheckpoint(int levelIndex) => checkpointNums[levelIndex -1 ] = 0;

    private void HandleCheckpointHit(Collider hit)
    {
        SetCheckpointNum(currentLevelIndex, int.Parse(hit.gameObject.transform.parent.gameObject.name[^1] + ""));
        ChangeCurrentCheckpointTransform(hit.transform);
    }

    private void InitializeCheckpoints(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != psm.mainMenuIndex)
        {
            currentLevelIndex = scene.buildIndex;
            Debug.Log("Scene Loaded: " + currentLevelIndex);
            Transform initialCheckpoint = GameObject.FindGameObjectWithTag("InitialCheckpoint").transform;
            initialCheckpoints[currentLevelIndex-1] = initialCheckpoint;
            if (checkpointNums[currentLevelIndex-1] != 0)
            {
                currentCheckpoints[currentLevelIndex-1] = GameObject.FindGameObjectWithTag("Environment").transform.Find("Checkpoint"+checkpointNums[scene.buildIndex-1]).transform;
            }
            else
            {
                if (initialCheckpoints[currentLevelIndex-1] != null) 
                {
                    initialCheckpoints[currentLevelIndex-1] = initialCheckpoint;
                }
                currentCheckpoints[currentLevelIndex-1] = initialCheckpoint;
            }
            EventManager.Instance.OnCheckpointsInitialized?.Invoke();
        }

    }

    public void SaveData(ref GameData data)
    {
        for (int i = 0; i < data.currentCheckpoints.Length; i++)
        {
            data.currentCheckpoints[i] = currentCheckpoints[i];
        }
    }

    public void LoadData(GameData data)
    {
        Transform[] _tList = data.currentCheckpoints;
        for (int i = 0; i < _tList.Length; i++)
        {
            currentCheckpoints[i] = _tList[i];
        }
    }

}
