using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using prm = PlayerResetManager;
using dpm = DataPersistenceManager;
using lfs = LevelFinishScript;
using System;

public class PlayerStatsManager : MonoBehaviour, IDataPersistence
{
    public static PlayerStatsManager Instance {get; private set;}
    public const int mainMenuIndex = 0;
    [SerializeField] private PlayerStatsContainer playerStats;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There is already a PlayerStatsManager in this script.\n"+
            "Destroying current instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnLevelFinish.AddListener(GoToMainMenu);
        SceneManager.sceneUnloaded += SetNewRecentLevel;
    }
    private void OnDisable()
    {
        EventManager.Instance.OnLevelFinish.RemoveListener(GoToMainMenu);
        SceneManager.sceneUnloaded -= SetNewRecentLevel;
    }
    private void KillPlayer()
    {
        Debug.Log("Player Dead!");
    }
    private void GoToMainMenu(int _) => SceneManager.LoadScene(mainMenuIndex);

    public void SetRecentLevelIndex(int newIndex) => playerStats.recentLevelIndex = newIndex;

    public int GetRecentLevelIndex() => playerStats.recentLevelIndex;

    public void SetLastUnlockedLevelIndex(int newIndex) => playerStats.lastLevelUnlocked = newIndex;

    public int GetLastUnlockedLevelIndex() => playerStats.lastLevelUnlocked;

    public void SetHighscore(int levelIndex, float value) => playerStats.playerHighscores[levelIndex-1] = value;

    public float[] GetHighscores() => playerStats.playerHighscores;

    public float GetHighscore(int levelIndex) => playerStats.playerHighscores[levelIndex-1];

    public void SetGravity(float newGravity) => playerStats.gravity = newGravity;

    public float GetGravity() => playerStats.gravity;

    public void SetSensitivity(float newSensitivity) => playerStats.sensitivity = newSensitivity;

    public float GetSensitivity() => playerStats.sensitivity;

    public void SetMenuTransitionDuration(float newMenuTransitionDuration) => playerStats.menuTransitionDuration = newMenuTransitionDuration;

    public float GetMenuTransitionDuration() => playerStats.menuTransitionDuration;

    private void SetNewRecentLevel(Scene scene)
    {
        if (scene.buildIndex >= 1) SetRecentLevelIndex(scene.buildIndex);
    }

    public void SaveData(ref GameData data)
    {
        if (playerStats.recentLevelIndex <= 0) playerStats.recentLevelIndex = 1;
        data.recentLevelIndex = playerStats.recentLevelIndex;
        if (playerStats.lastLevelUnlocked <= 0) playerStats.lastLevelUnlocked = 1;
        data.lastLevelUnlocked = playerStats.lastLevelUnlocked;
        data.gravity = playerStats.gravity;
        data.sensitivity = playerStats.sensitivity;
        data.menuTransitionDuration = playerStats.menuTransitionDuration;

        if (data.highscores.Length == playerStats.playerHighscores.Length)
        {
            Array.Copy(playerStats.playerHighscores, data.highscores, ButtonLoaderScript.numLevels);
        }
        else
        {
            Debug.LogError($"When Saving: Highscores array length mismatch.\nSizes of the following arrays:\ndata.highscores: {data.highscores.Length}\nplayerHighscores: {playerStats.playerHighscores.Length}");
        }
    }

    public void LoadData(GameData data)
    {
        playerStats.recentLevelIndex = data.recentLevelIndex;
        if (playerStats.recentLevelIndex <= 0) playerStats.recentLevelIndex = 1;
        playerStats.lastLevelUnlocked = data.lastLevelUnlocked;
        if (playerStats.lastLevelUnlocked <= 0) playerStats.lastLevelUnlocked = 1;
        playerStats.gravity = data.gravity;
        playerStats.sensitivity = data.sensitivity;
        playerStats.menuTransitionDuration = data.menuTransitionDuration;
        MouseScript.SetSensitivity(playerStats.sensitivity);
        MovementScript.SetGravity(playerStats.gravity);
        if (data.highscores.Length == playerStats.playerHighscores.Length)
        {
            Array.Copy(data.highscores, playerStats.playerHighscores, ButtonLoaderScript.numLevels);
        }
        else
        {
            Debug.LogError($"When Loading: Highscores array length mismatch.\nSizes of the following arrays:\ndata.highscores: {data.highscores.Length}\nplayerHighscores: {playerStats.playerHighscores.Length}");
        }
    }
}
