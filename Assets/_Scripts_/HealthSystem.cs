using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    public bool IsDead => currentHealth <= 0;

    public event Action<DamageInfo> OnDamageTaken;
    public event Action OnDeath;

    private void Awake()
    {
        // Set health to maximum at the start
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead) return;

        // Reduce health and prevent negative values
        currentHealth = Mathf.Max(0, currentHealth - damageInfo.Damage);

        // Notify other systems when damage is taken
        OnDamageTaken?.Invoke(damageInfo);

        // Notify other systems when the character dies
        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }

    public void AddHealth(float amount)
    {
        if (IsDead == false)
        {
            // Restore health without exceeding the maximum
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }
    }
}