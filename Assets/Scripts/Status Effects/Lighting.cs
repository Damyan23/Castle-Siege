using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [HideInInspector] public Transform start;
    [SerializeField] public Material material;
    private GameObject currentTarget;
    private LineRenderer lineRend;
    
    [Header("Lightning Settings")]
    [SerializeField] private float inaccuracy = 0.3f;
    [SerializeField] private float timeOfZap = 0.25f;
    [SerializeField] private float segmentLength = 0.5f;
    
    [Header("Chain Settings")]
    [SerializeField] private float chainRadius = 10f;
    [SerializeField] private int maxChains = 3;
    [SerializeField] private float widthDiminishFactor = 0.7f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float mainBeamWidth = 0.5f;
    
    [HideInInspector] public float zapTimer;
    private List<LineRenderer> chainLines = new List<LineRenderer>();
    private List<GameObject> hitTargets = new List<GameObject>();
    private float initialWidth;

    private DamageData damage;

    private AudioSource audioSource; // Reference to MageTower's AudioSource

    void Start()
    {
        SetupLineRenderer();
        initialWidth = mainBeamWidth;
    }

    private void SetupLineRenderer()
    {
        lineRend = gameObject.GetComponent<LineRenderer>();
        if (lineRend == null)
        {
            lineRend = gameObject.AddComponent<LineRenderer>();
        }

        lineRend.material = material;
        lineRend.startWidth = mainBeamWidth;
        lineRend.endWidth = mainBeamWidth;
        lineRend.positionCount = 1;
    }

    void Update()
    {
        if (zapTimer > 0)
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.loop = true;  // Ensure looping
                audioSource.Play();
            }
            UpdateLightning();
            zapTimer -= Time.deltaTime;
        }
        else
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.loop = false;  // Ensure looping
                audioSource.Stop();
            }
            ClearAllLightning();
        }
    }


    private void UpdateLightning()
    {
        if (currentTarget == null)
        {
            ClearAllLightning();
            return;
        }

        // Generate positions for main lightning bolt
        Vector3 startPos = start.position;
        Vector3 endPos = currentTarget.transform.position;
        GenerateLightningPositions(lineRend, startPos, endPos);

        // If we don't have any targets in the list, add the current target
        if (hitTargets.Count == 0)
        {
            hitTargets.Add(currentTarget);
        }

        // Find and create chain targets if we don't have enough
        if (hitTargets.Count <= maxChains)
        {
            FindAndCreateChains(hitTargets[hitTargets.Count - 1], hitTargets.Count, damage);
        }

        // Update positions for all chain lightnings
        UpdateChainPositions();
    }

    private void UpdateChainPositions()
    {
        // Make sure we have enough line renderers for our chains
        while (chainLines.Count < hitTargets.Count - 1)
        {
            CreateChainLightning(null, null, chainLines.Count);
        }

        // Update each chain's positions
        for (int i = 0; i < hitTargets.Count - 1; i++)
        {
            if (i < chainLines.Count && hitTargets[i] != null && hitTargets[i + 1] != null)
            {
                Vector3 start = hitTargets[i].transform.position;
                Vector3 end = hitTargets[i + 1].transform.position;
                GenerateLightningPositions(chainLines[i], start, end);
            }
        }
    }

    private void FindAndCreateChains(GameObject sourceTarget, int chainIndex, DamageData damage)
    {
        if (chainIndex > maxChains || sourceTarget == null) return;

        GameObject bestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Enemy enemy in Enemy.activeEnemies)
        {
            GameObject enemyObj = enemy.gameObject;

            // Exclude already hit targets
            if (!hitTargets.Contains(enemyObj) && enemyObj != sourceTarget)
            {
                float distance = Vector3.Distance(sourceTarget.transform.position, enemyObj.transform.position);

                // Pick the closest valid target
                if (distance < closestDistance && distance <= chainRadius)
                {
                    closestDistance = distance;
                    bestTarget = enemyObj;
                }
            }
        }

        if (bestTarget != null)
        {
            // Add the target to the list and apply damage
            hitTargets.Add(bestTarget);
            Enemy enemyComponent = bestTarget.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(damage);
            }
            else
            {
                Debug.LogError($"Enemy component not found on {bestTarget.name}");
            }

            // Create the chain lightning visual effect
            if (chainLines.Count < chainIndex)
            {
                CreateChainLightning(sourceTarget, bestTarget, chainIndex);
            }

            // Recursively find the next chain
            FindAndCreateChains(bestTarget, chainIndex + 1, damage);
        }
        else
        {
            Debug.LogWarning("No valid target found for chain lightning.");
        }
    }

    private void CreateChainLightning(GameObject from, GameObject to, int chainIndex)
    {
        GameObject chainObj = new GameObject($"ChainLightning_{chainIndex}");
        chainObj.transform.parent = transform;

        LineRenderer chainLineRend = chainObj.AddComponent<LineRenderer>();
        chainLineRend.material = lineRend.material;

        float chainWidth = initialWidth - (widthDiminishFactor * chainIndex);

        chainLineRend.startWidth = chainWidth;
        chainLineRend.endWidth = chainWidth;
        
        chainLines.Add(chainLineRend);
    }

    private void ClearAllLightning()
    {
        lineRend.positionCount = 1;
        foreach (LineRenderer chain in chainLines)
        {
            if (chain != null)
            {
                Destroy(chain.gameObject);
            }
        }
        chainLines.Clear();
        hitTargets.Clear();
    }

    public void ZapTarget(GameObject newTarget, DamageData damage, AudioSource mageTowerAudio)
    {
        if (newTarget == null) return;

        currentTarget = newTarget;
        zapTimer = timeOfZap;

        // Set MageTower's audio source
        audioSource = mageTowerAudio;

        // Clear previous lightning effects
        ClearAllLightning();

        // Start the chain lightning process
        hitTargets.Clear();
        hitTargets.Add(newTarget);

        newTarget.GetComponent<Enemy>()?.TakeDamage(damage);

        // Create chains and damage subsequent targets
        FindAndCreateChains(newTarget, 1, damage);
    }

    private void GenerateLightningPositions(LineRenderer lr, Vector3 start, Vector3 end)
    {
        // Apply height offset
        start.y += currentTarget.transform.localScale.y / 2;
        end.y += currentTarget.transform.localScale.y / 2;

        float distance = Vector3.Distance(start, end);
        int segments = Mathf.Max(3, Mathf.CeilToInt(distance / segmentLength));
        Vector3[] positions = new Vector3[segments + 1];
        
        positions[0] = start;
        positions[segments] = end;
        
        Vector3 direction = (end - start);
        float distancePerSeg = distance / segments;

        for (int i = 1; i < segments; i++)
        {
            float segmentDistance = ((float)i / segments);
            Vector3 basePos = Vector3.Lerp(start, end, segmentDistance);
            
            Vector3 perpendicular = Vector3.Cross(direction, Random.insideUnitSphere).normalized;
            float displacement = (Random.Range(-2f, 2f) * inaccuracy * distance / 5f); // Increase displacement
            displacement *= segmentDistance * (1 - segmentDistance) * 4;
            
            positions[i] = basePos + (perpendicular * displacement);
        }
        
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);
    }
}