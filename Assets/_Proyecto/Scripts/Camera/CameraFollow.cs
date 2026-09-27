/*
 * CameraFollow.cs
 *
 * Cámara cenital que sigue al tanque.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 24/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [Header("Angulo y distancia de camara")]
    [Range (30f, 90f)]
    [SerializeField] private float _pitch = 55f;
    [SerializeField] private float _distance = 24f;

    [Header("Suavizado")]
    [SerializeField] private float _smoothTime = 0.2f;

    private Vector3 _velocity = Vector3.zero;

    private Quaternion ViewRotation => Quaternion.Euler (_pitch, 0f, 0f);
    private Vector3 Offset => ViewRotation * new Vector3(0f, 0f, -_distance);
    private void Start()
    {
        SnapToTarget();
    }
    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 desiredPosition = _target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothTime);
        transform.rotation = ViewRotation;
    }

    private void OnValidate()
    {
        SnapToTarget();
    }

    private void SnapToTarget()
    {
        if (_target == null) return;
        transform.rotation = ViewRotation;
        transform.position = _target.position + Offset;
    }
}
