using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageLabel : MonoBehaviour
{
    [HideInInspector] public TMP_Text damageText; // Reference to the UI Text element
    public float moveSpeed = 1f; // Speed at which the text moves up
    public float fadeDuration = 1f; // Time for the text to fade out

    private float elapsedTime = 0f;
    private Color initialColor;

    void Start()
    {
        if (damageText != null)
        {
            initialColor = damageText.color;
        }

        damageText = this.GetComponent <TMP_Text> ();
    }

    void Update()
    {
        // Move the label upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); // Flip if needed

        // Fade out the text
        elapsedTime += Time.deltaTime;
        if (damageText != null)
        {
            float alpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / fadeDuration);
            damageText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
        }

        // Destroy the label after it fades out
        if (elapsedTime >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }

    // Set the damage value for the label
    public void SetDamage(float damage)
    {
        if (damageText != null)
        {
            damageText.text = damage.ToString();
        }
    }
}
