/*
 * PooledEffect.cs
 *
 * Particulas y sonido que se reproduce y vuelve al pool al terminar.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.Pool;

public class PooledEffect : MonoBehaviour
{
    [SerializeField] private float _duration = 1.5f;

    private ParticleSystem _particles;
    private AudioSource _audio;
    private IObjectPool<PooledEffect> _pool;
    private float _timer;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
    }

    public void SetPool (IObjectPool<PooledEffect> pool)
    {
        _pool = pool;
    }

    public void Play (Vector3 position)
    {
        transform.position = position;
        _timer = _duration;

        if (_particles != null) _particles.Play(true);
        if (_audio != null) _audio.Play();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if(_timer <= 0f)
        {
            if (_pool != null) _pool.Release(this);
            else Destroy(gameObject);
        }
    }
}
