using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public UnityEvent OnPlayerReset;
    public UnityEvent<int> OnPlayerHardReset;
    public UnityEvent<int> OnLevelFinish;
    public UnityEvent OnCheckpointsInitialized;
    public UnityEvent<Collider> OnCheckpointHit;
    public UnityEvent CreateNewLevel;
    public UnityEvent OnJumpOrbActivated;
    public UnityEvent OnMenuPaused;
    public UnityEvent OnMenuUnpaused;
    public UnityEvent<Transform> OnPlayerHitVoid;
    public UnityEvent OnCreateNewLevel;
    public UnityEvent<ObjectData> OnObjectSelected;
    public UnityEvent<RuntimeTransformGizmo> OnGizmoSelected;
    public UnityEvent<ObjectData> OnObjectDeselected;
    public UnityEvent<RuntimeTransformGizmo> OnGizmoDeselected;
    public UnityEvent OnUndoUnavailable;
    public UnityEvent OnRedoUnavailable;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Already an EventManager Instance in this scene.\nDestroying current instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        OnPlayerReset = new UnityEvent();
        OnPlayerHardReset = new UnityEvent<int>();
        OnLevelFinish = new UnityEvent<int>();
        OnCheckpointsInitialized = new UnityEvent();
        OnCheckpointHit = new UnityEvent<Collider>();
        CreateNewLevel = new UnityEvent();
        OnJumpOrbActivated = new UnityEvent();
        OnMenuPaused = new UnityEvent();
        OnMenuUnpaused = new UnityEvent();
        OnPlayerHitVoid = new UnityEvent<Transform>();
        OnCreateNewLevel = new UnityEvent();
        OnObjectSelected = new UnityEvent<ObjectData>();
        OnGizmoSelected = new UnityEvent<RuntimeTransformGizmo>();
        OnObjectDeselected = new UnityEvent<ObjectData>();
        OnGizmoDeselected = new UnityEvent<RuntimeTransformGizmo>();
        OnUndoUnavailable = new UnityEvent();
        OnRedoUnavailable = new UnityEvent();
    }
}
