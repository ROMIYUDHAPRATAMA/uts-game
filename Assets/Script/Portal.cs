using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Nama Scene Tujuan")]
    public string sceneToLoad;

    [Header("Delay Sebelum Teleport (detik)")]
    public float delayBeforeTeleport = 1.5f;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Portal] Trigger terdeteksi oleh: {other.gameObject.name}");

        if (isTeleporting)
        {
            Debug.Log("[Portal] Teleport sedang berlangsung. Abaikan.");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("[Portal] Pemain masuk portal. Mulai proses teleportasi...");
            isTeleporting = true;
            StartCoroutine(DelayTeleport());
        }
        else
        {
            Debug.Log($"[Portal] Objek {other.gameObject.name} bukan Player. Abaikan.");
        }
    }

    private System.Collections.IEnumerator DelayTeleport()
    {
        Debug.Log($"[Portal] Menunggu {delayBeforeTeleport} detik sebelum teleport...");
        yield return new WaitForSeconds(delayBeforeTeleport);

        Debug.Log($"[Portal] Memuat scene baru: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }
}
