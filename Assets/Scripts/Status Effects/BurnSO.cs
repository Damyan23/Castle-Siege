using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "StatusEffects/Burn", fileName = "BurnStatusEffect")]

public class BurnSO : StatusEffectSO
{
    public int tickDamage;

    public override void UpdateEffect(GameObject target)
    {
        base.UpdateEffect(target);

        if (isEffectActive)
        {
            health.TakeDamage (tickDamage);
        }
    }
}
