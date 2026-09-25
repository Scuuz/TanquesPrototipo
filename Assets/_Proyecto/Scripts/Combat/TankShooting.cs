/*
 * TankShooting.cs
 *
 * Disparar los proyectiles desde el FirePoint. Tiene un cooldown al disparar.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class TankShooting : MonoBehaviour
{
    [Header ("Input")]
    [SerializeField] private InputActionReference _shootAction;

    [Header ("Shooting")]
    [SerializeField] private Shell _shellPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.5f;

    [Header("Pool")]
    [SerializeField] private int _poolDefaultCapacity = 10;
    [SerializeField] private int _poolMaxSize = 30;

    private float _nextFireTime;
    private ObjectPool<Shell> _shellPool;

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

    private void OnEnable()
    {
        _shootAction.action.Enable();
    }

    private void OnDisable()
    {
        _shootAction.action.Disable();
    }

    private void Update()
    {
        if (_shootAction.action.WasPressedThisFrame() && Time.time >= _nextFireTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        _nextFireTime = Time.time + _fireRate;

        Shell shell = _shellPool.Get();
        shell.Launch(_firePoint.position, _firePoint.rotation, _firePoint.forward);
    }

    private Shell CreateShell()
    {
        Shell shell = Instantiate(_shellPrefab, _firePoint.position, _firePoint.rotation);
        shell.SetPool (_shellPool);
        return shell;
    }

    private void OnGetShell (Shell shell)
    {
        shell.gameObject.SetActive(true);
    }

    private void OnReleaseShell (Shell shell)
    {
        shell.gameObject.SetActive(false);
    }

    private void OnDestroyShell(Shell shell)
    {
        Destroy(shell.gameObject);
    }
}
