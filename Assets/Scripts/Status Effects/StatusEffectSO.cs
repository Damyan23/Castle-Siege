using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatusEffectType {Fire, Ice, Lightning}
public class StatusEffectSO : ScriptableObject
{
    public StatusEffectType statusEffectType;

    public float activationThreshold;
    public float thresholdReductionMultiplier = 1f;
    public float thresholdReductionEverySecond = 1f;

    public float actriveDuration;

    public GameObject visualEffectPrefab;

    private float currentThreshold;
    protected float remainingDuration;
    private GameObject vfxPlaying;

    [HideInInspector] public bool isBuildUpOnlyShown; 
    [HideInInspector] public bool isEffectActive; 

    public float tickInterval = 0.5f;
    private float tickIntervalCD;
    protected EnemyHealth health;
    protected Enemy movement;

    public float slowFactor;

    public virtual void AddBuildUp (float buildUpAmount, GameObject target)
    {
        isBuildUpOnlyShown = true;
        currentThreshold += buildUpAmount;

        visualEffectPrefab.transform.localScale *= 3;

        if (currentThreshold >= activationThreshold)
        {
            ApplyEffect (target);
        }
    }

    public virtual void ApplyEffect(GameObject target)
    {

        isEffectActive = true;
        remainingDuration = actriveDuration;

        SetTargetData(target);
        if (visualEffectPrefab != null)
        {
            Debug.Log (target);
            vfxPlaying = Instantiate(visualEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
            vfxPlaying.transform.localScale = Vector3.one * 5;
        }

        var particleSystem = vfxPlaying?.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
}

    
    private void SetTargetData (GameObject target)
    {
        health = target.GetComponent<EnemyHealth> ();   
        movement = target.GetComponent <Enemy>();
    }
    
    public void UpdateCall (GameObject target, float tickAmount)
    {
        if (isEffectActive)
        {
            isBuildUpOnlyShown = false;

            remainingDuration -= tickAmount;

            if (remainingDuration <= 0)
            {
                isEffectActive = false;
            }
        }
        else
        {
            currentThreshold -= tickAmount * thresholdReductionEverySecond * thresholdReductionMultiplier;

            if (currentThreshold <= 0)
            {
                isBuildUpOnlyShown = false;
            }
        }

        tickIntervalCD += tickAmount;

        if (tickIntervalCD >= tickInterval)
        {
            UpdateEffect (target);
            tickIntervalCD = 0;
        }
    }

    public virtual void UpdateEffect (GameObject target)
    {

    }

    public virtual void RemoveEffect (GameObject target)
    {
        isEffectActive = false;
        currentThreshold = 0;
        remainingDuration = 0;

        if (vfxPlaying != null)
        {
            Destroy (vfxPlaying);
        }
    }

    public virtual bool CanStatusVisualBeRemoved ()
    {
        return !(isEffectActive || isBuildUpOnlyShown);
    }

    public float GetCurrentThresholdNormalized ()
    {
        return currentThreshold / activationThreshold;
    }

        public float GetCurrentDurationNormalized ()
    {
        return remainingDuration / actriveDuration;
    }
}
