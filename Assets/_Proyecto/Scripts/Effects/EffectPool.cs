/*
 * EffectPool.cs
 *
 * Object Pool para los efectos visuales. Reutiliza en vez de crearlos y destruirlos.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : MonoBehaviour
{
    [SerializeField] private PooledEffect _effectPrefab;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 30;

    private ObjectPool<PooledEffect> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<PooledEffect>
        (  
            CreateEffect,
            OnGetEffect,
            OnReleaseEffect,
            OnDestroyEffect,
            true,
            _defaultCapacity, 
            _maxSize
        );
    }

    public void Play(Vector3 position)
    {
        PooledEffect effect = _pool.Get();
        effect.Play(position);
    }

    private PooledEffect CreateEffect()
    {
        PooledEffect effect = Instantiate(_effectPrefab, transform);
        effect.SetPool(_pool);
        return effect;
    }

    private void OnGetEffect (PooledEffect effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void OnReleaseEffect(PooledEffect effect)
    {
        effect.gameObject.SetActive(false);
    }

    private void OnDestroyEffect(PooledEffect effect)
    {
        Destroy(effect.gameObject);
    }
}
