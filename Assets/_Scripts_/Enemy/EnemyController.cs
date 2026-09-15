using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;
    
    [Header("Settings")]
    [SerializeField] private float pathUpdateInterval = 0.2f;

    private float _pathUpdateTimer;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (rb == null) rb = GetComponent<Rigidbody>();

        agent.updatePosition = false;
        agent.updateRotation = true;
        agent.nextPosition = rb.position;
    }

    private void Update()
    {
        _pathUpdateTimer += Time.deltaTime;
        if (_pathUpdateTimer >= pathUpdateInterval)
        {
            agent.SetDestination(target.position);
            _pathUpdateTimer = 0f;
        }
    }


    private void FixedUpdate()
    {
        Vector3 velocity = agent.velocity;
        
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        agent.nextPosition = rb.position;
    }
}