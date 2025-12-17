using System;
using TMPro;
using UnityEngine;
using psm = PlayerStatsManager;
using lfs = LevelFinishScript;
using System.Collections.Generic;
using bls = ButtonLoaderScript;
using cm = CheckpointManager;
using UnityEngine.SceneManagement;

public class PlayerResetManager : MonoBehaviour
{
    public static PlayerResetManager Instance {get; private set;}
    [Header("Player Instance Data")]
    private GameObject currPlayer;
    private CharacterController ctrl;
    private Camera cam;
    private Transform worldSpawn;

    [Header("Cached Components")]
    [SerializeField] private GameObject playerPrefab;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There is already a PlayerResetManager in this script.\n"+
            "Destroying current instance.");
            Destroy(gameObject);
            return;
        }
        worldSpawn = transform;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnCheckpointsInitialized.AddListener(SetCurrentWorldSpawn);
        SceneManager.sceneLoaded += InstantiatePlayer;
        SceneManager.sceneUnloaded += DestroyPlayer;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnCheckpointsInitialized.RemoveListener(SetCurrentWorldSpawn);
        SceneManager.sceneLoaded -= InstantiatePlayer;
        SceneManager.sceneUnloaded -= DestroyPlayer;
    }

    private void Update() => ResetChecks();

    public void ResetChar()
    {
        EventManager.Instance.OnPlayerReset?.Invoke();

        ctrl.enabled = false;
        currPlayer.transform.SetPositionAndRotation(cm.Instance.GetCurrentCheckpointTransform().position, Quaternion.identity);
        ctrl.enabled = true;

        cam.transform.rotation = Quaternion.identity;
    }
    public void ResetChar(Transform spawnPoint)
    {
        Debug.Log("cam is null: " + (cam  == null));
        EventManager.Instance.OnPlayerReset?.Invoke();
        
        ctrl.enabled = false;
        currPlayer.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        ctrl.enabled = true;

        cam.transform.rotation = spawnPoint.rotation;
    }
    public void ResetChar(Vector3 spawnPointPos)
    {
        EventManager.Instance.OnPlayerReset?.Invoke();

        ctrl.enabled = false;
        currPlayer.transform.position = spawnPointPos;
        currPlayer.transform.localRotation = Quaternion.identity;
        ctrl.enabled = true;

        cam.transform.localRotation = Quaternion.identity;
    }

    public void HardResetChar()
    {
        Debug.Log("worldSpawn when hard resetting" + worldSpawn.position);
        EventManager.Instance.OnPlayerHardReset?.Invoke(SceneManager.GetActiveScene().buildIndex);

        ctrl.enabled = false;
        currPlayer.transform.SetPositionAndRotation(worldSpawn.position, worldSpawn.rotation);
        ctrl.enabled = true;

        cam.transform.rotation = worldSpawn.rotation;
        cm.Instance.ChangeCurrentCheckpointPosition(worldSpawn.position);
    }

    private void ResetChecks()
    {
        //Debug.Log("Current checkpoint transform" + cm.Instance.GetCurrentCheckpointTransform().position);
        if (Input.GetButtonDown("Reset")) ResetChar(cm.Instance.GetCurrentCheckpointTransform());
        if (Input.GetButtonDown("HardReset")) HardResetChar();
    }

    private void SetCurrentWorldSpawn()
    {
        if (SceneManager.GetActiveScene().buildIndex != psm.mainMenuIndex)
        {
            Transform initialCheckpoint = cm.Instance.GetInitialCheckpointTransform(SceneManager.GetActiveScene().buildIndex);
            worldSpawn.SetPositionAndRotation(initialCheckpoint.position, initialCheckpoint.rotation);
            Debug.Log("worldSpawn after initialization: " + worldSpawn.position);
        }
    }

    private void InstantiatePlayer(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != psm.mainMenuIndex)
        {
            Debug.Log($"GameObjectWithTagPlayer {GameObject.FindWithTag("Player").transform.position}");
            currPlayer = GameObject.FindWithTag("Player");
            if (currPlayer == null)
            {
                Debug.Log($"Scene index null {scene.buildIndex}");
                currPlayer = Instantiate(playerPrefab, cm.Instance.GetCurrentCheckpointTransform().position, Quaternion.identity);
                ctrl = currPlayer.GetComponent<CharacterController>();
                cam = currPlayer.GetComponentInChildren<Camera>();
            }
            else
            {
                Debug.Log($"Scene index not null{scene.buildIndex}");
                GameObject newPlayer = currPlayer;
                ctrl = newPlayer.GetComponent<CharacterController>();
                cam = newPlayer.GetComponentInChildren<Camera>();
            }
        }
    }

    private void DestroyPlayer(Scene _)
    {
        Destroy(currPlayer);
    }
}