/*
 * AirStrikePool.cs
 *
 * Singleton con el Object Pool del ataque aereo. Las torres piden un ataque en una posicion
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.Pool;

public class AirStrikePool : MonoBehaviour
{
    public static AirStrikePool Instance { get; private set; }

    [SerializeField] private AirStrike _strikePrefab;
    [SerializeField] private EffectPool _explosionPool;
    [SerializeField] private int _defaultCapacity = 6;
    [SerializeField] private int _maxSize = 20;

    private ObjectPool<AirStrike> _pool;

    private void Awake()
    {
        Instance = this;

        _pool = new ObjectPool<AirStrike>(
            CreateStrike,
            OnGetStrike,
            OnReleaseStrike,
            OnDestroyStrike,
            true,
            _defaultCapacity,
            _maxSize);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Launch(Vector3 position)
    {
        AirStrike strike = _pool.Get();
        strike.Launch(position);
    }

    private AirStrike CreateStrike()
    {
        AirStrike strike = Instantiate(_strikePrefab, transform);
        strike.Init(_pool, _explosionPool);
        return strike;
    }

    private void OnGetStrike(AirStrike strike) { strike.gameObject.SetActive(true); }
    private void OnReleaseStrike(AirStrike strike) { strike.gameObject.SetActive(false); }
    private void OnDestroyStrike(AirStrike strike) { Destroy(strike.gameObject); }
}