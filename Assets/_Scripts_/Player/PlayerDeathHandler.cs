using System;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{ 
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable() => healthSystem.OnDeath += HandleDeath;
    private void OnDisable() => healthSystem.OnDeath -= HandleDeath;

    private void HandleDeath()
    {
        print("You Are Dead");
    }
}