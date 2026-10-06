using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private LayerMask playerMask;

    private Transform _target;
    private bool _isAttacking;
    private float _cooldownTimer;

    // Exposed so EnemyController can compare distance without duplicating the value
    public float AttackRange => attackRange;
    public bool IsAttacking => _isAttacking;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }

    // Called by EnemyController once the enemy is in range and facing the player
    public void TryAttack(Transform target)
    {
        if (_isAttacking || _cooldownTimer > 0f) return;

        _target = target;
        _isAttacking = true;
        print(12);
        DealDamage();
        // animator.SetTrigger("Attack");
    }

    // Hook this to an Animation Event, placed on the frame where the weapon actually connects
    private void DealDamage()
    {
        if (_target == null) return;

        // Re-check distance in case the player stepped away mid-swing
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance > attackRange) return;

        if (_target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(new DamageInfo { Damage = damage, Attacker = transform });
        }

        AttackFinished();
    }

    // Hook this to an Animation Event, placed on the last frame of the attack clip
    private void AttackFinished()
    {
        _isAttacking = false;
        _cooldownTimer = attackCooldown;
    }
}