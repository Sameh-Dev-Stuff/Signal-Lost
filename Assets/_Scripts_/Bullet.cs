using NaughtyAttributes;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField, Range(15, 30)] private float speed;
    [SerializeField, Range(0, 4)] private float lifeTime;
    [SerializeField, ReadOnly] private float damage;

    private void Start()
    {
        rb.AddForce(transform.forward * speed , ForceMode.Impulse);
        
        Destroy(gameObject, lifeTime);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }
    
}