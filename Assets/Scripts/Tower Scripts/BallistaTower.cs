using UnityEngine;

public class BallistaTower : Tower
{
    [Header("Ballista Specific Settings")]
    public float projectileSpeed = 30f; // Speed of the projectile
    public int projectileDamage = 50; // Damage dealt by the projectile
    public GameObject ballistaProjectilePrefab; // Assign in Inspector

    protected override void RotateToTarget()
    {
        if (currentTarget == null) return;

        // Rotate around Y-axis (tower base rotation)
        Vector3 directionToTarget = yAxisRotationPoint.position - currentTarget.position;
        directionToTarget.y = 0; // Ignore vertical component for Y-axis rotation
        yAxisRotationPoint.forward = directionToTarget.normalized;

        // Rotate around X-axis
        Vector3 directionForXAxis = xAxisRotationPoint.position - currentTarget.position;
        xAxisRotationPoint.forward = directionForXAxis.normalized;
    }

    protected override void Shoot()
    {
        base.Shoot ();
        if (currentTarget == null || ballistaProjectilePrefab == null) return;

        // Instantiate BallistaProjectile at the shooting point
        GameObject projectileObj = Instantiate(ballistaProjectilePrefab, xAxisRotationPoint.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            // Assign properties
            projectile.target = currentTarget;
            projectile.speed = projectileSpeed;
            projectile.damage = projectileDamage;
        }
    }

}
