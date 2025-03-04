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
    private int lastBuildIndex = 0;
    private readonly int mainMenuBuildIndex = 0;
    private bool gameWon = false;
    void Start()
    {
        PlayerPrefs.SetInt("recentlevel", SceneManager.GetActiveScene().buildIndex);
        SceneManager.sceneLoaded += ChangeHighScoreText;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            gameWon = true;
            int bIndex = SceneManager.GetActiveScene().buildIndex;
            moveCS.DisableMovement();
            MouseScript.UnlockCursor();

            //includes timer saving
            prs.PauseUnpauseTimer();

            // save data to file here

            if (!string.IsNullOrEmpty(SceneUtility.GetScenePathByBuildIndex(bIndex + 1)))
            {
                if (PlayerPrefs.GetInt("levelindex") <= bIndex)
                PlayerPrefs.SetInt("levelindex", PlayerPrefs.GetInt("levelindex") + 1);
                lastBuildIndex = bIndex + 1;
            }
            else
            {
                lastBuildIndex = bIndex;
            }
            PlayerPrefs.SetInt("checkpoint", 0);

            SceneManager.LoadScene(mainMenuBuildIndex);
            //Debug.Log($" buildIndex after {SceneManager.GetActiveScene().buildIndex} ; buildIndex Stored {lastBuildIndex} ; is buildPath there {!string.IsNullOrEmpty(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1))}");
        }
    }

    private void ChangeHighScoreText(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == mainMenuBuildIndex && gameWon)
        {
            gameWon = false;
            GameObject[] highScoreTexts = GameObject.FindGameObjectsWithTag("HighScoreText");

            if (highScoreTexts == null) { Debug.LogError("No High Score Texts Available"); return; }
            for (int i = 0; i < highScoreTexts.Length; i++)
            {
                GameObject textObj = highScoreTexts[i];
                if (textObj != null)
                {
                    int highScoreIndex = GetHighScoreTextIndex(textObj);
                    if (highScoreIndex <= lastBuildIndex && PlayerPrefs.HasKey("timer" + highScoreIndex))
                    {
                        float seconds = PlayerPrefs.GetFloat("timer" + highScoreIndex);
                        Debug.Log($"1 hsi: {highScoreIndex} ; current score: {seconds} ; has key {PlayerPrefs.HasKey("highscore"+highScoreIndex)} ; stored highscore: {PlayerPrefs.GetFloat("highscore" + highScoreIndex)}");
                        if (seconds > 1f && seconds < ((PlayerPrefs.GetFloat("highscore" + highScoreIndex) != 0) ? PlayerPrefs.GetFloat("highscore" + highScoreIndex) : float.MaxValue ))
                        {
                            textObj.GetComponent<TextMeshProUGUI>().text = PlayerResetScript.FormatTimer(seconds);
                            PlayerPrefs.SetFloat("highscore" + highScoreIndex, seconds);
                            PlayerPrefs.DeleteKey("timer" + highScoreIndex);
                        }
                        else if (seconds <= 1f && !textObj.GetComponent<TextMeshProUGUI>().text.Equals("N/A"))
                        {
                            textObj.GetComponent<TextMeshProUGUI>().text = "N/A";
                        }
                        else if (!PlayerPrefs.HasKey("highscore" + highScoreIndex))
                        {
                            PlayerPrefs.SetFloat("highscore", float.MaxValue);
                        }
                        prs.ResetTimerValue(highScoreIndex);
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
