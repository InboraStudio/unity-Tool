using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "GameSave";
    [SerializeField] private bool useEncryption = false;
    [SerializeField] private string encryptionKey = "YourEncryptionKey";
    [SerializeField] private bool useCompression = true;
    [SerializeField] private bool autoSave = false;
    [SerializeField] private float autoSaveInterval = 300f; // 5 minutes

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName + ".sav");
    private float autoSaveTimer = 0f;
    private Dictionary<string, object> saveData = new Dictionary<string, object>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadGame();
    }

    private void Update()
    {
        if (autoSave)
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                SaveGame();
                autoSaveTimer = 0f;
            }
        }
    }

    public void SaveGame()
    {
        // Collect data from registered save objects
        CollectSaveData();

        // Create the save data container
        SaveData data = new SaveData
        {
            savedData = saveData,
            saveDate = DateTime.Now.ToString()
        };

        try
        {
            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));

            // Serialize the data
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(SavePath, FileMode.Create))
            {
                byte[] dataToSave = SerializeData(data);
                
                if (useEncryption)
                {
                    dataToSave = EncryptDecrypt(dataToSave);
                }
                
                stream.Write(dataToSave, 0, dataToSave.Length);
            }
            
            Debug.Log("Game saved successfully to " + SavePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving game: " + e.Message);
        }
    }

    public bool LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found at " + SavePath);
            return false;
        }

        try
        {
            byte[] dataToLoad;
            
            using (FileStream stream = new FileStream(SavePath, FileMode.Open))
            {
                dataToLoad = new byte[stream.Length];
                stream.Read(dataToLoad, 0, dataToLoad.Length);
            }
            
            if (useEncryption)
            {
                dataToLoad = EncryptDecrypt(dataToLoad);
            }
            
            SaveData loadedData = DeserializeData(dataToLoad);
            
            if (loadedData != null)
            {
                saveData = loadedData.savedData;
                Debug.Log("Game loaded successfully from " + SavePath);
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading game: " + e.Message);
        }
        
        return false;
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            saveData.Clear();
            Debug.Log("Save file deleted from " + SavePath);
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }

    // Save specific data
    public void SaveData<T>(string key, T data)
    {
        if (saveData.ContainsKey(key))
        {
            saveData[key] = data;
        }
        else
        {
            saveData.Add(key, data);
        }
    }

    // Load specific data
    public T LoadData<T>(string key, T defaultValue = default)
    {
        if (saveData.ContainsKey(key) && saveData[key] is T)
        {
            return (T)saveData[key];
        }
        
        return defaultValue;
    }

    private void CollectSaveData()
    {
        // Find all objects that implement ISaveable
        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>().ToArray();
        
        foreach (ISaveable saveable in saveables)
        {
            string id = saveable.GetUniqueID();
            object data = saveable.SaveData();
            
            if (saveData.ContainsKey(id))
            {
                saveData[id] = data;
            }
            else
            {
                saveData.Add(id, data);
            }
        }
    }

    private void ApplySaveData()
    {
        // Find all objects that implement ISaveable
        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>().ToArray();
        
        foreach (ISaveable saveable in saveables)
        {
            string id = saveable.GetUniqueID();
            
            if (saveData.ContainsKey(id))
            {
                saveable.LoadData(saveData[id]);
            }
        }
    }

    private byte[] SerializeData(SaveData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, data);
            return stream.ToArray();
        }
    }

    private SaveData DeserializeData(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(data))
        {
            return (SaveData)formatter.Deserialize(stream);
        }
    }

    private byte[] EncryptDecrypt(byte[] data)
    {
        // Simple XOR encryption
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
        byte[] result = new byte[data.Length];
        
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] ^ keyBytes[i % keyBytes.Length]);
        }
        
        return result;
    }
}

[System.Serializable]
public class SaveData
{
    public Dictionary<string, object> savedData;
    public string saveDate;
}

// Interface for objects that can be saved
public interface ISaveable
{
    string GetUniqueID();
    object SaveData();
    void LoadData(object data);
} 