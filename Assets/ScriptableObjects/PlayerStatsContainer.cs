using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsContainer", menuName = "Scriptable Objects/PlayerStatsContainer")]
public class PlayerStatsContainer : ScriptableObject
{
    public readonly float[] playerHighscores = new float[ButtonLoaderScript.numLevels];
    public float gravity;
    public float sensitivity;
    public float menuTransitionDuration;
    public int recentLevelIndex;
    public int lastLevelUnlocked;
}
