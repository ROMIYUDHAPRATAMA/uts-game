using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Fungsi untuk tombol PILIH LEVEL
    public void GoToPilihLevel()
    {
        Debug.Log("Pindah ke scene: PilihLevel");
        SceneManager.LoadScene("PilihLevel"); // Pastikan scene ini sudah masuk di Build Settings
    }

    // Fungsi untuk tombol EXIT
    public void ExitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();

        // Untuk editor (biar tidak bingung saat test di Unity)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
