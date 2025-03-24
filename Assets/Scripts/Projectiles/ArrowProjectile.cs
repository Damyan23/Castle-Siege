using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : Projectile
{
    protected override void Update()
    {
        base.Update();
        RotateTowardsTarget ();
    }

    protected override void RotateTowardsTarget()
    {
        if (target != null)
        {
           // Get direction to target
            Vector3 directionToTarget = (transform.position - target.position).normalized;

            // Rotate arrow so its forward points in the movement direction
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            // Apply rotation and correct for models facing downward (-Y) instead of forward (Z)
            transform.rotation = lookRotation * Quaternion.Euler(90f, 0f, 0f);
        }
    }


    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Castle"))
        {
            EventManager.OnCastleHit(damage);
            Destroy(gameObject);
        }
    }
}
