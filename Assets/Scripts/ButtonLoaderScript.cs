using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonLoaderScript : MonoBehaviour
{
    public float menuTransitionDuration = 1.6f;
    private GameObject[] menus;
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
