using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonLoaderScript : MonoBehaviour
{
    public float menuTransitionDuration = 1.6f;
    private GameObject[] menus;
    private CharacterController player;
    private PlayerResetScript prs;
    // Start is called before the first frame update
    void Start()
    {
        menus = GameObject.FindGameObjectsWithTag("MenuBoard");
        System.Array.Sort(menus, (x, y) => GetMenuIndex(x).CompareTo(GetMenuIndex(y)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("Load Correct Scene")]
    public void LoadCorrectScene()
    {
        if (PlayerPrefs.GetInt("currentLevel")<1) 
        {
            PlayerPrefs.SetInt("levelIndex",1);
        }
        SceneManager.LoadScene(PlayerPrefs.GetInt("levelIndex"));
        SceneManager.sceneLoaded += LoadCheckpointData;
    }
    public void BringSettingsMenuLeft()
    {
        for (int i = 0;i<menus.Length;i++)
        {
            Transform[] transforms = menus[i].GetComponentsInChildren<Transform>();
            Transform mlp = transforms.FirstOrDefault(t => t.gameObject.name.Equals("MenuLeftPosition"+(i+1)));
            StartCoroutine(SlowMovePosition(menus[i].transform.position,mlp.position,menuTransitionDuration,i));
        }
    }
    public void BringSettingsMenuRight()
    {
        for (int i = 0;i<menus.Length;i++)
        {
            Transform[] transforms = menus[i].GetComponentsInChildren<Transform>();
            Transform mrp = transforms.FirstOrDefault(item => item.gameObject.name.Equals("MenuRightPosition"+(i+1)));
            StartCoroutine(SlowMovePosition(menus[i].transform.position,mrp.position,menuTransitionDuration,i));
        }
    }
    private void LoadCheckpointData(Scene scene, LoadSceneMode mode)
    {
        // if the loaded scene is the scene where the player has last left off
        if (scene.name.Equals(SceneManager.GetSceneByBuildIndex(PlayerPrefs.GetInt("levelIndex")).name))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
            prs = player.GetComponent<PlayerResetScript>();
            int checkpointNum = PlayerPrefs.GetInt("checkpoint");
            if (checkpointNum<1) // if the player has never played OR hard reset to have no checkpoint data
            {
                Debug.Log("Hard Reset The Character");
                prs.HardResetChar();
            }
            else
            {
                // sets the player's position to their saved checkpoint's position;
                prs.ResetChar(GameObject.FindGameObjectWithTag("Environment").transform.Find("Checkpoint"+checkpointNum).position);
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
    }
    public void SetSensitivity()
    {
    PlayerPrefs.SetFloat("sensitivity", GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>().value);
    }
    private static int GetMenuIndex(GameObject menu)
    {
        return int.Parse(menu.name[^1]+"");
    }
}
