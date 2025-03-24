using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float speed = 3.5f;
    public int carriedMoney = 7;

    [Header("Health Slider Settings")]
    [SerializeField] private float healthLerpSpeed = 2f;
    [SerializeField] private float distanceFromCamTillSliderDissapears = 25f;

    [Header("Movement")]
    public float detectionRange = 10f; // Detection range for the target
    [HideInInspector] public NavMeshAgent agent;
    protected float nextAttackTime;

    [Header("References")]
    private StatusEffectManager effectManager;
    private DamageManager damageManager;
    [SerializeField] private Transform damageLableCanvas;

    [Header("Damage Lable Settings")]
    [SerializeField] private Vector3 lableOffset;
    private Slider health;
    protected Transform target;
    private Camera cam;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip enemySound; // Assign this in the Inspector
    private AudioSource audioSource;

    // Animator reference
    private Animator animator;
    [HideInInspector] public static List<Enemy> activeEnemies = new List<Enemy>();

    void OnEnable ()
    {
        activeEnemies.Add (this);
    }

    void OnDisable ()
    {
        activeEnemies.Remove (this);
    }

    protected virtual void Start()
    {
        effectManager = this.GetComponentInParent<StatusEffectManager> ();
        damageManager = GameManager.instance.DamageManager;

        SetUpAgent();
        cam = Camera.main;
        currentHealth = maxHealth;

        target = GameObject.FindWithTag("Castle")?.transform;
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        SetUpHealthSlider();

        audioSource = GetComponent <AudioSource> ();

        // Try to get an Animator component from this enemy
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // If the enemy has an animator and starts moving, trigger "walk"
            animator.SetBool ("isWalking", true);
        }
    }

    public void PlaySound()
    {
        if (enemySound != null && audioSource != null)
        {
            if (!audioSource.isPlaying) // Prevents overlapping sounds
            {
                audioSource.clip = enemySound;
                audioSource.Play();
            }
        }
    }


    private void SetUpAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.stoppingDistance = 2f; // Set to desired attack range
        agent.speed = speed;
    }

    private void SetUpHealthSlider()
    {
        health = transform.GetComponentInChildren<Slider>();

        if (health != null)
        {
            health.maxValue = this.maxHealth;
            health.value = this.currentHealth;
            health.minValue = 0;
        }
        else
        {
            Debug.LogError("Health slider in enemy " + this.name + " not found");
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(cam.transform.position, transform.position);
        if (distance > distanceFromCamTillSliderDissapears)
        {
            health.GetComponentInParent<Canvas>().enabled = false;
        }
        else
        {
            if (!health.GetComponentInParent<Canvas>().isActiveAndEnabled)
                health.GetComponentInParent<Canvas>().enabled = true;
            health.transform.LookAt(cam.transform.position);
        }

        if (currentHealth != health.value)
            health.value = Mathf.Lerp(health.value, currentHealth, Time.deltaTime * healthLerpSpeed);

        DieLogic();
        AgnetLogic();
    }

    private void DieLogic()
    {
        if (currentHealth <= 0)
        {
            EventManager.OnEnemyDeath(this);
            activeEnemies.Remove(this);
            Destroy(gameObject);
        }
    }

    private void AgnetLogic()
    {
        if (target == null || agent == null || !agent.isOnNavMesh) return;
        


        if (agent.remainingDistance == 0) agent.destination = target.position;

        // Check distance to target
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            // Move toward the target
            agent.isStopped = false;
            agent.destination = target.position;

            // Trigger the "walk" animation if available
            if (animator != null)
            {
                animator.SetBool ("isWalking", true);
                animator.SetBool ("isAttacking", false);
            }
        }
        else
        {
            // Stop moving and attack
            agent.isStopped = true;
            AttackTarget ();
            
            // Trigger the "attack" animation if available
            if (animator != null)
            {
                animator.SetBool ("isAttacking", true);
                animator.SetBool ("isWalking", false);
            }
        }
    }

    protected virtual void AttackTarget()
    {
    }

    public virtual void TakeDamage(DamageData damage)
    {
       // currentHealth -= damage.damage;
        damageManager.ShowDamage(this.transform.position, damage.damage, damageLableCanvas);
        effectManager.OnStatusTriggerBuildUp(damage.effectType, damage.buildAmount, damage.slowFactor);
        TakeDamage (damage.damage);
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
    }
}

#region Damage Data
[Serializable]
public class DamageData
{
    public float damage;
    public StatusEffectType effectType;
    public float buildAmount;
    public float slowFactor;

    public DamageData(float damage, StatusEffectType effectType, float buildAmount, float slowFactor = 0)
    {
        this.damage = damage;
        this.effectType = effectType;
        this.buildAmount = buildAmount;

        if (effectType == StatusEffectType.Lightning)
        {
            this.slowFactor = slowFactor;
        }
        else
        {
            this.slowFactor = 0;
        }
    }
}
#endregion
