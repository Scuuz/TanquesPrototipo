/*
 * EnemyAI.cs
 *
 * AI del enemy, estados y patrulla
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 27/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {Patrol, Chase, Attack}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TankShooting))]

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private int _patrolPointCount = 3;
    [SerializeField] private float _patrolRadius = 12f;
    [SerializeField] private float _waypointTolerance = 1.5f;

    [Header("Detection")]
    [SerializeField] private float _detectionRange = 15f;
    [SerializeField] private float _attackRange = 10f;

    [Header("Aim")]
    [SerializeField] private float _aimTurnSpeed = 120f;
    [SerializeField] private float _aimTolerance = 8f;

    public bool IsAlerted {get; set;}
    public EnemyState CurrentState => _state;

    private NavMeshAgent _agent;
    private TankShooting _shooting;
    private Transform _player;
    private EnemyState _state = EnemyState.Patrol;
    private Vector3[] _patrolPoints;
    private int _patrolIndex;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _shooting = GetComponent<TankShooting>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) _player = playerObject.transform;
        else Debug.LogError($"{name}: no hay ningún objeto con el tag de 'Player' en la escena.");

        GeneratePatrolPoints();
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        if (_player == null) return;

        switch(_state)
        {
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Attack: UpdateAttack(); break;
        }
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        bool arrived = !_agent.pathPending && _agent.remainingDistance <= _waypointTolerance;
        bool unreachable = _agent.pathStatus == NavMeshPathStatus.PathInvalid;
        if (arrived || unreachable)
        {
            GoToNextPatrolPoint();
        }
    }

    private void UpdateChase()
    {
        float distance = DistanceToPlayer();

        if (distance > _detectionRange * 1.5f && !IsAlerted)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        if (distance <= _attackRange && HasLineOfSight())
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        _agent.SetDestination(_player.position);
    }

    private void UpdateAttack()
    {
        if (DistanceToPlayer() > _attackRange * 1.2f || !HasLineOfSight())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        AimAtPlayer();
        if (IsAimedAtPlayer())
        {
            _shooting.TryFire();
        }
    }

    private void ChangeState(EnemyState newState)
    {
        _state = newState;
        _agent.isStopped = (newState == EnemyState.Attack);

        if (newState == EnemyState.Patrol)
        {
            GoToNextPatrolPoint();
        }
    }

    private void GeneratePatrolPoints()
    {
        _patrolPoints = new Vector3[_patrolPointCount];

        for (int i = 0; i < _patrolPointCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * _patrolRadius;
            Vector3 candidate = transform.position + new Vector3(offset.x, 0f, offset.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, _patrolRadius, NavMesh.AllAreas))
                _patrolPoints[i] = hit.position;
            else
                _patrolPoints[i] = transform.position;
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0) return;

        _agent.SetDestination(_patrolPoints[_patrolIndex]);
        _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
    }

    private bool CanSeePlayer()
    {
        if (IsAlerted) return true;
        return DistanceToPlayer() <= _detectionRange && HasLineOfSight();
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = _shooting.FirePoint.position;
        Vector3 toPlayer = (_player.position + Vector3.up) - origin;

        if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit, toPlayer.magnitude + 1f, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.transform == _player || hit.transform.IsChildOf(_player);
        }
        return false;
    }

    private void AimAtPlayer()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _aimTurnSpeed * Time.deltaTime);
    }

    private bool IsAimedAtPlayer()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;
        return Vector3.Angle(transform.forward, direction) <= _aimTolerance;
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        if (_patrolPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (Vector3 point in _patrolPoints) Gizmos.DrawSphere(point, 0.4f);
    }
}
