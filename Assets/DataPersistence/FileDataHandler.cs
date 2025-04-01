using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private readonly string dataDirPath = "";
    private readonly string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }
    public GameData Load()
    {
        string dataFullPath = Path.Combine(dataDirPath,dataFileName);
        GameData loadedData = null;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dataFullPath));

            string dataToReceive = "";

            using FileStream stream = new(dataFullPath , FileMode.Open);
            using StreamReader reader = new(stream);
            dataToReceive = reader.ReadToEnd();
            
            loadedData = JsonUtility.FromJson<GameData>(dataToReceive);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading in the directory for {dataFullPath}\n{e}");
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        string dataFullPath = Path.Combine(dataDirPath,dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dataFullPath));

            string dataToStore = JsonUtility.ToJson(data,true);
    
            using FileStream stream = new(dataFullPath, FileMode.Create);
            using StreamWriter writer = new(stream);
            writer.Write(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving in the directory for {dataFullPath}\n{e}");
        }
    }
}
