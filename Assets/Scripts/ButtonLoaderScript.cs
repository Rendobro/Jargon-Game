using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using psm = PlayerStatsManager;
using prm = PlayerResetManager;
using cm = CheckpointManager;
using tm = TimerManager;
using dpm = DataPersistenceManager;
public class ButtonLoaderScript : MonoBehaviour
{
    [Header("Initialized Values")]
    public const ushort numLevels = 25;
    private const float standardGravity = 30f;
    private float menuTransitionDuration;

    [Header("Menu Instance Data")]
    [SerializeField] private Toggle gravityToggle;
    [SerializeField] private GameObject mainMenuBoard;
    private TextMeshProUGUI playText;
    private static int currentMenuIndex_X;
    private static int currentMenuIndex_Y;
    private GameObject[][] menus;
    private bool[][] readyToMove;

    private void Update()
    {
        // if (dpm.Instance != null && !dpm.Instance.HasGameData()) 
        // {
        //     dpm.Instance.NewGame();
        // }
    }
    private void OnEnable()
    {
        if (dpm.Instance != null && !dpm.Instance.HasGameData()) 
        {
            dpm.Instance.NewGame();
        }
        SceneManager.sceneLoaded += LoadCheckpointData;
        SceneManager.sceneLoaded += ChangePlayText;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadCheckpointData;
        SceneManager.sceneLoaded -= ChangePlayText;
    }
    private void Start()
    {
        InitializeMenus();
        InitializeHighScores();
        InitializeSettings();
    }
    public void BringSettingsMenuLeft()
    {
        bool incrementOkay = true;
        for (int i = 0; i < menus.Length; i++)
        {
            for (int j = 0; j < menus[i].Length; j++)
            {
                if (readyToMove[i][j])
                {
                    Transform _t = menus[i][j].transform;
                    readyToMove[i][j] = false;
                    Transform[] transforms = menus[i][j].GetComponentsInChildren<Transform>();
                    Transform mlp = transforms.FirstOrDefault(t => t.gameObject.name.Equals("MenuLeftPosition" + (i + 1) + (j + 1)));

                    StartCoroutine(SlowMovePosition(_t.position, mlp.position, menuTransitionDuration, (i, j)));
                }
                else
                {
                    incrementOkay = false;
                }
            }
        }
        if (incrementOkay)
            currentMenuIndex_X++;
    }
    public void BringSettingsMenuUp()
    {
        bool incrementOkay = true;
        for (int i = 0; i < menus.Length; i++)
        {
            for (int j = 0; j < menus[i].Length && menus[i][j] != null; j++)
            {
                if (readyToMove[i][j])
                {
                    Transform _t = menus[i][j].transform;
                    readyToMove[i][j] = false;
                    Transform[] transforms = menus[i][j].GetComponentsInChildren<Transform>();
                    Transform mup = transforms.FirstOrDefault(t => t.gameObject.name.Equals("MenuUpPosition" + (i + 1) + (j + 1)));

                    StartCoroutine(SlowMovePosition(_t.position, mup.position, menuTransitionDuration, (i, j)));
                }
                else
                {
                    incrementOkay = false;
                }
            }
        }
        if (incrementOkay)
            currentMenuIndex_Y++;
    }
    public void BringSettingsMenuDown()
    {
        bool incrementOkay = true;
        for (int i = 0; i < menus.Length; i++)
        {
            for (int j = 0; j < menus[i].Length; j++)
            {
                if (readyToMove[i][j])
                {
                    Transform _t = menus[i][j].transform;
                    readyToMove[i][j] = false;
                    Transform[] transforms = menus[i][j].GetComponentsInChildren<Transform>();
                    Transform mdp = transforms.FirstOrDefault(t => t.gameObject.name.Equals("MenuDownPosition" + (i + 1) + (j + 1)));

                    StartCoroutine(SlowMovePosition(_t.position, mdp.position, menuTransitionDuration, (i, j)));
                }
                else
                {
                    incrementOkay = false;
                }
            }
        }
        if (incrementOkay)
            currentMenuIndex_Y--;
    }
    public void BringSettingsMenuRight()
    {
        bool incrementOkay = true;
        for (int i = 0; i < menus.Length; i++)
        {
            for (int j = 0; j < menus[i].Length; j++)
            {
                if (readyToMove[i][j])
                {
                    Transform _t = menus[i][j].transform;
                    readyToMove[i][j] = false;
                    Transform[] transforms = menus[i][j].GetComponentsInChildren<Transform>();
                    Transform mrp = transforms.FirstOrDefault(t => t.gameObject.name.Equals("MenuRightPosition" + (i + 1) + (j + 1)));

                    StartCoroutine(SlowMovePosition(_t.position, mrp.position, menuTransitionDuration, (i, j)));
                }
                else
                {
                    incrementOkay = false;
                }
            }
        }
        if (incrementOkay)
            currentMenuIndex_X--;
    }
    public void LoadSpecificLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
    public void OnPlayButtonClicked()
    {
        Debug.Log("Man, that play button be getting pressed bruh");
        MouseScript.UnlockCursor();
        SceneManager.LoadScene(psm.Instance.GetRecentLevelIndex());
    }
    private void ChangePlayText(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != psm.mainMenuIndex)
        {
            if (playText != null)
            {
                playText.text = "Continue Playing!";
                playText.fontSize = 20;
            }
            else
            {
                Debug.LogError("playText is not assigned.");
            }
        }
    }
    private void LoadCheckpointData(Scene scene, LoadSceneMode mode)
    {
        // if the loaded scene is the scene where the player has last left off
        if (scene.buildIndex != psm.mainMenuIndex)
        {
            int checkpointNum = cm.Instance.GetCheckpointNum(scene.buildIndex);

            // if the player has never played this level
            if (checkpointNum < 1)
            {
                prm.Instance.HardResetChar();
                tm.Instance.ResetTimerValue(scene.buildIndex);
            }
            else
            {
                // Resets player to their saved checkpoint's position, including no checkpoints
                Transform playerSavedTransform = GameObject.FindGameObjectWithTag("Environment").transform.Find("Checkpoint" + checkpointNum) ?? GameObject.FindGameObjectWithTag("InitialCheckpoint").transform;

                prm.Instance.ResetChar(playerSavedTransform);
            }
        }
    }
    private IEnumerator SlowMovePosition(Vector3 startPosition, Vector3 endPosition, float duration, (int row, int col) indexCoords)
    {
        Transform _t = menus[indexCoords.row][indexCoords.col].transform;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easedT = t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
            _t.position = Vector3.Lerp(startPosition, endPosition, easedT);
            yield return null;
        }
        _t.position = endPosition;
        readyToMove[indexCoords.row][indexCoords.col] = true;
    }
    public void SetSensitivity(float value)
    {
        Slider sensitivitySlider = menus[1][1].transform.Find("SensitivitySlider").gameObject.GetComponent<Slider>();
        TextMeshProUGUI sensitivityText = menus[1][1].transform.Find("SensitivityValue").gameObject.GetComponent<TextMeshProUGUI>();

        ClampValues(ref value, sensitivitySlider.minValue, sensitivitySlider.maxValue);
        psm.Instance.SetSensitivity(value);

        sensitivitySlider.value = value;
        sensitivityText.text = Mathf.Round(value).ToString();
    }
    public void SetGravity(float value)
    {
        gravityToggle.isOn = value == 30f;
        Slider gravitySlider = menus[1][1].transform.Find("GravitySlider").gameObject.GetComponent<Slider>();
        TextMeshProUGUI gravityText = menus[1][1].transform.Find("GravityValue").gameObject.GetComponent<TextMeshProUGUI>();

        ClampValues(ref value, gravitySlider.minValue, gravitySlider.maxValue);
        psm.Instance.SetGravity(-value);

        gravitySlider.value = -value;
        gravityText.text = value.ToString("F2");
    }

    public void SetMenuTransitionDuration(float value)
    {
        Slider menuSpeedSlider = menus[1][1].transform.Find("MenuSpeedSlider").gameObject.GetComponent<Slider>();
        TextMeshProUGUI menuSpeedText = menus[1][1].transform.Find("MenuSpeedValue").gameObject.GetComponent<TextMeshProUGUI>();

        ClampValues(ref value, menuSpeedSlider.minValue, menuSpeedSlider.maxValue);
        psm.Instance.SetMenuTransitionDuration(value);
        menuTransitionDuration = value;

        menuSpeedSlider.value = value;
        menuSpeedText.text = value.ToString("F2");
    }
    private void InitializeMenus()
    {
        currentMenuIndex_Y = GetMenuIndex(mainMenuBoard).row;
        currentMenuIndex_X = GetMenuIndex(mainMenuBoard).col;

        playText = mainMenuBoard.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<TextMeshProUGUI>();
        playText.text = "Play";

        GameObject[] allMenus = GameObject.FindGameObjectsWithTag("MenuBoard");
        int maxRow = allMenus.Max(m => GetMenuIndex(m).row);

        (int row, int col)[] rowsAndCols = allMenus.Select(m => GetMenuIndex(m)).ToArray();
        Dictionary<int, int> rowColumnCounts = new();

        for (int i = 0; i < allMenus.Length; i++)
        {
            int currentRow = rowsAndCols[i].row;

            if (rowColumnCounts.ContainsKey(currentRow)) rowColumnCounts[currentRow]++;
            else rowColumnCounts.Add(currentRow, 1);
        }

        menus = new GameObject[maxRow][];
        foreach (var kvp in rowColumnCounts)
        {
            menus[kvp.Key - 1] = new GameObject[kvp.Value];
        }

        readyToMove = new bool[maxRow][];
        for (int i = 0; i < maxRow; i++)
        {
            int itemsInRow = rowColumnCounts[i + 1];
            readyToMove[i] = new bool[itemsInRow];

            for (int j = 0; j < itemsInRow; j++) readyToMove[i][j] = true;
        }

        for (int i = 0; i < allMenus.Length; i++)
        {
            (int currentRow, int currentCol) = rowsAndCols[i];
            menus[currentRow - 1][currentCol - 1] = allMenus[i];
        }
    }
    private void InitializeSettings()
    {
        float tolerance = 0.00001f;
        menuTransitionDuration = psm.Instance.GetMenuTransitionDuration();
        if (psm.Instance.GetGravity() - standardGravity < tolerance) UseStandardGravity(true);
        else gravityToggle.isOn = false;
        for (int i = 0; i < menus.Length; i++)
        {
            for (int k = 0; k < menus[i].Length; k++)
            {
                Toggle[] valueToggle = menus[i][k].GetComponentsInChildren<Toggle>();
                Slider[] valueSlider = menus[i][k].GetComponentsInChildren<Slider>();
                for (int j = 0; j < valueSlider.Length; j++)
                {
                    string nameOfSlider = valueSlider[j].gameObject.name[..^nameof(Slider).Length];
                    if (nameOfSlider.Equals("Sensitivity"))
                    {
                        valueSlider[j].value = psm.Instance.GetSensitivity();
                        valueSlider[j].onValueChanged.AddListener(SetSensitivity);
                    }
                    else if (nameOfSlider.Equals("Gravity"))
                    {
                        valueSlider[j].value = psm.Instance.GetGravity();
                        valueSlider[j].onValueChanged.AddListener(SetGravity);
                    }
                    else if (nameOfSlider.Equals("MenuSpeed"))
                    {
                        valueSlider[j].value = psm.Instance.GetMenuTransitionDuration();
                        valueSlider[j].onValueChanged.AddListener(SetMenuTransitionDuration);
                    }
                }
                for (int j = 0; j < valueToggle.Length; j++)
                {
                    string nameOfToggle = valueToggle[j].gameObject.name[..^nameof(Toggle).Length];
                    if (nameOfToggle.Equals("StandardGravity"))
                    {
                        valueToggle[j].onValueChanged.AddListener(UseStandardGravity);
                    }
                }
            }
        }
    }

    private void InitializeHighScores()
    {
        GameObject[] highScoreTexts = GameObject.FindGameObjectsWithTag("HighScoreText");

        if (highScoreTexts == null) { Debug.LogError("No High Score Texts Available"); return; }
        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            GameObject textObj = highScoreTexts[i];
            if (textObj != null)
            {
                int highScoreIndex = GetHighScoreTextIndex(textObj);

                textObj.GetComponent<TextMeshProUGUI>().text =
                psm.Instance.GetHighscore(highScoreIndex) != 0
                ? tm.Instance.FormatTimer(psm.Instance.GetHighscore(highScoreIndex))
                : "N/A";
            }
        }
    }
    public void QuitGame() => Application.Quit();

    [ContextMenu("Reset High Scores")]
    public void ResetHighScores()
    {
        GameObject[] highScoreTexts = GameObject.FindGameObjectsWithTag("HighScoreText");

        if (highScoreTexts == null) { Debug.LogError("No High Score Texts Available"); return; }
        for (int i = 1; i <= highScoreTexts.Length; i++)
        {
            GameObject textObj = highScoreTexts[i];
            if (textObj != null)
            {
                psm.Instance.SetHighscore(i, 0);
                textObj.GetComponent<TextMeshProUGUI>().text = "N/A";
            }
        }
    }

    private void ClampValues(ref float value, float min, float max) => value = Mathf.Clamp(value, min, max);

    public void UseStandardGravity(bool checkedOn) { if (checkedOn) SetGravity(standardGravity); }

    public static (int row, int col) GetMenuIndex(GameObject menu) => (int.Parse(menu.name[^2] + ""), int.Parse(menu.name[^1] + ""));

    private static int GetHighScoreTextIndex(GameObject highScoreText) => int.Parse(highScoreText.name[^1].ToString());
}
