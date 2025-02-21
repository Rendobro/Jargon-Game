using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonLoaderScript : MonoBehaviour
{
    public float menuTransitionDuration = 1.6f;
    private int currentMenuIndex = 1;
    private GameObject[] menus;
    private CharacterController player;
    private PlayerResetScript prs;
    private bool[] readyToMove;
    // Start is called before the first frame update
    void Start()
    {
        menus = GameObject.FindGameObjectsWithTag("MenuBoard");
        System.Array.Sort(menus, (x, y) => GetMenuIndex(x).CompareTo(GetMenuIndex(y)));
        {
            readyToMove = new bool[menus.Length];
            for (int i = 0; i<readyToMove.Length;i++)
                readyToMove[i] = true;
        }
        for (int i = 0; i<menus.Length;i++)
        {
            Slider[] valueSlider = menus[i].GetComponentsInChildren<Slider>();
            for (int j = 0; j<valueSlider.Length;j++)
            {
                string nameOfSlider = valueSlider[j].gameObject.name.Substring(0,valueSlider[j].gameObject.name.Length-6);
                float value = PlayerPrefs.GetFloat(nameOfSlider.ToLower());
                valueSlider[j].value = value;
                menus[i].transform.Find(nameOfSlider+"Value").GetComponent<TextMeshProUGUI>().text = Mathf.Round(value).ToString();
                if (nameOfSlider.Equals("Sensitivity"))
                {
                    valueSlider[j].onValueChanged.AddListener(SetSensitivity);
                }
                else if (nameOfSlider.Equals("Gravity"))
                {
                    valueSlider[j].onValueChanged.AddListener(SetGravity);
                }
            }
        }
    }
    public void LoadCorrectScene()
    {
        if (PlayerPrefs.GetInt("levelIndex",0)<1) 
        {
            PlayerPrefs.SetInt("levelIndex",1);
        }
        SceneManager.LoadScene(PlayerPrefs.GetInt("levelIndex"));
        SceneManager.sceneLoaded += LoadCheckpointData;
    }
    public void BringSettingsMenuLeft()
    {
        Debug.Log(readyToMove);
        Debug.Log(menus == null);
        for (int i = 0;i<menus.Length;i++)
        {
            if (readyToMove[i])
            {
                readyToMove[i] = false;
                Transform[] transforms = menus[i].GetComponentsInChildren<Transform>();
                Transform mlp = transforms.FirstOrDefault(t => t.gameObject.name.Equals("MenuLeftPosition"+(i+1)));
                
                StartCoroutine(SlowMovePosition(menus[i].transform.position,mlp.position,menuTransitionDuration,i));
            }
        }
        currentMenuIndex++;
    }
    public void BringSettingsMenuRight()
    {
        Debug.Log(readyToMove);
        Debug.Log(menus == null);
        for (int i = 0;i<menus.Length;i++)
        {
            if (readyToMove[i])
            {
                readyToMove[i] = false;
                Transform[] transforms = menus[i].GetComponentsInChildren<Transform>();
                Transform mrp = transforms.FirstOrDefault(item => item.gameObject.name.Equals("MenuRightPosition"+(i+1)));
                StartCoroutine(SlowMovePosition(menus[i].transform.position,mrp.position,menuTransitionDuration,i));
            }
        }
        currentMenuIndex--;
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
            if (checkpointNum<1)
            {
                prs.HardResetChar();
            }
            else
            {
                // sets the player's position to their saved checkpoint's position, including no checkpoint
                Vector3 playerSavedPosition = GameObject.FindGameObjectWithTag("Environment").transform.Find("Checkpoint"+checkpointNum).position;
                
                prs.ResetChar(playerSavedPosition);
            }
        }
    }
    private IEnumerator SlowMovePosition(Vector3 startPosition, Vector3 endPosition, float duration, int index)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float easedT = t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
            menus[index].transform.position = Vector3.Lerp(startPosition, endPosition, easedT);
            yield return null;
        }
        menus[index].transform.position = endPosition;
        readyToMove[index] = true;
    }
    public void SetSensitivity(float value)
    {
        Slider sensitivitySlider = menus[currentMenuIndex-1].transform.Find("SensitivitySlider").gameObject.GetComponent<Slider>();
        float sensitivityValue = PlayerPrefs.GetFloat("sensitivity");
        TextMeshProUGUI sensitivityText = menus[currentMenuIndex-1].transform.Find("SensitivityValue").gameObject.GetComponent<TextMeshProUGUI>();

        ClampValues(sensitivityValue,sensitivitySlider.minValue,sensitivitySlider.maxValue);
        PlayerPrefs.SetFloat("sensitivity", value);

        sensitivityText.text = Mathf.Round(value).ToString();
    }
    private void ClampValues(float value, float min, float max)
    {
        value = value < min ? min : value;
        value = value > max ? max : value;
    }
    public void SetGravity(float value)
    {
        Slider gravitySlider = menus[currentMenuIndex-1].transform.Find("GravitySlider").gameObject.GetComponent<Slider>();
        float gravityValue = PlayerPrefs.GetFloat("sensitivity");
        TextMeshProUGUI gravityText = menus[currentMenuIndex-1].transform.Find("GravityValue").gameObject.GetComponent<TextMeshProUGUI>();
        
        ClampValues(gravityValue,gravitySlider.minValue,gravitySlider.maxValue);
        PlayerPrefs.SetFloat("gravity", value);
        
        gravityText.text = Mathf.Round(value).ToString();
    }
    private static int GetMenuIndex(GameObject menu)
    {
        return int.Parse(menu.name[^1]+"");
    }
}
