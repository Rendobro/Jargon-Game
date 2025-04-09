using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using prm = PlayerResetManager;
using dpm = DataPersistenceManager;
using mvs = MovementScript;
using mos = MouseScript;
using lfs = LevelFinishScript;
using bls = ButtonLoaderScript;
using System;

public class PlayerStatsManager : MonoBehaviour, IDataPersistence
{
    public static PlayerStatsManager Instance {get; private set;}
    public const int mainMenuIndex = 0;
    private readonly float[] playerHighscores = new float[bls.numLevels];
    private float gravity;
    private float sensitivity;
    private float editorSensitivity;
    private float menuTransitionDuration;
    private int recentLevelIndex;
    private int lastLevelUnlocked;
    private void Awake()
    {
        if (Instance != null)
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

    public void SetRecentLevelIndex(int newIndex) => recentLevelIndex = newIndex;

    public int GetRecentLevelIndex() => recentLevelIndex;

    public void SetLastUnlockedLevelIndex(int newIndex) => lastLevelUnlocked = newIndex;

    public int GetLastUnlockedLevelIndex() => lastLevelUnlocked;

    public void SetHighscore(int levelIndex, float value) => playerHighscores[levelIndex-1] = value;

    public float[] GetHighscores() => playerHighscores;

    public float GetHighscore(int levelIndex) => playerHighscores[levelIndex-1];

    public void SetGravity(float newGravity) => gravity = newGravity;

    public float GetGravity() => gravity;

    public void SetSensitivity(float newSensitivity) => sensitivity = newSensitivity;

    public float GetSensitivity() => sensitivity;

    public void SetMenuTransitionDuration(float newMenuTransitionDuration) => menuTransitionDuration = newMenuTransitionDuration;

    public float GetMenuTransitionDuration() => menuTransitionDuration;

    private void SetNewRecentLevel(Scene scene)
    {
        if (scene.buildIndex >= 1) SetRecentLevelIndex(scene.buildIndex);
    }

    public void SaveData(ref GameData data)
    {
        if (recentLevelIndex <= 0) recentLevelIndex = 1;
        data.recentLevelIndex = recentLevelIndex;
        if (lastLevelUnlocked <= 0) lastLevelUnlocked = 1;
        data.lastLevelUnlocked = lastLevelUnlocked;
        data.gravity = gravity;
        data.sensitivity = sensitivity;
        data.menuTransitionDuration = menuTransitionDuration;

        if (data.highscores.Length == playerHighscores.Length)
        {
            Array.Copy(playerHighscores, data.highscores, bls.numLevels);
        }
        else
        {
            Debug.LogError($"When Saving: Highscores array length mismatch.\nSizes of the following arrays:\ndata.highscores: {data.highscores.Length}\nplayerHighscores: {playerHighscores.Length}");
        }
    }

    public void LoadData(GameData data)
    {
        recentLevelIndex = data.recentLevelIndex;
        if (recentLevelIndex <= 0) recentLevelIndex = 1;
        lastLevelUnlocked = data.lastLevelUnlocked;
        if (lastLevelUnlocked <= 0) lastLevelUnlocked = 1;
        gravity = data.gravity;
        sensitivity = data.sensitivity;
        menuTransitionDuration = data.menuTransitionDuration;
        mos.SetSensitivity(sensitivity);
        mvs.SetGravity(gravity);
        if (data.highscores.Length == playerHighscores.Length)
        {
            Array.Copy(data.highscores, playerHighscores, bls.numLevels);
        }
        else
        {
            Debug.LogError($"When Loading: Highscores array length mismatch.\nSizes of the following arrays:\ndata.highscores: {data.highscores.Length}\nplayerHighscores: {playerHighscores.Length}");
        }
    }
}
