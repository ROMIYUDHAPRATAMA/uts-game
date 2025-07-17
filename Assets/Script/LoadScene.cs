using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Fungsi untuk mengganti scene berdasarkan nama scene
    public void ChangeScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi untuk mengganti scene berdasarkan index scene di Build Settings
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Fungsi untuk kembali ke Main Menu berdasarkan nama scene
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu"); // Ganti "MainMenu" jika nama scenemu berbeda
    }

    // Fungsi untuk kembali ke Main Menu berdasarkan index scene
    public void LoadMainMenuByIndex()
    {
        SceneManager.LoadScene(0); // Ganti 0 jika index MainMenu berbeda di Build Settings
    }

    // Fungsi untuk keluar dari game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed"); // Untuk debug di editor
    }

    // Fungsi untuk pause game
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    // Fungsi untuk resume game
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
