using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusEffectManager : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<StatusEffectType, StatusEffectSO> statusEffectToApplyDic = new ();
    private SerializableDictionary<StatusEffectType, StatusEffectSO> enabledEffects = new ();
    private Dictionary <StatusEffectType, StatusEffectSO> statusEffectCacheDict = new Dictionary<StatusEffectType, StatusEffectSO> ();

    [SerializeField, Tooltip("Run the updateCall in StatusEffectSO every what interval?")] private float interval = 0.1f;
    private float currentInterval = 0f;
    private float lastInterval = 0f;

    public UnityAction<StatusEffectSO, float> ActivateEffect;
    public UnityAction<StatusEffectSO> DeactivateEffect;    
    public UnityAction<StatusEffectSO, float, float> UpdateEffect;  

    // Update is called once per frame
    void Update()
    {
        currentInterval += Time.deltaTime;

        if (currentInterval > lastInterval + interval)
        {
            UpdateEffects (gameObject);
            lastInterval = currentInterval;
        }
    }

    public void OnStatusTriggerBuildUp(StatusEffectType effectType, float buildUpAmount, float slowFactor = 0)
    {
        if (!enabledEffects.ContainsKey(effectType))
        {
            var effectToAdd = CreateEffectObject(effectType, statusEffectToApplyDic[effectType]);

            enabledEffects[effectType] = effectToAdd;

            ActivateEffect?.Invoke(effectToAdd, effectToAdd.GetCurrentDurationNormalized());
        }

        if (!enabledEffects[effectType].isEffectActive)
        {
            enabledEffects[effectType].AddBuildUp(buildUpAmount, gameObject);

            UpdateEffect?.Invoke(enabledEffects[effectType], enabledEffects[effectType].GetCurrentThresholdNormalized(),
                                  enabledEffects[effectType].GetCurrentDurationNormalized());
        }
        else
        {
            if (effectType == StatusEffectType.Lightning)
            {
                enabledEffects[effectType].slowFactor = slowFactor;
                
            }
        }
    }

    private StatusEffectSO CreateEffectObject (StatusEffectType effectType, StatusEffectSO effectSO)
    {
        if (!statusEffectCacheDict.ContainsKey (effectType))
        {
            statusEffectCacheDict[effectType] = Instantiate (effectSO);
        }

        return statusEffectCacheDict[effectType];
    }

    public void UpdateEffects (GameObject target)
    {
        foreach (var effect in enabledEffects.ToList()) 
        {
            effect.Value.UpdateCall (target, interval);

            UpdateEffect?.Invoke (effect.Value, effect.Value.GetCurrentThresholdNormalized(), effect.Value.GetCurrentDurationNormalized());

            if (effect.Value.CanStatusVisualBeRemoved())
            {
                RemoveEffect (effect.Key);
            }
        }
    }


    public void RemoveEffect (StatusEffectType effectType)
    {
        if (enabledEffects.ContainsKey (effectType))
        {
            enabledEffects[effectType].RemoveEffect (gameObject);

            DeactivateEffect?.Invoke (enabledEffects[effectType]);

            enabledEffects.Remove (effectType);
        }
    }
}
