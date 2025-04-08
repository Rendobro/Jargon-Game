using System;
using TMPro;
using UnityEngine;
using psm = PlayerStatsManager;
using lfs = LevelFinishScript;
using prm = PlayerResetManager;
using bls = ButtonLoaderScript;
using UnityEngine.SceneManagement;
public class TimerManager : MonoBehaviour, IDataPersistence
{
    public static TimerManager Instance {get; private set;}

    // public static event Action OnOneMinutePassed;

    private TextMeshProUGUI timerText;
    private bool timerPaused = false;
    private bool timerToggledOn = true;

    [Header("Persisting Data")]
    private readonly float[] timers = new float[bls.numLevels];

    [Header("Current Level Index")]
    private int currentLevelIndex;
    
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is already a TimerManager in this script.\n"+
            "Destroying current instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnLevelFinish.AddListener(PauseTimer);
        EventManager.Instance.OnPlayerHardReset.AddListener(ResetTimerValue);
        SceneManager.sceneLoaded += SetTimerText;
        SceneManager.sceneLoaded += SetLevelIndex;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnLevelFinish.RemoveListener(PauseTimer);
        EventManager.Instance.OnPlayerHardReset.RemoveListener(ResetTimerValue);
        SceneManager.sceneLoaded -= SetTimerText;
        SceneManager.sceneLoaded -= SetLevelIndex;
    }


    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != psm.mainMenuIndex) ManageTimer();
    }
    public void ToggleTimerVisibility()
    {
        timerToggledOn = !timerToggledOn;
        RectTransform _tTimer = timerText.rectTransform;
        _tTimer.anchoredPosition = _tTimer.anchoredPosition.Equals(new Vector2(0, 625)) ? new Vector3(0, 625 * (timerToggledOn ? 1 : 2), 0) : new Vector2(0, 625);
    }
    public void PauseTimer(int _) => timerPaused = true;

    public void PauseTimer() => timerPaused = true;

    public void UnpauseTimer() => timerPaused = false;

    private void ManageTimer()
    {
        if (!timerPaused)
        {
            timers[currentLevelIndex > 0 ? currentLevelIndex - 1 : 0] += Time.deltaTime;
            if (timerToggledOn) ChangeTimerText();
        }
        else if (timerToggledOn) ChangeTimerText();
    }
    public float GetTimerValue(int levelIndex) => timers[levelIndex-1];

    public void ResetTimerValue(int levelIndex) => timers[levelIndex-1] = 0;

    public string FormatTimer(float seconds)
    {
        int numHours = (int)Mathf.Floor(seconds / 3600);

        string additionalSegment = "";
        if (numHours > 0) additionalSegment = numHours.ToString("D2") + ":";
        
        string formattedTimer = additionalSegment
        + ((int)Mathf.Floor((seconds - numHours * 3600f) / 60f)).ToString("D2") + ":"
        + ((int)(seconds % 60f)).ToString("D2") + ":"
        + ((int)(seconds % 1f * 1000)).ToString("D3");
        return formattedTimer;
    }

    private void ChangeTimerText()
    {
        if (timerText != null) timerText.text = FormatTimer(timers[currentLevelIndex-1]);
    }

    private void SetTimerText(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != psm.mainMenuIndex) timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
    }

    private void SetLevelIndex(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != psm.mainMenuIndex) currentLevelIndex = scene.buildIndex;
    }

    public void SaveData(ref GameData data) 
    {
        for (int i = 0; i < data.ongoingLevelTimers.Length; i++)
        {
            data.ongoingLevelTimers[i] = timers[i];
        }
    }

    public void LoadData(GameData data) 
    {
        float[] _timerList = data.ongoingLevelTimers;
        for (int i = 0; i < _timerList.Length; i++)
        {
            timers[i] = _timerList[i];
        }
    }
}
