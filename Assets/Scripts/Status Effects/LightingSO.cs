using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu (menuName = "StatusEffects/Lighting", fileName = "LightingStatusEffect")]

public class LightingSO : StatusEffectSO
{
    private bool isTargetSlowed;

    public override void UpdateEffect(GameObject target)
    {
        base.UpdateEffect(target);

        if (isEffectActive)
        {
            if (!isTargetSlowed)
            {
                movement.agent.speed -= slowFactor;
                isTargetSlowed = true;
            }
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        base.RemoveEffect(target);

        if (isTargetSlowed)
        {
            movement.agent.speed += slowFactor;
            isTargetSlowed = false;
        }
    }
}
