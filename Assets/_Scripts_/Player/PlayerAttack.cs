using System;
using UnityEngine;
using MyUtilities;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform bullet;
    [SerializeField] private InputManager input;

    [Header("Settings")]
    [SerializeField , Range(0, 0.4f)] private float fireRate;

    private void Update()
    {
        if (input.AttackInputIsPressed())
        {
            Utilities.RepeatAction(fireRate, Fire); 
        }
    }

    private void Fire()
    {
        Instantiate(bullet, firePoint.position, firePoint.rotation);
    }
}
