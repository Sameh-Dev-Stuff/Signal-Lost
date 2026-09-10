using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class HealthSystem : MonoBehaviour , IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField, ReadOnly] private float currentHealth;

    public bool IsDead => currentHealth <= 0;

    public event Action<float> OnDamageTaken;
    public event Action OnDeath;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnDamageTaken?.Invoke(damage);
        
        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }
    
    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
    
    [Button]
    private void DebugTakeDamage()
    {
        if (IsDead) return;
        
        currentHealth = Mathf.Max(0, currentHealth - 10);
        OnDamageTaken?.Invoke(10);
        
        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }
    
    [Button]
    private void DebugAddHealth()
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + 10);
    }
}
