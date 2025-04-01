using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public bool isGamePaused { get; private set; }
    public bool isGameOver { get; private set; }
    
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
    
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
    
    public void GameOver()
    {
        isGameOver = true;
        // Add your game over logic here
    }
    
    public void RestartGame()
    {
        isGameOver = false;
        // Add your restart logic here
    }
} 