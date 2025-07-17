using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Fungsi umum, bisa dipanggil dari Unity dengan parameter string
    /// Contoh: LoadLevel("level1")
    /// </summary>
    public void LoadLevel(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is empty!");
        }
    }

    /// <summary>
    /// Khusus untuk tombol Level 1 (tanpa isi parameter)
    /// </summary>
    public void LoadLevel1()
    {
        LoadLevel("level1");
    }

    /// <summary>
    /// Khusus untuk tombol Level 2 (tanpa isi parameter)
    /// </summary>
    public void LoadLevel2()
    {
        LoadLevel("level2");
    }
}
