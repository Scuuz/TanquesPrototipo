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

public class TankShooting : MonoBehaviour
{
    [Header ("Input")]
    [SerializeField] private InputActionReference _shootAction;

    [Header ("Shooting")]
    [SerializeField] private Shell _shellPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.5f;

    private float _nextFireTime;

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

        Shell shell = Instantiate(_shellPrefab, _firePoint.position, _firePoint.rotation);
        shell.Launch(_firePoint.forward);
    }
}
