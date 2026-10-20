/*
 * PlayerShootInput.cs
 *
 * Lee el disparo tanto del player 1 como del 2. Le pide a TankShooting que dispare
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 26/09/2026
 * Modificación: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TankShooting))]
public class PlayerShootInput : MonoBehaviour
{
    [SerializeField] private ControlScheme _controlScheme = ControlScheme.KeyboardMouse;

    private TankShooting _shooting;

    private void Awake()
    {
        _shooting = GetComponent<TankShooting>();
    }

    private void Update()
    {
        if (PauseMenu.IsPaused) return;
        
        if (FirePressed())
        {
            _shooting.TryFire();
        }
    }

    private bool FirePressed()
    {
        if (_controlScheme == ControlScheme.Gamepad)
        {
            Gamepad pad = Gamepad.current;
            if (pad == null) return false;

            return pad.rightTrigger.wasPressedThisFrame
                || pad.rightShoulder.wasPressedThisFrame
                || pad.buttonSouth.wasPressedThisFrame;
        }

        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
    }
}