using UnityEngine;

public class BallistaProjectile : Projectile
{
    protected override void OnHitTarget(Collider target)
    {
        // Check if the target is an enemy
        if (target.gameObject.layer == 8)
        {
            Enemy enemy = target.GetComponentInParent<Enemy> ();
            if (enemy != null)
            {
                enemy.TakeDamage (damage);
            }
            Destroy (gameObject);
        }
    
    }
}
