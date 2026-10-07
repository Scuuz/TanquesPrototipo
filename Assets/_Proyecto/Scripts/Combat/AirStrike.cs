/*
 * AirStrike.cs
 *
 * Ataque aereo: marca el area en el piso y deja caer un misil. Daña al jugador dentro del radio y vuelve al pool al terminar
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class AirStrike : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform _marker;
    [SerializeField] private Transform _markerFill;
    [SerializeField] private Transform _missile;

    [Header("Ataque")]
    [SerializeField] private float _warningTime = 1.5f;
    [SerializeField] private float _radius = 3.5f;
    [SerializeField] private float _damage = 40f;
    [SerializeField] private float _dropHeight = 30f;

    private IObjectPool<AirStrike> _pool;
    private EffectPool _explosionPool;

    public void Init(IObjectPool<AirStrike> pool, EffectPool explosionPool)
    {
        _pool = pool;
        _explosionPool = explosionPool;
    }

    public void Launch(Vector3 position)
    {
        position.y = 0f;
        transform.position = position;
        StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        float diameter = _radius * 2f;
        _marker.localScale = new Vector3(diameter, 0.01f, diameter);

        float elapsed = 0f;
        while (elapsed < _warningTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _warningTime);

            float fill = diameter * t;
            _markerFill.localScale = new Vector3(fill, 0.01f, fill);

            _missile.localPosition = Vector3.up * Mathf.Lerp(_dropHeight, 0f, t * t);

            yield return null;
        }

        Explode();
    }

    private void Explode()
    {
        if (_explosionPool != null) _explosionPool.Play(transform.position);

        foreach (PlayerRespawn player in PlayerRegistry.Players)
        {
            if (player == null || !player.IsAlive) continue;

            Vector3 offset = player.transform.position - transform.position;
            offset.y = 0f;
            if (offset.magnitude <= _radius)
            {
                player.GetComponent<Health>().TakeDamage(_damage);
            }
        }

        if (_pool != null) _pool.Release(this);
        else Destroy(gameObject);
    }
}
