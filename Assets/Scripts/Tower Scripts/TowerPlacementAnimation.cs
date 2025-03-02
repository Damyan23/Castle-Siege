using UnityEngine;
using System.Collections;

public class TowerPlacementAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float dropHeight = 10f;
    [SerializeField] private float dropDuration = 1f;
    [SerializeField] private float squashAmount = 0.3f;
    [SerializeField] private float stretchAmount = 0.2f;
    [SerializeField] private float bounceBackDuration = 0.3f;
    
    [Header("VFX Settings")]
    [SerializeField] private GameObject impactVFXPrefab;
    [SerializeField] private Vector3 vfxOffset = Vector3.zero;
    [SerializeField] private Vector3 vfxScale = Vector3.one;
    [SerializeField] private bool destroyVFXAfterDelay = true;
    [SerializeField] private float vfxLifetime = 2f;
    [Tooltip("When to spawn VFX before impact (0 = at impact, 1 = at start of fall)")]
    [Range(0f, 1f)]
    [SerializeField] private float vfxSpawnTiming = 0f;
    
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool vfxSpawned = false;
    
    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position;
        vfxSpawned = false;
    }

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }

    public void StartAnim ()
    {
        // Start the drop animation
        StartCoroutine(DropAndSquash());
    }
    
    IEnumerator DropAndSquash()
    {
        // Cache the original position in world space
        Vector3 startPos = originalPosition + Vector3.up * dropHeight;
        Vector3 endPos = originalPosition;

        // Ensure the initial position is set correctly in world space
        transform.position = startPos;

        float elapsedTime = 0f;

        while (elapsedTime < dropDuration)
        {
            // Calculate drop progress
            float t = elapsedTime / dropDuration;

            // Spawn VFX if timing matches
            if (!vfxSpawned && t >= (1f - vfxSpawnTiming))
            {
                SpawnImpactVFX();
                vfxSpawned = true;
            }

            // Apply vertical stretch during the fall
            float stretchFactor = 1f + (stretchAmount * (1f - t));
            transform.localScale = new Vector3(
                originalScale.x / stretchFactor,
                originalScale.y * stretchFactor,
                originalScale.z / stretchFactor
            );

            // Linearly interpolate position along the world Y-axis
            transform.position = Vector3.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Spawn VFX at impact if we haven't already
        if (!vfxSpawned)
        {
            SpawnImpactVFX();
        }

        // Start squash and recovery animation
        StartCoroutine(SquashAndRecover());
    }


    
    void SpawnImpactVFX()
    {
        if (impactVFXPrefab != null)
        {
            // Calculate spawn position at bottom center of tower
            Vector3 spawnPosition = transform.position + vfxOffset;
            spawnPosition.y = transform.position.y - (transform.localScale.y * 0.5f);
            
            // Spawn and scale the VFX
            GameObject vfx = Instantiate(impactVFXPrefab, spawnPosition, Quaternion.identity);
            vfx.transform.localScale = Vector3.Scale(vfx.transform.localScale, vfxScale);
            
            // Optionally destroy the VFX after delay
            if (destroyVFXAfterDelay)
            {
                Destroy(vfx, vfxLifetime);
            }
        }
    }
    
    IEnumerator SquashAndRecover()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < bounceBackDuration)
        {
            // Calculate recovery progress
            float t = elapsedTime / bounceBackDuration;
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f); // Smooth easing
            
            // Calculate squash factor
            float squashFactor = 1f + (squashAmount * (1f - smoothT));
            
            // Apply squash scale
            transform.localScale = new Vector3(
                originalScale.x * squashFactor,
                originalScale.y / squashFactor,
                originalScale.z * squashFactor
            );
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Reset to original scale
        transform.localScale = originalScale;
    }
}