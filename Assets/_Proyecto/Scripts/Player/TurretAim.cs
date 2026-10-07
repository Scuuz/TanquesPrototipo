/*
 * TurretAim.cs
 *
 * Rotacion de la torreta del tanque hacia la posicion del mouse
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform _turretPivot;    // objeto vacío que contiene al FirePoint
    [SerializeField] private Transform _turretVisual;   // malla de la torreta (Model/TankRenderers/TankTurret)

    [Header("Aim")]
    [SerializeField] private float _turnSpeed = 360f;   // grados por segundo
    [SerializeField] private float _aimHeight = 1f;     // altura del plano donde "apoya" el mouse

    private Camera _camera;
    private Quaternion _visualOffset;   // rotación original de la malla respecto del tanque
    private float _yaw;                 // ángulo actual de la torreta, en el mundo

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
        // Al aparecer o reaparecer, la torreta arranca mirando hacia adelante
        _yaw = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;   // juego en pausa (victoria / game over)

        if (TryGetMouseYaw(out float targetYaw))
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
}
