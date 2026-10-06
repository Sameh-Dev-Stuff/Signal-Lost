using System;
using NaughtyAttributes;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField, Range(15, 30)] private float speed;
    [SerializeField, Range(0, 4)] private float lifeTime;
    private DamageInfo _damageInfo;

    private void Start()
    {
        // Apply force to move the bullet forward
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        // Destroy the bullet after its lifetime
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable enemyDamageable))
        {
            // Send damage info to the damaged object
            enemyDamageable.TakeDamage(_damageInfo);
        }

        // Destroy the bullet after hitting an object
        Destroy(gameObject);
    }

    // Set the damage and attacker information
    public void SetDamageInfo(float dmg, Transform attacker)
    {
        _damageInfo.Damage = dmg;
        _damageInfo.Attacker = attacker;
    }
}