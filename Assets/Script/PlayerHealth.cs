using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public Image healthFillImage; // Drag & drop Image dari UI Health Bar Fill di Inspector

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Kurangi darah
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        Debug.Log("Pemain terkena damage: " + damage);
    }

    // Tambah darah
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        Debug.Log("Pemain disembuhkan: " + amount);
    }

    // Update UI Health Bar
    void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthFillImage.fillAmount = fillAmount;
        }
    }

    // Tombol uji manual (optional)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10); // Uji damage manual
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(5); // Uji heal manual
        }
    }

    // Jika bertabrakan dengan musuh (Collider biasa)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10);
            Debug.Log("Tabrakan dengan Enemy");
        }
    }

    // Jika menyentuh Trigger (Jebakan, musuh, dll)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ambil tag objek yang disentuh
        string tag = other.tag;

        // Deteksi hanya objek yang benar-benar menyebabkan damage
        if (tag == "Trap")
        {
            TakeDamage(10);
            Debug.Log("Tersentuh jebakan (Trap)");
        }
        else if (tag == "Enemy")
        {
            TakeDamage(10);
            Debug.Log("Tersentuh musuh (Enemy Trigger)");
        }

        // Objek lain seperti "Coin", "PowerUp", dll tidak akan menyebabkan damage
    }
}
