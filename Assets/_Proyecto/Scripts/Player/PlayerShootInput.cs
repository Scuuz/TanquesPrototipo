/*
 * PlayerShootInput.cs
 *
 * Se usa el input viejo y le pide a TankShooting que dispare cuando se efectua la acción.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 26/09/2026
 * Modificación: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(TankShooting))]

public class PlayerShootInput : MonoBehaviour
{
    private TankShooting _shooting;

    private void Awake()
    {
        _shooting = GetComponent<TankShooting>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            _shooting.TryFire();
        }
    }
}
