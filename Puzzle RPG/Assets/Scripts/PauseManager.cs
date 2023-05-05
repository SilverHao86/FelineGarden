using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.Pause();
    }

    /// <summary>
    /// Returns the user to the start screen whenever called
    /// </summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1.0f;
        
        SceneManager.LoadScene("MainMenu");
    }

    public void LastCheckpoint()
    {
        GameManager.Instance.SpawnAtLastCheckpoint();
    }

    public void LoadSave()
    {
        GameManager.Instance.LoadSavedCheckpoint();
    }

    /// <summary>
    /// Exits the game whenever called
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

}
