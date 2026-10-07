/*
 * PlayerRespawn.cs
 *
 * Muerte y reaparicion del player. Tiene una invulnerabilidad temporal al respawnear. Se registra en PlayerRegistry.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 29/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private int _playerIndex = 0;
    [SerializeField] private float _respawnDelay = 2.5f;
    [SerializeField] private float _invulnerabilityTime = 2f;
    [SerializeField] private EffectPool _deathEffectPool;
    [SerializeField] private GameObject _model;
    [SerializeField] private Behaviour[] _disableWhileDead;

    public int PlayerIndex => _playerIndex;
    public bool IsAlive => _health != null && !_health.IsDead;

    public event Action OnPlayerEliminated;
    public event Action OnPlayerDied;
    public event Action OnPlayerRespawned;

    private Health _health;
    private Rigidbody _rb;
    private Collider _collider;
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _spawnPosition = transform.position;
        _spawnRotation = transform.rotation;
    }

    private void OnEnable()
    {
        _health.OnDied += HandleDied;
        PlayerRegistry.Register(this);
    }

    private void OnDisable()
    {
        _health.OnDied -= HandleDied;
        PlayerRegistry.Unregister(this);
    }

    private void HandleDied()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (_deathEffectPool != null) _deathEffectPool.Play(transform.position);
        SetAlive (false);

        bool canRespawn = GameManager.Instance.TryUseRespawn(_playerIndex);
        if (!canRespawn)
        {
            OnPlayerEliminated?.Invoke();
            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.NotifyPlayerEliminated();
            yield break;
        }

        OnPlayerDied?.Invoke();

        yield return new WaitForSeconds(_respawnDelay);

        _rb.position = _spawnPosition;
        _rb.rotation = _spawnRotation;
        transform.SetPositionAndRotation(_spawnPosition, _spawnRotation);
        _health.ResetHealth();
        SetAlive(true);
        OnPlayerRespawned?.Invoke();

        _health.IsInvulnerable = true;
        yield return new WaitForSeconds(_invulnerabilityTime);
        _health.IsInvulnerable = false;
    }

    private void SetAlive(bool alive)
    {
        if (_model != null) _model.SetActive(alive);
        _collider.enabled = alive;
        
        if (!alive)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        _rb.isKinematic = !alive;

        foreach (Behaviour behaviour in _disableWhileDead)
        {
            if (behaviour != null) behaviour.enabled = alive;
        }
    }
}
