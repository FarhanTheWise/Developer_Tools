using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    
    [Header("Data Holders")]
    public PlayerData playerData;
    //public SessionData sessionData;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        CheckDirectory();
        LoadData();
    }

    private static void CheckDirectory() => DataSystem.Init();

    public static void SaveData<T>(SaveFileType fileType, T data) where T : class
    {
        var saveString = JsonUtility.ToJson(data, true);
        DataSystem.Save(fileType, saveString);
       // UIManager.instance.debugText.text += $"Player cash saved";
    }

    private void LoadData()
    {
        
        //player data
        playerData = LoadClass(SaveFileType.PlayerData, playerData);
        //session data
        //sessionData = LoadClass(SaveFileType.SessionData, sessionData);
    }
    
    private T LoadClass<T>(SaveFileType type, T data) where T : class
    {
        
        //Player Data
        var saveString = DataSystem.Load(type);
        if (saveString == null)
        {
            SaveData(type, data);
            saveString = DataSystem.Load(type);
        }
        
        data = JsonUtility.FromJson<T>(saveString);

        return data;

    }

}


public static class DataSystem
{
    #if UNITY_EDITOR
    private static readonly string SAVE_PATH = Application.dataPath + "/Saves/";
    #elif UNITY_ANDROID
    private static readonly string SAVE_PATH = Application.persistentDataPath + "/Saves/";
#endif

    private static readonly string[] _dataPaths = new[]
    {
        "PlayerData.json",
        "SessionStats.json",
        "ShelvingSystem.json",
        "CrateData.json",
        "BoxData.json",
    };
    
    public static void Init()
    {
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }
    }
    
    public static void Save(SaveFileType fileType, string saveString)
    {
        File.WriteAllText($"{SAVE_PATH}{_dataPaths[fileType.GetHashCode()]}", saveString);
    }
    
    public static string Load(SaveFileType type)
    {

        if (!File.Exists($"{SAVE_PATH}{_dataPaths[type.GetHashCode()]}")) return null;
        Debug.Log("Loading save file");
        var saveString = File.ReadAllText($"{SAVE_PATH}{_dataPaths[type.GetHashCode()]}");
        return saveString;
    }
}

public enum SaveFileType
{
    PlayerData = 0,
    SessionData = 1
}

#region Player Data
[Serializable]
public class PlayerData
{
    public bool tutorialDone;
   // public int savedTutorialStep;
    public float playerMoney;
/*    public bool isStoreOpen;
    public bool isDayOver;
    public bool isDayRunning;
    public bool firstEntry;
    public int currentDay;
    public float totalWorkingHours;
    public int timeOfDay;
    public float currentHours;
    public float currentMinutes;*/
    public bool isVibrating;
    public float musicVolume;
    public float sfxVolume;
    public float sensitivity;
    public bool isAutoShoot;
/*    public bool isVanBought;
    public int expansionIndexA;
    public int expansionIndexB;
    public int boxAmount;*/
}
#endregion


#region Session Stats
[Serializable]
public class SessionData
{
    public int currentMode;
    public int currentMap;
}
#endregion


