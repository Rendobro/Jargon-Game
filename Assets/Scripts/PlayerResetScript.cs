using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerResetScript : MonoBehaviour
{

    [SerializeField] private Transform worldSpawn;
    [SerializeField] private VoidScript vs;
    [SerializeField] private MovementScript moveCS;
    [SerializeField] private CharacterController ctrl;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;
    [SerializeField] private TextMeshProUGUI timerText;
    private int levelIndex;
    private float timer = 0f;
    private bool timerPaused = false;
    private bool timerToggledOn = true;
    void Start()
    {
        levelIndex = SceneManager.GetActiveScene().buildIndex;
        timer = PlayerPrefs.GetFloat("timer"+levelIndex);
    }
    void Update()
    {
        ResetChecks();
        ManageTimer();
    }
    public void ToggleTimerVisibility()
    {
        timerToggledOn = !timerToggledOn;
        RectTransform _tTimer = timerText.rectTransform;
        _tTimer.anchoredPosition = _tTimer.anchoredPosition.Equals(new Vector2(0, 625)) ? new Vector3(0, 625 * (timerToggledOn ? 1 : 2), 0) : new Vector2(0, 625);
    }
    public void PauseUnpauseTimer()
    {
        timerPaused = !timerPaused;
        PlayerPrefs.SetFloat("timer"+levelIndex, timer);
    }
    private void ManageTimer()
    {
        if (!timerPaused)
        {
            timer += Time.deltaTime;
            if (timerToggledOn) ChangeTimerText();
        }
        else if (timerToggledOn) ChangeTimerText();
    }
    public float GetTimerValue()
    {
        return timer;
    }
    public void ResetTimerValue()
    {
        timer = 0;
        PlayerPrefs.SetFloat("timer"+levelIndex, 0f);
    }
    public void ResetTimerValue(int levelIndex)
    {
        timer = 0;
        PlayerPrefs.SetFloat("timer"+levelIndex, 0f);
    }

    public static string FormatTimer(float seconds)
    {
        int numHours = (int)Mathf.Floor(seconds / 3600);

        string additionalSegment = "";
        if (numHours > 0) additionalSegment = numHours.ToString("D2") + ":";
        
        string formattedTimer = additionalSegment
        + ((int)Mathf.Floor((seconds - numHours * 3600f) / 60f)).ToString("D2") + ":"
        + ((int)(seconds % 60f)).ToString("D2") + ":"
        + ((int)(seconds % 1f * 1000)).ToString("D3");
        return formattedTimer;
    }

    private void ChangeTimerText()
    {
        timerText.text = FormatTimer(timer);
    }
    public void ResetChar()
    {
        ctrl.enabled = false;
        transform.SetPositionAndRotation(worldSpawn.position, Quaternion.identity);
        ctrl.enabled = true;

        cam.transform.rotation = Quaternion.identity;
        moveCS.ResetVelocityVertical();
    }
    public void ResetChar(Transform spawnPoint)
    {
        ctrl.enabled = false;
        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        ctrl.enabled = true;

        cam.transform.rotation = spawnPoint.rotation;
        moveCS.ResetVelocityVertical();
    }
    public void ResetChar(Vector3 spawnPointPos)
    {
        ctrl.enabled = false;
        transform.position = spawnPointPos;
        transform.localRotation = Quaternion.identity;
        ctrl.enabled = true;

        cam.transform.localRotation = Quaternion.identity;
        moveCS.ResetVelocityVertical();
    }

    public void HardResetChar()
    {
        timer = 0;
        PlayerPrefs.SetFloat("timer"+levelIndex, 0f);

        PlayerPrefs.SetInt("checkpoint", 0);

        ctrl.enabled = false;
        transform.SetPositionAndRotation(worldSpawn.position, worldSpawn.rotation);
        ctrl.enabled = true;

        cam.transform.rotation = worldSpawn.rotation;
        moveCS.ResetVelocityVertical();
        vs.ChangeCheckpoint(worldSpawn.position);
    }

    private void ResetChecks()
    {
        if (Input.GetButtonDown("Reset"))
        {
            ResetChar(transform);
        }
        if (Input.GetButtonDown("HardReset"))
        {
            HardResetChar();
        }
    }
}