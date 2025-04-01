using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
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
        }
    }
    
    public void SaveData(string key, object value)
    {
        if (value is int intValue)
            PlayerPrefs.SetInt(key, intValue);
        else if (value is float floatValue)
            PlayerPrefs.SetFloat(key, floatValue);
        else if (value is string stringValue)
            PlayerPrefs.SetString(key, stringValue);
        else
            Debug.LogError($"Unsupported data type for saving: {value.GetType()}");
            
        PlayerPrefs.Save();
    }
    
    public T LoadData<T>(string key, T defaultValue)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;
            
        if (typeof(T) == typeof(int))
            return (T)Convert.ChangeType(PlayerPrefs.GetInt(key), typeof(T));
        else if (typeof(T) == typeof(float))
            return (T)Convert.ChangeType(PlayerPrefs.GetFloat(key), typeof(T));
        else if (typeof(T) == typeof(string))
            return (T)Convert.ChangeType(PlayerPrefs.GetString(key), typeof(T));
        else
        {
            Debug.LogError($"Unsupported data type for loading: {typeof(T)}");
            return defaultValue;
        }
    }
    
    public void DeleteData(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
    
    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
} 