using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private EnemyAttack enemyAttack;
    private Transform _playerTarget;

    [Header("Settings")]
    [SerializeField] private float pathUpdateInterval = 0.2f;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask playerMask;

    private float _pathUpdateTimer;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (enemyAttack == null) enemyAttack = GetComponent<EnemyAttack>();

        // Related to pathfinding and movement (;
        agent.updatePosition = false;
        agent.updateRotation = true;  
        agent.nextPosition = rb.position;
    }

    private void OnEnable()
    {
        healthSystem.OnDamageTaken += HandleDamageTaken;
    }

    private void OnDisable()
    {
        healthSystem.OnDamageTaken -= HandleDamageTaken;
    }

    private void Update()
    {
        // Player Detection
        if (!_playerTarget)
        {
            Collider[] detectionResult = Physics.OverlapSphere(transform.position, detectionRadius, playerMask);

            if (detectionResult.Length > 0)
            {
                _playerTarget = detectionResult[0].transform;
            }

            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _playerTarget.position);

        // Close enough to attack: stop moving, face the player and let EnemyAttack handle the hit
        if (distanceToTarget <= enemyAttack.AttackRange)
        {
            HandleAttackState();
        }
        else
        {
            HandleChaseState();
        }
    }

    private void HandleChaseState()
    {
        agent.isStopped = false;

        // Handle Pathfinding
        _pathUpdateTimer += Time.deltaTime;

        if (_pathUpdateTimer >= pathUpdateInterval)
        {
            agent.SetDestination(_playerTarget.position);
            _pathUpdateTimer = 0f;
        }
    }

    private void HandleAttackState()
    {
        agent.isStopped = true;

        FaceTarget();

        // Don't interrupt an attack animation that's already playing
        if (!enemyAttack.IsAttacking)
        {
            enemyAttack.TryAttack(_playerTarget);
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = _playerTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Handle Movement
    private void FixedUpdate()
    {
        Vector3 velocity = agent.velocity;
 
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        agent.nextPosition = rb.position;
    }

    // Set the attacker as the target when the enemy takes damage
    private void HandleDamageTaken(DamageInfo damageInfo)
    {
        _playerTarget = damageInfo.Attacker;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (enemyAttack != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyAttack.AttackRange);
        }
    }
}