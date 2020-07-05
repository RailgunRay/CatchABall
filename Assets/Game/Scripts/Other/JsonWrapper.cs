using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class JsonWrapper
{
    private string path = Application.persistentDataPath + "/gameData.json";
    public GameData gameData;

    public void SaveData()
    {
        string contents = JsonUtility.ToJson(this, true);
        File.WriteAllText(path, contents);
    }

    public string ReadData() {
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else {
            return "Game data file is missing!";
        }
    }
}
