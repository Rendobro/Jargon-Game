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
    public static event Action OnPlayerReset;
    public static event Action<int> OnPlayerHardReset;
    [Header("Player Instance Data")]
    private GameObject newPlayer;
    private CharacterController ctrl;
    private Camera cam;
    private Transform worldSpawn;

    [Header("Cached Components")]
    [SerializeField] private GameObject playerPrefab;
    
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is already a PlayerResetManager in this script.\n"+
            "Destroying current instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        cm.OnCheckpointsInitialized += SetCurrentWorldSpawn;
        SceneManager.sceneLoaded += InstantiatePlayer;
        SceneManager.sceneUnloaded += DestroyPlayer;
    }

    private void OnDisable()
    {
        cm.OnCheckpointsInitialized -= SetCurrentWorldSpawn;
        SceneManager.sceneLoaded -= InstantiatePlayer;
        SceneManager.sceneUnloaded -= DestroyPlayer;
    }

    private void Update() => ResetChecks();

    public void ResetChar()
    {
        OnPlayerReset?.Invoke();

        ctrl.enabled = false;
        transform.SetPositionAndRotation(cm.Instance.GetCurrentCheckpointTransform().position, Quaternion.identity);
        ctrl.enabled = true;

        cam.transform.rotation = Quaternion.identity;
    }
    public void ResetChar(Transform spawnPoint)
    {
        Debug.Log("cam is null: " + (cam  == null));
        OnPlayerReset?.Invoke();
        
        ctrl.enabled = false;
        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        ctrl.enabled = true;

        cam.transform.rotation = spawnPoint.rotation;
    }
    public void ResetChar(Vector3 spawnPointPos)
    {
        OnPlayerReset?.Invoke();

        ctrl.enabled = false;
        transform.position = spawnPointPos;
        transform.localRotation = Quaternion.identity;
        ctrl.enabled = true;

        cam.transform.localRotation = Quaternion.identity;
    }

    public void HardResetChar()
    {
        Debug.Log("hrc worldSpawn is null" + (worldSpawn == null));
        OnPlayerHardReset?.Invoke(SceneManager.GetActiveScene().buildIndex);

        ctrl.enabled = false;
        transform.SetPositionAndRotation(worldSpawn.position, worldSpawn.rotation);
        ctrl.enabled = true;

        cam.transform.rotation = worldSpawn.rotation;
        cm.Instance.ChangeCurrentCheckpointPosition(worldSpawn.position);
    }

    private void ResetChecks()
    {
        if (Input.GetButtonDown("Reset")) ResetChar(transform);
        if (Input.GetButtonDown("HardReset")) HardResetChar();
    }

    private void SetCurrentWorldSpawn()
    {
        if (SceneManager.GetActiveScene().buildIndex != psm.mainMenuIndex)
        {
            worldSpawn = cm.Instance.GetInitialCheckpointTransform(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("worldSpawn after initialization: " + (worldSpawn.position));
        }
    }

    private void InstantiatePlayer(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != psm.mainMenuIndex)
        {
            Debug.Log($"GameObjectWithTagPlayer {GameObject.FindWithTag("Player")}");
            if (GameObject.FindWithTag("Player") == null)
            {
                Debug.Log($"post: GameObjectWithTagPlayer {GameObject.FindWithTag("Player")}");
                GameObject newPlayer = Instantiate(playerPrefab, cm.Instance.GetCurrentCheckpointTransform().position, Quaternion.identity);
                ctrl = newPlayer.GetComponent<CharacterController>();
                cam = newPlayer.GetComponentInChildren<Camera>();
            }
        }
    }

    private void DestroyPlayer(Scene _)
    {
        Destroy(newPlayer);
    }
}