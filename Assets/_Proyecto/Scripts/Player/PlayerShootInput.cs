/*
 * PlayerShootInput.cs
 *
 * Se usa el input system y le pide a TankShooting que dispare cuando se efectua la acción.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 26/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TankShooting))]

public class PlayerShootInput : MonoBehaviour
{
    [SerializeField] private InputActionReference _shootAction;

    private TankShooting _shooting;

    private void Awake()
    {
        _shooting = GetComponent<TankShooting>();
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
        if (_shootAction.action.WasPressedThisFrame())
        {
            _shooting.TryFire();
        }
    }
}
