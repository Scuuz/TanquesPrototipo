/*
 * TankShooting.cs
 *
 * Se usa Object Pool para disparar los proyectiles desde el FirePoint respetando el cooldown (Player y Enemigos).
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Modificación: 26/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.Pool;

public class TankShooting : MonoBehaviour
{

    [Header ("Shooting")]
    [SerializeField] private Shell _shellPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private EffectPool _impactEffectPool;

    [Header("Pool")]
    [SerializeField] private int _poolDefaultCapacity = 10;
    [SerializeField] private int _poolMaxSize = 30;


    private float _nextFireTime;
    private ObjectPool<Shell> _shellPool;

    public Transform FirePoint => _firePoint;
    public bool CanFire => Time.time >= _nextFireTime;

    private void Awake()
    {
        _shellPool = new ObjectPool<Shell>(
            CreateShell,
            OnGetShell,
            OnReleaseShell,
            OnDestroyShell,
            true,
            _poolDefaultCapacity,
            _poolMaxSize);
    }

    

    public bool TryFire()
    {
        if (!CanFire) return false;

        _nextFireTime = Time.time + _fireRate;

        Shell shell = _shellPool.Get();
        shell.Launch(_firePoint.position, _firePoint.rotation, _firePoint.forward);
        return true;
    }

    private Shell CreateShell()
    {
        Shell shell = Instantiate(_shellPrefab, _firePoint.position, _firePoint.rotation);
        shell.Init (_shellPool, _impactEffectPool);
        return shell;
    }

    private void OnGetShell (Shell shell) { shell.gameObject.SetActive(true);}
    private void OnReleaseShell(Shell shell){shell.gameObject.SetActive(false);}
    private void OnDestroyShell(Shell shell){Destroy(shell.gameObject);}

}
