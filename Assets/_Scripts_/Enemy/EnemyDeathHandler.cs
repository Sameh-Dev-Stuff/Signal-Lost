using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        healthSystem.OnDeath += HandleDeath;
        healthSystem.OnDamageTaken += HandleDamageTaken;
    }

    private void OnDisable()
    {
        healthSystem.OnDeath -= HandleDeath;
        healthSystem.OnDamageTaken -= HandleDamageTaken;
    }

    private void HandleDeath()
    {
        print( gameObject.name + " Is Dead");
        
        Destroy(gameObject);
    }
    
    private void HandleDamageTaken(float damageAmount)
    {
        
    }
}
