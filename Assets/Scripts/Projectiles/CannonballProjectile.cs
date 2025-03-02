using UnityEngine;
using System.Collections.Generic;

public class CannonballProjectile : Projectile
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public int explosionDamage = 70;
    public float explosionFalloffFactor = 0.5f;
    public LayerMask enemyLayer;

    [Header("Bezier Curve Settings")]
    public float apexHeight = 10f;
    public float projectileSpeed = 10f;

    [Header("VFX Settings")]
    public GameObject explosionVFX; // Assign an explosion VFX prefab in the Inspector

    private Vector3 startPoint;
    private Vector3 controlPoint;
    private Vector3 endPoint;
    private float t;
    private bool isInitialized = false;

    new void Start()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        startPoint = transform.position;
        endPoint = target.position;
        Vector3 midPoint = (startPoint + endPoint) / 2;
        controlPoint = midPoint + Vector3.up * apexHeight;

        t = 0f;
        isInitialized = true;
    }

    new void Update()
    {
        if (!isInitialized || target == null)
        {
            Destroy(gameObject);
            return;
        }

        t += Time.deltaTime * projectileSpeed;

        if (t >= 1f)
        {
            Explode();
            return;
        }

        Vector3 newPosition = QuadraticCurve.Evaluate(startPoint, controlPoint, endPoint, t);
        transform.position = newPosition;

        if (t < 1f)
        {
            Vector3 nextPosition = QuadraticCurve.Evaluate(startPoint, controlPoint, endPoint, t + 0.01f);
            Vector3 moveDirection = (nextPosition - transform.position).normalized;
            transform.forward = moveDirection;
        }
    }

    private void Explode()
    {
        List<Enemy> hitEnemies = new List<Enemy>();
        foreach (Enemy enemy in Enemy.activeEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRadius)
            {
                hitEnemies.Add(enemy);
            }
        }

        Debug.Log(hitEnemies.Count);
        foreach (Enemy enemy in hitEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            float damageMultiplier = Mathf.Clamp01(1f - (distance / explosionRadius));
            int finalDamage = Mathf.RoundToInt(explosionDamage * damageMultiplier);

            if (finalDamage > 0)
            {
                enemy.TakeDamage(finalDamage);
            }
        }

        PlayExplosionVFX(); // Trigger explosion effect

        Destroy(gameObject);
    }

    private void PlayExplosionVFX()
    {
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * explosionRadius; // Scale effect to match explosion range
            Destroy(vfx, 2f); // Auto-destroy VFX after 2 seconds
        }
    }
}
