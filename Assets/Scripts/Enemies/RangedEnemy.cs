using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectilePrefab;
    private bool canAttack;

    protected override void Start()
    {
        damage = 8f;
        attackSpeed = 0.75f;

        base.Start();

        agent.speed = 3f;
    }

    public void CanAttack ()
    {
        canAttack = true;
    }

    protected override void AttackTarget()
    {
        if (!canAttack) return;

        if (Time.time >= nextAttackTime)
        {
            PlaySound ();
            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(
                    projectilePrefab,
                    transform.position + transform.forward,
                    Quaternion.LookRotation((target.position - transform.position).normalized)
                );

                Projectile projectileScript = projectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.damage = damage;
                    projectileScript.target = target;
                }
            }

            nextAttackTime = Time.time + 1f / attackSpeed;

        }
    }
}
