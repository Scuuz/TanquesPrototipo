/*
 * Shell.cs
 *
 * Proyectil del tanque. Se aplica daño al impactar y vuelve al pool al chocar o terminar su tiempo de vida
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Modificación: 05/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Shell : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _damage = 25f;

    private Rigidbody _rb;
    private IObjectPool<Shell> _pool;
    private EffectPool _impactEffectPool;
    private Team _team;
    private float _lifeTimer;
    private bool _isReleased;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(IObjectPool<Shell> pool, EffectPool impactEffectPool, Team team)
    {
        _pool = pool;
        _impactEffectPool = impactEffectPool;
        _team = team;
    }

    public void Launch(Vector3 position, Quaternion rotation, Vector3 direction)
    {
        transform.SetPositionAndRotation(position, rotation);
        _rb.position = position;
        _rb.rotation = rotation;

        _isReleased = false;
        _lifeTimer = _lifeTime;
        _rb.linearVelocity = direction.normalized * _speed;
    }

    private void Update()
    {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        TeamMember member = other.GetComponentInParent<TeamMember>();
        bool isAlly = member != null && member.Team == _team;

        // Tanque aliado: la bala lo atraviesa como si no estuviera
        if (isAlly && member.AlliedShootsPassThrough) return;

        // Torre aliada: la bala explota, pero sin hacer daño
        if (!isAlly)
        {
            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target != null) target.TakeDamage(_damage);
        }

        if (_impactEffectPool != null) _impactEffectPool.Play(transform.position);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_isReleased) return;
        _isReleased = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        if (_pool != null) _pool.Release(this);
        else Destroy(gameObject);
    }
}
