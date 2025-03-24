using Cinemachine.Utility;
using Unity.Mathematics;
using UnityEngine;

public class CannonTower : Tower
{
    [Header("Cannon Specific Settings")]
    public float projectileSpeed = 20f; // Speed of the cannonball
    public int explosionDamage = 70;    // Explosion damage
    public float explosionRadius = 5f;  // Radius of the explosion
protected override void RotateToTarget()
{
    if (currentTarget == null) return;

    // Y-axis rotation (tower base)
    Vector3 directionToTarget = currentTarget.position - yAxisRotationPoint.position;
    directionToTarget.y = 0; // Ignore vertical movement for base rotation
    
    // Adjust direction for the 180-degree default offset on Y-axis
    Vector3 adjustedDirectionY = Quaternion.Euler(0, 180, 0) * directionToTarget.normalized;
    
    // Smoothly rotate the Y-axis base
    yAxisRotationPoint.forward = Vector3.Lerp(
        yAxisRotationPoint.forward,
        adjustedDirectionY,
        Time.deltaTime * 5f
    );

    // Calculate direction for barrel rotation
    Vector3 directionToTargetX = currentTarget.position - xAxisRotationPoint.position;
    
    // Get the direction in local space of the Y-axis rotation point
    Vector3 localDirection = yAxisRotationPoint.InverseTransformDirection(directionToTargetX);
    
    // Calculate the rotation only around X axis
    float targetAngle = Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;
    
    // Create the target rotation
    Quaternion targetRotation = Quaternion.Euler(-targetAngle + 90, 0, 0);
    
    // Smoothly rotate the barrel
    xAxisRotationPoint.localRotation = Quaternion.Slerp(
        xAxisRotationPoint.localRotation,
        targetRotation,
        Time.deltaTime * 3f  // Reduced speed for more smoothness
    );
}

    private void OnDrawGizmos()
    {
        if (xAxisRotationPoint == null) return;

        // Draw the forward vector of the xAxisRotationPoint
        Vector3 start = xAxisRotationPoint.position;
        Vector3 end = start + yAxisRotationPoint.forward * 2f; // Scale the forward vector for visibility

        Debug.DrawLine(start, end, Color.red);
    }

    protected override void Shoot()
    {
        base.Shoot();

        // Instantiate the cannonball and set properties
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        CannonballProjectile cannonball = projectileInstance.GetComponent<CannonballProjectile>();
        if (cannonball != null)
        {
            cannonball.speed = projectileSpeed;
            cannonball.damage = towerDamage; // Full damage for direct hit
            cannonball.explosionDamage = explosionDamage;
            cannonball.explosionRadius = explosionRadius;
            cannonball.enemyLayer = enemyLayer;
            cannonball.target = currentTarget;
        }

   
    }
}
