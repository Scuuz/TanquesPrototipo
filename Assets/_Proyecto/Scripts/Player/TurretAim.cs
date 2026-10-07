/*
 * TurretAim.cs
 *
 * Rotacion de la torreta del tanque hacia la posicion del mouse(Player 1) o stick derecho(Player 2)
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.InputSystem;

public class TurretAim : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private ControlScheme _controlScheme = ControlScheme.KeyboardMouse;

    [Header("Referencias")]
    [SerializeField] private Transform _turretPivot;
    [SerializeField] private Transform _turretVisual;

    [Header("Aim")]
    [SerializeField] private float _turnSpeed = 360f;
    [SerializeField] private float _aimHeight = 1f;

    private Camera _camera;
    private Quaternion _visualOffset;
    private float _yaw;

    private void Awake()
    {
        _camera = Camera.main;
        if (_turretVisual != null)
        {
            _visualOffset = Quaternion.Inverse(transform.rotation) * _turretVisual.rotation;
        }
    }

    private void OnEnable()
    {
        _yaw = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;

        float targetYaw;
        bool hasTarget = _controlScheme == ControlScheme.Gamepad
            ? TryGetGamepadYaw(out targetYaw)
            : TryGetMouseYaw(out targetYaw);

        if (hasTarget)
        {
            _yaw = Mathf.MoveTowardsAngle(_yaw, targetYaw, _turnSpeed * Time.deltaTime);
        }

        Quaternion rotation = Quaternion.Euler(0f, _yaw, 0f);
        _turretPivot.rotation = rotation;
        if (_turretVisual != null) _turretVisual.rotation = rotation * _visualOffset;
    }

    private bool TryGetMouseYaw(out float yaw)
    {
        yaw = 0f;
        if (_camera == null) return false;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0f, _aimHeight, 0f));
        if (!plane.Raycast(ray, out float distance)) return false;

        Vector3 direction = ray.GetPoint(distance) - _turretPivot.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.25f) return false;

        yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        return true;
    }

    private bool TryGetGamepadYaw(out float yaw)
    {
        yaw = 0f;
        Gamepad pad = Gamepad.current;
        if (pad == null) return false;

        Vector2 stick = pad.rightStick.ReadValue();
        if (stick.sqrMagnitude < 0.09f) return false;

        yaw = Mathf.Atan2(stick.x, stick.y) * Mathf.Rad2Deg;
        return true;
    }
}