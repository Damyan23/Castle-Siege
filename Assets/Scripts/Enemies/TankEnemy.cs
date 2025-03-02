using UnityEngine;

public class TankEnemy : Enemy
{
    public GameObject projectilePrefab; // Assign the cannonball prefab here
    public Transform shootPoint;        // Optional: shooting point transform
    public float shootAngle = 45f;      // Angle in degrees
    public float projectileSpeed = 0.2f;
    public float arcHeight = 5f;        // Height of the projectile arc

    protected override void Start()
    {
        // Tank enemy stats
        maxHealth = 250f;
        damage = 25f;
        attackSpeed = 0.2f;

        base.Start();

        agent.speed = 2f;
    }

    protected override void AttackTarget()
    {
        if (Time.time >= nextAttackTime && projectilePrefab != null && target != null)
        {
            LaunchProjectile();
            nextAttackTime = Time.time + 1f / attackSpeed;
        }
    }

    protected void LaunchProjectile()
    {
    //     // Instantiate the projectile
    //     GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

    //     // Calculate the target point at the "feet" of the target
    //     float targetHeight = target.GetComponent<Collider>().bounds.size.y;  // Assuming target has a collider
    //     Vector3 targetPoint = new Vector3(target.position.x, target.position.y - targetHeight / 2f, target.position.z);

    //     // Get the projectile script and initialize it
    //     BombProjectile projectileScript = projectile.GetComponent<BombProjectile>();
    //     if (projectileScript != null)
    //     {
    //         projectileScript.Initialize(shootPoint.position, targetPoint, arcHeight, projectileSpeed);
    //     }
    }
}