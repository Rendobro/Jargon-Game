using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using mvs = MovementScript;
using mos = MouseScript;
using prm = PlayerResetManager;
using System;
using tm = TimerManager;
using bls = ButtonLoaderScript;
using psm = PlayerStatsManager;
public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private Transform _t;
    [SerializeField] private float pauseMenuDuration = 3;
    private bool readyToMove = true;
    private const float tolerance = 0.00001f;


    void Start()
    {
        _t.localPosition = new Vector3(0,2000,0);
    }
    void Update()
    {
        PauseChecker();
    }
    private void PauseChecker()
    {
        if (Input.GetButtonDown("Pause") && readyToMove) TogglePauseMenu();
    }
    public void ToggleTimer()
    {
        tm.Instance.ToggleTimerVisibility();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void TogglePauseMenu()
    {
        if (readyToMove)
        {
            readyToMove = false;
            if (_t.localPosition.y < tolerance) ClosePauseMenu();
            else CenterPauseMenu(); 
        }
    }
    public void ReturnToMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex != psm.mainMenuIndex) SceneManager.LoadScene(psm.mainMenuIndex);
    }

    private void CenterPauseMenu()
    {
        EventManager.Instance.OnMenuPaused?.Invoke();

        StartCoroutine(SlowMovePosition(_t.localPosition,Vector3.zero, pauseMenuDuration));
        tm.Instance.PauseTimer();
    }

    private void ClosePauseMenu()
    {
        EventManager.Instance.OnMenuUnpaused?.Invoke();
        if (_t.localPosition.z < tolerance && _t.localPosition.y < tolerance && _t.localPosition.x < tolerance)
        StartCoroutine(SlowMovePosition(_t.localPosition,Vector3.up * 2000, pauseMenuDuration));
        tm.Instance.UnpauseTimer();
    }

    private IEnumerator SlowMovePosition(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float easedT = elapsedTime / duration;
            easedT = easedT < 0.5f ? 2 * easedT * easedT : -1 + (4 - 2 * easedT) * easedT;
            _t.localPosition = Vector3.Lerp(startPosition, endPosition, easedT);
            yield return null;
        }
        _t.localPosition = endPosition;
        readyToMove = true;
    }
}
