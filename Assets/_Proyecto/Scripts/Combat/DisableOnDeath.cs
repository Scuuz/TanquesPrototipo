/*
 * DisableOnDeath.cs
 *
 * Cuando muere el objeto reproduce una explosión y lo desactiva.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(Health))]
public class DisableOnDeath : MonoBehaviour
{
    [SerializeField] private EffectPool _deathEffectPool;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        _health.OnDied -= HandleDied;
    }

    private void HandleDied()
    {
        if (_deathEffectPool != null) _deathEffectPool.Play(transform.position);
        gameObject.SetActive(false);
    }
}
