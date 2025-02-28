using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private MovementScript moveCS;
    [SerializeField] private MouseScript mouseCS;
    [SerializeField] private PlayerResetScript prs;
    [SerializeField] private Transform _t;
    [SerializeField] private float pauseMenuDuration = 3;
    private bool readyToMove = true;
    private readonly float tolerance = 0.00001f;
    private readonly int titleScreenIndex = 0;

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
        prs.ToggleTimerVisibility();
    }
    public void QuitGame()
    {
        // remember to save game before this happens
        prs.PauseUnpauseTimer();
        PlayerPrefs.SetFloat("timer",prs.GetTimerValue());
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("levelindex",currentSceneIndex);
        Cursor.lockState = CursorLockMode.None;
        if (currentSceneIndex != titleScreenIndex) SceneManager.LoadScene(titleScreenIndex);
    }

    private void CenterPauseMenu()
    {
        StartCoroutine(SlowMovePosition(_t.localPosition,Vector3.zero, pauseMenuDuration));
        prs.PauseUnpauseTimer();
        moveCS.DisableMovement();
        mouseCS.DisableRotation();
        Cursor.lockState = CursorLockMode.None;
    }

    private void ClosePauseMenu()
    {
        if (_t.localPosition.z < tolerance && _t.localPosition.y < tolerance && _t.localPosition.x < tolerance)
        StartCoroutine(SlowMovePosition(_t.localPosition,Vector3.up * 2000, pauseMenuDuration));
        prs.PauseUnpauseTimer();
        moveCS.EnableMovement();
        mouseCS.EnableRotation();
        Cursor.lockState = CursorLockMode.Locked;
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
