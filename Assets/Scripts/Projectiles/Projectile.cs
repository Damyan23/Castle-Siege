using Unity.VisualScripting;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 10; // Default damage dealt by the projectile
    public float speed = 15f; // Default speed of the projectile
    public float lifetime = 5f; // Time before the projectile is destroyed

    [HideInInspector] public Transform target; // Target to hit, assigned in the prefab or dynamically by the tower
    [SerializeField] private LayerMask towerLayer;
    bool hit;

    protected virtual void Start()
    {
        // Destroy the projectile after its lifetime expires
        Destroy(gameObject, lifetime);

        // Rotate the projectile to face its target if one exists
        if (target != null)
        {
            RotateTowardsTarget();
        }
    }

    protected virtual void RotateTowardsTarget()
    {
        if (target == null) return;

        // Calculate the direction to the target
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Set the forward vector to point towards the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.forward);

        // Adjust the rotation so the arrow's "Y-axis forward" points correctly
        Quaternion arrowAdjustment = Quaternion.Euler(90, 0, 0);

        // Apply the corrected rotation
        transform.rotation = targetRotation * arrowAdjustment;
    }

    protected virtual void Update()
    {
        // Move the projectile towards the target every frame
        MoveTowardsTarget();
    }

    protected void MoveTowardsTarget()
    {
        if (speed > 0 && target != null)
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Move towards the target
            transform.position += directionToTarget * speed * Time.deltaTime;

            // Rotate the projectile to face the target
            RotateTowardsTarget();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == towerLayer || other.CompareTag("Ground")) return;
        //Destroy(gameObject); // Destroy the projectile after hitting its target

        OnHitTarget(other);
    }

    protected virtual void OnHitTarget(Collider target)
    {
        // Implement logic for when the projectile hits the target
    }
}
