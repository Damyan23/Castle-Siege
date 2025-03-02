using UnityEngine;
using TMPro;

public class FloatingText  : MonoBehaviour
{
    public float moveSpeed = 50f;         // Speed at which the text moves upward
    public float fadeDuration = 1f;       // How long it takes to fade out
    public float floatDistance = 50f;     // Total distance to move upward before disappearing

    private TextMeshProUGUI tmp;
    private float elapsedTime;
    private Vector3 startPosition;
    private Color startColor;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        startPosition = transform.position;
        startColor = tmp.color;
    }

    private void Update()
    {
        this.transform.LookAt (Camera.main.transform.position); 
        transform.Rotate(0, 180, 0);
        // Move the text upward
        float delta = moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.up * delta);

        // Update elapsed time
        elapsedTime += Time.deltaTime;

        // Fade out the text over fadeDuration
        float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
        tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // Optionally, destroy the text after fadeDuration or once it has moved a certain distance
        if (elapsedTime >= fadeDuration || Vector3.Distance(startPosition, transform.position) >= floatDistance)
        {
            Destroy (gameObject.transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}
