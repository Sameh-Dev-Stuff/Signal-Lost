using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    private HealthSystem _healthSystem;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        _healthSystem.OnDeath += HandleDeath;
        _healthSystem.OnDamageTaken += HandleDamageTaken;
    }

    private void OnDisable()
    {
        _healthSystem.OnDeath -= HandleDeath;
        _healthSystem.OnDamageTaken -= HandleDamageTaken;
    }

    private void HandleDeath()
    {
        print( gameObject.name + " Is Dead");
        
        Destroy(gameObject);
    }
    
    private void HandleDamageTaken(DamageInfo damageInfo)
    {
        
    }
}
