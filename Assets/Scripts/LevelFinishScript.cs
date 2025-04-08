using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using psm = PlayerStatsManager;
using cm = CheckpointManager;
using prm = PlayerResetManager;
using tm = TimerManager;
public class LevelFinishScript : MonoBehaviour
{
    private int finishedLevelIndex;
    private const int mainMenuBuildIndex = 0;
    private bool gameWon = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += ChangeHighScoreText;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ChangeHighScoreText;
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            finishedLevelIndex = SceneManager.GetActiveScene().buildIndex;
            EventManager.Instance.OnLevelFinish?.Invoke(finishedLevelIndex);

            gameWon = true;

            if (!string.IsNullOrEmpty(SceneUtility.GetScenePathByBuildIndex(finishedLevelIndex + 1)))
            psm.Instance.SetLastUnlockedLevelIndex(finishedLevelIndex + 1);

            Debug.Log("Current Last Unlocked Level: "+psm.Instance.GetLastUnlockedLevelIndex());

            SceneManager.LoadScene(mainMenuBuildIndex);
        }
    }

    private void ChangeHighScoreText(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == mainMenuBuildIndex && gameWon)
        {
            gameWon = false;
            GameObject[] highScoreTexts = GameObject.FindGameObjectsWithTag("HighScoreText");

            if (highScoreTexts == null) 
            { 
                Debug.LogError("No High Score Texts Available"); 
                return; 
            }
            for (int i = 0; i < highScoreTexts.Length; i++)
            {
                GameObject textObj = highScoreTexts[i];
                if (textObj != null)
                {
                    int highScoreIndex = GetHighScoreTextIndex(textObj);
                    if (highScoreIndex <= psm.Instance.GetLastUnlockedLevelIndex())
                    {
                        float seconds = tm.Instance.GetTimerValue(finishedLevelIndex);
                        float previousHighscore = psm.Instance.GetHighscore(highScoreIndex);
                        if (seconds > 1f && seconds < ((previousHighscore != 0) ? previousHighscore : float.MaxValue ))
                        {
                            textObj.GetComponent<TextMeshProUGUI>().text = tm.Instance.FormatTimer(seconds);
                            psm.Instance.SetHighscore(highScoreIndex,seconds);
                        }
                        else if (seconds <= 1f && !textObj.GetComponent<TextMeshProUGUI>().text.Equals("N/A"))
                        {
                            textObj.GetComponent<TextMeshProUGUI>().text = "N/A";
                        }
                        tm.Instance.ResetTimerValue(highScoreIndex);
                    }
                }
            }

        }
    }

    private static int GetHighScoreTextIndex(GameObject highScoreText) => int.Parse(highScoreText.name[^1].ToString());
}
