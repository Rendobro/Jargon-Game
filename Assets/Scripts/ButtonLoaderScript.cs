using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonLoaderScript : MonoBehaviour
{
    [SerializeField] private float menuTransitionDuration = 1.6f;
    private static int currentMenuIndex_X = 1;
    private static int currentMenuIndex_Y = 1;
    private GameObject[][] menus;
    private CharacterController player;
    private PlayerResetScript prs;
    private TextMeshProUGUI playText;
    private bool[][] readyToMove;
    // Start is called before the first frame update
    void Start()
    {
        playText = GameObject.Find("MenuBoard11").GetComponentInChildren<Button>().gameObject.GetComponentInChildren<TextMeshProUGUI>();
        InitializeMenus();
        InitializeSliders();
    }
    void Update()
    {
        menuTransitionDuration = PlayerPrefs.GetFloat("menuspeed");
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
                    if (mup != null)
                    {
                        StartCoroutine(SlowMovePosition(_t.position, mup.position, menuTransitionDuration, (i, j)));
                    }
                    else
                    {
                        Debug.LogError($"MenuUpPosition{i + 1}{j + 1} not found");
                        readyToMove[i][j] = true; // Reset readyToMove if mup is null
                    }

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
    public void LoadSpecificScene(int sceneIndex)
    {
        SceneManager.sceneLoaded -= LoadCheckpointData;
        SceneManager.sceneLoaded -= ChangePlayText;
        SceneManager.sceneLoaded += LoadCheckpointData;
        SceneManager.sceneLoaded += ChangePlayText;
        SceneManager.LoadScene(sceneIndex);
    }
    public void LoadCorrectScene()
    {
        if (PlayerPrefs.GetInt("levelIndex", 0) < 1) PlayerPrefs.SetInt("levelIndex", 1);
        SceneManager.sceneLoaded -= LoadCheckpointData;
        SceneManager.sceneLoaded -= ChangePlayText;
        SceneManager.sceneLoaded += LoadCheckpointData;
        SceneManager.sceneLoaded += ChangePlayText;
        SceneManager.LoadScene(PlayerPrefs.GetInt("levelIndex"));
    }
    private void ChangePlayText(Scene scene, LoadSceneMode mode)
    {
        PlayerPrefs.SetInt("playtext", 1);
        playText.text = "Continue Playing!";
        playText.fontSize = 20;
    }
    private void LoadCheckpointData(Scene scene, LoadSceneMode mode)
    {
        // if the loaded scene is the scene where the player has last left off
        if (scene.name.Equals(SceneManager.GetSceneByBuildIndex(PlayerPrefs.GetInt("levelIndex")).name))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
            prs = player.GetComponent<PlayerResetScript>();
            int checkpointNum = PlayerPrefs.GetInt("checkpoint");
            // if the player has never played
            if (checkpointNum < 1)
            {
                prs.HardResetChar();
            }
            else
            {
                // sets the player's position to their saved checkpoint's position, including no checkpoint
                Vector3 playerSavedPosition = GameObject.FindGameObjectWithTag("Environment").transform.Find("Checkpoint" + checkpointNum).position;

                prs.ResetChar(playerSavedPosition);
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
        Slider sensitivitySlider = menus[currentMenuIndex_Y - 1][currentMenuIndex_X - 1].transform.Find("SensitivitySlider").gameObject.GetComponent<Slider>();
        float sensitivityValue = PlayerPrefs.GetFloat("sensitivity");
        TextMeshProUGUI sensitivityText = menus[currentMenuIndex_Y - 1][currentMenuIndex_X - 1].transform.Find("SensitivityValue").gameObject.GetComponent<TextMeshProUGUI>();

        ClampValues(sensitivityValue, sensitivitySlider.minValue, sensitivitySlider.maxValue);
        PlayerPrefs.SetFloat("sensitivity", value);

        sensitivityText.text = Mathf.Round(value).ToString();
    }
    public void SetGravity(float value)
    {
        Slider gravitySlider = menus[currentMenuIndex_Y - 1][currentMenuIndex_X - 1].transform.Find("GravitySlider").gameObject.GetComponent<Slider>();
        float gravityValue = PlayerPrefs.GetFloat("sensitivity");
        TextMeshProUGUI gravityText = menus[0][currentMenuIndex_X - 1].transform.Find("GravityValue").gameObject.GetComponent<TextMeshProUGUI>();

        ClampValues(gravityValue, gravitySlider.minValue, gravitySlider.maxValue);
        PlayerPrefs.SetFloat("gravity", value);

        gravityText.text = Mathf.Round(value).ToString();
    }

    public void SetMenuSpeed(float value)
    {
        Slider menuSpeedSlider = menus[currentMenuIndex_Y - 1][currentMenuIndex_X - 1].transform.Find("MenuSpeedSlider").gameObject.GetComponent<Slider>();
        float menuSpeedValue = PlayerPrefs.GetFloat("menuspeed");
        TextMeshProUGUI menuSpeedText = menus[currentMenuIndex_Y - 1][currentMenuIndex_X - 1].transform.Find("MenuSpeedValue").gameObject.GetComponent<TextMeshProUGUI>();

        ClampValues(menuSpeedValue, menuSpeedSlider.minValue, menuSpeedSlider.maxValue);
        PlayerPrefs.SetFloat("menuspeed", value);

        menuSpeedText.text = value.ToString("F2");
    }
    private void InitializeMenus()
    {
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
            for (int j = 0; j < itemsInRow; j++)
            {
                readyToMove[i][j] = true;
            }
        }
        for (int i = 0; i < allMenus.Length; i++)
        {
            (int currentRow, int currentCol) = rowsAndCols[i];
            menus[currentRow - 1][currentCol - 1] = allMenus[i];
        }
    }
    private void InitializeSliders()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            for (int k = 0; k < menus[i].Length; k++)
            {
                Slider[] valueSlider = menus[i][k].GetComponentsInChildren<Slider>();
                for (int j = 0; j < valueSlider.Length; j++)
                {
                    string nameOfSlider = valueSlider[j].gameObject.name.Substring(0, valueSlider[j].gameObject.name.Length - 6);
                    float value = PlayerPrefs.GetFloat(nameOfSlider.ToLower());
                    valueSlider[j].value = value;
                    menus[i][k].transform.Find(nameOfSlider + "Value").GetComponent<TextMeshProUGUI>().text = nameOfSlider.Equals("MenuSpeed") ? value.ToString("F2") : Mathf.Round(value).ToString();

                    if (nameOfSlider.Equals("Sensitivity"))
                    {
                        valueSlider[j].onValueChanged.AddListener(SetSensitivity);
                    }
                    else if (nameOfSlider.Equals("Gravity"))
                    {
                        valueSlider[j].onValueChanged.AddListener(SetGravity);
                    }
                    else if (nameOfSlider.Equals("MenuSpeed"))
                    {
                        valueSlider[j].onValueChanged.AddListener(SetMenuSpeed);
                    }
                }
            }
        }
    }
    private void ClampValues(float value, float min, float max)
    {
        value = value < min ? min : value;
        value = value > max ? max : value;
    }
    private static (int row, int col) GetMenuIndex(GameObject menu)
    {
        return (int.Parse(menu.name[^2] + ""), int.Parse(menu.name[^1] + ""));
    }
}
