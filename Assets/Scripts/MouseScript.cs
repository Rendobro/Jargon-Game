using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using psm = PlayerStatsManager;
public class MouseScript : MonoBehaviour
{
    [SerializeField] private Transform _tPlayer;
    private static float sensitivity;
    private float xRot = 0f;
    private static bool isDisabled = false;

    private void Awake()
    {
        LockCursor();
    }

    void Update()
    {
        StartCoroutine(WaitForInitialization());
        // Debug.Log("sens of script prior:" + sensitivity);
        // Debug.Log($"sens in psm prior: {psm.Instance.GetSensitivity()}");
        if (isDisabled) return;
        MouseDetector();
    }

    private IEnumerator WaitForInitialization()
    {
        while (psm.Instance == null)
        {
            Debug.LogWarning("Waiting for PlayerStatsManager.Instance to initialize...");
            yield return null; // Wait for the next frame
        }
        // Debug.Log("sens of script after:" + sensitivity);
        // Debug.Log($"sens in psm after: {psm.Instance.GetSensitivity()}");
    }

    private void OnEnable()
    {
        sensitivity = psm.Instance.GetSensitivity();
        EventManager.Instance.OnLevelFinish.AddListener(UnlockCursor);
        EventManager.Instance.OnMenuPaused.AddListener(DisableRotation);
        EventManager.Instance.OnMenuPaused.AddListener(UnlockCursor);
        EventManager.Instance.OnMenuUnpaused.AddListener(EnableRotation);
        EventManager.Instance.OnMenuUnpaused.AddListener(LockCursor);
        SceneManager.sceneLoaded += UnlockCursor;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnLevelFinish.RemoveListener(UnlockCursor);
        EventManager.Instance.OnMenuPaused.RemoveListener(DisableRotation);
        EventManager.Instance.OnMenuPaused.RemoveListener(UnlockCursor);
        EventManager.Instance.OnMenuUnpaused.RemoveListener(EnableRotation);
        EventManager.Instance.OnMenuUnpaused.RemoveListener(LockCursor);
        SceneManager.sceneLoaded -= UnlockCursor;
    }

    private void MouseDetector()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90, 90); //Clamp Vertical Rotation

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        _tPlayer.transform.Rotate(Vector3.up * mouseX);
    }
    public static void UnlockCursor(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == psm.mainMenuIndex) Cursor.lockState = CursorLockMode.None;
    }
    public static void UnlockCursor(int _) => Cursor.lockState = CursorLockMode.None;

    public static void UnlockCursor() => Cursor.lockState = CursorLockMode.None;

    public static void LockCursor() => Cursor.lockState = CursorLockMode.Locked;

    public static void DisableRotation() => isDisabled = true;

    public static void EnableRotation() => isDisabled = false;

    public static void SetSensitivity(float sensNew) => sensitivity = sensNew;
}