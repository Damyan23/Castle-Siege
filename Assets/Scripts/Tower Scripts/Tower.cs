using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (TowerHemisphereIndicator))]
public abstract class Tower : MonoBehaviour
{
    [Header("Tower Settings")]
    public Transform yAxisRotationPoint;
    public Transform xAxisRotationPoint;
    public float detectionRange = 20f;
    public float fireRate = 1f;
    public LayerMask enemyLayer;
    public float towerDamage;
    [HideInInspector] public bool isClicked;
    public int cost = 0;

    [Header("Rotation Settings")]
    public float rotationThreshold = 2f;

    [Header("Weapon Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Audio Settings")]
    [SerializeField] protected AudioClip shootSound; // Assign in Inspector
    protected AudioSource audioSource;

    protected Transform currentTarget;
    private float fireCooldown;
    private Quaternion lastYRotation;
    private Quaternion lastXRotation;

    private TowerUpgradeAnimation upgradeAnim;
    
    private enum TargetingStrategy
    {
        Closest,
        MostHealth,
        Fastest,
        ByTag
    }
    
    private TargetingStrategy currentStrategy = TargetingStrategy.Closest;
    private string targetTag = "";

    protected virtual void Start()
    {
        if (yAxisRotationPoint) lastYRotation = yAxisRotationPoint.rotation;
        if (xAxisRotationPoint) lastXRotation = xAxisRotationPoint.rotation;

        if (gameObject.GetComponent<TowerUpgradeAnimation>() != null) 
            upgradeAnim = gameObject.GetComponent<TowerUpgradeAnimation>();
        if (upgradeAnim != null) upgradeAnim.StartAnim();

        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (currentTarget != null)
        {
            // Have this here so we dont get an error if the current target is null
            float distanceToTarget = Vector3.Distance (this.transform.position, currentTarget.transform.position);
            if (distanceToTarget >= detectionRange) return;

            RotateToTarget();
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }

            if (yAxisRotationPoint) lastYRotation = yAxisRotationPoint.rotation;
            if (xAxisRotationPoint) lastXRotation = xAxisRotationPoint.rotation;
        }
        else
        {
            UpdateTarget();
        }
    }

    protected virtual void Shoot()
    {
        PlayShootSound();
    }

    // Play a shoot sound - Can be overridden
    protected virtual void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private void UpdateTarget()
    {
        if (currentTarget != null && Vector3.Distance (currentTarget.transform.position, this.transform.position) > detectionRange) 
            currentTarget = null;

        switch (currentStrategy)
        {
            case TargetingStrategy.Closest:
                FindClosestTarget();
                break;
            case TargetingStrategy.MostHealth:
                FindTargetWithMostHealth();
                break;
            case TargetingStrategy.Fastest:
                FindFastestTarget();
                break;
            case TargetingStrategy.ByTag:
                FindTargetByTag(targetTag);
                break;
        }
    }

   private void FindClosestTarget()
    {
        if (currentTarget != null) return;

        Enemy[] allObjects = FindObjectsOfType<Enemy>();
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (Enemy enemy in allObjects)
        {
            if (((1 << enemy.gameObject.layer) & enemyLayer) != 0)  
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestTarget = enemy.transform;
                }
            }
        }

        currentTarget = nearestTarget;
    }

    private void FindTargetWithMostHealth()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        float maxHealth = 0;
        float shortestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            Enemy enemyClass = hit.GetComponent<Enemy>();
            if (enemyClass != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position);
                if (enemyClass.maxHealth > maxHealth || 
                    (enemyClass.maxHealth == maxHealth && distanceToEnemy < shortestDistance))
                {
                    maxHealth = enemyClass.maxHealth;
                    shortestDistance = distanceToEnemy;
                    bestTarget = hit.transform;
                }
            }
        }

        currentTarget = bestTarget;
    }

    private void FindFastestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        float maxSpeed = 0;
        float shortestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            Enemy enemyClass = hit.GetComponent<Enemy>();
            if (enemyClass != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position);
                if (enemyClass.speed > maxSpeed || 
                    (enemyClass.speed == maxSpeed && distanceToEnemy < shortestDistance))
                {
                    maxSpeed = enemyClass.speed;
                    shortestDistance = distanceToEnemy;
                    bestTarget = hit.transform;
                }
            }
        }

        currentTarget = bestTarget;
    }

    private void FindTargetByTag(string tag)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        float shortestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(tag))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    bestTarget = hit.transform;
                }
            }
        }

        currentTarget = bestTarget;
    }

    public void TargetEnemiesWithMostHealth()
    {
        currentStrategy = TargetingStrategy.MostHealth;
    }

    public void TargetFastestEnemies()
    {
        currentStrategy = TargetingStrategy.Fastest;
    }

    public void TargetEnemiesByTag(string tag)
    {
        currentStrategy = TargetingStrategy.ByTag;
        targetTag = tag;
    }

    public void ResetToDefaultTargeting()
    {
        currentStrategy = TargetingStrategy.Closest;
        targetTag = "";
    }

    protected abstract void RotateToTarget();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
