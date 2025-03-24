using UnityEngine;

public class MeleeEnemy : Enemy
{
    private SwordController sword;
    protected override void Start()
    {
        // Melee enemy stats
        maxHealth = 100f;
        damage = 15f;
        attackSpeed = 1f;

        base.Start();

        agent.speed = 3.5f;

        sword = GetComponentInChildren<SwordController> ();
        sword.damage = damage;
    }

    public void EnableSwrod ()
    {
        sword.swordCollider.enabled = true;
    }

    public void DisableSword ()
    {
        sword.swordCollider.enabled = false;
    }
}