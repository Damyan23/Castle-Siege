using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public float damage = 10f;       // Projectile damage
    public float launchAngle = 45f;  // Launch angle in degrees
    public float initialSpeed = 10f; // Initial launch speed

    private Rigidbody rb;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }
    }

    public void Initialize(Transform targetTransform, float projectileDamage)
    {
        target = targetTransform;
        damage = projectileDamage;
    }

    void Update ()
    {
        // Calculate and apply initial velocity
        Vector3 velocity = CalculateBallisticVelocity(target.position);
        rb.velocity = velocity;
    }

    Vector3 CalculateBallisticVelocity(Vector3 targetPosition)
    {
        // Calculate direction to target
        Vector3 direction = targetPosition - transform.position;
        
        // Horizontal direction and distance
        Vector3 horizontalDirection = direction;
        horizontalDirection.y = 0;
        float horizontalDistance = horizontalDirection.magnitude;

        // Height difference
        float heightDifference = direction.y;

        // Convert launch angle to radians
        float angle = launchAngle * Mathf.Deg2Rad;

        // Calculate initial velocity components
        float velocityY = initialSpeed * Mathf.Sin(angle);
        float velocityXZ = initialSpeed * Mathf.Cos(angle);

        // Adjust for height difference
        float timeToTarget = (2 * velocityY) / Mathf.Abs(Physics.gravity.y);
        float horizontalVelocityScale = horizontalDistance / (velocityXZ * timeToTarget);

        // Combine velocity components
        Vector3 velocity = horizontalDirection.normalized * (velocityXZ * horizontalVelocityScale);
        velocity.y = velocityY;

        return velocity;
    }

    void OnCollisionEnter(Collision collision)
    {

    }
}