using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Castle : MonoBehaviour
{
    [Header("Castle Stats")]
    public float maxHealth = 500f;  // Maximum health of the castle
    public float currentHealth;     // Current health of the castle

    [Header("UI Components")]
    public Slider healthSlider;     // Reference to the health slider
    public TextMeshProUGUI healthText;         // Reference to the health text

    void OnEnable ()
    {
        EventManager.OnCaslteHitEvent += TakeDamage;
    }

    void OnDisable ()
    {
        EventManager.OnCaslteHitEvent -= TakeDamage;
    }

    void Start()
    {
        // Initialize current health to max health
        currentHealth = maxHealth;

        // Update UI
        UpdateHealthUI();

    }

    void Update ()
    {
        
        if (Input.GetKeyDown (KeyCode.J)) TakeDamage (currentHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent negative health

        // Update UI
        UpdateHealthUI();

        // Check if the castle is destroyed
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("The castle has been destroyed!");

        EventManager.OnCastleDeath ();

        // Optionally disable or destroy the castle
        gameObject.SetActive(false);
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
