using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelFinishScript : MonoBehaviour
{
    [SerializeField] private MovementScript moveCS;
    [SerializeField] private PlayerResetScript prs;
    [SerializeField] private MouseScript mouseCS;
    private int lastBuildIndex;
    private readonly ButtonLoaderScript bls;
    private int mainMenuBuildIndex = 0;
    void Start()
    {
        SceneManager.sceneLoaded += ChangeHighScoreText;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            moveCS.DisableMovement();
            MouseScript.UnlockCursor();

            //includes timer saving
            prs.PauseUnpauseTimer();

            // save data to file here
            PlayerPrefs.SetFloat("HighScore" + SceneManager.GetActiveScene().buildIndex, prs.GetTimerValue());

            if (!string.IsNullOrEmpty(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1)))
            {
                PlayerPrefs.SetInt("levelindex", PlayerPrefs.GetInt("levelindex") + 1);
            }
            else
            {
                lastBuildIndex = SceneManager.GetActiveScene().buildIndex;
            }

            PlayerPrefs.SetInt("checkpoint", 0);

            SceneManager.LoadScene(mainMenuBuildIndex);
        }
    }

    private void ChangeHighScoreText(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == mainMenuBuildIndex)
        {
            GameObject[] highScoreTexts = GameObject.FindGameObjectsWithTag("HighScoreText");
            // not needed yet
            // System.Array.Sort(highScoreTexts,(x,y) => GetHighScoreTextIndex(x).CompareTo(GetHighScoreTextIndex(y)));
            if (highScoreTexts == null) Debug.LogError("No High Score Texts Available");
            for (int i = 0; i < highScoreTexts.Length; i++)
            {
                GameObject textObj = highScoreTexts[i];
                if (textObj != null)
                {
                    int highScoreIndex = GetHighScoreTextIndex(textObj);
                    if (highScoreIndex <= lastBuildIndex)
                    {
                        float seconds = PlayerPrefs.GetFloat("timer" + highScoreIndex);
                        if (seconds < PlayerPrefs.GetFloat("highscore" + highScoreIndex, float.MaxValue))
                        {
                            textObj.GetComponent<TextMeshProUGUI>().text = PlayerResetScript.FormatTimer(seconds);
                            PlayerPrefs.SetFloat("highscore" + highScoreIndex, seconds);
                        }
                    }
                }
            }
        }
    }

    private static int GetHighScoreTextIndex(GameObject highScoreText)
    {
        return int.Parse(highScoreText.name[^1].ToString());
    }
}
