using UnityEngine;
using System.Collections;

public class TowerUpgradeAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float stretchAmountY = 1.2f; // Amount of vertical stretching
    [SerializeField] private float stretchAmountXZ = 1.1f; // Amount of horizontal stretching
    [SerializeField] private float stretchDuration = 0.5f; // Duration of stretching
    [SerializeField] private float bounceAmount = 0.8f; // How much it compresses before recovering
    [SerializeField] private float bounceBackDuration = 0.3f; // Duration of bounce-back
    [SerializeField] private float recoveryDuration = 0.5f; // Time to return to original size

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = Vector3.one;
    }

    public void StartAnim()
    {
        StartCoroutine(StretchAndBounce());
    }

    private IEnumerator StretchAndBounce()
    {
        // Stretch upward and outward
        float elapsedTime = 0f;
        while (elapsedTime < stretchDuration)
        {
            float t = elapsedTime / stretchDuration;

            // Smooth stretch using a sine function
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            transform.localScale = new Vector3(
                originalScale.x * (1f + (stretchAmountXZ * smoothT)),
                originalScale.y * (1f + (stretchAmountY * smoothT)),
                originalScale.z * (1f + (stretchAmountXZ * smoothT))
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Bounce back compression
        elapsedTime = 0f;
        while (elapsedTime < bounceBackDuration)
        {
            float t = elapsedTime / bounceBackDuration;

            // Smooth compression
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.localScale = new Vector3(
                originalScale.x * (1f - (bounceAmount * smoothT)),
                originalScale.y * (1f - (bounceAmount * smoothT)),
                originalScale.z * (1f - (bounceAmount * smoothT))
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Recover to original scale
        elapsedTime = 0f;
        while (elapsedTime < recoveryDuration)
        {
            float t = elapsedTime / recoveryDuration;

            // Smooth recovery
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale,
                t
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure exact original scale after recovery
        transform.localScale = originalScale;
    }
}
