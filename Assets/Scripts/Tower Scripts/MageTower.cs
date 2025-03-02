using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MageTower : Tower
{
    private Lightning lighting;

    private DamageData damage;

    public float slowFactor;

    void OnEnable ()
    {
        lighting = FindObjectOfType<Lightning> ();
    }

    protected override void Start ()
    {
        lighting.start = firePoint;

        damage = new DamageData (towerDamage, StatusEffectType.Lightning, slowFactor);

           // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected override void Update ()
    {
        base.Update ();
        if (damage.slowFactor == 0 && slowFactor != 0) damage.slowFactor = slowFactor;
    }

    protected override void RotateToTarget()
    {
        // Rotate the tower around the Y-axis to face the target
        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - yAxisRotationPoint.position;
            direction.y = 0; // Ignore vertical differences
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            yAxisRotationPoint.rotation = Quaternion.Lerp(yAxisRotationPoint.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        lighting.ZapTarget(currentTarget.gameObject, damage, audioSource);
    }
}
