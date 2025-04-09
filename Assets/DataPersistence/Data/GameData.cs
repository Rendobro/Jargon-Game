using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using bls = ButtonLoaderScript;

[System.Serializable]
public class GameData
{
    [Header("Player Settings")]
    public float gravity;
    public float sensitivity;
    public float menuTransitionDuration;

    [Header("Level Data")]
    public int lastLevelUnlocked;
    public int recentLevelIndex;

    [Header("Editor Data")]
    public List<LevelData> playerLevels;
    public float editorSensitivity;

    [Header("Speedrun Stats")]
    public float[] highscores;
    public float[] ongoingLevelTimers;

    [Header("Checkpoint Data")]
    public int[] checkpointNums;
    public Transform[] currentCheckpoints;

    public GameData()
    {
        recentLevelIndex = 1;
        lastLevelUnlocked = 1;
        sensitivity = 400f;
        editorSensitivity = 400f;
        gravity = -30f;
        menuTransitionDuration = 0.9f;
        checkpointNums = new int[bls.numLevels];
        currentCheckpoints = new Transform[bls.numLevels];
        highscores = new float[bls.numLevels];
        ongoingLevelTimers = new float[bls.numLevels];
        playerLevels = new();
    }

    public override string ToString()
    {
        return
        $"recentLevelIndex: {recentLevelIndex}" +
        $"\nlastLevelUnlocked: {lastLevelUnlocked}" +
        $"\nsensitivity: {sensitivity}" +
        $"\ngravity: {gravity}" +
        $"\nmenuTransitionDuration: {menuTransitionDuration}" +
        $"\ncheckpointNums: {ArrayToString(checkpointNums)}" +
        $"\ncurrentCheckpoints: {ArrayToString(currentCheckpoints)}" +
        $"\nhighscores: {ArrayToString(highscores)}" +
        $"\nongoingLevelTimers: {ArrayToString(ongoingLevelTimers)}" +
        $"\nplayerLevels: {playerLevels.ToLineSeparatedString()}" +
        $"\neditorSensitivity: {editorSensitivity}";
    }
    private string ArrayToString<T>(T[] array)
    {
        if (array == null || array.Length == 0)
        {
            return "[]";
        }

        return "[" + string.Join(", ", array) + "]";
    }
}
